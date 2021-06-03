using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{

    /// <summary>
    /// Base class for displaying a data grid view of the checkbook register.
    /// </summary>
    public class CheckbookRegisterView
    {
        /// <summary>
        /// Source/sink of data.
        /// </summary>
        protected UndoableDbAccess _db = null;

        /// <summary>
        /// List of Displayable Transactions that constitute the rows of the datagridview.
        /// </summary>
        protected List<RowOfCheckbook> _transactions = null;

        /// <summary>
        /// View into checkbook register.
        /// </summary>
        protected DataGridView _dataGridView = null;

        /// <summary>
        /// For laying out the checkbook register as a data grid view.
        /// </summary>
        protected DataGridViewLayout _layout = null;

        /// <summary>
        /// How are entries sorted?
        /// </summary>
        protected SortEntriesBy _sortedBy = SortEntriesBy.TranDate;

        /// <summary>
        /// Search result matches.
        /// </summary>
        protected List<Guid> _matches = new List<Guid>();

        /// <summary>
        /// How many search result matches have been appended? 
        /// </summary>
        protected int _matchCount = 0;

        /// <summary>
        /// Use this to prevent conflicts with the BgWorkerThread.
        /// </summary>
        protected Mutex _mutex = new Mutex();

        /// <summary>
        /// List splits in register view?
        /// </summary>
        private bool _itemizedSplits = false;

        /// <summary>
        /// Use this to autofill entries.
        /// </summary>
        protected AutofillPayee _autofillPayee = null;

        /// <summary>
        /// Flag to rebuild RowsOfCheckbook before repopulating _transactions.
        /// </summary>
        protected bool _entriesChanged = false; // currently works, but unused

        /// <summary>
        /// Diagnostics?
        /// </summary>
        protected bool _diagsEnabled = false;

        /// <summary>
        /// Keeps track of last entry created/modified for scrolling purposes. Guid.Empty=reset
        /// </summary>
        protected Guid _currentEntryId = Guid.Empty;

        // Getters/setters 
        public bool DiagsEnabled { get => _diagsEnabled; set => _diagsEnabled = value; }
        public Guid CurrentEntryId { get => _currentEntryId; set => _currentEntryId = value; }
        public bool ItemizedSplits { get => _itemizedSplits; set => _itemizedSplits = value; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="dataGridView">To be managed by this class</param>
        protected CheckbookRegisterView(DataGridView dataGridView)
        {
            _dataGridView = dataGridView;
            string dbFilename = Configuration.Instance.LastDbName;
            if (dbFilename == null || dbFilename.Length < 1)
            {
                dbFilename = "Checking";
            }
            _db = new UndoableDbAccess(dbFilename);
            Configuration.Instance.LastDbName = dbFilename;
            _layout = new DataGridViewLayout();
            _autofillPayee = new AutofillPayee(_db);
        }

        /// <summary>
        /// Open a different (possibly new) database.
        /// </summary>
        /// <param name="dbFilename">account file name with no path, year, or extension</param>
        public void OpenDb(string dbFilename)
        {
            _db = new UndoableDbAccess(dbFilename);
            Configuration.Instance.LastDbName = dbFilename;
            _layout = new DataGridViewLayout();
            _autofillPayee = new AutofillPayee(_db);
        }

        /// <summary>
        /// How are the entries currently sorted?
        /// </summary>
        public SortEntriesBy SortedBy
        {
            get
            {
                return _sortedBy;
            }
        }

        /// <summary>
        /// Lay out the data grid view.
        /// </summary>
        public void LayoutDataGridView()
        {
            _layout.LayoutColumns(_dataGridView, _diagsEnabled, _db.InProgress == InProgress.Reconcile);
            foreach(DataGridViewRow gridRow in _dataGridView.Rows)
            {
                RowOfCheckbook entryRow = gridRow.DataBoundItem as RowOfCheckbook;
                if(_itemizedSplits && entryRow != null && entryRow.Entry.Splits.Length > 1)
                {
                    int hgt = (gridRow.InheritedStyle.Font.Height + 2) * entryRow.Entry.Splits.Length + 5;
                    gridRow.Height = hgt;
                }
            }
        }

        /// <summary>
        /// Expose the DB.
        /// </summary>
        public UndoableDbAccess Db
        {
            get
            {
                return _db;
            }
        }

        /// <summary>
        /// Remove a match from the matchlist of search/checked entries. (innocuous if it is not present)
        /// </summary>
        /// <param name="guid">to be removed</param>
        public void DeleteMatch(Guid guid)
        {
            _matches.Remove(guid);
        }

        /// <summary>
        /// Add a match to the matchlist for search or checked entries. (innocuous if it is already present)
        /// </summary>
        /// <param name="guid">to be added</param>
        public void AddMatch(Guid guid)
        {
            _matches.Remove(guid);
            _matches.Add(guid);
        }

        /// <summary>
        /// Update the datagridview.
        /// </summary>
        /// <param name="sortedBy">How to sort</param>
        /// <param name="matches">Guids of search matches (for SearchResults only)</param>
        /// <returns>success</returns>
        public bool ReloadTransactions(SortEntriesBy sortedBy = SortEntriesBy.NoChange, List<Guid> matches = null)
        {
            if (sortedBy != SortEntriesBy.NoChange && sortedBy != _sortedBy) // sort changed?
            {
                CurrentEntryId = Guid.Empty;
            }
            Guid guid = Guid.Empty;
            try
            {
                if (_dataGridView.CurrentRow != null && _dataGridView.CurrentRow.DataBoundItem != null) {; }
            }
            catch (Exception)
            {
                _dataGridView.CurrentCell = _dataGridView.Rows[0].Cells[3]; // .NET workaroound
            }
            if (_dataGridView.CurrentRow != null && _dataGridView.CurrentRow.DataBoundItem != null)
            {
                guid = ((RowOfCheckbook)_dataGridView.CurrentRow.DataBoundItem).Entry.Id;
            }
            _layout.LayoutColumns(_dataGridView, _diagsEnabled, _db.InProgress == InProgress.Reconcile);
            _layout.AdjustWidths(_dataGridView);
            bool placedNewEntryRow = false;
            if (sortedBy != SortEntriesBy.NoChange)
            {
                _sortedBy = sortedBy;
            }
            if (matches != null)
            {
                _matches = matches;
            }
            _transactions = new List<RowOfCheckbook>();
            _db.AdjustBalances();
            List<CheckbookEntry> entries = new CheckbookSorter().GetSortedEntries(_db, _sortedBy, _matches);
            try
            {
                _matchCount = 0;
                _mutex.WaitOne();
                foreach (CheckbookEntry entry in entries)
                {
                    if (!placedNewEntryRow && ShouldInsertNewEntryRow(entry))
                    {
                        InsertNewEntryRow();
                        placedNewEntryRow = true;
                    }
                    AddTransaction(entry, true, true);
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            if (!placedNewEntryRow)
            {
                InsertNewEntryRow();
            }
            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = _transactions;
            _dataGridView.DataSource = bindingSource1;
            LayoutDataGridView();
            ScrollToActiveRow();
            _entriesChanged = false;
            return true;
        }

        /// <summary>
        /// Scroll the datagridview to the cu location else to NewEntryRow.
        /// </summary>
        public void ScrollToActiveRow()
        {
            if (_dataGridView == null || _dataGridView.DisplayedRowCount(true) < 2 || _dataGridView.DisplayedColumnCount(true) < 2)
            {
                return;
            }
            if (_dataGridView.Parent != null)
            {
                _dataGridView.Parent.BringToFront();
                _dataGridView.Parent.Focus();
            }
            _dataGridView.BringToFront();
            _dataGridView.Focus();
            _dataGridView.Capture = false;
            int columnNbr = 1; // actually, any "visible" column
            while(columnNbr < _dataGridView.Columns.Count-1 && !_dataGridView.Columns[columnNbr].Visible)
            {
                ++columnNbr;
            }
            DataGridViewCell cell = null;
            cell = _dataGridView.Rows[1].Cells[columnNbr];
            int index = 999999;
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                RowOfCheckbook rowEntry = (RowOfCheckbook)row.DataBoundItem;
                if (CurrentEntryId == Guid.Empty)
                {
                    if (rowEntry.NewEntryRow)
                    {
                        index = row.Index;
                        cell = _dataGridView.Rows[row.Index].Cells[columnNbr];
                        break;
                    }
                }
                else
                {
                    if (rowEntry.EntryBeforeEdit.Id == CurrentEntryId || rowEntry.Entry.Id == CurrentEntryId)
                    {
                        index = row.Index;
                        cell = _dataGridView.Rows[row.Index].Cells[columnNbr];
                        break;
                    }
                }
            }
            _dataGridView.CurrentCell = cell;
            if (index == 999999 && CurrentEntryId != Guid.Empty)
            {
                foreach (DataGridViewRow row in _dataGridView.Rows)
                {
                    if (((RowOfCheckbook)row.DataBoundItem).Id == CurrentEntryId)
                    {
                        cell = row.Cells[columnNbr];
                    }
                }
                // Finally, scroll to the NewEntryRow
                _dataGridView.CurrentCell = cell;
            }
        }

        /// <summary>
        /// When should be insert the NewEntryRow entry?
        /// </summary>
        /// <param name="entry">the current entry as we move forward through them after sorting</param>
        /// <returns>false prior to the insertion point, true at or after the insertion point</returns>
        private bool ShouldInsertNewEntryRow(CheckbookEntry entry)
        {
            switch (_sortedBy)
            {
                case SortEntriesBy.SearchResults:  // placed at end of matches
                case SortEntriesBy.CheckBox:
                    return ++_matchCount > _matches.Count;
                case SortEntriesBy.CheckNumber:    // placed at end of those with numbers
                    return entry.CheckNumber.Trim().Length < 1;
                case SortEntriesBy.TranDate:       // placed by date
                    return (entry.DateOfTransaction.Date.CompareTo(DateTime.Now.Date) > 0);
                case SortEntriesBy.Category:       // placed at end
                case SortEntriesBy.Payee:
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Add a row that is a clickable NewEntryRow to insert a "new entry" into the checkbook.
        /// </summary>
        private void InsertNewEntryRow()
        {
            CheckbookEntry newEntry = new CheckbookEntry();
            newEntry.Payee = Strings.Get("Click to Insert");
            RowOfCheckbook newTrans = new RowOfCheckbook(_db, newEntry, Strings.Get("New Entry"), true);
            _transactions.Add(newTrans);
            if (CurrentEntryId == Guid.Empty)
            {
                CurrentEntryId = newTrans.Id;
                Logger.Info("NewEntryRow.Id=" + CurrentEntryId);
            }
        }

        /// <summary>
        /// Add a checkbook entry to _transactions
        /// </summary>
        /// <param name="entry">To be added to _transactions</param>
        /// <param name="append">true to append, false to insert by transaction date</param>
        /// <param name="isNewEntry">true of this is a new entry</param>
        /// <returns>Row number - _transactions element number</returns>
        protected int AddTransaction(CheckbookEntry entry, bool append, bool isNewEntry)
        {
            string categoryName = Strings.Get("(Split)");
            if (entry.Splits.Length == 1)
            {
                FinancialCategory category = _db.GetFinancialCategoryById(entry.Splits[0].CategoryId);
                if (category == null)
                {
                    category = UtilityMethods.GetCategoryOrUnknown(_db, "Unknown");
                }
                categoryName = category.Name;
            }
            RowOfCheckbook trans = new RowOfCheckbook(_db, entry, categoryName);
            int index = 0;
            if (append)
            {
                _transactions.Add(trans);
                index = _transactions.Count;
            }
            else if (isNewEntry)
            {
                for (; index < _transactions.Count; ++index)
                {
                    RowOfCheckbook testEntry = _transactions[index];
                    if (trans.DateOfTransaction.Date.CompareTo(testEntry.DateOfTransaction.Date) < 0)
                    {
                        break;
                    }
                }
                _transactions.Insert(index, trans);
            }
            else
            {
                for (; index < _transactions.Count; ++index)
                {
                    RowOfCheckbook testEntry = _transactions[index];
                    if (testEntry.Entry.Id == entry.Id)
                    {
                        trans = (RowOfCheckbook)_dataGridView.Rows[index].DataBoundItem;
                        trans.Entry = entry;
                        trans.Category = categoryName;
                    }
                }
            }
            return index;
        }
    }
}
