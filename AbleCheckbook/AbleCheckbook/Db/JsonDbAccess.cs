using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Checkbook Database implemented as flat files.
    /// </summary>
    /// <remarks>
    /// This is NOT currently implemented as a DB but as in-memory collections. That means that
    /// returned records are "live" and that any change to them is immediately changed in the 
    /// in-memory DB. This is quick and handy but introduces a potential Undo problem: When you
    /// call UpdateEntry() or UpdateOrInsertEntry() it will not update the Undo queue properly 
    /// because the "previous" version of that entry, to be queued for possible future "undo",
    /// will be the changed version - the "previous" and "updated" records will be identical.
    /// Therefore you must hold onto a copy of the prior version so that you can delete/insert
    /// instead of updating the record.
    /// </remarks>
    public class JsonDbAccess : IDbAccess
    {

        /// <summary>
        /// How often to autosave if changes were made.
        /// </summary>
        public const long AUTO_SAVE_SECONDS = 19;

        /// <summary>
        /// WHen did we last autosave?
        /// </summary>
        private static DateTime _lastAutoSave = DateTime.Now;

        /// <summary>
        /// This is the full path to the JSON DB file. 
        /// </summary>
        private string _fullPath = null;

        /// <summary>
        /// Description of last error. "" if none.
        /// </summary>
        private string _errorMessage = "";

        /// <summary>
        /// Has the data changed since the last save/sync was done?
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// This is called to track operations that modify the DB.
        /// </summary>
        private IUndoTracker _undoTracker = null;

        /// <summary>
        /// This struct holds all of the data.
        /// </summary>
        private JsonDbHeader _dbHeader = new JsonDbHeader();

        /// <summary>
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex _innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex _outerMutex = new Mutex();

        // Getters/Setters
        public static Mutex Mutex { get => _innerMutex; }
        public static Mutex Mutex2 { get => _outerMutex; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connection">Name of connection, i.e. "Checking" - opt w hyphen and 4-digit year</param>
        /// <param name="undoTracker">To be called for tracking operations that can later be undone.</param>
        /// <param name="startEmpty">true to start empty, first deleting the db if it exists</param>
        public JsonDbAccess(string connection, IUndoTracker undoTracker, bool startEmpty = false)
        {
            AppException.SetDb(null);
            _errorMessage = "";
            _fullPath = connection;
            _isDirty = false;
            _undoTracker = undoTracker;
            string account = connection;
            int hyphenIndex = account.LastIndexOf('-');
            if(hyphenIndex > 0)
            {
                account = account.Substring(0, hyphenIndex);
            }
            if (_fullPath.Equals("csvtemp"))
            {
                _dbHeader = new JsonDbHeader();
                _dbHeader.DbName = _fullPath;
            }
            else
            {
                if (Path.GetDirectoryName(connection).Length < 3)
                {
                    if (!Configuration.Instance.GetLegalFilenames().Contains(account))
                    {
                        if (!connection.Contains("UtEsT"))
                        {
                            connection = "Banking";
                        }
                    }
                    _fullPath = UtilityMethods.GetDbFilename(connection.Trim(), false, false);
                }
                if (IsAcctOpenElsewhere(connection))
                {
                    _errorMessage = "File/acct/DB is open in another window: " + _fullPath;
                    System.Windows.Forms.MessageBox.Show(_errorMessage); // is this necessary here?
                    AppException.SetDb(null);
                    throw new AppException(_errorMessage, null, ExceptionHandling.CompleteFailure);
                }
                if (startEmpty)
                {
                    File.Delete(_fullPath);
                }
                if (File.Exists(_fullPath))
                {
                    JsonDbAccess.Mutex.WaitOne();
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.AllowTrailingCommas = true;
                    try
                    {
                        using (FileStream stream = File.OpenRead(_fullPath))
                        {
                            _dbHeader = JsonSerializer.DeserializeAsync<JsonDbHeader>(stream, options).GetAwaiter().GetResult();
                            AdjustDbForCompatibility();
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorMessage = ex.Message;
                        throw new AppException("Cannot deserialize " + _fullPath, ex, ExceptionHandling.NoSaveThenRestart);
                    }
                    finally
                    {
                        JsonDbAccess.Mutex.ReleaseMutex();
                    }
                }
                else
                {
                    _dbHeader = new JsonDbHeader();
                }
                _dbHeader.DbName = Path.GetFileNameWithoutExtension(_fullPath);
            }
            AppException.SetDb(this);
        }

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public void CloseWithoutSync()
        {
            AppException.SetDb(null);
            Logger.Info("Close Without Sync " + _fullPath);
            _fullPath = null;
            _dbHeader = null;
            _isDirty = false;
            _errorMessage = "Close Without Sync";
            Dispose();
        }

        /// <summary>
        /// Close the DB
        /// </summary>
        public void SyncAndClose()
        {
            AppException.SetDb(null);
            Logger.Info("SyncAndClose " + _fullPath);
            Sync();
            Dispose();
        }

        /// <summary>
        /// This must be closed to release the potential mutex (does not save DB!)
        /// </summary>
        public void Dispose()
        {
            AppException.SetDb(null);
        }

        /// <summary>
        /// Is a specified account already open in another window?
        /// </summary>
        /// <param name="connection">acct DB filename, with or without suffix</param>
        /// <returns>true if it is already open</returns>
        public bool IsAcctOpenElsewhere(string connection)
        {
            // Note: initially this was done w/ a global mutex - bad idea, esp when the program crashes, etc.
            string acctName = Path.GetFileNameWithoutExtension(
                    UtilityMethods.GetDbFilename(connection.Trim(), false, false)).ToUpper();
            Process thisProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if(process == thisProcess)
                {
                    continue;
                }
                if(process.MainWindowTitle.ToUpper().Contains(acctName) &&
                   process.MainWindowTitle.Contains(Strings.Get("Able Strategies AbleCheckbook")))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Full path the current DB account file.
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }

        /// <summary>
        /// Get account info, i.e. online connection settings.)
        /// </summary>
        public IAccount Account
        {
            get
            {
                return _dbHeader;
            }
        }

        /// <summary>
        /// Iterate over the checkbook entries, updating the balance in each.
        /// </summary>
        /// <returns>Final balance.</returns>
        public long AdjustBalances()
        {
            long balance = 0L;
            List<CheckbookEntry> entries = new CheckbookSorter().GetSortedEntries(this, SortEntriesBy.TranDate);
            foreach(CheckbookEntry entry in entries)
            {
                balance += entry.Amount;
                entry.Balance = balance;
            }
            return balance;
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public bool Sync()
        {
            _errorMessage = "";
            if (_fullPath.Equals("csvtemp"))
            {
                _isDirty = false;
                return true;
            }
            // Always saves to the DB directory with a .db extension...
            _fullPath = Path.Combine(Configuration.Instance.DirectoryDatabase, _dbHeader.DbName + ".acb");
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            using (FileStream stream = File.Create(_fullPath))
            {
                _dbHeader.LastSaved = DateTime.Now;
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                JsonDbAccess.Mutex.WaitOne();
                try
                {
                    JsonSerializer.SerializeAsync<JsonDbHeader>(stream, _dbHeader, options).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    throw new AppException("Cannot serialize " + _fullPath, ex, ExceptionHandling.NoSaveCleanupContinue);
                }
                finally
                {
                    JsonDbAccess.Mutex.ReleaseMutex();
                }
            }
            _isDirty = false;
            return true;
        }

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        public void IdleTimeSync()
        {
            if (!_isDirty || DateTime.Now.Subtract(_lastAutoSave).TotalSeconds < AUTO_SAVE_SECONDS)
            {
                return;
            }
            _lastAutoSave = DateTime.Now;
            Sync();
            _isDirty = false;
        }

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }

        /// <summary>
        /// Get the name of the db (i.e. Checking, Personal, Business, ALternate)
        /// </summary>
        public string Name
        {
            get
            {
                return _dbHeader.DbName;
            }
            set
            {
                _dbHeader.DbName = value;
            }
        }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        public bool Successful
        {
            get
            {
                return _errorMessage.Length > 0;
            }
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public string LastMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        /// <summary>
        /// Are we in the middle of something?
        /// </summary>
        public InProgress InProgress
        {
            get
            {
                return _dbHeader.InProgress;
            }
            set
            {
                _dbHeader.InProgress = value;
            }
        }

        /////////////////////////// Compatibility ////////////////////////////

        /// <summary>
        /// Normalize keys in case of old version of DB file.
        /// </summary>
        private void AdjustDbForCompatibility()
        {
            AdjustCheckbookKeys();
            AdjustCategoryKeys();
            AdjustEventKeys();
        }

        /// <summary>
        /// Adjust keys for latest version, using Id only.
        /// </summary>
        private void AdjustCheckbookKeys()
        {
            Dictionary<string, CheckbookEntry> entries = new Dictionary<string, CheckbookEntry>();
            if (_dbHeader.CheckbookEntries.Count > 0 && _dbHeader.CheckbookEntries.First().Key.Length > 50)
            {
                JsonDbAccess.Mutex.WaitOne();
                try
                {
                    CheckbookEntryIterator iterator = CheckbookEntryIterator;
                    while (iterator.HasNextEntry())
                    {
                        CheckbookEntry entry = iterator.GetNextEntry();
                        entries.Add(entry.UniqueKey(), entry);
                    }
                    _dbHeader.CheckbookEntries = entries;
                }
                finally
                {
                    JsonDbAccess.Mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Adjust keys for latest version, using Id only.
        /// </summary>
        private void AdjustCategoryKeys()
        {
            Dictionary<string, FinancialCategory> categories = new Dictionary<string, FinancialCategory>();
            if (_dbHeader.FinancialCategories.Count > 0)
            {
                IEnumerator<KeyValuePair<string, FinancialCategory>> enumerator = _dbHeader.FinancialCategories.GetEnumerator();
                int possibleOldKindCount = 0;
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Key.EndsWith("D") || enumerator.Current.Key.EndsWith("C"))
                    {
                        ++possibleOldKindCount;
                    }
                }
                if (possibleOldKindCount > _dbHeader.FinancialCategories.Count / 2)
                {
                    JsonDbAccess.Mutex.WaitOne();
                    try
                    {
                        FinancialCategoryIterator iterator = FinancialCategoryIterator;
                        while (iterator.HasNextEntry())
                        {
                            FinancialCategory entry = iterator.GetNextEntry();
                            categories.Add(entry.UniqueKey(), entry);
                        }
                        _dbHeader.FinancialCategories = categories;
                    }
                    finally
                    {
                        JsonDbAccess.Mutex.ReleaseMutex();
                    }
                }
            }
        }

        /// <summary>
        /// Adjust keys for latest version, using Id only.
        /// </summary>
        private void AdjustEventKeys()
        {
            Dictionary<string, ScheduledEvent> entries = new Dictionary<string, ScheduledEvent>();
            if (_dbHeader.ScheduledEvents.Count > 0 && _dbHeader.ScheduledEvents.First().Key.Length > 36)
            {
                JsonDbAccess.Mutex.WaitOne();
                try
                {
                    ScheduledEventIterator iterator = ScheduledEventIterator;
                    while (iterator.HasNextEntry())
                    {
                        ScheduledEvent entry = iterator.GetNextEntry();
                        entries.Add(entry.UniqueKey(), entry);
                    }
                    _dbHeader.ScheduledEvents = entries;
                }
                finally
                {
                    JsonDbAccess.Mutex.ReleaseMutex();
                }
            }
        }

        //////////////////////////// Reconcliliation /////////////////////////

        /// <summary>
        /// Get recon values - balance & date.
        /// </summary>
        /// <returns>values</returns>
        public ReconciliationValues GetReconciliationValues()
        {
            return new ReconciliationValues(_dbHeader.ReconciliationBalance, _dbHeader.ReconciliationDate);
        }

        /// <summary>
        /// Insert reconciliation values.
        /// </summary>
        /// <param name="values">recon values</param>
        /// <returns>success</returns>
        public bool InsertEntry(ReconciliationValues values)
        {
            _dbHeader.ReconciliationBalance = values.Balance;
            _dbHeader.ReconciliationDate = values.Date;
            _undoTracker.TrackInsertion(values);
            return true;
        }

        /// <summary>
        /// Delete reconciliation values.
        /// </summary>
        /// <param name="values">ignored - merely stashes old values to the undo queue</param>
        /// <returns>success</returns>
        public bool DeleteEntry(ReconciliationValues values)
        {
            _undoTracker.TrackDeletion(GetReconciliationValues());
            return true;
        }

        /////////////////////////// CheckbookEntry ///////////////////////////

        /// <summary>
        /// Iterator to travese checkbook entries by date entered.
        /// </summary>
        public CheckbookEntryIterator CheckbookEntryIterator
        {
            get
            {
                return GetCheckbookEntryIterator(SortEntriesBy.TranDate);
            }
        }

        /// <summary>
        /// Iterator to travese checkbook entries.
        /// </summary>
        /// <param name="sortEntriesBy">currently ignored</param>
        public CheckbookEntryIterator GetCheckbookEntryIterator(SortEntriesBy sortEntriesBy)
        {
            _errorMessage = "";
            CheckbookEntryIterator iterator = null;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                iterator = new CheckbookEntryIterator(_dbHeader.CheckbookEntries.GetEnumerator(), "");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetCheckbookEntryIterator()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return iterator;
        }

        /// <summary>
        /// Add a new entry into the checkbook.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <param name="highlight">how to highlight the entry, Modified if by default</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(CheckbookEntry entry, Highlight highlight = Highlight.Modified)
        {
            _errorMessage = "";
            if (entry.Id == null || entry.Id.Equals(Guid.Empty))
            {
                entry.Id = Guid.NewGuid();
            }
            entry.ModifiedBy = System.Environment.UserName;
            entry.DateModified = DateTime.Now;
            entry.SetHighlight(highlight);
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                _dbHeader.CheckbookEntries.Add(entry.UniqueKey(), entry);
                if (_undoTracker != null)
                {
                    _undoTracker.TrackInsertion(entry);
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in InsertEntry(CheckbookEntry)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true; 
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <param name="updateModDate">true to update the modified date</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(CheckbookEntry newEntry, CheckbookEntry oldEntry, bool updateModDate)
        {
            DeleteEntry(oldEntry);
            DeleteEntry(newEntry);
            newEntry.Id = oldEntry.Id; // <-- probably no longer necessary, but just in case
            bool ok = InsertEntry(newEntry);
            if (!updateModDate)
            {
                newEntry.DateModified = oldEntry.DateModified;
            }
            return ok;
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public CheckbookEntry GetCheckbookEntryById(Guid id)
        {
            _errorMessage = "";
            if (id == null)
            {
                return null;
            }
            CheckbookEntry entry = null;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                IEnumerator<KeyValuePair<string, CheckbookEntry>> enumerator = _dbHeader.CheckbookEntries.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, CheckbookEntry> pair = enumerator.Current;
                    if (pair.Value.Id == id)
                    {
                        entry = pair.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetCheckbookEntryById()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return entry;
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(CheckbookEntry entry)
        {
            _errorMessage = "";
            entry.ModifiedBy = System.Environment.UserName;
            entry.DateModified = DateTime.Now; // needed for undo
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                if(_dbHeader.CheckbookEntries.ContainsKey(entry.UniqueKey()))
                { 
                    _dbHeader.CheckbookEntries.Remove(entry.UniqueKey());
                    if (_undoTracker != null)
                    {
                        _undoTracker.TrackDeletion(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in RemoveEntry(CheckbookEntry)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true; 
        }

        /////////////////////////// ScheduledEvent ///////////////////////////

        /// <summary>
        /// Iterator to travese entries by day of month.
        /// </summary>
        public ScheduledEventIterator ScheduledEventIterator
        {
            get
            {
                _errorMessage = "";
                ScheduledEventIterator iterator = null;
                JsonDbAccess.Mutex.WaitOne();
                try
                {
                    iterator = new ScheduledEventIterator(_dbHeader.ScheduledEvents.GetEnumerator());
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    throw new AppException("Error in ScheduledEventIterator()", ex, ExceptionHandling.NoSaveCleanupContinue);
                }
                finally
                {
                    JsonDbAccess.Mutex.ReleaseMutex();
                }
                return iterator;
            }
        }

        /// <summary>
        /// Add a new entry into the scheduled events
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(ScheduledEvent entry)
        {
            _errorMessage = "";
            if (entry.Id == null || entry.Id.Equals(Guid.Empty))
            {
                entry.Id = Guid.NewGuid();
            }
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                _dbHeader.ScheduledEvents.Add(entry.UniqueKey(), entry);
                if (_undoTracker != null)
                {
                    _undoTracker.TrackInsertion(entry);
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in InsertEntry(ScheduledEvent)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true;
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(ScheduledEvent newEntry, ScheduledEvent oldEntry)
        {
            bool ok = DeleteEntry(oldEntry);
            if (ok)
            {
                newEntry.Id = oldEntry.Id; // <-- probably no longer necessary, but just in case
                ok = InsertEntry(newEntry);
            }
            return ok;
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public ScheduledEvent GetScheduledEventById(Guid id)
        {
            _errorMessage = "";
            ScheduledEvent schEvent = null;
            if (id == null)
            {
                return null;
            }
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                IEnumerator<KeyValuePair<string, ScheduledEvent>> enumerator = _dbHeader.ScheduledEvents.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, ScheduledEvent> pair = enumerator.Current;
                    if (pair.Value.Id == id)
                    {
                        schEvent = pair.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetScheduledEventById()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return schEvent;
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(ScheduledEvent entry)
        {
            _errorMessage = "";
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                if(_dbHeader.ScheduledEvents.ContainsKey(entry.UniqueKey()))
                { 
                    _dbHeader.ScheduledEvents.Remove(entry.UniqueKey());
                    if (_undoTracker != null)
                    {
                        _undoTracker.TrackDeletion(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in RemoveEntry(ScheduledEvent)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true;
        }

        ////////////////////////////// Category //////////////////////////////

        /// <summary>
        /// Iterator to travese categories in alphabetic order.
        /// </summary>
        public FinancialCategoryIterator FinancialCategoryIterator
        {
            get
            {
                _errorMessage = "";
                FinancialCategoryIterator iterator = null;
                JsonDbAccess.Mutex.WaitOne();
                try
                {
                    iterator = new FinancialCategoryIterator(_dbHeader.FinancialCategories.GetEnumerator(), "");
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    throw new AppException("Error in FinancialCategoryIterator()", ex, ExceptionHandling.NoSaveCleanupContinue);
                }
                finally
                {
                    JsonDbAccess.Mutex.ReleaseMutex();
                }
                return iterator;
            }
        }

        /// <summary>
        /// Iterator to travese entries by partial match of category name. ("" = all)
        /// </summary>
        public FinancialCategoryIterator GetFinancialCategoryIterator(string startsWith)
        {
            _errorMessage = "";
            FinancialCategoryIterator iterator = null;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                // Not yet implemented: collator for categories
                iterator = new FinancialCategoryIterator(_dbHeader.FinancialCategories.GetEnumerator(), "");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetFinancialCategoryIterator()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return iterator;
        }

        /// <summary>
        /// Add a new entry into the categories.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(FinancialCategory entry)
        {
            if(entry.Name.Trim().Length < 1)
            {
                return false;
            }
            _errorMessage = "";
            if (entry.Id == null || entry.Id.Equals(Guid.Empty))
            {
                entry.Id = Guid.NewGuid();
            }
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                if (_dbHeader.FinancialCategories.ContainsKey(entry.UniqueKey()))
                {
                    if (_undoTracker != null)
                    {
                        _undoTracker.TrackDeletion(entry);
                    }
                    _dbHeader.FinancialCategories.Remove(entry.UniqueKey()); // Just in case
                }
                _dbHeader.FinancialCategories.Add(entry.UniqueKey(), entry);
                if (_undoTracker != null)
                {
                    _undoTracker.TrackInsertion(entry);
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in InsertEntry(FinancialCategory)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true;
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(FinancialCategory newEntry, FinancialCategory oldEntry)
        {
            bool ok = DeleteEntry(oldEntry);
            if (ok)
            {
                newEntry.Id = oldEntry.Id; // <-- probably no longer necessary, but just in case
                ok = InsertEntry(newEntry);
            }
            return ok;
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public FinancialCategory GetFinancialCategoryById(Guid id)
        {
            _errorMessage = "";
            FinancialCategory finCateg = null;
            if (id == null)
            {
                return null;
            }
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                IEnumerator<KeyValuePair<string, FinancialCategory>> enumerator = _dbHeader.FinancialCategories.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, FinancialCategory> pair = enumerator.Current;
                    if (pair.Value.Id == id)
                    {
                        finCateg = pair.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetFinancialCategoryById()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return finCateg;
        }

        /// <summary>
        /// Get an entry based on the category name.
        /// </summary>
        /// <param name="id">The category name to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public FinancialCategory GetFinancialCategoryByName(string name)
        {
            _errorMessage = "";
            FinancialCategory finCateg = null;
            if (name == null || name == "")
            {
                return null;
            }
            name = name.ToUpper().Trim();
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                IEnumerator<KeyValuePair<string, FinancialCategory>> enumerator = _dbHeader.FinancialCategories.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, FinancialCategory> pair = enumerator.Current;
                    if (pair.Value.Name.ToUpper().Trim() == name)
                    {
                        finCateg = pair.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in GetFinancialCategoryByName()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return finCateg;
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(FinancialCategory entry)
        {
            _errorMessage = "";
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                if(_dbHeader.FinancialCategories.ContainsKey(entry.UniqueKey()))
                { 
                    _dbHeader.FinancialCategories.Remove(entry.UniqueKey());
                    if (_undoTracker != null)
                    {
                        _undoTracker.TrackDeletion(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                throw new AppException("Error in RemoveEntry(FinancialCategory)", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            _isDirty = true;
            return true;
        }

    }

}
