using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbleCheckbook.Db
{

    public enum InProgress
    {
        Nothing = 0,       // Not in-progress
        Reconcile = 1,     // reconcile in-progress
    }

    public class JsonDbHeader
    {
        /// <summary>
        /// Name of the db (i.e. Checking, Personal, Business, Alternate).
        /// </summary>
        private string _dbName = "Checking";

        /// <summary>
        /// This gets incremented once for every compatibility-impacting change.
        /// </summary>
        private int _dbVersion = 0;

        /// <summary>
        /// When was it created?
        /// </summary>
        private DateTime _created = new DateTime();

        /// <summary>
        /// When was it last saved?
        /// </summary>
        private DateTime _lastSaved = new DateTime();

        /// <summary>
        /// Are we in the middle of something?
        /// </summary>
        private InProgress _inProgress = InProgress.Nothing;

        /// <summary>
        /// Date for use when in-progress.
        /// </summary>
        private DateTime _reconciliationDate = DateTime.Now;

        /// <summary>
        /// Amount for use when in-progress.
        /// </summary>
        private long _reconciliationBalance = 0L;

        /// <summary>
        /// In-memory store of checkbook entries.
        /// </summary>
        private SortedList<string, CheckbookEntry> _checkbookEntries = new SortedList<string, CheckbookEntry>();

        /// <summary>
        /// In-memory store of financial categories.
        /// </summary>
        private SortedList<string, FinancialCategory> _financialCategories = new SortedList<string, FinancialCategory>();

        /// <summary>
        /// In-memory store of scheduled events.
        /// </summary>
        private SortedList<string, ScheduledEvent> _scheduledEvents = new SortedList<string, ScheduledEvent>();

        /// <summary>
        /// In-memory store of memorized payees.
        /// </summary>
        private SortedList<string, MemorizedPayee> _memorizedPayees = new SortedList<string, MemorizedPayee>();

        // Getters/Setters for serialization
        public string DbName { get => _dbName; set => _dbName = value; }
        public int DbVersion { get => _dbVersion; set => _dbVersion = value; }
        public DateTime Created { get => _created; set => _created = value; }
        public DateTime LastSaved { get => _lastSaved; set => _lastSaved = value; }
        public InProgress InProgress { get => _inProgress; set => _inProgress = value; }
        public DateTime ReconciliationDate { get => _reconciliationDate; set => _reconciliationDate = value; }
        public long ReconciliationBalance { get => _reconciliationBalance; set => _reconciliationBalance = value; }
        public SortedList<string, CheckbookEntry> CheckbookEntries { get => _checkbookEntries; set => _checkbookEntries = value; }
        public SortedList<string, FinancialCategory> FinancialCategories { get => _financialCategories; set => _financialCategories = value; }
        public SortedList<string, ScheduledEvent> ScheduledEvents { get => _scheduledEvents; set => _scheduledEvents = value; }
        public SortedList<string, MemorizedPayee> MemorizedPayees { get => _memorizedPayees; set => _memorizedPayees = value; }
    }
}
