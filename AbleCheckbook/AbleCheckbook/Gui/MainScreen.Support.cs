using AbleCheckbook.Db;
using AbleCheckbook.Gui;
using AbleCheckbook.Logic;
using AbleLicensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace AbleCheckbook
{

    /// <summary>
    /// Main GUI Support Methods.
    /// </summary>
    partial class MainScreen
    {

        /// <summary>
        /// If the user is idle this long, check to see if we need to auto save the DB.
        /// </summary>
        public const int IdleSeconds = 20;

        /// <summary>
        /// How often should we check fo schedule updates?
        /// </summary>
        public static int ScheduleUpdateCheckMinutes = 5;

        /// <summary>
        /// The client backend here has significant support functionality.
        /// </summary>
        private CheckbookRegisterBackend _backend = null;

        /// <summary>
        /// Indicates that the screen is being shown for the first time since it was launched.
        /// </summary>
        private bool _firstShown = false;

        /// <summary>
        /// Indicates the type of alert that gets shown to the right of the main menu.
        /// </summary>
        private AlertType _alertType = AlertType.None;

        /// <summary>
        /// Currently in an undo or redo?
        /// </summary>
        private bool _inUndoRedo = false;

        /// <summary>
        /// [static] to prevent recursive event handling
        /// </summary>
        private static int _recursionLevel = 0;

        /// <summary>
        /// To be displayed in DueNotice.
        /// </summary>
        private string _dueMessage = "";

        /// <summary>
        /// When did we last update the schedule?
        /// </summary>
        private DateTime _whenLastScheduleUpdate = DateTime.Now.AddMinutes(-ScheduleUpdateCheckMinutes);

        /// <summary>
        /// The HELP window.
        /// </summary>
        private BrowserForm _helpForm = null;

        /// <summary>
        /// When did we last see user activity?
        /// </summary>
        private static System.Timers.Timer _activityTimer = null;

        /// <summary>
        /// this!
        /// </summary>
        private static MainScreen _mainScreen = null;

        /// <summary>
        /// Are we in the middle of a command?
        /// </summary>
        private bool _busy = false;

        /// <summary>
        /// Getters
        /// </summary>
        public CheckbookRegisterBackend Backend { get => _backend; }

        /// <summary>
        /// Load, set up menu, toolbar, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainScreen_Load(object sender, EventArgs e)
        {
            _mainScreen = this;
            superToolStripMenuItem.Visible = superToolStripMenuItem.Enabled = false;
#if DEBUG
#if SUPERUSER
            superToolStripMenuItem.Visible = superToolStripMenuItem.Enabled = (Configuration.Instance.GetUserLevel() == UserLevel.SuperUser);
#endif
#endif
            _backend = new CheckbookRegisterBackend(dataGridView1);
            this.Text = Strings.Get("Able Strategies AbleCheckbook") + " - " + _backend.Db.Name;
            int left, top, width, height;
            Configuration.Instance.GetWindowBounds(out left, out top, out width, out height);
            Screen screen = Screen.FromRectangle(new Rectangle(left, top, width, height));
            this.Left = Math.Max(screen.Bounds.Left, left);
            this.Top = Math.Max(screen.Bounds.Top, top);
            this.Width = Math.Max(620, Math.Min(screen.Bounds.Width - this.Left, width));
            this.Height = Math.Max(320, Math.Min(screen.Bounds.Height - this.Top, height));
            newAcctToolStripMenuItem.Text = Strings.Get("&New Acct");
            openAcctToolStripMenuItem.Text = Strings.Get("&Open Acct");
            saveAcctToolStripMenuItem.Text = Strings.Get("&Save Acct");
            openBackupToolStripMenuItem.Text = Strings.Get("Open &Backup File");
            yearEndToolStripMenuItem.Text = Strings.Get("&Year-End Wrap-Up");
            printRegisterToolStripMenuItem1.Text = Strings.Get("&Print Register");
            exitToolStripMenuItem.Text = Strings.Get("E&xit");
            undoToolStripMenuItem.Text = Strings.Get("&Undo");
            redoToolStripMenuItem.Text = Strings.Get("&Redo");
            copyToolStripMenuItem.Text = Strings.Get("&Copy Entry");
            newEntryToolStripMenuItem.Text = Strings.Get("&New Entry");
            deleteEntryToolStripMenuItem.Text = Strings.Get("&Delete Entry");
            renamePayeeToolStripMenuItem.Text = Strings.Get("Rename &Payee");
            searchToolStripMenuItem.Text = Strings.Get("&Search Entries");
            byDateToolStripMenuItem.Text = Strings.Get("Sort by &Date");
            byPayeeToolStripMenuItem.Text = Strings.Get("Sort by &Payee");
            byCategoryToolStripMenuItem.Text = Strings.Get("Sort by &Category");
            byCheckNumberToolStripMenuItem.Text = Strings.Get("Sort by Check &Number");
            byMatchToolStripMenuItem.Text = Strings.Get("Sort by Match");
            byReconcileToolStripMenuItem.Text = Strings.Get("Sort by Reconcile");
            itemizeSplitsToolStripMenuItem.Text = Strings.Get("&Itemize Splits");
            scheduledTransactionsToolStripMenuItem.Text = Strings.Get("&Scheduled Events");
            reconcileToolStripMenuItem.Text = Strings.Get("&Reconcile (Monthly)");
            categoryReportToolStripMenuItem1.Text = Strings.Get("C&ategory Report");
            diagnosticsToolStripMenuItem.Text = Strings.Get("&Diagnostics");
            aboutToolStripMenuItem.Text = Strings.Get("&About");
            helpContentsToolStripMenuItem.Text = Strings.Get("&Help Contents");
            preferencesToolStripMenuItem.Text = Strings.Get("&Preferences");
            activateStripMenuItem.Text = Strings.Get("Activate &License");
            toolStripButtonSave.ToolTipText = toolStripButtonHelp.Text = Strings.Get("Save");
            toolStripButtonCategoryReport.ToolTipText = toolStripButtonCategoryReport.Text = Strings.Get("Category Report");
            toolStripButtonPrintRegister.ToolTipText = toolStripButtonPrintRegister.Text = Strings.Get("Print Register");
            toolStripButtonUndo.ToolTipText = toolStripButtonUndo.Text = Strings.Get("Undo");
            toolStripButtonRedo.ToolTipText = toolStripButtonRedo.Text = Strings.Get("Redo");
            toolStripButtonCopy.ToolTipText = toolStripButtonCopy.Text = Strings.Get("Copy Entry");
            toolStripButtonNewEntry.ToolTipText = toolStripButtonCopy.Text = Strings.Get("New Entry");
            toolStripButtonDeleteEntry.ToolTipText = toolStripButtonCopy.Text = Strings.Get("Delete Entry");
            toolStripTextBoxSearchForPayee.ToolTipText = Strings.Get("Search for Payee");
            toolStripButtonSearchMemo.ToolTipText = toolStripButtonSearchMemo.Text = Strings.Get("Search Memos");
            toolStripButtonScheduled.ToolTipText = toolStripButtonScheduled.Text = Strings.Get("Scheduled Events");
            toolStripButtonCategoryReport.ToolTipText = toolStripButtonCategoryReport.Text = Strings.Get("Category Report");
            toolStripButtonPreferences.ToolTipText = toolStripButtonPreferences.Text = Strings.Get("Preferences");
            toolStripTextBoxHelp.ToolTipText = Strings.Get("Search Help For...");
            toolStripButtonHelp.ToolTipText = toolStripButtonHelp.Text = Strings.Get("Help");
            labelInstructions.Text = Strings.Get("Check off entries that are cleared in bank statement");
            labelReconDisparity.Text = Strings.Get("Disparity (should be zero):");
            buttonAllDone.Text = Strings.Get("Create Balance Adjustment");
            buttonAbandonReconcile.Text = Strings.Get("Abandon Reconcile");
            buttonReconcileTips.Text = Strings.Get("Tips");
            labelLastClosing.Text = Strings.Get("Prev Closing:");
            labelThisClosing.Text = Strings.Get("This Closing:");
            AddStartingBalanceIfNeeded();
            PeriodicalCheck();
            _backend.ReloadTransactions();
            if (_backend.Db.InProgress == InProgress.Reconcile)
            {
                StartReconciliation();
            }
        }

        /// <summary>
        /// Call this after the splash screen vanishes.
        /// </summary>
        public void AfterSplash()
        {
            this.Opacity = 1.00;
        }

        /// <summary>
        /// Call this before each operation that the user initiates (i.e. Menu/Toolbar item)
        /// </summary>
        /// <param name="description">Very brief description, ~12 chars max</param>
        /// <param name="undoable">true if this modifies the DB</param>
        public void BeforeOperation(string description, bool undoable)
        {
            _busy = true;
            if (description != null && description.Length > 0)
            {
                Logger.Info("Beginning Operation: " + description);
                if (undoable)
                {
                    _backend.Db.MarkUndoBlock(Strings.GetIff(description));
                }
                DiagnosticNotice("");
            }
        }

        /// <summary>
        /// Call this after each operation that the user initiates.
        /// </summary>
        public void AfterOperation()
        {
            _backend.Db.MarkUndoBlockEnd();
            if(PeriodicalCheck())
            {
                _backend.ReloadTransactions(SortEntriesBy.NoChange);
            }
            int expDays = Activation.Instance.UpdateSiteSettings();
            if(expDays < 0)
            {
                if(DateTime.Now.Second % 20 == 5) // occassionally, remind the user
                {
                    MessageBox.Show(Strings.Get("Expired Trial Evaluation Period"), Strings.Get("Expired"), MessageBoxButtons.OK);
                }
                Thread.Sleep(500 + 20 * Math.Abs(expDays));
            }
            AdjustVisibilities();
            _busy = false;
            SetActivityTimer();
        }

        /// <summary>
        /// Are we currently in an undo or redo?
        /// </summary>
        protected bool InUndoRedo
        {
            get
            {
                return _backend.Db.InUndoRedo || _inUndoRedo;
            }
        }

        /// <summary>
        /// Adjust the visibility or enablement of various controls.
        /// </summary>
        public void AdjustVisibilities()
        {
            string undoDesc = Strings.Get("&Undo");
            undoToolStripMenuItem.Enabled = false;
            toolStripButtonUndo.Enabled = false;
            if (_backend.Db.DescriptionOfNextUndo.Length > 0)
            {
                undoDesc = undoDesc + " " + Strings.Get(_backend.Db.DescriptionOfNextUndo);
                undoToolStripMenuItem.Enabled = true;
                toolStripButtonUndo.Enabled = true;
            }
            byDateToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.TranDate;
            byPayeeToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.Payee;
            byCategoryToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.Category;
            byCheckNumberToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.CheckNumber;
            byMatchToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.SearchResults;
            byReconcileToolStripMenuItem.Checked = _backend.SortedBy == SortEntriesBy.CheckBox;
            dataGridView1.Columns["Id"].Visible = diagnosticsToolStripMenuItem.Checked;
            undoToolStripMenuItem.Text = undoDesc;
            undoToolStripMenuItem.ToolTipText = undoDesc;
            toolStripButtonUndo.Text = undoDesc;
            toolStripButtonUndo.ToolTipText = undoDesc;
            string redoDesc = Strings.Get("&Redo");
            redoToolStripMenuItem.Enabled = false;
            toolStripButtonRedo.Enabled = false;
            if (_backend.Db.DescriptionOfNextRedo.Length > 0)
            {
                redoDesc = redoDesc + " " + Strings.Get(_backend.Db.DescriptionOfNextRedo);
                redoToolStripMenuItem.Enabled = true;
                toolStripButtonRedo.Enabled = true;
            }
            redoToolStripMenuItem.Text = redoDesc;
            redoToolStripMenuItem.ToolTipText = redoDesc;
            toolStripButtonRedo.Text = redoDesc;
            toolStripButtonRedo.ToolTipText = redoDesc;
            diagnosticsToolStripMenuItem.Visible = adminModeToolStripMenuItem.Visible = 
                (Configuration.Instance.GetUserLevel() != UserLevel.Unlicensed);
            copyToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 1;
            toolStripButtonCopy.Enabled = dataGridView1.SelectedRows.Count == 1;
            deleteEntryToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 1;
            toolStripButtonDeleteEntry.Enabled = dataGridView1.SelectedRows.Count == 1;
            bool reconciling = false;
            if (_backend.Db.InProgress == InProgress.Reconcile)
            {
                reconciling = true;
                deleteEntryToolStripMenuItem.Enabled = false;
                toolStripButtonDeleteEntry.Enabled = false;
                yearEndToolStripMenuItem.Enabled = false;
                dataGridView1.Height = dataGridView1.Parent.Height - 150;
                labelLastClosing.Visible = labelThisClosing.Visible = this.Width > 552;
                textBoxLastBalance.Left = textBoxThisBalance.Left = (this.Width > 552) ? 160 : 88;
                labelInstructions.Left = (this.Width > 552) ? 224 : 154;
            }
            else
            {
                labelLastClosing.Visible = labelThisClosing.Visible = false;
                dataGridView1.Height = dataGridView1.Parent.Height - 100;
            }
            dataGridView1.Columns["Balance"].Visible = _backend.SortedBy == SortEntriesBy.TranDate;
            dataGridView1.Columns["IsChecked"].Visible = reconciling;
            toolStripTextBoxSearchForPayee.Enabled = toolStripButtonSearchMemo.Enabled =
                byCategoryToolStripMenuItem.Enabled = byCheckNumberToolStripMenuItem.Enabled = 
                byDateToolStripMenuItem.Enabled = byPayeeToolStripMenuItem.Enabled = 
                searchToolStripMenuItem.Enabled = !reconciling;
            dateTimePickerLastRecon.ShowUpDown = dateTimePickerThisRecon.ShowUpDown = 
                !Configuration.Instance.ShowCalendars;
            dateTimePickerLastRecon.Visible = dateTimePickerThisRecon.Visible = 
                textBoxLastBalance.Visible = textBoxThisBalance.Visible = 
                buttonAbandonReconcile.Visible =
                buttonAllDone.Visible = reconciling;
            pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
            pictureBoxLogo.Location = new Point(
                (toolStripButtonPreferences.Bounds.Left + 34 + toolStripButtonHelp.Bounds.Left) / 2 
                    - this.SizeFromClientSize(new Size(160, 0)).Width, 2);
            pictureBoxLogo.Visible = (this.Width > this.SizeFromClientSize(new Size(820, 0)).Width);
        }

        /// <summary>
        /// Handle periodical things like scheduled events, etc. 
        /// </summary>
        /// <returns>true if any events were due and, therefore, added</returns>
        private bool PeriodicalCheck()
        {
            if (DateTime.Now.Subtract(_whenLastScheduleUpdate).Minutes < ScheduleUpdateCheckMinutes)
            {
                return false;
            }
            if (_backend.Db.InProgress != InProgress.Nothing || InUndoRedo)
            {
                return false;
            }
            if (Configuration.Instance.GetAdminMode())
            {
                Configuration.Instance.SetAdminMode(false);
                adminModeToolStripMenuItem.Checked = false;
                MessageBox.Show(this, Strings.Get("Turning Off Admin Mode"), Strings.Get("Time's Up"), MessageBoxButtons.OK);
            }
            _whenLastScheduleUpdate = DateTime.Now;
            bool didSomething = ProcessScheduledEvents();
            if(didSomething)
            {
                ProcessScheduledEvents(); // in case of generating more than one transaction
            }
            return didSomething;
        }

        /// <summary>
        /// Check for scheduled events that are due and insert them into the checkbook.
        /// </summary>
        /// <returns>true if any events were due and therefore inserted</returns>
        private bool ProcessScheduledEvents()
        {
            bool didSomething = false;
            DateTime now = DateTime.Now;
            List<ScheduledEvent> events = GetDueEvents();
            foreach (ScheduledEvent schEvent in events)
            {
                ScheduledEvent schEventBeforeEdit = schEvent;
                ScheduledEvent schEventAfterEdit = schEvent.Clone();
                long amount = EventDueAmount(schEvent);
                SplitEntry split = new SplitEntry(
                    UtilityMethods.GetOrCreateCategory(_backend.Db, schEventAfterEdit.CategoryName, schEventAfterEdit.IsCredit).Id,
                    schEventAfterEdit.IsCredit ? TransactionKind.Deposit : TransactionKind.Payment, amount);
                CheckbookEntry entry = new CheckbookEntry();
                entry.Splits = new SplitEntry[] { split };
                entry.Payee = schEventAfterEdit.Payee;
                entry.MadeBy = schEvent.IsReminder ? EntryMadeBy.Reminder : EntryMadeBy.Scheduler;
                entry.DateOfTransaction = schEventAfterEdit.DueNext();
                entry.AppendMemo(schEvent.Memo);
                _backend.Db.InsertEntry(entry, Highlight.Processed); // insert checkbook entry
                schEventAfterEdit.LastPosting = entry.DateOfTransaction;
                _backend.Db.UpdateEntry(schEventAfterEdit, schEventBeforeEdit); // update scheduled event
                didSomething = true;
            }
            return didSomething;
        }

        /// <summary>
        /// Get a list of scheduled events that are due.
        /// </summary>
        /// <returns>Due events</returns>
        private List<ScheduledEvent> GetDueEvents()
        {
            List<ScheduledEvent> events = new List<ScheduledEvent>();
            ScheduledEventIterator iterator = _backend.Db.ScheduledEventIterator;
            while (iterator.HasNextEntry())
            {
                ScheduledEvent schEvent = iterator.GetNextEntry();
                if (schEvent.DueNext().CompareTo(DateTime.Now.Date.AddDays(Configuration.Instance.PostEventAdvanceDays)) <= 0)
                {
                    events.Add(schEvent);
                }
            }
            return events;
        }

        /// <summary>
        /// Estimate the amount of a transaction by cumulatively reviewing payee history.
        /// </summary>
        /// <param name="schEvent">The event to be updated</param>
        /// <returns>Estimated amount</returns>
        private long EventDueAmount(ScheduledEvent schEvent)
        {
            long amount = schEvent.Amount;
            if (schEvent.IsEstimatedAmount)
            {
                CheckbookEntryIterator checks = _backend.Db.CheckbookEntryIterator;
                while (checks.HasNextEntry())
                {
                    CheckbookEntry check = checks.GetNextEntry();
                    if (check.Payee.Trim().ToUpper().Equals(schEvent.Payee.Trim().ToUpper())
                        && check.IsCredit == schEvent.IsCredit)
                    {
                        if (amount == 0L)
                        {
                            amount = check.Amount;
                        }
                        amount = (amount * 2 + check.Amount * 3) / 5;
                    }
                }
            }
            if (schEvent.GetRepeatCount(DateTime.Now) <= 1 && schEvent.FinalPaymentAmount != 0)
            {
                amount = schEvent.FinalPaymentAmount;
            }
            return Math.Abs(amount) * (schEvent.IsCredit ? 1 : -1);
        }

        /// <summary>
        /// Display a diagnostic in the due notice area if diags are enabled.
        /// </summary>
        /// <param name="message"></param>
        private void DiagnosticNotice(string message)
        {
            if (_backend.DiagsEnabled)
            {
                _dueMessage = message;
                _alertType = AlertType.Message;
                AdjustAlert();
            }
        }

        /// <summary>
        /// This brings the datagrid into focus.
        /// </summary>
        private void FocusOnDataGridView()
        {
            if (_firstShown || dataGridView1.DisplayedRowCount(true) < 1)
            {
                dataGridView1.Focus();
                dataGridView1.Capture = true;
                return;
            }
            _firstShown = true;
            _backend.ScrollToActiveRow();
        }

        /// <summary>
        /// Call this occassionally (i.e. ten minutes after launch) to see if the user must be alerted to do something.
        /// </summary>
        private void AdjustAlert()
        {
            if (_dueMessage.Length > 0)
            {
                _alertType = AlertType.Message;
            }
            else if(_alertType == AlertType.CheckToSee)
            {
                _alertType = _backend.PeriodicAlertType;
            }
            switch (_alertType)
            {
                case AlertType.None:
                    toolStripMenuItemDueNotice.Text = "";
                    break;
                case AlertType.Message:
                    toolStripMenuItemDueNotice.Text = _dueMessage;
                    _alertType = AlertType.None; // clear on next call
                    break;
                case AlertType.Reconcile:
                    toolStripMenuItemDueNotice.Text = Strings.Get("Monthly Reconciliation Due");
                    break;
                case AlertType.YearEndWrapUp:
                    toolStripMenuItemDueNotice.Text = Strings.Get("Year-End Wrap-Up Overdue");
                    break;
                case AlertType.CheckToSee:
                    break;
            }
            toolStripMenuItemDueNotice.ForeColor = System.Drawing.Color.Red;
            _dueMessage = "";
        }

        /// <summary>
        /// If this is a new DB acct, then prompt user for a starting balance.
        /// </summary>
        private void AddStartingBalanceIfNeeded()
        {
            if(_backend.Db.CheckbookEntryIterator.HasNextEntry())
            {
                return;
            }
            StartingBalanceForm form = new StartingBalanceForm();
            DialogResult result = form.ShowDialog();
            if(result == DialogResult.OK)
            {
                CheckbookEntry balanceAdjustment = new CheckbookEntry();
                FinancialCategory category =
                    UtilityMethods.GetOrCreateCategory(_backend.Db, Strings.Get("Adjustment"), true);
                balanceAdjustment.AddSplit(category.Id, TransactionKind.Adjustment, form.Amount);
                balanceAdjustment.DateCleared = balanceAdjustment.DateOfTransaction = form.AsOfDate;
                balanceAdjustment.IsCleared = true;
                balanceAdjustment.MadeBy = EntryMadeBy.User;
                balanceAdjustment.Payee = Strings.Get("Starting Balance");
                _backend.Db.InsertEntry(balanceAdjustment);
                ReconciliationValues reconValue = new ReconciliationValues(form.Amount, form.AsOfDate);
                _backend.Db.InsertEntry(reconValue);
            }
        }

        /// <summary>
        /// Delete the currently selected entry
        /// </summary>
        /// <returns>true if deleted</returns>
        private bool DeleteCurrentEntry()
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                return false;
            }
            RowOfCheckbook rowCheckbook = (RowOfCheckbook)dataGridView1.SelectedRows[0].DataBoundItem;
            if (rowCheckbook.Entry.IsCleared || rowCheckbook.NewEntryRow)
            {
                return false;
            }
            _backend.CurrentEntryId = Guid.Empty;
            if (dataGridView1.SelectedRows[0].Index > 1)
            {   // set up to make the previous entry the active one
                _backend.CurrentEntryId = ((RowOfCheckbook)dataGridView1.Rows[dataGridView1.SelectedRows[0].Index-1].DataBoundItem).Id;
            }
            BeforeOperation("Delete Entry", true);
            CheckbookEntry entry = rowCheckbook.EntryBeforeEdit;
            _backend.Db.DeleteEntry(entry);
            _backend.DeleteTransaction(rowCheckbook);
            AfterOperation();
            DataGridContentChanged();
            return true;
        }

        /// <summary>
        /// Data grid was updated.
        /// </summary>
        public void DataGridContentChanged()
        {
            if (_backend.Db.InProgress == InProgress.Reconcile && _reconHelper != null)
            {
                _reconHelper = new ReconciliationHelper(_backend.Db);
                List<Guid> matches = _reconHelper.OpenEntries.Keys.ToList();
                UpdateReconcileControls(false, false);
                _backend.ReloadTransactions(SortEntriesBy.CheckBox, matches);
            }
            else
            {
                _backend.ReloadTransactions();
            }
        }

        /// <summary>
        /// Bring up the SearchEntriesForm and do the search.
        /// </summary>
        private void PerformSearch()
        {
            BeforeOperation("Search", false);
            SearchEntriesForm form = new SearchEntriesForm(_backend);
            List<Guid> matches = null;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if(form.ListAll)
                {
                    matches = form.Matches;
                }
                else
                {
                    matches = new List<Guid> { form.Matches.Last() };
                }
            }
            if (matches == null || matches.Count < 1)
            {
                _backend.CurrentEntryId = Guid.Empty;
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
            else if (form.ListAll)
            {
                _backend.CurrentEntryId = matches[0];
                _backend.ReloadTransactions(SortEntriesBy.SearchResults, matches);
            }
            else
            {
                _backend.CurrentEntryId = matches[0];
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
            AfterOperation();
        }

        /// <summary>
        /// Copy an entry to the clipboard
        /// </summary>
        /// <param name="rowCheckbook">To be copied</param>
        private void CopyEntry(RowOfCheckbook rowCheckbook)
        {
            if (rowCheckbook == null || rowCheckbook.NewEntryRow)
            {
                return;
            }
            StringBuilder buffer = new StringBuilder();
            buffer.Append(rowCheckbook.DateOfTransaction.ToShortDateString() + ", ");
            buffer.Append(rowCheckbook.CheckNumber + ", ");
            buffer.Append(rowCheckbook.Payee + ", ");
            buffer.Append(rowCheckbook.Category + ", ");
            buffer.Append(rowCheckbook.Amount + ", ");
            buffer.Append(rowCheckbook.IsCleared);
            Clipboard.Clear();
            Clipboard.SetText(buffer.ToString());
        }

        /// <summary>
        /// This handles an undo operation from start to finish, updating the DateGridView as well.
        /// </summary>
        private void HandleUndo()
        {
            InProgress progressWas = _backend.Db.InProgress;
            BeforeOperation("Undo", false);
            _inUndoRedo = true;
            _backend.Db.UndoToLastMark();
            _inUndoRedo = false;
            if (progressWas != _backend.Db.InProgress)
            {
                if (_backend.Db.InProgress == InProgress.Reconcile)
                {
                    StartReconciliation();
                }
                else
                {
                    EndReconciliation(false);
                }
            }
            if (_reconHelper == null)
            {
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
            else
            {
                _reconHelper.UpdateOpenEntries();
                List<Guid> matches = _reconHelper.OpenEntries.Keys.ToList();
                _backend.ReloadTransactions(SortEntriesBy.CheckBox, matches);
                UpdateReconcileControls(false, false);
            }
            _inUndoRedo = true;
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions();
            _inUndoRedo = false;
            AfterOperation();
        }

        /// <summary>
        /// This handles a redo operation from start to finish, updating the DateGridView as well.
        /// </summary>
        private void HandleRedo()
        {
            InProgress progressWas = _backend.Db.InProgress;
            BeforeOperation("Redo", false);
            _inUndoRedo = true;
            _backend.Db.RedoToNextMark();
            _inUndoRedo = false;
            if (progressWas != _backend.Db.InProgress)
            {
                if (_backend.Db.InProgress == InProgress.Reconcile)
                {
                    StartReconciliation();
                }
                else
                {
                    EndReconciliation(false);
                }
            }
            if (_reconHelper == null)
            {
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
            else
            {
                _reconHelper.UpdateOpenEntries();
                List<Guid> matches = _reconHelper.OpenEntries.Keys.ToList();
                _backend.ReloadTransactions(SortEntriesBy.CheckBox, matches);
                UpdateReconcileControls(false, false);
            }
            _inUndoRedo = true;
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions();
            _inUndoRedo = false;
            AfterOperation();
        }

        /// <summary>
        /// Show the help form.
        /// </summary>
        /// <param name="searchPattern">Search pattern, omit or null for table of contents</param>
        private void ShowHelp(string searchPattern = null)
        {
            if (_helpForm != null)
            {
                try
                {
                    _helpForm.ReShow(searchPattern);
                    return;
                }
                catch (Exception)
                {
                    // ignore, fall thru
                }
            }
            _helpForm = new BrowserForm("Help", "https://ablestrategies.com/ablecheckbook/help", 
                "https://www.google.com/search?q=site%3Aablestrategies.com+checkbook+help+", this);
            _helpForm.Show();
            if (searchPattern != null)
            {
                _helpForm.ReShow(searchPattern);
            }
        }

        //////////////////////////// Other Threads ///////////////////////////

        /// <summary>
        /// Set a timer to auto save hte DB when the user is idle for so many seconds.
        /// </summary>
        private static void SetActivityTimer()
        {
            if (_activityTimer != null)
            {
                _activityTimer.Stop();
                Thread.Sleep(100);
                _activityTimer.Dispose();
            }
            _activityTimer = new System.Timers.Timer(IdleSeconds * 1000);
            _activityTimer.Elapsed += OnTimedEvent1;
            _activityTimer.AutoReset = false;
            _activityTimer.Enabled = true;
        }

        /// <summary>
        /// Handle timer expiration.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent1(Object source, ElapsedEventArgs e)
        {
            _mainScreen.Invoke(new HandleTimerEventDelegate(_mainScreen.HandleTimerEvent), 0);
        }

        /// <summary>
        /// Delegate used to call HandleTimerEvent() on the main GUI thread
        /// </summary>
        /// <param name="dummy">Unused at this time</param>
        private delegate void HandleTimerEventDelegate(int dummy);

        /// <summary>
        /// Sync the DB to disk.
        /// </summary>
        /// <param name="dummy">Unused at this time</param>
        private void HandleTimerEvent(int dummy)
        {
            if(_mainScreen._busy)
            {
                SetActivityTimer();
                return;
            }
            _mainScreen._dueMessage = Strings.Get("Autosaving...");
            _mainScreen.AdjustAlert();
            _mainScreen._backend.Db.IdleTimeSync(); // sync
        }

        /// <summary>
        /// Occassionally check for overdue year-end or reconcile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            long elapsedSeconds = 0L;
            while (!BgWorkerThread.CancellationPending)
            {
                Thread.Sleep(3000); // sleep 3 seconds
                ++elapsedSeconds;
                if (elapsedSeconds % 1800 == 480) // 8 minutes then every 30
                {
                    _alertType = AlertType.CheckToSee;
                }
                AdjustAlert();
            }
        }

        //////////////////////////////////////////////////////////////////////
        ///  If Visual Studio appends code here, delete it then build-->   ///
        ///  clean. Otherwise it will cause "duplicate definition" errors. ///
        ///  To prevent this from happening, instead of double-clicking on ///
        ///  the cs file in Solution Explorer, right-click then view code. ///
        //////////////////////////////////////////////////////////////////////

    }

}
