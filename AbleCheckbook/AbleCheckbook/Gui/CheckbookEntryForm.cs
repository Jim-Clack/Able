using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{

    public partial class CheckbookEntryForm : Form
    {

        private const int MaxSplits = 6;

        /// <summary>
        /// This form is based on one row in the following data grid view.
        /// </summary>
        private DataGridView _dataGridView = null;

        /// <summary>
        /// Which row in the datagridview are we dealing with?
        /// </summary>
        private int _dataGridIndex = 0;

        /// <summary>
        /// Support methods.
        /// </summary>
        private UiBackend _backend = null;

        /// <summary>
        /// This is the displayed checkbook entry (grid row) that we're dealing with.
        /// </summary>
        private RowOfCheckbook _rowCheckbook = null;

        /// <summary>
        /// Did the user click Delete?
        /// </summary>
        private bool _deleteEntry = false;

        /// <summary>
        /// For use in handling category splits.
        /// </summary>
        private SplitEntry[] _splits = null;

        // TODO: Instead of a bunch of managed arrays, we need a SplitRowControl

        /// <summary>
        /// Array of combos that are each a category list.
        /// </summary>
        private ComboBox[] _comboCategories = null;

        /// <summary>
        /// Array of combos that are each an TransactionKind list.
        /// </summary>
        private ComboBox[] _comboKinds = null;

        /// <summary>
        /// Array of text entries, each for the amount of a certain split.
        /// </summary>
        private TextBox[] _textBoxAmounts = null;

        /// <summary>
        /// Array of category labels.
        /// </summary>
        private Label[] _labelCategories = null;

        /// <summary>
        /// Array of amount labels.
        /// </summary>
        private Label[] _labelAmounts = null;

        /// <summary>
        /// The following controls can slide downward to accomodate more splits.
        /// </summary>
        private Control[] _slidingControls = null;

        /// <summary>
        /// Offsets of _slidingControls
        /// </summary>
        private int[] _yOffsets = null;

        /// <summary>
        /// Original window dims.
        /// </summary>
        private int _winWidth = 600, _winHeight = 400;

        /// <summary>
        /// Where do the splits begin?
        /// </summary>
        private int _yTopSplits = 81;

        /// <summary>
        /// Used for layout scaling coordinates 
        /// </summary>
        private double _xScale = 1.0, _yScale = 1.0;

        /// <summary>
        /// How many splits are visible?
        /// </summary>
        private int _visibleSplitCount = 2;

        /// <summary>
        /// Are the fields set up correctly?
        /// </summary>
        private bool _readyForSubmit = false;

        /// <summary>
        /// Is this a new entry or editing an existing one?
        /// </summary>
        private bool _isNewEntry = false;

        /// <summary>
        /// [static] to prevent recursive event handling
        /// </summary>
        private static int _recursionLevel = 0;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="backend">Source of data and support methods.</param>
        /// <param name="dataGridView">Source of displayed rows of data.</param>
        /// <param name="dataGridIndex">The corresponding row in the datagridview.</param>
        public CheckbookEntryForm(UiBackend backend, DataGridView dataGridView, int dataGridIndex)
        {
            _backend = backend;
            _dataGridView = dataGridView;
            _dataGridIndex = dataGridIndex;
            InitializeComponent();
        }

        /// <summary>
        /// Load strings and populate the form.
        /// </summary>
        private void LoadFormControls()
        {
            LoadStrings();
            _slidingControls = new Control[]
            {
                textBoxMemo, labelMemoOverlay, textBoxAssistance, buttonBar, textBoxTotalAmt, labelTotal, checkBoxCleared, buttonDelete, buttonCancel, buttonOk
            };
            int index = 0;
            _yOffsets = new int[_slidingControls.Length];
            foreach (Control control in _slidingControls)
            {
                _yOffsets[index++] = control.Top;
            };
            _winWidth = this.Width;
            _winHeight = this.Height;
            _yTopSplits = textBoxScaling.Top;
            _xScale = textBoxScaling.Width / 500.0;
            _yScale = textBoxScaling.Height / 50.0;
            LoadSplitRows();
            _rowCheckbook = (RowOfCheckbook)_dataGridView.Rows[_dataGridIndex].DataBoundItem;
            if (_rowCheckbook.NewEntryRow)
            {
                _rowCheckbook = new RowOfCheckbook(_backend.Db, new CheckbookEntry(), "New");
                PopulateNewEntry();
            }
            else
            {
                PopulateExistingEntry();
            }
        }

        /// <summary>
        /// Load the controls that display the splits.
        /// </summary>
        private void LoadSplitRows()
        {
            _comboCategories = new ComboBox[MaxSplits];
            _comboKinds = new ComboBox[MaxSplits];
            _textBoxAmounts = new TextBox[MaxSplits];
            _labelAmounts = new Label[MaxSplits];
            _labelCategories = new Label[MaxSplits];

            for (int rowNumber = 0; rowNumber < MaxSplits; ++rowNumber)
            {
                int yOffset = (int)(_yTopSplits + rowNumber * 34 * _yScale);

                Label labelCategory = new Label();
                labelCategory.Location = new Point((int)(7 * _xScale), (int)((yOffset + 2 * _yScale)));
                labelCategory.Size = new Size((int)(65 * _xScale), (int)(17 * _yScale));
                labelCategory.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                labelCategory.AutoSize = true;
                labelCategory.ForeColor = System.Drawing.Color.Black;
                labelCategory.Name = "labelCategory" + rowNumber;
                this.Controls.Add(labelCategory);
                labelCategory.Text = Strings.Get("Category");
                labelCategory.SendToBack();

                ComboBox comboCategory = new ComboBox();
                comboCategory.Location = new Point((int)(80 * _xScale), yOffset);
                comboCategory.Size = new Size((int)(160 * _xScale), (int)(24 * _yScale));
                comboCategory.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                comboCategory.AllowDrop = true;
                comboCategory.DropDownStyle = ComboBoxStyle.DropDown;
                comboCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
                comboCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
                comboCategory.FormattingEnabled = true;
                comboCategory.Name = "comboCategory" + rowNumber;
                comboCategory.SelectedIndexChanged += new System.EventHandler(this.comboCategory1_SelectedIndexChanged);
                comboCategory.Leave += new System.EventHandler(this.comboCategory_Leave);
                comboCategory.DataSource = _backend.Categories;
                comboCategory.BindingContext = new BindingContext();
                this.Controls.Add(comboCategory);
                comboCategory.BringToFront();
                comboCategory.Text = "";

                ComboBox comboKind = new ComboBox();
                comboKind.Location = new Point((int)(257 * _xScale), yOffset);
                comboKind.Size = new Size((int)(124 * _xScale), (int)(24 * _yScale));
                comboKind.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                comboKind.AllowDrop = true;
                comboKind.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
                comboKind.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
                comboKind.FormattingEnabled = true;
                comboKind.Name = "comboKind" + rowNumber;
                comboKind.DataSource = new string[]
                {
                    Strings.Get("Payment"), // must be in same sequence as TransactionKind enum
                    Strings.Get("Deposit"),
                    Strings.Get("Refund"),
                    Strings.Get("XferOut"),
                    Strings.Get("XferIn"),
                    Strings.Get("Adjustment"),
                };
                comboKind.BindingContext = new BindingContext();
                this.Controls.Add(comboKind);
                comboKind.BringToFront();

                Label labelAmount = new Label();
                labelAmount.Location = new Point((int)(392 * _xScale), (int)(yOffset + 2 * _yScale));
                labelAmount.Size = new Size((int)(58 * _xScale), (int)(17 * _yScale));
                labelAmount.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                labelAmount.AutoSize = true;
                labelAmount.ForeColor = System.Drawing.Color.Black;
                labelAmount.Name = "labelAmount" + rowNumber;
                this.Controls.Add(labelAmount);
                labelAmount.Text = Strings.Get("Amount");
                labelAmount.SendToBack();

                TextBox textBoxAmount = new TextBox();
                textBoxAmount.Location = new Point((int)(458 * _xScale), (int)(yOffset + 1 * _yScale));
                textBoxAmount.Size = new Size((int)(103 * _xScale), (int)(22 * _yScale));
                textBoxAmount.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                textBoxAmount.AllowDrop = true;
                textBoxAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
                textBoxAmount.TextChanged += new System.EventHandler(this.textBoxAmount_TextChanged);
                textBoxAmount.Enter += new System.EventHandler(this.textBoxAmount_Enter);
                textBoxAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAmount_KeyPress);
                this.Controls.Add(textBoxAmount);
                textBoxAmount.Text = "";
                textBoxAmount.BringToFront();

                _labelCategories[rowNumber] = labelCategory;
                _comboCategories[rowNumber] = comboCategory;
                _comboKinds[rowNumber] = comboKind;
                _labelAmounts[rowNumber] = labelAmount;
                _textBoxAmounts[rowNumber] = textBoxAmount;
            }

            _comboCategories[0].TabIndex = 3;
            _comboKinds[0].TabIndex = 4;
            _textBoxAmounts[0].TabIndex = 5;
        }

        /// <summary>
        /// Load region-specific strings.
        /// </summary>
        private void LoadStrings()
        {
            this.Text = Strings.Get("Checkbook Entry");
            labelTransDate.Text = Strings.Get("Transaction Date");
            labelCheckNumber.Text = Strings.Get("Check#");
            labelPayee.Text = Strings.Get("Payee");
            labelTotal.Text = Strings.Get("Total");
            checkBoxCleared.Text = Strings.Get("Cleared / Reconciled");
            buttonDelete.Text = Strings.Get("Delete");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("OK");
            comboBoxPayee.DataSource = _backend.Payees;
            comboBoxPayee.Text = "";
            ValidateReadyForSubmit();
        }

        /// <summary>
        /// Adjust for populated splits.
        /// </summary>
        /// <param name="rowNumber">zero-based number of rows to be displayed, 1...5 for rows 2...6</param>
        private void AdjustSplitRow(int rowNumber)
        {
            rowNumber = Math.Max(1, Math.Min(MaxSplits - 1, rowNumber));
            int yOffset = (int)((rowNumber - 1) * 34 * _yScale);
            Size winSize = new System.Drawing.Size(_winWidth, _winHeight + yOffset);
            this.MinimumSize = winSize;
            this.MaximumSize = winSize;
            this.MinimumSize = winSize;
            this.Size = winSize;
            int controlNumber = 0;
            foreach(Control control in _slidingControls)
            {
                control.Top = _yOffsets[controlNumber++] + yOffset;
            }
            textBoxMemo.Height = (int)(this.Height - (116 * _yScale + textBoxMemo.Top));
            _visibleSplitCount = rowNumber + 1;
            for(int checkRow = 0; checkRow < MaxSplits; ++checkRow)
            {
                _labelCategories[checkRow].Visible = _labelAmounts[checkRow].Visible =
                _comboCategories[checkRow].Visible = _comboCategories[checkRow].Enabled =
                _comboKinds[checkRow].Visible = _comboKinds[checkRow].Enabled =
                _textBoxAmounts[checkRow].Visible = _textBoxAmounts[checkRow].Enabled =
                    (checkRow <= rowNumber);
            }
        }

        /// <summary>
        /// Populate the form for a new entry.
        /// </summary>
        private void PopulateNewEntry()
        {
            _isNewEntry = true;
            this.BackColor = Color.FromArgb(232, 248, 255);
            textBoxAssistance.BackColor = this.BackColor;
            buttonDelete.Enabled = false;
            UpdateVisibilities();
        }

        /// <summary>
        /// Populate the form for editing an existing entry.
        /// </summary>
        private void PopulateExistingEntry()
        {
            datePickerTransaction.Value = _rowCheckbook.DateOfTransaction;
            textBoxCheckNbr.Text = _rowCheckbook.CheckNumber;
            comboBoxPayee.SelectedItem = _rowCheckbook.Payee;
            comboBoxPayee.Text = _rowCheckbook.Payee;
            textBoxMemo.Text = _rowCheckbook.Memo;
            checkBoxCleared.Checked = (_rowCheckbook.IsCleared.Length > 0);
            foreach (TextBox textBox in _textBoxAmounts)
            {
                textBox.Text = "0";
            }
            _splits = _rowCheckbook.Entry.Splits;
            int rowNum = 0;
            for (int index = 0; index < _splits.Length; ++index)
            {
                if(index > 0 && _splits[index].Amount == 0)
                {
                    continue; // only the first split can have a zero amount
                }
                FinancialCategory categ = _backend.Db.GetFinancialCategoryById(_splits[index].CategoryId);
                if (categ == null)
                {
                    categ = UtilityMethods.GetCategoryOrUnknown(_backend.Db, null);
                }
                _comboCategories[rowNum].Text = categ.Name;
                _comboKinds[rowNum].SelectedIndex = (int)_splits[index].Kind;
                _textBoxAmounts[rowNum].Text = UtilityMethods.FormatCurrency(_splits[index].Amount, 3);
                ++rowNum;
            }
            while(rowNum < MaxSplits)
            {
                _comboCategories[rowNum].Text = "";
                _comboKinds[rowNum].SelectedIndex = 0;
                _textBoxAmounts[rowNum].Text = "0";
                ++rowNum;
            }
            this.BackColor = _rowCheckbook.Entry.Amount > 0 ? Color.FromArgb(232, 255, 248) : Color.FromArgb(255, 234, 234);
            textBoxAssistance.BackColor = this.BackColor;
            if (checkBoxCleared.Checked)
            {
                if (!Configuration.Instance.GetAdminMode())
                {
                    for (int index = 0; index < _comboCategories.Length; ++index)
                    {
                        _comboCategories[index].Enabled = false;
                        _comboKinds[index].Enabled = false;
                        _textBoxAmounts[index].Enabled = false;
                    }
                    comboBoxPayee.Enabled = false;
                    textBoxCheckNbr.Enabled = false;
                    datePickerTransaction.Enabled = false;
                    buttonDelete.Enabled = false;
                    checkBoxCleared.Enabled = false;
                }
            }
            foreach (TextBox textBox in _textBoxAmounts)
            {
                AdjustAmountsPerKinds(textBox);
            }
            if(_rowCheckbook.Entry.MadeBy == EntryMadeBy.Reminder)
            {
                _rowCheckbook.Entry.MadeBy = EntryMadeBy.Scheduler;
            }
            UpdateVisibilities();
            ValidateReadyForSubmit();
        }

        /// <summary>
        /// Validate TransactionKind and, if necessary, confirm with the user.
        /// </summary>
        /// <param name="control">The control that sent the event</param>
        private void ValidateChangeOfKind(Control control)
        {
            int splitNumber = GetControlIndex(control);
            if (_rowCheckbook == null || splitNumber < 0)
            {
                return;
            }
            // Some kinds should not be set by the user
            if (_comboKinds[splitNumber].SelectedIndex != (int)TransactionKind.Deposit &&
                _comboKinds[splitNumber].SelectedIndex != (int)TransactionKind.Payment &&
                _comboKinds[splitNumber].SelectedIndex != (int)TransactionKind.Refund)
            {
                if (Configuration.Instance.GetAdminMode() && _backend.Db.InProgress == InProgress.Nothing)
                {
                    NotificationForm form = new NotificationForm(false, "Danger Danger Danger",
                        "Direct modification can corrupt your account data!", true);
                    DialogResult result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        return;
                    }
                }
                if (_recursionLevel > 0)
                {
                    --_recursionLevel;
                    return;
                }
                _recursionLevel = UtilityMethods.Clamp(1, _recursionLevel + 1, 99);
                _comboKinds[splitNumber].SelectedIndex = (int)TransactionKind.Deposit;
                if (_splits != null && _splits.Length > splitNumber)
                {
                    _comboKinds[splitNumber].SelectedIndex = (int)_splits[splitNumber].Kind;
                }
                _recursionLevel = UtilityMethods.Clamp(0, _recursionLevel - 1, 99);
            }
        }

        /// <summary>
        /// Adjust the signs of the amounts, based on the corresponding entry kinds.
        /// </summary>
        /// <param name="control">TextBoxAmount or ComboKind</param>
        private void AdjustAmountsPerKinds(Control control)
        {
            int splitNumber = GetControlIndex(control);
            if (_rowCheckbook == null || splitNumber < 0)
            {
                return;
            }
            TextBox textBox = _textBoxAmounts[splitNumber];
            bool wasEmpty = textBox.Text.Trim().Length < 1;
            long amount = UtilityMethods.ParseCurrency(textBox.Text);
            if (SplitEntry.IsDebit(_comboKinds[splitNumber].SelectedIndex, amount))
            {
                amount = -Math.Abs(amount);
            }
            else if (_comboKinds[splitNumber].SelectedIndex != (int)TransactionKind.Adjustment)
            {
                amount = Math.Abs(amount);
            }
            if (amount == 0L)
            {
                textBox.Text = "0";
            }
            else
            {
                textBox.Text = UtilityMethods.FormatCurrency(amount, 3);
            }
            textBox.ForeColor = amount == 0 ? Color.Blue : (amount < 0 ? Color.Red : Color.Green);
            if(wasEmpty && textBox.Text.Trim().Length > 0)
            {
                textBox.SelectAll(); // reselect
            }
            UpdateVisibilities();
        }

        /// <summary>
        /// Allow the user to type in a new item into a combo box - call this upon the Leave event.
        /// </summary>
        /// <param name="sender">ComboBox</param>
        private void AllowTypedInComboItem(object sender)
        {
            ComboBox comboBox = sender as ComboBox;
            if(comboBox == null)
            {
                return;
            }
            string text = comboBox.Text.Trim();
            if (text == null)
            {
                return;
            }
            text = UtilityMethods.UberCaps(text);
            comboBox.Text = text;
            if (!((List<string>)comboBox.DataSource).Contains(text))
            {
                ((List<string>)comboBox.DataSource).Add(text);
                comboBox.BindingContext = new BindingContext();
            }
            int index = ((List<string>)comboBox.DataSource).IndexOf(text);
            comboBox.SelectedItem = text;
        }

        /// <summary>
        /// Enable/disable controls.
        /// </summary>
        private void UpdateVisibilities()
        {
            labelMemoOverlay.Visible = (textBoxMemo.Text.Length < 3);
            int row = 1; // zero-based, so this defaults to 2, the second row is always visible
            while (row < MaxSplits - 1 && _comboCategories[row].Text.Trim().Length > 0 && UtilityMethods.ParseCurrency(_textBoxAmounts[row].Text) != 0)
            {
                ++row;
            }
            AdjustSplitRow(row);
            long total = 0L;
            for (int index = 0; index < _textBoxAmounts.Length; ++index)
            {
                long amount = UtilityMethods.ParseCurrency(_textBoxAmounts[index].Text);
                total = total + amount;
                if(amount == 0 && index > 0)
                {
                    break;
                }
            }
            textBoxTotalAmt.Text = UtilityMethods.FormatCurrency(total, 3);
            textBoxTotalAmt.ForeColor = total == 0 ? Color.Blue : (total < 0 ? Color.Red : Color.Green);
            buttonOk.Enabled = ValidateReadyForSubmit();
        }

        /// <summary>
        /// Check that everything is entered properly for a proper checkbook entry.
        /// </summary>
        /// <returns>true if ready to be submitted</returns>
        private bool ValidateReadyForSubmit()
        {
            this.AcceptButton = null;
            textBoxAssistance.Text = "";
            if (_textBoxAmounts == null || _textBoxAmounts.Length == 0 || _textBoxAmounts[MaxSplits-1] == null)
            {
                return false; // still loading
            }
            _readyForSubmit = true;
            bool isDeposit = _comboKinds[0].SelectedIndex == (int)TransactionKind.Deposit;
            bool isPayment = _comboKinds[0].SelectedIndex == (int)TransactionKind.Payment;
            for (int index = 1; index < _comboKinds.Length; ++index)
            {
                if (_comboCategories[index].Text.Length > 0 && _textBoxAmounts[index].Text.Length > 0)
                {
                    if (isDeposit && _comboKinds[index].SelectedIndex == (int)TransactionKind.Payment ||
                        isPayment && _comboKinds[index].SelectedIndex == (int)TransactionKind.Deposit)
                    {
                        _readyForSubmit = false;
                        textBoxAssistance.Text = Strings.Get("Cannot mix Payments and Deposits in the same entry");
                    }
                }
            }
            _readyForSubmit = false;
            for (int rowNumber = 0; rowNumber < _visibleSplitCount; ++rowNumber)
            {
                if(_comboCategories[rowNumber].Text.Trim().Length > 0 && _textBoxAmounts[rowNumber].Text.Trim().Length > 0)
                {
                    _readyForSubmit = true;
                }
                if(UtilityMethods.ParseCurrency(_textBoxAmounts[rowNumber].Text) != 0 && _comboCategories[rowNumber].Text.Trim().Length < 1)
                {
                    _readyForSubmit = false;
                    break;
                }
            }
            if(!_readyForSubmit)
            { 
                textBoxAssistance.Text = Strings.Get("Category and amount required");
            }
            if (comboBoxPayee.Text.Trim().Length == 0)
            {
                _readyForSubmit = false;
                textBoxAssistance.Text = Strings.Get("Payee must be specified");
            }
            if (checkBoxCleared.Checked && !Configuration.Instance.GetAdminMode())
            {
                textBoxAssistance.Text = Strings.Get("Reconciled/cleared entry - cannot be changed");
                if (Configuration.Instance.GetAdminMode())
                {
                    textBoxAssistance.Text = Strings.Get("Reconciled/cleared entry - DO NOT CHANGE ANYTHING");
                }
            }
            this.AcceptButton = _readyForSubmit ? buttonOk : null;
            return _readyForSubmit;
        }

        /// <summary>
        /// Only allow keypresses for currency to be entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateCurrencyKey(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.') && (e.KeyChar != '(') && (e.KeyChar != ')') && (e.KeyChar != '-') &&
                (e.KeyChar != CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol[0]))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Autofill fields based on Payee.
        /// </summary>
        private void HandlePayeeChanged()
        {
            if (_comboCategories[0].Text.Length > 0)
            {
                ValidateReadyForSubmit();
                return;
            }
            // Populate the first split if we can
            MemorizedPayee payee = _backend.Db.GetMemorizedPayeeByName(comboBoxPayee.Text);
            if (payee == null)
            {
                _comboCategories[0].Text = UtilityMethods.GuessAtCategory(comboBoxPayee.Text);
            }
            else
            {
                comboBoxPayee.Text = payee.Payee;
                FinancialCategory category = _backend.Db.GetFinancialCategoryById(payee.CategoryId);
                if (category != null)
                {
                    _comboCategories[0].Text = category.Name;
                    _comboKinds[0].SelectedIndex = (int)payee.Kind;
                    if (UtilityMethods.ParseCurrency(_textBoxAmounts[0].Text) == 0L)
                    {
                        _textBoxAmounts[0].Text = UtilityMethods.FormatCurrency(payee.Amount, 3);
                    }
                }
            }
            ValidateReadyForSubmit();
        }

        /// <summary>
        /// CheckBoxCleared was changed - deal with it.
        /// </summary>
        private void HandleCheckBoxClearedChanged()
        {
            if (_rowCheckbook == null || _backend.Db.InProgress != InProgress.Nothing)
            {
                return;
            }
            if (Configuration.Instance.GetAdminMode())
            {
                NotificationForm form = new NotificationForm(false, "Danger Danger Danger",
                    "Direct modification can corrupt your account data!", true);
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return;
                }
            }
            checkBoxCleared.Checked = (_rowCheckbook.IsCleared.Length > 0);
        }

        /// <summary>
        /// Get the index of a particular split-row control
        /// </summary>
        /// <param name="control">To be indexed</param>
        /// <returns>the zero-based split row</returns>
        private int GetControlIndex(Control control)
        {
            for (int index = 0; index < _comboKinds.Length; ++index)
            {
                if(_comboKinds[index] == control)
                {
                    return index;
                }
                if (_comboCategories[index] == control)
                {
                    return index;
                }
                if (_textBoxAmounts[index] == control)
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Did the user click "Delete"?
        /// </summary>
        public bool DeleteEntry
        {
            get
            {
                return _deleteEntry;
            }
        }

        /////////////////////////////// Getters //////////////////////////////

        public DateTime DateOfTransaction
        {
            get
            {
                return datePickerTransaction.Value;
            }
        }

        public RowOfCheckbook rowCheckbook
        {
            get
            {
                return _rowCheckbook;
            }
        }

        public string Payee
        {
            get
            {
                return comboBoxPayee.Text.Trim();
            }
        }

        public string CheckNumber
        {
            get
            {
                return textBoxCheckNbr.Text.Trim();
            }
        }

        public int CategoryCount
        {
            get
            {
                int count = 0;
                while (_comboCategories[count].Text.Trim().Length > 0)
                {
                    long amount = UtilityMethods.ParseCurrency(_textBoxAmounts[count].Text) ;
                    if(amount == 0 && count > 0)
                    {
                        break;
                    }
                    ++count;
                }
                return count;
            }
        }

        public string Category(int index)
        {
            if(index < 0 || index >= _visibleSplitCount)
            {
                return "";
            }
            return _comboCategories[index].Text;
        }

        public int Kind(int index)
        {
            if (index < 0 || index >= _visibleSplitCount)
            {
                return 0;
            }
            return _comboKinds[index].SelectedIndex;
        }

        public long Amount(int index)
        {
            if (index < 0 || index >= _visibleSplitCount)
            {
                return 0;
            }
            return UtilityMethods.ParseCurrency(_textBoxAmounts[index].Text);
        }

        public string Memo
        {
            get
            {
                return textBoxMemo.Text;
            }
        }

        public bool Cleared
        {
            get
            {
                return checkBoxCleared.Checked;
            }
        }

        public bool IsNewEntry
        {
            get
            {
                return _isNewEntry;
            }
        }

        /////////////////////////// Event Handlers ///////////////////////////

        private void CheckbookEntryForm_Load(object sender, EventArgs e)
        {
            LoadFormControls();
        }

        private void checkBoxCleared_CheckedChanged(object sender, EventArgs e)
        {
            HandleCheckBoxClearedChanged();
        }

        private void comboBoxPayee_Leave(object sender, EventArgs e)
        {
            AllowTypedInComboItem(sender);
            HandlePayeeChanged();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (!ValidateReadyForSubmit())
            {
                return;
            }
            if(_rowCheckbook.Entry.MadeBy == EntryMadeBy.Scheduler)
            {
                _rowCheckbook.Entry.MadeBy = EntryMadeBy.User;
                _rowCheckbook.Entry.SetHighlight(Highlight.Modified);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBoxMemo_TextChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
        }

        private void textBoxAmount_TextChanged(object sender, EventArgs e)
        {
            AdjustAmountsPerKinds((Control)sender as Control);
            UpdateVisibilities();
            ValidateReadyForSubmit();
        }

        private void comboKind_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender as ComboBox != null)
            {
                ValidateChangeOfKind((ComboBox)sender);
                AdjustAmountsPerKinds((ComboBox)sender);
            }
        }

        private void comboCategory1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
        }

        private void textBoxAmount2_TextChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateReadyForSubmit();
        }

        private void comboCategory2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
        }

        private void comboCategory_Leave(object sender, EventArgs e)
        {
            AllowTypedInComboItem(sender);
        }

        private void textBoxAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender as TextBox != null)
            {
                ValidateCurrencyKey(sender, e);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if(_rowCheckbook.NewEntryRow)
            {
                return; // not allowed on the NewEntryRow entry
            }
            _deleteEntry = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBoxCheckNbr_Enter(object sender, EventArgs e)
        {
            textBoxCheckNbr.SelectAll();
        }

        private void textBoxAmount_Enter(object sender, EventArgs e)
        {
            if (sender as TextBox != null)
            {
                ((TextBox)sender).SelectAll();
            }
        }
    }
}
