using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{

    /// <summary>
    /// These are the alert types that may appear to the right of the main menu
    /// </summary>
    public enum AlertType
    {
        None =           0,   // No alert
        Message =        1,   // Display _dueMessage
        YearEndWrapUp =  2,   // Do year-end wrap-up
        Reconcile =      3,   // Do reconciliation
        CheckToSee =     4,   // Check then display
    }

    /// <summary>
    /// Provides support for Main GUI - This is, essentially, the main app.
    /// </summary>
    public class CheckbookRegisterBackend : CheckbookRegisterView
    {

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="dataGridView">To be managed by this class</param>
        public CheckbookRegisterBackend(DataGridView dataGridView) : base(dataGridView)
        {
        }

        /// <summary>
        /// List of previous payee names.
        /// </summary>
        public List<string> Payees
        {
            get
            {
                return _autofillPayee.Payees();
            }
        }

        /// <summary>
        /// Lookup a payee by substirng
        /// </summary>
        /// <param name="name">starts with</param>
        /// <returns>best match or null</returns>
        public MemorizedPayee LookupPayeeBySubstring(string name)
        {
            List<MemorizedPayee> payees = _autofillPayee.LookUp(name);
            if(payees.Count > 0)
            {
                return payees[0];
            }
            return null;
        }

        /// <summary>
        /// List of potential categories.
        /// </summary>
        public List<string> Categories
        {
            get
            {
                List<string> categories = new List<string>();
                FinancialCategoryIterator iterator = _db.FinancialCategoryIterator;
                while (iterator.HasNextEntry())
                {
                    FinancialCategory category = iterator.GetNextEntry();
                    categories.Add(category.Name);
                }
                return categories;
            }
        }

        /// <summary>
        /// Copy the currently selected row to the clipboard.
        /// </summary>
        public void CopyToClipboard()
        {
            if (_dataGridView.SelectedRows.Count != 1)
            {
                return;
            }
            RowOfCheckbook rowCheckbook = (RowOfCheckbook)_dataGridView.SelectedRows[0].DataBoundItem;
            if(rowCheckbook == null)
            {
                return;
            }
            string rowString = rowCheckbook.DateOfTransaction.ToShortDateString() + ", " +
                rowCheckbook.CheckNumber + ", " +
                rowCheckbook.Payee + ", " +
                rowCheckbook.Category + ", " +
                rowCheckbook.Amount + ", " +
                rowCheckbook.IsCleared;
            Clipboard.SetData(DataFormats.Text, (Object)rowString);
        }

        /// <summary>
        /// Handle a click on a row of the datagridview.
        /// </summary>
        /// <param name="parent">Its parent form.</param>
        /// <param name="rowIndex">which row was clicked.</param>
        public void EditTransaction(Form parent, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _dataGridView.RowCount)
            {
                return;
            }
            _dataGridView.Rows[rowIndex].Selected = true;
            RowOfCheckbook rowCheckbook = (RowOfCheckbook)_dataGridView.Rows[rowIndex].DataBoundItem;
            if(rowCheckbook == null || (rowCheckbook.EntryBeforeEdit.IsCleared && !Configuration.Instance.GetAdminMode()))
            {
                return;
            }
            CheckbookEntryForm form = new CheckbookEntryForm(this, _dataGridView, rowIndex);
            Point location = _dataGridView.GetRowDisplayRectangle(rowIndex, true).Location;
            location.Offset(_dataGridView.Location);
            location.Offset(parent.Location);
            location.X = location.X + 20;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = location;
            CurrentEntryId = Guid.Empty;
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                if(form.UnmergeEntry)
                {
                    CheckbookEntry newEntry = form.rowCheckbook.Entry.Clone();
                    newEntry.DeleteSplits();
                    string catName = UtilityMethods.GuessAtCategory(newEntry.BankPayee);
                    Guid catId = UtilityMethods.GetCategoryOrUnknown(_db, catName).Id;
                    TransactionKind tranKind = (newEntry.BankAmount < 0 ? TransactionKind.Payment : TransactionKind.Deposit);
                    newEntry.AddSplit(catId, tranKind, Math.Abs(newEntry.BankAmount));
                    newEntry.CheckNumber = (newEntry.BankCheckNumber == 0 ? "" : "" + newEntry.BankCheckNumber);
                    newEntry.Payee = newEntry.BankPayee;
                    newEntry.DateOfTransaction = newEntry.BankTranDate;
                    newEntry.MadeBy = EntryMadeBy.Reconciler;
                    newEntry.IsCleared = form.rowCheckbook.Entry.IsCleared;
                    newEntry.IsChecked = form.rowCheckbook.Entry.IsChecked;
                    newEntry.ModifiedBy = System.Environment.UserName;
                    _db.InsertEntry(newEntry);
                    form.rowCheckbook.Entry.IsCleared = false;
                    form.rowCheckbook.Entry.IsChecked = false;
                    form.rowCheckbook.Entry.BankAmount = 0;
                    form.rowCheckbook.Entry.BankPayee = "";
                    form.rowCheckbook.Entry.BankCheckNumber = 0;
                    form.rowCheckbook.Entry.BankTransaction = "";
                    form.rowCheckbook.Entry.BankMergeAccepted = false;
                    CurrentEntryId = newEntry.Id;
                }
                if (form.DeleteEntry)
                {
                    DeleteEntry(form, parent);
                    if (rowIndex > 1)
                    {   // set up to make the previous entry the active one
                        CurrentEntryId = ((RowOfCheckbook)_dataGridView.Rows[_dataGridView.SelectedRows[0].Index - 1].DataBoundItem).Id;
                    }
                }
                else
                {
                    SaveEntry(form);
                    if (form.rowCheckbook != null && form.rowCheckbook.Entry != null)
                    {
                        CurrentEntryId = form.rowCheckbook.Entry.Id;
                    }
                }
            }
        }

        /// <summary>
        /// Create a new entry in the checkbook.
        /// </summary>
        /// <param name="parent">Parent form - i.e. MainForm</param>
        public void NewEntry(Form parent)
        {
            foreach (DataGridViewRow newEntryRow in _dataGridView.Rows)
            {
                RowOfCheckbook rowEntry = (RowOfCheckbook)newEntryRow.DataBoundItem;
                if (rowEntry.NewEntryRow)
                {
                    DataGridViewSelectedRowCollection rows = _dataGridView.SelectedRows;
                    foreach (DataGridViewRow row in rows)
                    {
                        row.Selected = false;
                    }
                    newEntryRow.Selected = true;
                    EditTransaction(parent, newEntryRow.Index);
                    break;
                }
            }
        }

        /// <summary>
        /// Tentatively clear (in ReconciliationHelper) any checked entries.
        /// </summary>
        /// <param name="reconHelper">Source of checked-off entries</param>
        public void UpdateCheckedEntries(ReconciliationHelper reconHelper)
        {
            foreach (RowOfCheckbook rowEntry in _transactions)
            {
                if(rowEntry.NewEntryRow)
                {
                    continue;
                }
                if (rowEntry.IsChecked)
                {
                    reconHelper.CheckIt(rowEntry.EntryBeforeEdit.Id);
                }
                else
                {
                    reconHelper.UnCheckIt(rowEntry.EntryBeforeEdit.Id);
                }
            }
        }

        /// <summary>
        /// Update DB with any checked-off reconciliation entries.
        /// </summary>
        /// <param name="reconHelper">Source of checked-off entries</param>
        public void CommitCheckedEntries(ReconciliationHelper reconHelper)
        {
            foreach (RowOfCheckbook rowEntry in _transactions)
            {
                if (rowEntry.NewEntryRow || rowEntry.EntryBeforeEdit.IsCleared)
                {
                    continue;
                }
                rowEntry.Entry.IsCleared = rowEntry.IsChecked;
                rowEntry.Entry.IsChecked = false;
                rowEntry.Entry.DateCleared = DateTime.Now;
                _db.UpdateEntry(rowEntry.Entry, rowEntry.EntryBeforeEdit, true);
            }
        }

        /// <summary>
        /// Uncheck any checked entries.
        /// </summary>
        public void UncheckAll()
        {
            foreach(RowOfCheckbook rowEntry in _transactions)
            { 
                if (rowEntry.Entry.IsChecked)
                {
                    rowEntry.Entry.IsChecked = false;
                    _db.UpdateEntry(rowEntry.Entry, rowEntry.EntryBeforeEdit, true);
                }
            }
        }

        /// <summary>
        /// Chat kind of periodic alert should be shown to the user?
        /// </summary>
        public AlertType PeriodicAlertType
        {
            get
            {
                AlertType alertType = AlertType.None;
                try
                {
                    _mutex.WaitOne();
                    if (!Configuration.Instance.SuppressReconcileAlert)
                    {
                        ReconciliationHelper reconHelper = new ReconciliationHelper(_db.UnderlyingDb);
                        if (reconHelper.IsTimeToReconcile)
                        {
                            alertType = AlertType.Reconcile;
                        }
                    }
                    if (alertType == AlertType.None && !Configuration.Instance.SuppressYearEndAlert)
                    {
                        YearEndWrapup yearEnd = new YearEndWrapup(_db.UnderlyingDb);
                        if (yearEnd.IsTimeToWrapUp)
                        {
                            alertType = AlertType.YearEndWrapUp;
                        }
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
                return alertType;
            }
        }

        /// <summary>
        /// Rename a payee, updating all related records as well.
        /// </summary>
        /// <param name="oldName">The current payee</param>
        /// <param name="newName">The desired replacement name</param>
        /// <returns>Number of changes made to checkbook entry records in db</returns>
        public int RenamePayee(string oldName, string newName)
        {
            int numChanges = 0;
            try
            {
                _mutex.WaitOne();
                oldName = oldName.Trim().ToUpper();
                newName = UtilityMethods.UberCaps(newName);
                // Rename occurences in CheckbookEntries...
                List<CheckbookEntry> entries = new List<CheckbookEntry>();
                CheckbookEntryIterator iterator1 = _db.CheckbookEntryIterator;
                while (iterator1.HasNextEntry())
                {
                    CheckbookEntry entry = iterator1.GetNextEntry();
                    if (entry.Payee.Trim().ToUpper().Equals(oldName))
                    {
                        entries.Add(entry);
                    }
                }
                foreach (CheckbookEntry oldEntry in entries)
                {
                    ++numChanges;
                    CheckbookEntry newEntry = oldEntry.Clone(false);
                    newEntry.Payee = newName;
                    _db.UpdateEntry(newEntry, oldEntry, true);
                    newEntry.MadeBy = EntryMadeBy.Process;
                }
                // Rename occurences in ScheduledEvents...
                List<ScheduledEvent> schEvents = new List<ScheduledEvent>();
                ScheduledEventIterator iterator2 = _db.ScheduledEventIterator;
                while (iterator2.HasNextEntry())
                {
                    ScheduledEvent schEvent = iterator2.GetNextEntry();
                    if (schEvent.Payee.Trim().ToUpper().Equals(oldName))
                    {
                        schEvents.Add(schEvent);
                    }
                }
                foreach (ScheduledEvent oldEvent in schEvents)
                {
                    ScheduledEvent newEvent = oldEvent.Clone();
                    newEvent.Payee = newName;
                    _db.UpdateEntry(newEvent, oldEvent);
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return numChanges;
        }

        /// <summary>
        /// Adjust the data grid view by changing critical columns' width to suit a resized area.
        /// </summary>
        public void AdjustWidths()
        {
            _layout.AdjustWidths(_dataGridView);
        }

        /// <summary>
        /// Open a new or existing DB file.
        /// </summary>
        public void OpenOrNewDb()
        {
            _db.BackupAndSave();
            OpenFileDialog form = new OpenFileDialog();
            form.InitialDirectory = Configuration.Instance.DirectoryDatabase;
            form.AddExtension = true;
            form.CheckFileExists = true;
            form.CheckPathExists = true;
            form.DefaultExt = ".acb";
            form.DereferenceLinks = true;
            form.Filter = Strings.Get("database files (*.acb)|*.acb");
            form.Multiselect = false;
            form.RestoreDirectory = true;
            form.Title = Strings.Get("Open DB");
            DialogResult result = form.ShowDialog();
            if (result != DialogResult.OK || form.FileName.Trim().Length < 1)
            {
                return;
            }
            string filename = form.FileName.Trim();
            if (!File.Exists(filename))
            {
                return;
            }
            OpenDb(filename);
        }

        /// <summary>
        /// Save the user-entered or user-edited entry per the checkbook entry form.
        /// </summary>
        /// <param name="form">Source of user-input data</param>
        public void SaveEntry(CheckbookEntryForm form)
        {
            CheckbookEntry entry = form.rowCheckbook.Entry;
            if (form.IsNewEntry)
            {
                entry = new CheckbookEntry();
            }
            entry.DateOfTransaction = form.DateOfTransaction;
            entry.Payee = form.Payee;
            entry.CheckNumber = form.CheckNumber;
            entry.ResetMemo();
            entry.Memo = form.Memo;
            entry.MadeBy = form.rowCheckbook.Entry.MadeBy;
            entry.SetHighlight(form.rowCheckbook.Entry.Highlight);
            if ((entry.IsCleared != form.Cleared) && Configuration.Instance.GetAdminMode() && _db.InProgress == InProgress.Nothing)
            {
                entry.IsCleared = form.Cleared;
                entry.DateCleared = DateTime.Now;
            }
            entry.DeleteSplits();
            for (int splitNbr = 0; splitNbr < form.CategoryCount; ++splitNbr)
            {
                Guid categoryId = UtilityMethods.GetOrCreateCategory(_db,
                    form.Category(splitNbr), form.Amount(splitNbr) >= 0).Id;
                entry.AddSplit(categoryId, (TransactionKind)form.Kind(splitNbr), form.Amount(splitNbr));
            }
            try
            {
                _mutex.WaitOne();
                _db.UpdateEntry(entry, form.rowCheckbook.EntryBeforeEdit, true); // Undoable, so must use this method
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            int rowNum = AddTransaction(entry, false, form.IsNewEntry);
            _entriesChanged = true;
            _autofillPayee.UpdateFromCheckbookEntry(entry);
            ReloadTransactions();
        }

        public void DeleteTransaction(RowOfCheckbook rowToDelete)
        {
            for (int index = 0; index < _transactions.Count; ++index)
            {
                RowOfCheckbook testEntry = _transactions[index];
                if (testEntry.Entry.Id == rowToDelete.EntryBeforeEdit.Id)
                {
                    _entriesChanged = true;
                    _transactions.RemoveAt(index);
                    break;
                }
            }
        }

        /// <summary>
        /// Delete the selected entry.
        /// </summary>
        /// <param name="form">Source of user-input data</param>
        /// <param name="mainScreen">Form to display "DELETING..." message in.</param>
        public void DeleteEntry(CheckbookEntryForm form, Form mainScreen)
        {
            CheckbookEntry entry = form.rowCheckbook.EntryBeforeEdit;
            if (_dataGridView.SelectedRows.Count != 1)
            {
                return;
            }
            RowOfCheckbook rowCheckbook = (RowOfCheckbook)_dataGridView.SelectedRows[0].DataBoundItem;
            if (rowCheckbook == null || rowCheckbook.EntryBeforeEdit.Id != form.rowCheckbook.EntryBeforeEdit.Id)
            {
                return;
            }
            try
            {
                _mutex.WaitOne();
                _db.DeleteEntry(entry);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            DeleteTransaction(rowCheckbook);
            CurrentEntryId = Guid.Empty;
            ReloadTransactions();
        }

        /// <summary>
        /// Year-end wrap-up.
        /// </summary>
        /// <returns>name of new (current year) file, i.e. "Checking-2023, "" on error</returns>
        public string PerformYearEndWrapUp(Form win)
        {
            if(_db.InProgress != InProgress.Nothing)
            {
                return "";
            }
            YearEndWrapup yearEndWrapUp = new YearEndWrapup(_db);
            if (_db.InProgress != InProgress.Nothing || !yearEndWrapUp.IsTimeToWrapUp)
            {
                MessageBox.Show(win, Strings.Get("Many old entries not yet been cleared, or current year acct already exists."),
                    Strings.Get("Not Yet"), MessageBoxButtons.OK);
                return "";
            }
            bool ok = PerformYearEndWrapUp(yearEndWrapUp, win);
            if (!ok && Configuration.Instance.GetAdminMode())
            {
                if (MessageBox.Show(win, Strings.Get("Admin Mode - Force Year-End Wrap-Up?"),
                    Strings.Get("Try Again?"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    ok = PerformYearEndWrapUp(yearEndWrapUp, win);
                }
            }
            if(ok)
            {
                return _db.Name;
            }
            return "";
        }

        private bool PerformYearEndWrapUp(YearEndWrapup yearEndWrapUp, Form win)
        {
            string windowTitle = win.Text;
            win.Text = Strings.Get("Year-End Wrap-Up, Please Wait . . .");
            if (yearEndWrapUp.SplitDbsAtDec31(true))
            {
                ReloadTransactions(SortEntriesBy.TranDate);
                return true;
            }
            else
            {
                MessageBox.Show(win, Strings.GetIff(yearEndWrapUp.Message), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return false;
            }
        }

        /// <summary>
        /// Prompt the user for, and import, a CSV file.
        /// </summary>
        public void ImportCsv(IWin32Window win)
        {
            OpenFileDialog form = new OpenFileDialog();
            form.AddExtension = true;
            form.CheckFileExists = true;
            form.CheckPathExists = true;
            form.DefaultExt = "csv";
            form.DereferenceLinks = true;
            form.Filter = "CSV/Excel|*.csv|All Files|*.*";
            form.InitialDirectory = Configuration.Instance.DirectoryImportExport;
            form.Multiselect = false;
            form.RestoreDirectory = true;
            form.Title = Strings.Get("Import CSV");
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filepath = form.FileName.Trim();
                if (filepath.Length > 0)
                {
                    CsvImporter importer = new CsvImporter(_db);
                    int count = importer.Import(filepath);
                    string message = null;
                    if (count < 1)
                    {
                        message = "Note: No Importable Entries Read";
                    }
                    if (importer.ErrorOccurred)
                    {
                        message = importer.ErrorMessage;
                    }
                    if (message != null)
                    {
                        MessageBox.Show(win, Strings.GetIff(message), Strings.Get("Import CSV"), MessageBoxButtons.OK);
                    }
                }
            }
        }

        /// <summary>
        /// Prompt the user for, and import, a QIF file.
        /// </summary>
        public void ImportQif(IWin32Window win)
        {
            OpenFileDialog form = new OpenFileDialog();
            form.AddExtension = true;
            form.CheckFileExists = true;
            form.CheckPathExists = true;
            form.DefaultExt = "qif";
            form.DereferenceLinks = true;
            form.Filter = "QIF/Quicken|*.qif|All Files|*.*";
            form.InitialDirectory = Configuration.Instance.DirectoryImportExport;
            form.Multiselect = false;
            form.RestoreDirectory = true;
            form.Title = "Import QIF";
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filepath = form.FileName.Trim();
                if (filepath.Length > 0)
                {
                    QifImporter importer = new QifImporter(_db);
                    int count = importer.Import(filepath);
                    string message = null;
                    if (count < 1)
                    {
                        message = "Note: No Importable Entries Read";
                    }
                    if (importer.ErrorMessage.Trim().Length > 0)
                    {
                        message = importer.ErrorMessage;
                    }
                    if (message != null)
                    {
                        MessageBox.Show(win, Strings.GetIff(message), Strings.Get("Import QIF"), MessageBoxButtons.OK);
                    }
                }
            }
        }

        /// <summary>
        /// Prompt the user for, and export the entries, to a CSV file.
        /// </summary>
        public void ExportCsv(IWin32Window win)
        {
            SaveFileDialog form = new SaveFileDialog();
            form.AddExtension = true;
            form.FileName = _db.Name + ".csv";
            form.DefaultExt = "csv";
            form.DereferenceLinks = true;
            form.Filter = "CSV/Excel|*.csv|All Files|*.*";
            form.InitialDirectory = Configuration.Instance.DirectoryImportExport;
            form.OverwritePrompt = true;
            form.RestoreDirectory = true;
            form.Title = "Export CSV";
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filepath = form.FileName.Trim();
                if (filepath.Length > 0)
                {
                    CsvExporter exporter = new CsvExporter(_db);
                    bool ok = exporter.Export(filepath);
                    string message = "Successful";
                    if (!ok)
                    {
                        message = exporter.ErrorMessage;
                    }
                    MessageBox.Show(win, Strings.GetIff(message), Strings.Get("Export CSV"), MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Prompt the user for, and export the entries, to a QIF file.
        /// </summary>
        public void ExportQif(IWin32Window win)
        {
            SaveFileDialog form = new SaveFileDialog();
            form.AddExtension = true;
            form.DefaultExt = "qif";
            form.FileName = _db.Name + ".qif";
            form.DereferenceLinks = true;
            form.Filter = "QIF/Quicken|*.qif|All Files|*.*";
            form.InitialDirectory = Configuration.Instance.DirectoryImportExport;
            form.OverwritePrompt = true;
            form.RestoreDirectory = true;
            form.Title = "Export QIF";
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filepath = form.FileName.Trim();
                if (filepath.Length > 0)
                {
                    QifExporter exporter = new QifExporter(_db);
                    bool ok = exporter.Export(filepath);
                    string message = "Successful";
                    if (!ok)
                    {
                        message = exporter.ErrorMessage;
                    }
                    MessageBox.Show(win, Strings.GetIff(message), Strings.Get("Export QIF"), MessageBoxButtons.OK);
                }
            }
        }

    }

}
