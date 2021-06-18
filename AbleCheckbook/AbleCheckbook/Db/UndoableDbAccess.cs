using AbleCheckbook.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Used for storing reconciliation values.
    /// </summary>
    public class ReconciliationValues
    {
        long _balance = 0L;
        DateTime _date = DateTime.Now;

        internal ReconciliationValues(long balance, DateTime date)
        {
            _balance = balance;
            _date = date;
        }

        public ReconciliationValues Clone()
        {
            return new ReconciliationValues(_balance, _date);
        }

        public long Balance { get => _balance; set => _balance = value; }
        public DateTime Date { get => _date; set => _date = value; }
    }

    /// <summary>
    /// Undoable Checkbook Database implemented as flat files.
    /// </summary>
    public class UndoableDbAccess : IDbAccess, IUndoTracker
    {

        /// <summary>
        /// Each step of the undo/redo stacks is one of these types.
        /// </summary>
        internal enum UndoStepKind
        {
            Marker = 0,
            InsertInProgress = 1,
            DeleteInProgress = 2,
            InsertReconciliation = 3,
            DeleteReconciliation = 4,
            DeleteCheckbookEntry = 5,
            InsertCheckbookEntry = 6,
            DeleteFinancialCategory = 7,
            InsertFinancialCategory = 8,
            DeleteScheduledEvent = 9,
            InsertScheduledEvent = 10,
            DeleteMemorizedPayee = 11,
            InsertMemorizedPayee = 12,
        }

        /// <summary>
        /// Each step of the undo/redo stacks is one of these.
        /// </summary>
        internal class UndoStep
        {
            /// <summary>
            /// What kind of operation step involved?
            /// </summary>
            UndoStepKind _kind;

            /// <summary>
            /// This may be a String, CheckbookEntry, FinancialCategory, or ScheduledEvent.
            /// </summary>
            object _record;

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="kind">What kind of operation step involved?</param>
            /// <param name="record">String, CheckbookEntry, FinancialCategory, or ScheduledEvent.</param>
            internal UndoStep(UndoStepKind kind, object record)
            {
                _kind = kind;
                _record = record;
            }

            // Getters/Setters
            internal object Record { get => _record; set => _record = value; }
            internal UndoStepKind Kind { get => _kind; set => _kind = value; }
        }

        /// <summary>
        /// Here's the underlying DB.
        /// </summary>
        private IDbAccess _underlyingDb = null;

        /// <summary>
        /// Here is the Undo stack.
        /// </summary>
        private List<UndoStep> _undoSteps = new List<UndoStep>();

        /// <summary>
        /// Here is the Redo stack. (reverse order from _undoSteps)
        /// </summary>
        private List<UndoStep> _redoSteps = new List<UndoStep>();

        /// <summary>
        /// Number of Undos, so that we don't need to count them each time.
        /// </summary>
        private int _undoMarkerCount = 0;

        /// <summary>
        /// Number of Redos, so that we don't need to count them each time.
        /// </summary>
        private int _redoMarkerCount = 0;

        /// <summary>
        /// Should we be tracking undos? (false if we are currently performing an undo/redo)
        /// </summary>
        private bool _trackUndo = true;

        /// <summary>
        /// Description of top Undo marker.
        /// </summary>
        private string _undoDescription = "";

        /// <summary>
        /// Description of top Redo marker.
        /// </summary>
        private string _redoDescription = "";

        /// <summary>
        /// Mutex to avoid collisions.
        /// </summary>
        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// Mutex property used for synchronization of undo/redo queues
        /// </summary>
        public static Mutex Mutex { get => _mutex; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connection">account file name with no path or extension, possibly no year</param>
        public UndoableDbAccess(string connection)
        {
            _underlyingDb = new JsonDbAccess(connection, this);
        }

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public void CloseWithoutSync()
        {
            _underlyingDb.CloseWithoutSync();
        }

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _underlyingDb.IsDirty;
            }
            set
            {
                _underlyingDb.IsDirty = value;
            }
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public bool Sync()
        {
            return _underlyingDb.Sync();
        }

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        public void IdleTimeSync()
        {
            _underlyingDb.IdleTimeSync();
        }

        /// <summary>
        /// Write the DB to the persistent store along with backups.
        /// </summary>
        /// <returns>success</returns>
        public bool BackupAndSave()
        {
            string filepath = UtilityMethods.GetDbFilename(_underlyingDb.Name, false, false);
            Backups.BackupNow(filepath, 8, ".bu", false, false);
            Configuration.Instance.LastDbName = _underlyingDb.FullPath;
            bool ok = _underlyingDb.Sync();
            Backups.PeriodicBackup(filepath, 7, 8, ".bw", false, true);
            return ok;
        }

        /// <summary>
        /// Full path the current DB account file.
        /// </summary>
        public string FullPath
        {
            get
            {
                return _underlyingDb.FullPath;
            }
        }

        /// <summary>
        /// Iterate over the checkbook entries, updating the balance in each.
        /// </summary>
        /// <returns>Final balance.</returns>
        public long AdjustBalances()
        {
            return _underlyingDb.AdjustBalances();
        }

        /// <summary>
        /// Are we in the middle of something?
        /// </summary>
        public InProgress InProgress
        {
            get
            {
                return _underlyingDb.InProgress;
            }
            set
            {
                TrackDeletion(_underlyingDb.InProgress);
                TrackInsertion(value);
                _underlyingDb.InProgress = value;
            }
        }

        /// <summary>
        /// Are we in the middle of undo/redo?
        /// </summary>
        public bool InUndoRedo
        {
            get
            {
                return !_trackUndo;
            }
        }

        ////////////////////////////// Undo/Redo /////////////////////////////

        /// <summary>
        /// Perform an Undo.
        /// </summary>
        public void UndoToLastMark()
        {
            Logger.Info("Starting Undo");
            UndoableDbAccess.Mutex.WaitOne();
            try
            {
                UndoStep step = null;
                do
                {
                    if (_undoSteps.Count < 1)
                    {
                        _undoDescription = "";
                        _undoMarkerCount = 0;
                        break;
                    }
                    step = _undoSteps.Last<UndoStep>();
                    if (step == null)
                    {
                        _undoDescription = "";
                        _undoMarkerCount = 0;
                        break;
                    }
                    _undoSteps.RemoveAt(_undoSteps.Count - 1);
                    _redoSteps.Add(step);
                    _trackUndo = false;
                    switch (step.Kind)
                    {
                        // Note that the Marker for Undo is BEFORE the entries, not after them
                        case UndoStepKind.Marker:
                            _redoDescription = (string)step.Record;
                            _undoMarkerCount--;
                            _redoMarkerCount++;
                            step = null; // exit loop
                            break;
                        case UndoStepKind.DeleteInProgress:
                            _underlyingDb.InProgress = (InProgress)step.Record;
                            break;
                        case UndoStepKind.InsertInProgress:
                            _underlyingDb.InProgress = (InProgress)step.Record;
                            break;
                        case UndoStepKind.DeleteReconciliation:
                            _underlyingDb.InsertEntry((ReconciliationValues)step.Record);
                            break;
                        case UndoStepKind.InsertReconciliation:
                            _underlyingDb.DeleteEntry((ReconciliationValues)step.Record);
                            break;
                        case UndoStepKind.DeleteCheckbookEntry:
                            InsertEntry((CheckbookEntry)step.Record, Highlight.UndoRedo);
                            break;
                        case UndoStepKind.InsertCheckbookEntry:
                            DeleteEntry((CheckbookEntry)step.Record);
                            break;
                        case UndoStepKind.DeleteFinancialCategory:
                            InsertEntry((FinancialCategory)step.Record);
                            break;
                        case UndoStepKind.InsertFinancialCategory:
                            DeleteEntry((FinancialCategory)step.Record);
                            break;
                        case UndoStepKind.DeleteScheduledEvent:
                            InsertEntry((ScheduledEvent)step.Record);
                            break;
                        case UndoStepKind.InsertScheduledEvent:
                            DeleteEntry((ScheduledEvent)step.Record);
                            break;
                        case UndoStepKind.DeleteMemorizedPayee:
                            InsertEntry((MemorizedPayee)step.Record);
                            break;
                        case UndoStepKind.InsertMemorizedPayee:
                            DeleteEntry((MemorizedPayee)step.Record);
                            break;
                    }
                    _trackUndo = true;
                }
                while (step != null);
            }
            catch (Exception ex)
            {
                throw new AppException("Error in UndoToLastMark()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                UndoableDbAccess.Mutex.ReleaseMutex();
            }
            // Update undoDescription
            _undoDescription = "";
            for (int index = _undoSteps.Count - 1; index >= 0; index--)
            {
                UndoStep step = _undoSteps.ElementAt<UndoStep>(index);
                if(step.Kind == UndoStepKind.Marker)
                {
                    _undoDescription = (string)step.Record;
                    break;
                }
            }
        }

        /// <summary>
        /// Perform a Redo, after having done an Undo.
        /// </summary>
        public void RedoToNextMark()
        {
            Logger.Info("Starting Redo");
            UndoableDbAccess.Mutex.WaitOne();
            try
            {
                bool gotMarker = false;
                UndoStep step = null;
                do
                {
                    if (_redoSteps.Count < 1)
                    {
                        _redoDescription = "";
                        _redoMarkerCount = 0;
                        break;
                    }
                    step = _redoSteps.Last<UndoStep>();
                    if (gotMarker && step.Kind == UndoStepKind.Marker)
                    {
                        _redoDescription = (string)step.Record;
                        break;
                    }
                    if (step == null)
                    {
                        _redoDescription = "";
                        _redoMarkerCount = 0;
                        break;
                    }
                    _redoSteps.RemoveAt(_redoSteps.Count - 1);
                    _undoSteps.Add(step);
                    _trackUndo = false;
                    switch (step.Kind)
                    {
                        // Note that the Marker for Redo is AFTER the entries, not before them
                        case UndoStepKind.Marker:
                            gotMarker = true;
                            _undoDescription = (string)step.Record;
                            _redoMarkerCount--;
                            _undoMarkerCount++;
                            break;
                        case UndoStepKind.DeleteInProgress:
                            _underlyingDb.InProgress = (InProgress)step.Record;
                            break;
                        case UndoStepKind.InsertInProgress:
                            _underlyingDb.InProgress = (InProgress)step.Record;
                            break;
                        case UndoStepKind.DeleteReconciliation:
                            DeleteEntry((ReconciliationValues)step.Record);
                            break;
                        case UndoStepKind.InsertReconciliation:
                            InsertEntry((ReconciliationValues)step.Record);
                            break;
                        case UndoStepKind.DeleteCheckbookEntry:
                            DeleteEntry((CheckbookEntry)step.Record);
                            break;
                        case UndoStepKind.InsertCheckbookEntry:
                            InsertEntry((CheckbookEntry)step.Record, Highlight.UndoRedo);
                            break;
                        case UndoStepKind.DeleteFinancialCategory:
                            DeleteEntry((FinancialCategory)step.Record);
                            break;
                        case UndoStepKind.InsertFinancialCategory:
                            InsertEntry((FinancialCategory)step.Record);
                            break;
                        case UndoStepKind.DeleteScheduledEvent:
                            DeleteEntry((ScheduledEvent)step.Record);
                            break;
                        case UndoStepKind.InsertScheduledEvent:
                            InsertEntry((ScheduledEvent)step.Record);
                            break;
                        case UndoStepKind.DeleteMemorizedPayee:
                            DeleteEntry((MemorizedPayee)step.Record);
                            break;
                        case UndoStepKind.InsertMemorizedPayee:
                            InsertEntry((MemorizedPayee)step.Record);
                            break;
                    }
                    _trackUndo = true;
                }
                while (step != null);
            }
            catch (Exception ex)
            {
                throw new AppException("Error in RedoToNextMark()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                UndoableDbAccess.Mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Very brief man-readable.
        /// </summary>
        public string DescriptionOfNextUndo
        {
            get
            {
                return _undoDescription;
            }
        }

        /// <summary>
        /// Very brief man-readable.
        /// </summary>
        public string DescriptionOfNextRedo
        {
            get
            {
                return _redoDescription;
            }
        }

        /// <summary>
        /// Get the underlying actual DB that this wraps.
        /// </summary>
        public IDbAccess UnderlyingDb
        {
            get
            {
                return _underlyingDb;
            }
        }

        /// <summary>
        /// Get the name of the checkbook (account, etc.). Typically set by the ctor.
        /// </summary>
        public string Name
        {
            get
            {
                return _underlyingDb.Name;
            }
            set
            {
                _underlyingDb.Name = value;
            }
        }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        public bool Successful
        {
            get
            {
                return _underlyingDb.Successful;
            }
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public string LastMessage
        {
            get
            {
                return _underlyingDb.LastMessage;
            }
        }

        /// <summary>
        /// Release some memory/resources because we're running low.
        /// </summary>
        /// <param name="percentage">How many markers (will not completely purge undo/redo if less than 100)</param>
        /// <returns>number of entries/resource released, -1 on error</returns>
        public int ReleaseWeakData(int percentage)
        {
            Logger.Info("Starting ReleaseWeakData");
            int undoReleaseCount = 0;
            if (percentage < 1)
            {
                return -1;
            }
            if (percentage >= 100)
            {
                int total = _undoMarkerCount + _redoMarkerCount;
                _undoSteps = new List<UndoStep>();
                _undoMarkerCount = 0;
                _undoDescription = "";
                _redoSteps = new List<UndoStep>();
                _redoMarkerCount = 0;
                _redoDescription = "";
                return total;
            }
            UndoableDbAccess.Mutex.WaitOne();
            try
            {
                for (undoReleaseCount = 0; undoReleaseCount <= percentage; ++undoReleaseCount)
                {
                    int stepsReleased = 0;
                    do
                    {
                        if (_undoSteps.Count < 1)
                        {
                            _undoDescription = "";
                            break;
                        }
                        UndoStep step = _undoSteps.First<UndoStep>();
                        if (stepsReleased > 0 && step.Kind == UndoStepKind.Marker)
                        {
                            break;
                        }
                        if (undoReleaseCount < percentage)
                        {
                            _undoSteps.Remove(step);
                            stepsReleased++;
                            if (step.Kind == UndoStepKind.Marker)
                            {
                                _undoMarkerCount--;
                            }
                        }
                    }
                    while (true);
                    undoReleaseCount++;
                }
            }
            catch (Exception ex)
            {
                throw new AppException("Error in ReleaseWeakData()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                UndoableDbAccess.Mutex.ReleaseMutex();
            }
            return undoReleaseCount;
        }

        /////////////////////////// Reconciliation ///////////////////////////

        /// <summary>
        /// Get recon values - balance & date.
        /// </summary>
        /// <returns>values</returns>
        public ReconciliationValues GetReconciliationValues()
        {
            return _underlyingDb.GetReconciliationValues();
        }

        /// <summary>
        /// Insert reconciliation values.
        /// </summary>
        /// <param name="values">recon values</param>
        /// <returns>success</returns>
        public bool InsertEntry(ReconciliationValues values)
        {
            return _underlyingDb.InsertEntry(values);
        }

        /// <summary>
        /// Delete reconciliation values.
        /// </summary>
        /// <param name="values">ignored - merely stashes old values to the undo queue</param>
        /// <returns></returns>
        public bool DeleteEntry(ReconciliationValues values)
        {
            return _underlyingDb.DeleteEntry(GetReconciliationValues());
        }

        ////////////////////////// CheckbookEntry ////////////////////////////

        /// <summary>
        /// Iterator to travese checkbook entries by date entered.
        /// </summary>
        public CheckbookEntryIterator CheckbookEntryIterator
        {
            get
            {
                Logger.Trace("Operation: CheckbookEntryIterator");
                return _underlyingDb.CheckbookEntryIterator;
            }
        }

        /// <summary>
        /// Iterator to travese checkbook entries.
        /// </summary>
        public CheckbookEntryIterator GetCheckbookEntryIterator(SortEntriesBy sortEntriesBy)
        {
            Logger.Trace("Operation: GetCheckbookEntryIterator");
            return _underlyingDb.CheckbookEntryIterator; // sorting not implemented
        }

        /// <summary>
        /// Add a new entry into the checkbook.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <param name="highlight">how to highlight the entry, Modified if by default</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(CheckbookEntry entry, Highlight highlight = Highlight.Modified)
        {
            Logger.Trace("Operation: InsertEntry(CheckbookEntry) " + entry.ToShortString());
            return _underlyingDb.InsertEntry(entry, highlight);
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
            Logger.Trace("Operation: UpdateEntry(CheckbookEntry) " + newEntry.ToShortString() + ", old=" + oldEntry.ToShortString());
            return _underlyingDb.UpdateEntry(newEntry, oldEntry, updateModDate);
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public CheckbookEntry GetCheckbookEntryById(Guid id)
        {
            Logger.Trace("Operation: GetCheckbookEntryById " + id);
            return _underlyingDb.GetCheckbookEntryById(id);
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(CheckbookEntry entry)
        {
            Logger.Trace("Operation: RemoveEntry(CheckbookEntry) " + entry.ToShortString());
            return _underlyingDb.DeleteEntry(entry);
        }

        /////////////////////////// ScheduledEvent ///////////////////////////

        /// <summary>
        /// Iterator to travese scheduled events
        /// </summary>
        public ScheduledEventIterator ScheduledEventIterator
        {
            get
            {
                Logger.Trace("Operation: ScheduledEventIterator");
                return _underlyingDb.ScheduledEventIterator;
            }
        }

        /// <summary>
        /// Add a new entry into the scheduled events
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(ScheduledEvent entry)
        {
            Logger.Trace("Operation: InsertEntry(ScheduledEvent)");
            return _underlyingDb.InsertEntry(entry);
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(ScheduledEvent newEntry, ScheduledEvent oldEntry)
        {
            Logger.Trace("Operation: UpdateEntry(ScheduledEvent)");
            return _underlyingDb.UpdateEntry(newEntry, oldEntry);
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public ScheduledEvent GetScheduledEventById(Guid id)
        {
            Logger.Trace("Operation: GetScheduledEventById");
            return _underlyingDb.GetScheduledEventById(id);
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(ScheduledEvent entry)
        {
            Logger.Trace("Operation: RemoveEntry(ScheduledEvent)");
            return _underlyingDb.DeleteEntry(entry);
        }

        ////////////////////////////// Category //////////////////////////////

        /// <summary>
        /// Iterator to travese categories.
        /// </summary>
        public FinancialCategoryIterator FinancialCategoryIterator
        {
            get
            {
                Logger.Trace("Operation: FinancialCategoryIterator");
                return _underlyingDb.FinancialCategoryIterator;
            }
        }

        /// <summary>
        /// Iterator to travese entries by partial match of category name. ("" = all)
        /// </summary>
        public FinancialCategoryIterator GetFinancialCategoryIterator(string startsWith)
        {
            Logger.Trace("Operation: GetFinancialCategoryIterator");
            return _underlyingDb.GetFinancialCategoryIterator(startsWith);
        }

        /// <summary>
        /// Add a new entry into the categories.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID will be updated</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(FinancialCategory entry)
        {
            Logger.Trace("Operation: InsertEntry(FinancialCategory)");
            return _underlyingDb.InsertEntry(entry);
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(FinancialCategory newEntry, FinancialCategory oldEntry)
        {
            Logger.Trace("Operation: UpdateEntry(FinancialCategory)");
            return _underlyingDb.UpdateEntry(newEntry, oldEntry);
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public FinancialCategory GetFinancialCategoryById(Guid id)
        {
            // Logger.Trace("Operation: GetFinancialCategoryById"); // commented-out due to avalanche!
            return _underlyingDb.GetFinancialCategoryById(id);
        }

        /// <summary>
        /// Get an entry based on the category name.
        /// </summary>
        /// <param name="id">The category name to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public FinancialCategory GetFinancialCategoryByName(string name)
        {
            Logger.Trace("Operation: GetFinancialCategoryByName");
            return _underlyingDb.GetFinancialCategoryByName(name);
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(FinancialCategory entry)
        {
            Logger.Trace("Operation: RemoveEntry(FinancialCategory)");
            return _underlyingDb.DeleteEntry(entry);
        }

        /////////////////////////// MemorizedPayee ///////////////////////////

        /// <summary>
        /// Iterator to travese Memorized Payees
        /// </summary>
        public MemorizedPayeeIterator MemorizedPayeeIterator
        {
            get
            {
                Logger.Trace("Operation: MemorizedPayeeIterator");
                return _underlyingDb.MemorizedPayeeIterator;
            }
        }

        /// <summary>
        /// Add a new entry into the Memorized Payees
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID may be updated</param>
        /// <returns>True if successful</returns>
        public bool InsertEntry(MemorizedPayee entry)
        {
            Logger.Trace("Operation: InsertEntry(MemorizedPayee)");
            return _underlyingDb.InsertEntry(entry);
        }

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        public bool UpdateEntry(MemorizedPayee newEntry, MemorizedPayee oldEntry)
        {
            // Logger.Trace("Operation: UpdateEntry(MemorizedPayee)"); // commented-out due to avalanche!
            return _underlyingDb.UpdateEntry(newEntry, oldEntry);
        }

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public MemorizedPayee GetMemorizedPayeeById(Guid id)
        {
            Logger.Trace("Operation: GetMemorizedPayeeById");
            return _underlyingDb.GetMemorizedPayeeById(id);
        }

        /// <summary>
        /// Get an entry based on the payee name.
        /// </summary>
        /// <param name="name">The full name of the payee to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public MemorizedPayee GetMemorizedPayeeByName(string name)
        {
            // Logger.Trace("Operation: GetMemorizedPayeeByName"); // commented-out due to avalanche!
            return _underlyingDb.GetMemorizedPayeeByName(name);
        }

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        public bool DeleteEntry(MemorizedPayee entry)
        {
            Logger.Trace("Operation: RemoveEntry(MemorizedPayee)");
            return _underlyingDb.DeleteEntry(entry);
        }

        //////////////////////// IUndoTracker methods ////////////////////////

        /// <summary>
        /// Mark the start of an Undo block, also celaring ant pending redo's.
        /// </summary>
        /// <param name="description">Very brief man-readable.</param>
        public void MarkUndoBlock(string description)
        {
            if(!_trackUndo)
            {
                return;
            }
            Logger.Info("Starting MarkUndoBlock");
            UndoableDbAccess.Mutex.WaitOne();
            try
            {
                if (_undoSteps.Count > 0 && _undoSteps.Last<UndoStep>().Kind == UndoStepKind.Marker)
                {
                    _undoSteps.RemoveAt(_undoSteps.Count - 1); // remove vacant step
                    _undoMarkerCount--;
                }
                _undoDescription = description;
                _undoSteps.Add(new UndoStep(UndoStepKind.Marker, description));
                _undoMarkerCount++;
            }
            catch (Exception ex)
            {
                throw new AppException("Error in MarkUndoBlock()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                UndoableDbAccess.Mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Call this to clean up after each operation, just in case it resulted in no DB changes.
        /// </summary>
        public void MarkUndoBlockEnd()
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Info("Starting MarkUndoBlockEnd");
            UndoableDbAccess.Mutex.WaitOne();
            try
            {
                if (_undoSteps.Count > 0 && _undoSteps.Last<UndoStep>().Kind == UndoStepKind.Marker)
                {
                    _undoSteps.RemoveAt(_undoSteps.Count - 1); // remove vacant step
                    _undoMarkerCount--;
                }
                _undoDescription = ""; // update description...
                for (int index = _undoSteps.Count - 1; index >= 0; --index)
                {
                    if (_undoSteps[index].Kind == UndoStepKind.Marker)
                    {
                        _undoDescription = ((string)_undoSteps[index].Record).ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AppException("Error in MarkUndoBlockEnd()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                UndoableDbAccess.Mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Track the deletion of an InProgrss change such as Reconciliation. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="inProgress">entry that was deleted</param>
        public void TrackDeletion(InProgress inProgress)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackDeletion(InProgress) " + inProgress);
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteInProgress, inProgress));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of a InProgrss change such as Reconciliation. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="inProgress">entry that was inserted</param>
        public void TrackInsertion(InProgress inProgress)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackInsertion(InProgress) " + inProgress);
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertInProgress, inProgress));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the deletion of reconciliation values. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="values">entry that was deleted</param>
        public void TrackDeletion(ReconciliationValues values)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackDeletion(ReconciliationValues) " + values.Balance);
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteReconciliation, values.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of reconciliation values. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="values">entry that was inserted</param>
        public void TrackInsertion(ReconciliationValues values)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackInsertion(ReconciliationValues) " + values.Balance);
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertReconciliation, values.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the deletion of a checkbook entry. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        public void TrackDeletion(CheckbookEntry entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackDeletion(CheckbookEntry) " + entry.Id);
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteCheckbookEntry, entry.Clone(false)));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of a new entry. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        public void TrackInsertion(CheckbookEntry entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            Logger.Trace("TrackInsertion(CheckbookEntry) " + entry.Id);
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertCheckbookEntry, entry.Clone(false)));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the deletion of a financial category. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        public void TrackDeletion(FinancialCategory entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteFinancialCategory, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of a new entry. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        public void TrackInsertion(FinancialCategory entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertFinancialCategory, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
            while(!Configuration.Instance.GetIsLicensedVersion() && _undoMarkerCount > 3)
            {
                ReleaseWeakData(1);
            }
        }

        /// <summary>
        /// Track the deletion of a scheduled event. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        public void TrackDeletion(ScheduledEvent entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteScheduledEvent, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of a new entry. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        public void TrackInsertion(ScheduledEvent entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertScheduledEvent, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the deletion of a Memorized Payee. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        public void TrackDeletion(MemorizedPayee entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.DeleteMemorizedPayee, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

        /// <summary>
        /// Track the insertion of a new entry. (caller: this is non-re-entrant)
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        public void TrackInsertion(MemorizedPayee entry)
        {
            if (!_trackUndo)
            {
                return;
            }
            _undoSteps.Add(new UndoStep(UndoStepKind.InsertMemorizedPayee, entry.Clone()));
            _redoSteps = new List<UndoStep>();
            _redoMarkerCount = 0;
            _redoDescription = "";
        }

    }

}
