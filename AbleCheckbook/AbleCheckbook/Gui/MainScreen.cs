using AbleCheckbook.Db;
using AbleCheckbook.Gui;
using AbleCheckbook.Logic;
using AbleLicensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AbleCheckbook
{

    /// <summary>
    /// Main GUI Ctor and Event Handlers only.
    /// </summary>
    public partial class MainScreen : Form
    {

        /// <summary>
        /// Ctor.
        /// </summary>
        public MainScreen()
        {
            this.Opacity = 0.35;
            new SplashForm(this).Show(this);
            InitializeComponent();
            BgWorkerThread.WorkerReportsProgress = false;
            BgWorkerThread.WorkerSupportsCancellation = true;
            BgWorkerThread.RunWorkerAsync();
        }

        /////////////////////////// Event Handlers ///////////////////////////

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Workaround for Microsoft oversight - calls the CommitEdit method.
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            _backend.PrepForRepaint();
            AdjustVisibilities();
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex <= 1 && _backend.Db.InProgress != InProgress.Nothing)
            {
                RowOfCheckbook rowCheckbook = (RowOfCheckbook)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                if (rowCheckbook != null && rowCheckbook.EntryBeforeEdit.IsCleared)
                {
                    ((DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1]).Value = false;
                }
            }
        }

        private void MainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            _backend.Db.BackupAndSave();
            Configuration.Instance.SetWindowBounds(this.Left, this.Top, this.Width, this.Height);
            Configuration.Instance.Save();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HandleCheckBox(sender, e.RowIndex, e.ColumnIndex);
            dataGridView1.Invalidate();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int displayIndex = dataGridView1.Columns[e.ColumnIndex].DisplayIndex;
            if (displayIndex <= 1 && _backend.Db.InProgress == InProgress.Reconcile)
            {
                return;
            }
            if (e.RowIndex < 0 && _backend.Db.InProgress == InProgress.Nothing)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                if (columnName == "CheckNumber")
                {
                    _backend.ReloadTransactions(SortEntriesBy.CheckNumber, null);
                }
                else if (columnName == "Payee")
                {
                    _backend.ReloadTransactions(SortEntriesBy.Payee, null);
                }
                else if (columnName == "Category")
                {
                    _backend.ReloadTransactions(SortEntriesBy.Category, null);
                }
                else
                {
                    _backend.ReloadTransactions(SortEntriesBy.TranDate, null);
                }
                return;
            }
            RowOfCheckbook rowEntry = (RowOfCheckbook)dataGridView1.CurrentRow.DataBoundItem;
            if (rowEntry.Entry.IsCleared && !Configuration.Instance.GetAdminMode())
            {
                return;
            }
            BeforeOperation(rowEntry.NewEntryRow ? "New Entry" : "Edit/Del Entry", true);
            _backend.EditTransaction(this, e.RowIndex);
            AfterOperation();
            DataGridContentChanged();        
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != ' ' && e.KeyValue != '\r' && e.KeyValue != '\n') // Allow user to use keyboard
            {
                return;
            }
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row == null)
            {
                return;
            }
            DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(1, row.Index);
            dataGridView1_CellClick(dataGridView1, args);
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            FocusOnDataGridView();
        }

        private void MainScreen_ResizeEnd(object sender, EventArgs e)
        {
            _backend.AdjustWidths();
            AdjustVisibilities();
            UpdateReconcileControls(false, false);
        }

        /// <summary>
        /// Constrain row height to a reasonable range.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            if(e.Row.Height < 12)
            {
                e.Row.Height = 12;
            }
            if (e.Row.Height > 100)
            {
                e.Row.Height = 100;
            }
            if (e.Row.DataBoundItem != null)
            {
                ((RowOfCheckbook)e.Row.DataBoundItem).ShowSplits = (e.Row.Height > 26);
            }
        }

        /////////////////////////// Tool Bar Icons ///////////////////////////

        private void toolStripButtonCategoryReport_Click(object sender, EventArgs e)
        {
            CategoryChartForm form = new CategoryChartForm(_backend.Db);
            form.ShowDialog();
        }

        private void toolStripButtonPrintRegister_Click(object sender, EventArgs e)
        {
            ReportPrinter reportPrinter = new ReportPrinter();
            reportPrinter.PrintRegisterReport(_backend.Db);
        }

        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                return;
            }
            _backend.CopyToClipboard();
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            HandleUndo();
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            HandleRedo();
        }

        private void toolStripMenuItemDueNotice_Click(object sender, EventArgs e)
        {
            switch (_alertType)
            {
                case AlertType.None:
                    // Do nothing
                    break;
                case AlertType.Message:
                    // Do nothing
                    break;
                case AlertType.Reconcile:
                    BeforeOperation("Reconcile", true);
                    StartReconciliation(false);
                    AfterOperation();
                    _alertType = AlertType.None;
                    break;
                case AlertType.YearEndWrapUp:
                    BeforeOperation("Year End", true);
                    string account = _backend.PerformYearEndWrapUp(this);
                    AfterOperation();
                    _alertType = AlertType.None;
                    if(account.Length > 5)
                    {
                        LoadNewDb(account);
                    }
                    break;
            }
            AdjustAlert();
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            if (toolStripTextBoxSearchForPayee.Text.Length == 0)
            {
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
                return;
            }
            List<Guid> matches = UtilityMethods.SearchDb(_backend.Db,
                UtilityMethods.EntryField.MemoSubstring, toolStripTextBoxSearchForPayee.Text, Guid.NewGuid(), 0, 0, false, ScheduledEvent.Eternity);
            if (matches.Count > 0)
            {
                _backend.ReloadTransactions(SortEntriesBy.SearchResults, matches);
            }
        }

        private void toolStripButtonDeleteEntry_Click(object sender, EventArgs e)
        {
            DeleteCurrentEntry();
        }

        private void toolStripButtonNewEntry_Click(object sender, EventArgs e)
        {
            BeforeOperation("New Entry", true);
            _backend.NewEntry(this);
            AfterOperation();
        }

        private void toolStripTextBoxSearchForPayee_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != '\r' && e.KeyValue != '\n')
            {
                return;
            }
            if (toolStripTextBoxSearchForPayee.Text.Length == 0)
            {
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
                return;
            }
            List<Guid> matches = UtilityMethods.SearchDb(_backend.Db,
                UtilityMethods.EntryField.PayeeSubstring, toolStripTextBoxSearchForPayee.Text, Guid.NewGuid(), 0, 0, false, ScheduledEvent.Eternity);
            if (matches.Count > 0)
            {
                _backend.ReloadTransactions(SortEntriesBy.SearchResults, matches);
            }
        }

        private void toolStripTextBoxHelp_Leave(object sender, EventArgs e)
        {
            if (toolStripTextBoxHelp.Text.Trim().Length < 1)
            {
                return;
            }
            ShowHelp(toolStripTextBoxHelp.Text);
        }

        private void toolStripTextBoxHelp_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode != Keys.Enter)
            {
                return;
            }
            ShowHelp(toolStripTextBoxHelp.Text);
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            ShowHelp(toolStripTextBoxHelp.Text);
        }

        ///////////////////////////// File Menu //////////////////////////////

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAcctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.Db.BackupAndSave();
        }

        private void openAcctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.OpenOrNewDb();
            AddStartingBalanceIfNeeded();
            PeriodicalCheck();
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.TranDate);
            Configuration.Instance.LastDbName = _backend.Db.UnderlyingDb.FullPath;
            Configuration.Instance.Save();
            this.Text = Strings.Get("Able Strategies AbleCheckbook") + " - " + _backend.Db.Name;
        }

        private void newAcctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDbForm form = new NewDbForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filename = form.AccountName;
                _backend.OpenDb(filename);
                AddStartingBalanceIfNeeded();
                _backend.CurrentEntryId = Guid.Empty;
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
                this.Text = Strings.Get("Able Strategies AbleCheckbook") + " - " + _backend.Db.Name;
            }
        }

        private void openBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBackupForm form = new OpenBackupForm(_backend.Db);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // _backend.Db.BackupAndSave(); // jbc removed 2/9/2022 = Added five lines below
                _backend.Db.Sync();
                string acbPath = _backend.Db.FullPath;
                string bu0Path = acbPath.Replace(".acb", ".bu0");
                File.Delete(bu0Path);
                File.Move(acbPath, bu0Path);
                // end of chnages
                _backend.Db.CloseWithoutSync();
                _backend.OpenDb(form.Filepath);
                AddStartingBalanceIfNeeded();
                _backend.CurrentEntryId = Guid.Empty;
                _backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
        }

        private void acctSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountSettingsForm form = new AccountSettingsForm(_backend.Db);
            form.ShowDialog();
        }

        private void importQifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Import QIF", true);
            _backend.ImportQif(this);
            AfterOperation();
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.NoChange);
        }

        private void importCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Import CSV", true);
            _backend.ImportCsv(this);
            AfterOperation();
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.NoChange);
        }

        private void exportQifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Export QIF", false);
            _backend.ExportQif(this);
            AfterOperation();
        }

        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Export CSV", false);
            _backend.ExportCsv(this);
            AfterOperation();
        }

        ///////////////////////////// Edit Menu //////////////////////////////

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleRedo();
        }

        private void renamePayeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePayeeForm form = new RenamePayeeForm(this);
            BeforeOperation("Rename Payee", true);
            DialogResult result = form.ShowDialog();
            AfterOperation();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void newEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("New Entry", true);
            _backend.NewEntry(this);
            AfterOperation();
            DataGridContentChanged();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.CopyToClipboard();
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCurrentEntry();
        }

        ///////////////////////////// View Menu //////////////////////////////

        private void byDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.TranDate);
        }

        private void byCheckNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.CheckNumber);
        }

        private void byPayeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.Payee);
        }

        private void byCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.Category);
        }

        private void itemizeSplitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            itemizeSplitsToolStripMenuItem.Checked = !itemizeSplitsToolStripMenuItem.Checked;
            _backend.ItemizedSplits = itemizeSplitsToolStripMenuItem.Checked;
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.NoChange);
        }

        ///////////////////////////// Tools Menu /////////////////////////////

        private void scheduledTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScheduledEventsListForm form = new ScheduledEventsListForm(_backend);
            form.ShowDialog();
            _whenLastScheduleUpdate = DateTime.Now.AddMinutes(-ScheduleUpdateCheckMinutes);
            _backend.CurrentEntryId = Guid.Empty;
            if (PeriodicalCheck())
            {
                _backend.ReloadTransactions(SortEntriesBy.NoChange);
            }
        }

        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void memorizedPayeesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void categoryReportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CategoryChartForm form = new CategoryChartForm(_backend.Db);
            form.ShowDialog();
        }

        private void reconcileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Reconcile", true);
            StartReconciliation(false);
            AfterOperation();
        }

        private void yearEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeforeOperation("Year End", true);
            string account = _backend.PerformYearEndWrapUp(this);
            AfterOperation();
            if (account.Length > 5)
            {
                LoadNewDb(account);
            }
        }

        private void printRegisterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ReportPrinter reportPrinter = new ReportPrinter();
            reportPrinter.PrintRegisterReport(_backend.Db);
        }

        ///////////////////////////// Help Menu //////////////////////////////

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineActivation.Instance.DoNothing();
            OnlineActivation.Instance.CheckConnection();
            /*
            Activation.Instance.SetDefaultDays(180, 366); 
            Activation.Instance.LicenseCode = "MYNAME@99999"; 
            string pin = Activation.Instance.CalculatePin(Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification));
            Activation.Instance.SetActivationPin(pin);
            Activation.Instance.SetFeatureBitmask(0x000000000000000FL, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification)); 
            Activation.Instance.SetExpiration(2, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification)); 
            */
            AboutForm form = new AboutForm();
            form.ShowDialog();
        }

        private void helpContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesForm form = new PreferencesForm();
            form.ShowDialog();
            _backend.CurrentEntryId = Guid.Empty;
            _backend.ReloadTransactions(SortEntriesBy.NoChange);
        }

        private void activateStripMenuItem_Click(object sender, EventArgs e)
        {
            ActivationForm form = new ActivationForm();
            form.ShowDialog();
            // TODO
        }

        private void diagnosticsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            diagnosticsToolStripMenuItem.Checked = !diagnosticsToolStripMenuItem.Checked;
            _backend.CurrentEntryId = Guid.Empty;
            _backend.DiagsEnabled = diagnosticsToolStripMenuItem.Checked;
            _backend.ReloadTransactions();
            DiagnosticNotice(Strings.Get("Diagnostics"));
        }

        private void adminModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _whenLastScheduleUpdate = DateTime.Now;
            adminModeToolStripMenuItem.Checked = !adminModeToolStripMenuItem.Checked;
            Configuration.Instance.SetAdminMode(adminModeToolStripMenuItem.Checked);

        }

        /////////////////////////// Super User Menu //////////////////////////

        private void userMgmtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuUserManagementForm form = new SuUserManagementForm();
            form.ShowDialog();
        }

        private void readLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuLogFileReaderForm form = new SuLogFileReaderForm();
            form.ShowDialog();
        }

        ///////////////////////////// Reconcile //////////////////////////////

        private void dateTimePickerThisRecon_ValueChanged(object sender, EventArgs e)
        {
        }

        private void buttonReconcileTips_Click(object sender, EventArgs e)
        {
            ShowTips();
        }

        private void textBoxThisBalance_Validated(object sender, EventArgs e)
        {
            UpdateReconcileControls(false, false);
        }

        private void buttonAllDone_Click(object sender, EventArgs e)
        {
            BeforeOperation("Commit", true);
            EndReconciliation(true);
            AfterOperation();
        }

        private void buttonAbandonReconcile_Click(object sender, EventArgs e)
        {
            BeforeOperation("Abandon", true);
            EndReconciliation(false);
            AfterOperation();
        }

        private void MainScreen_Click(object sender, EventArgs e)
        {
            UpdateReconcileControls(false, false);
        }

        private void textBoxThisBalance_Leave(object sender, EventArgs e)
        {
            UpdateReconcileControls(false, false);
            _backend.Db.PendingReconcileEndAmount = UtilityMethods.ParseCurrency(textBoxThisReconBalance.Text);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dateTimePickerLastRecon_Enter(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.Get("Are you sure you want to change this value?"), Strings.Get("Confirm"), MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                dateTimePickerPrevRecon.Enter -= dateTimePickerLastRecon_Enter;
                ReconciliationValues reconValues = _backend.Db.GetReconciliationValues();
                dateTimePickerPrevRecon.Value = reconValues.Date;
                textBoxPrevReconBalance.Text = UtilityMethods.FormatCurrency(reconValues.Balance, 3);
                dateTimePickerPrevRecon.Enter += dateTimePickerLastRecon_Enter;
            }
        }

        private void textBoxLastBalance_Enter(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.Get("Are you sure you want to change this value?"), Strings.Get("Confirm"), MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                textBoxPrevReconBalance.Enter -= textBoxLastBalance_Enter;
                ReconciliationValues reconValues = _backend.Db.GetReconciliationValues();
                textBoxPrevReconBalance.Text = UtilityMethods.FormatCurrency(reconValues.Balance, 3);
                textBoxPrevReconBalance.Enter += textBoxLastBalance_Enter;
            }
        }

        private void textBoxLastBalance_Leave(object sender, EventArgs e)
        {
            UpdateReconcileControls(false, false);
        }

        private void dateTimePickerThisRecon_Leave(object sender, EventArgs e)
        {
            _backend.Db.PendingReconcileEndDate = dateTimePickerThisRecon.Value;
        }
    }

}
