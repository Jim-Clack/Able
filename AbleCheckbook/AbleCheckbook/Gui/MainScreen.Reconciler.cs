using AbleCheckbook.Db;
using AbleCheckbook.Gui;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook
{

    /// <summary>
    /// Main GUI Reconciliation Support.
    /// </summary>
    partial class MainScreen
    {

        /// <summary>
        /// This guy does all the heavy lifting for reconciling.
        /// </summary>
        private ReconciliationHelper _reconHelper = null;

        /// <summary>
        /// For displaying tips.
        /// </summary>
        private ReconcileCandidatesForm _tipsForm = new ReconcileCandidatesForm();

        /// <summary>
        /// Have entries been checked off yet?
        /// </summary>
        private bool _checkedOff = false;

        /// <summary>
        /// Update the reconciliation controls at bottom of main screen.
        /// </summary>
        /// <param name="checkedOff">True if an entry was checked.</param>
        /// <param name="hideAll">True to hide all controls (in addition to that done by UpdateVisibilities)</param>
        private void UpdateReconcileControls(bool checkedOff, bool hideAll)
        {
            if (_reconHelper == null)
            {
                return;
            }
            _checkedOff = _checkedOff || checkedOff;
            bool showInstructions = true;
            if (textBoxThisReconBalance.Text.Trim().Length < 3)
            {
                labelInstructions.Text = Strings.Get("Fill in bottom/left from your bank statement");
                // dataGridView1.Enabled = false; // nope, causes user confusion
            }
            else
            {
                labelInstructions.Text = Strings.Get("Check off any remaining cleared entries");
                dataGridView1.Enabled = true;
                if (_checkedOff)
                {
                    showInstructions = false;
                }
            }
            bool showDisparity = (dataGridView1.Width > 830) || !showInstructions;
            labelInstructions.Visible = !hideAll && showInstructions;
            buttonReconcileTips.Visible = !hideAll && showDisparity;
            labelReconDisparity.Visible = !hideAll && showDisparity;
            textBoxReconDisparity.Visible = !hideAll && showDisparity;
            string balanceAsAString = textBoxThisReconBalance.Text.Trim();
            long closingBalance = 0L;
            if (balanceAsAString.Length <= 1)
            {
                balanceAsAString = "";
            }
            else
            {
                closingBalance = UtilityMethods.ParseCurrency(balanceAsAString);
                balanceAsAString = UtilityMethods.FormatCurrency(closingBalance);
            }
            textBoxThisReconBalance.Text = balanceAsAString;
            _backend.UpdateCheckedEntries(_reconHelper);
            long disparity = _reconHelper.GetDisparity(closingBalance);
            if (textBoxPrevReconBalance.Text.Trim().Length > 0)
            {
                disparity = _reconHelper.GetDisparity(closingBalance, UtilityMethods.ParseCurrency(textBoxPrevReconBalance.Text));
            }
            textBoxReconDisparity.Text = UtilityMethods.FormatCurrency(disparity);
            if (disparity == 0)
            {
                buttonAllDone.Text = Strings.Get("Done With Reconciliation");
                buttonAllDone.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                buttonAllDone.Text = Strings.Get("Create Balance Adjustment");
                buttonAllDone.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            buttonAllDone.Enabled = textBoxThisReconBalance.Text.Trim().Length > 1;
            buttonReconcileTips.Enabled = false;
            List<CandidateEntry> candidates =
                _reconHelper.FindTipCandidates(-disparity, dateTimePickerThisRecon.Value.AddDays(1));
            if (candidates.Count == 0 || candidates.Count > 20)
            {
                _tipsForm.Hide();
            }
            else
            {
                buttonReconcileTips.Enabled = true;
                _tipsForm.Clear();
                foreach (CandidateEntry candidate in candidates)
                {
                    _tipsForm.AppendCandidate(candidate);
                }
            }
        }

        /// <summary>
        /// Show the tips form.
        /// </summary>
        private void ShowTips()
        {
            if (_tipsForm.TipsAvailable())
            {
                _tipsForm.ToFront();
            }
        }

        /// <summary>
        /// Start the reconciliation process.
        /// </summary>
        /// <param name="continuation">true if continuing from a reconciliation in progress.</param>
        private void StartReconciliation(bool continuation)
        {
            if (_reconHelper != null)
            {
                return;
            }
            dateTimePickerPrevRecon.ShowUpDown = !Configuration.Instance.ShowCalendars;
            dateTimePickerThisRecon.ShowUpDown = !Configuration.Instance.ShowCalendars;
            ReconciliationValues reconValues = _backend.Db.GetReconciliationValues();
            dateTimePickerPrevRecon.Value = reconValues.Date.Date;
            DateTime now = DateTime.Now;
            dateTimePickerThisRecon.Value = reconValues.Date.AddMonths(1).Date;
            if(dateTimePickerThisRecon.Value.Date.Date <= dateTimePickerPrevRecon.Value.Date.Date)
            {
                dateTimePickerThisRecon.Value = dateTimePickerThisRecon.Value.AddMonths(1).Date;
            }
            textBoxPrevReconBalance.Text = UtilityMethods.FormatCurrency(reconValues.Balance, 3);
            textBoxThisReconBalance.Text = "";
            if (!continuation) // only when starting, not when reloading an acct w reconciliation in progress
            {
                ReconcileStartForm form = new ReconcileStartForm(Backend.Db);
                form.PrevReconDate = dateTimePickerPrevRecon.Value;
                form.ThisReconDate = dateTimePickerThisRecon.Value;
                form.PrevReconBalance = textBoxPrevReconBalance.Text;
                form.ThisReconBalance = textBoxThisReconBalance.Text;
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK)
                {
                    EndReconciliation(false);
                    _backend.ReloadTransactions(SortEntriesBy.TranDate);
                    return;
                }
                dateTimePickerPrevRecon.Value = form.PrevReconDate;
                dateTimePickerThisRecon.Value = form.ThisReconDate;
                textBoxPrevReconBalance.Text = form.PrevReconBalance;
                textBoxThisReconBalance.Text = form.ThisReconBalance;
                dateTimePickerPrevRecon.Enabled = dateTimePickerThisRecon.Enabled = false;
                textBoxPrevReconBalance.Enabled = textBoxThisReconBalance.Enabled = false;
            }
            itemizeSplitsToolStripMenuItem.Checked = true;
            _backend.ItemizedSplits = true;
            _backend.Db.InProgress = InProgress.Reconcile;
            _reconHelper = new ReconciliationHelper(_backend.Db);
            List<Guid> matches = _reconHelper.OpenEntries.Keys.ToList();
            _backend.ReloadTransactions(SortEntriesBy.CheckBox, matches);
            UpdateReconcileControls(false, false);
        }

        /// <summary>
        /// All done with reconciling entries.
        /// </summary>
        /// <param name="commitReconcile">true to perform the reconcile, false to abandon it</param>
        private void EndReconciliation(bool commitReconcile)
        {
            _tipsForm.Hide();
            dataGridView1.Enabled = true;
            if (_backend.Db.InProgress != InProgress.Reconcile || _reconHelper == null)
            {
                _reconHelper = null;
                return;
            }
            if (commitReconcile)
            {
                CommitReconcile();
            }
            _backend.UncheckAll();
            _backend.Db.InProgress = InProgress.Nothing;
            UpdateReconcileControls(false, true);
            _backend.ReloadTransactions(SortEntriesBy.TranDate);
            _reconHelper = null;
            dataGridView1.Enabled = true;
        }

        /// <summary>
        /// Commit the checkboxes to clears, insert a balance adjustment, and commit the reconcile.
        /// </summary>
        private void CommitReconcile()
        {
            ReconciliationValues reconValues = null;
            long closingBalance = 0L;
            if (textBoxThisReconBalance.Text.Trim().Length > 0)
            {
                closingBalance = UtilityMethods.ParseCurrency(textBoxThisReconBalance.Text.Trim());
            }
            DateTime closingDate = dateTimePickerThisRecon.Value;
            long disparity = UtilityMethods.ParseCurrency(textBoxReconDisparity.Text);
            AddBalanceAdjustmentEntry(-disparity, closingDate.AddDays(-1)); // is the sign correct?
            _backend.CommitCheckedEntries(_reconHelper);
            _backend.Db.DeleteEntry(_backend.Db.GetReconciliationValues());
            reconValues = new ReconciliationValues(closingBalance, closingDate);
            _backend.Db.InsertEntry(reconValues);
        }

        /// <summary>
        /// If this is the first reconciliation, update the starting balance, else add a new adjustment.
        /// </summary>
        /// <param name="disparity">adjustment amount</param>
        /// <param name="closingDate">date of adjustment</param>
        private void AddBalanceAdjustmentEntry(long disparity, DateTime closingDate)
        {
            CheckbookEntryIterator iterator = _backend.Db.CheckbookEntryIterator;
            CheckbookEntry balanceAdjustment = null;
            balanceAdjustment = new CheckbookEntry();
            FinancialCategory category =
                UtilityMethods.GetOrCreateCategory(_backend.Db, Strings.Get("Adjustment"), true);
            balanceAdjustment.AddSplit(category.Id, TransactionKind.Adjustment, disparity);
            balanceAdjustment.DateCleared = balanceAdjustment.DateOfTransaction = closingDate;
            balanceAdjustment.Payee = Strings.Get("Reconcile");
            balanceAdjustment.IsCleared = true;
            balanceAdjustment.MadeBy = EntryMadeBy.Reconciler;
            _backend.Db.InsertEntry(balanceAdjustment);
        }

        /// <summary>
        /// Common event handler for checkbox click.
        /// </summary>
        /// <param name="sender">Unused at this time</param>
        /// <param name="rowIndex">Row of datagridview</param>
        /// <param name="columnIndex">Column of datagridview</param>
        private void HandleCheckBox(object sender, int rowIndex, int columnIndex)
        {
            int displayIndex = dataGridView1.Columns[columnIndex].DisplayIndex;
            if (_backend == null || rowIndex < 0 || dataGridView1.Rows.Count <= rowIndex ||
                InUndoRedo || dataGridView1.Rows[rowIndex].DataBoundItem == null ||
                displayIndex > 1 || _backend.Db.InProgress != InProgress.Reconcile)
            {
                return;
            }
            if (_recursionLevel > 0)
            {
                --_recursionLevel;
                return;
            }
            _recursionLevel = UtilityMethods.Clamp(1, _recursionLevel + 1, 99);
            RowOfCheckbook rowCheckbook = (RowOfCheckbook)dataGridView1.Rows[rowIndex].DataBoundItem;
            if (rowCheckbook != null)
            {
                if (rowCheckbook.IsCleared == "X")
                {
                    DataGridViewCheckBoxCell cell = dataGridView1.Rows[rowIndex].Cells["IsChecked"] as DataGridViewCheckBoxCell;
                    dataGridView1.Rows[rowIndex].Cells["IsChecked"].Value = cell.FalseValue;
                    return;
                }
                BeforeOperation("Checkbox", true);
                ReconcileCheckboxClicked(rowIndex, columnIndex);
                AfterOperation();
                UpdateReconcileControls(true, false);
            }
            _recursionLevel = UtilityMethods.Clamp(0, _recursionLevel - 1, 99);
        }

        /// <summary>
        /// Handle a click on the check box.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        public void ReconcileCheckboxClicked(int rowIndex, int columnIndex)
        {
            RowOfCheckbook rowEntry = (RowOfCheckbook)dataGridView1.Rows[rowIndex].DataBoundItem;
            if (rowEntry != null)
            {
                DataGridViewCheckBoxCell cell = dataGridView1.Rows[rowIndex].Cells[columnIndex] as DataGridViewCheckBoxCell;
                if(cell != null)
                {
                    if (rowEntry.NewEntryRow || rowEntry.Entry.IsCleared)
                    {
                        cell.Value = cell.FalseValue;
                    }
                    else
                    {
                        rowEntry.IsChecked = (bool)cell.EditedFormattedValue;
                        _backend.Db.UpdateEntry(rowEntry.Entry, rowEntry.EntryBeforeEdit, true);
                        rowEntry.EntryBeforeEdit = rowEntry.Entry.Clone(false);
                    }
                }
            }
            int nextRowNbr = rowIndex + 1;
            if (dataGridView1.Rows.Count > nextRowNbr && !InUndoRedo)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[nextRowNbr].Cells[columnIndex];
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
