using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    /// <summary>
    /// [singleton] User Database implemented as flat files.
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
    public class JsonDb
    {

        public class JsonDbContent { string j; };
        private JsonDbContent _dbContent = null;

        /// <summary>
        /// Our one-and-only instance.
        /// </summary>
        private static JsonDb _instance = null;

        /// <summary>
        /// Path and name of JSON file with all data.
        /// </summary>
        private static string _connection = "./users.json";

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
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex _innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex _outerMutex = new Mutex();

        // Getters/Setters
        public static Mutex InnerMutex { get => _innerMutex; }
        public static Mutex OuterMutex { get => _outerMutex; }

        /// <summary>
        /// Fetch the singleton.
        /// </summary>
        public static JsonDb Instance
        {
            get
            {
                // This is NOT a redundant "if" but it is here so we don't lock for 
                // initial simultaneous Instance reference, crippling performance.
                if (_instance == null)
                {
                    _outerMutex.WaitOne();
                    if (_instance == null)
                    {
                        _instance = new JsonDb();
                    }
                    _outerMutex.ReleaseMutex();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private JsonDb()
        {
            if (!new StackTrace().GetFrame(1).GetMethod().Name.EndsWith("JsonDb.Instance"))
            {
                Logger.Error(null, "Attempt to subvert singleton JsonDb");
                return;
            }
            _fullPath = _connection;
            _errorMessage = "";
            _isDirty = false;
            if (File.Exists(_connection))
            {
                JsonDb.InnerMutex.WaitOne();
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.AllowTrailingCommas = true;
                try
                {
                    using (FileStream stream = File.OpenRead(_fullPath))
                    {
                        // _dbContent = JsonSerializer.DeserializeAsync<JsonDbHeader>(stream, options).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    throw new Exception("Cannot deserialize " + _fullPath, ex);
                }
                finally
                {
                    JsonDb.InnerMutex.ReleaseMutex();
                }
            }
            else
            {
                _dbContent = new JsonDbContent();
            }
            // _dbContent.DbName = Path.GetFileNameWithoutExtension(_fullPath);
        }

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public void CloseWithoutSync()
        {
            Logger.Info(null, "Close Without Sync " + _fullPath);
            _fullPath = null;
            _dbContent = null;
            _isDirty = false;
            _errorMessage = "Close Without Sync";
            Dispose();
        }

        /// <summary>
        /// Close the DB
        /// </summary>
        public void SyncAndClose()
        {
            Logger.Info(null, "SyncAndClose " + _fullPath);
            Sync();
            Dispose();
        }

        /// <summary>
        /// This must be closed to release the potential mutex (does not save DB!)
        /// </summary>
        public void Dispose()
        {
            //
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public bool Sync()
        {
            _errorMessage = "";
            Directory.CreateDirectory(Path.GetDirectoryName(_connection));
            using (FileStream stream = File.Create(_fullPath))
            {
                //_dbContent.LastSaved = DateTime.Now;
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                JsonDb.OuterMutex.WaitOne();
                try
                {
                    //JsonSerializer.SerializeAsync<JsonDbHeader>(stream, _dbHeader, options).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    Logger.Error(null, _errorMessage);
                    throw new Exception("Cannot serialize " + _fullPath, ex);
                }
                finally
                {
                    JsonDb.OuterMutex.ReleaseMutex();
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

        /////////////////////////// CheckbookEntry ///////////////////////////

    //    /// <summary>
    //    /// Iterator to travese checkbook entries by date entered.
    //    /// </summary>
    //    public CheckbookEntryIterator CheckbookEntryIterator
    //    {
    //        get
    //        {
    //            return GetCheckbookEntryIterator(SortEntriesBy.TranDate);
    //        }
    //    }

    //    /// <summary>
    //    /// Iterator to travese checkbook entries.
    //    /// </summary>
    //    /// <param name="sortEntriesBy">currently ignored</param>
    //    public CheckbookEntryIterator GetCheckbookEntryIterator(SortEntriesBy sortEntriesBy)
    //    {
    //        _errorMessage = "";
    //        CheckbookEntryIterator iterator = null;
    //        JsonDbAccess.Mutex.WaitOne();
    //        try
    //        {
    //            iterator = new CheckbookEntryIterator(_dbHeader.CheckbookEntries.GetEnumerator(), "");
    //        }
    //        catch (Exception ex)
    //        {
    //            _errorMessage = ex.Message;
    //            throw new AppException("Error in GetCheckbookEntryIterator()", ex, ExceptionHandling.NoSaveCleanupContinue);
    //        }
    //        finally
    //        {
    //            JsonDbAccess.Mutex.ReleaseMutex();
    //        }
    //        return iterator;
    //    }

    //    /// <summary>
    //    /// Add a new entry into the checkbook.
    //    /// </summary>
    //    /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
    //    /// <param name="highlight">how to highlight the entry, Modified if by default</param>
    //    /// <returns>True if successful</returns>
    //    public bool InsertEntry(CheckbookEntry entry, Highlight highlight = Highlight.Modified)
    //    {
    //        _errorMessage = "";
    //        if (entry.Id == null || entry.Id.Equals(Guid.Empty))
    //        {
    //            entry.Id = Guid.NewGuid();
    //        }
    //        entry.ModifiedBy = System.Environment.UserName;
    //        entry.DateModified = DateTime.Now;
    //        entry.SetHighlight(highlight);
    //        JsonDbAccess.Mutex.WaitOne();
    //        try
    //        {
    //            _dbHeader.CheckbookEntries.Add(entry.UniqueKey(), entry);
    //            if (_undoTracker != null)
    //            {
    //                _undoTracker.TrackInsertion(entry);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _errorMessage = ex.Message;
    //            throw new AppException("Error in InsertEntry(CheckbookEntry)", ex, ExceptionHandling.NoSaveCleanupContinue);
    //        }
    //        finally
    //        {
    //            JsonDbAccess.Mutex.ReleaseMutex();
    //        }
    //        _isDirty = true;
    //        return true;
    //    }

    //    /// <summary>
    //    /// Flag an entry as "Updated."
    //    /// </summary>
    //    /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
    //    /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
    //    /// <param name="updateModDate">true to update the modified date</param>
    //    /// <returns>True if successful</returns>
    //    public bool UpdateEntry(CheckbookEntry newEntry, CheckbookEntry oldEntry, bool updateModDate)
    //    {
    //        DeleteEntry(oldEntry);
    //        DeleteEntry(newEntry);
    //        newEntry.Id = oldEntry.Id; // <-- probably no longer necessary, but just in case
    //        bool ok = InsertEntry(newEntry);
    //        if (!updateModDate)
    //        {
    //            newEntry.DateModified = oldEntry.DateModified;
    //        }
    //        return ok;
    //    }

    //    /// <summary>
    //    /// Get an entry based on the ID.
    //    /// </summary>
    //    /// <param name="id">The GUID to look-up</param>
    //    /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
    //    public CheckbookEntry GetCheckbookEntryById(Guid id)
    //    {
    //        _errorMessage = "";
    //        if (id == null)
    //        {
    //            return null;
    //        }
    //        CheckbookEntry entry = null;
    //        JsonDbAccess.Mutex.WaitOne();
    //        try
    //        {
    //            IEnumerator<KeyValuePair<string, CheckbookEntry>> enumerator = _dbHeader.CheckbookEntries.GetEnumerator();
    //            while (enumerator.MoveNext())
    //            {
    //                KeyValuePair<string, CheckbookEntry> pair = enumerator.Current;
    //                if (pair.Value.Id == id)
    //                {
    //                    entry = pair.Value;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _errorMessage = ex.Message;
    //            throw new AppException("Error in GetCheckbookEntryById()", ex, ExceptionHandling.NoSaveCleanupContinue);
    //        }
    //        finally
    //        {
    //            JsonDbAccess.Mutex.ReleaseMutex();
    //        }
    //        return entry;
    //    }

    //    /// <summary>
    //    /// Remove an entry from the DB.
    //    /// </summary>
    //    /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
    //    /// <returns>True if successful</returns>
    //    public bool DeleteEntry(CheckbookEntry entry)
    //    {
    //        _errorMessage = "";
    //        entry.ModifiedBy = System.Environment.UserName;
    //        entry.DateModified = DateTime.Now; // needed for undo
    //        JsonDbAccess.Mutex.WaitOne();
    //        try
    //        {
    //            if (_dbHeader.CheckbookEntries.ContainsKey(entry.UniqueKey()))
    //            {
    //                _dbHeader.CheckbookEntries.Remove(entry.UniqueKey());
    //                if (_undoTracker != null)
    //                {
    //                    _undoTracker.TrackDeletion(entry);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _errorMessage = ex.Message;
    //            throw new AppException("Error in RemoveEntry(CheckbookEntry)", ex, ExceptionHandling.NoSaveCleanupContinue);
    //        }
    //        finally
    //        {
    //            JsonDbAccess.Mutex.ReleaseMutex();
    //        }
    //        _isDirty = true;
    //        return true;
    //    }

    }

}
