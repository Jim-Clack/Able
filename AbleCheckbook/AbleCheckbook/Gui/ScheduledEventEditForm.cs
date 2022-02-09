using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using AbleCheckbook.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace AbleCheckbook.Gui
{
    public partial class ScheduledEventEditForm : Form
    {

        /// <summary>
        /// The original event, if any.
        /// </summary>
        private ScheduledEvent _schEvent = null;

        /// <summary>
        /// Corresponding row of datagridview.
        /// </summary>
        private RowOfSchEvents _rowEvent = null;

        /// <summary>
        /// Was this a new event or an edit of an existing one?
        /// </summary>
        private bool _isNewEvent = false;

        /// <summary>
        /// True if the Delete button was clicked.
        /// </summary>
        private bool _userRequestedDelete = false;

        /// <summary>
        /// DB and support methods.
        /// </summary>
        private CheckbookRegisterBackend _backend = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="backend">Backend for DB and support methods</param>
        /// <param name="schEvent">Original scheduled event; null for a new event.</param>
        public ScheduledEventEditForm(CheckbookRegisterBackend backend, RowOfSchEvents rowEvent)
        {
            _backend = backend;
            _rowEvent = rowEvent;
            InitializeComponent();
            if (rowEvent != null)
            {
                _schEvent = rowEvent.GetScheduledEvent();
            }
            else
            {
                _isNewEvent = true;
                _schEvent = new ScheduledEvent();
                _schEvent.DayOfMonthBits = 0L;
                _schEvent.DayOfWeekBits = 0;
                _schEvent.EndingDate = DateTime.Now.AddMonths(11);
                _schEvent.FinalPaymentAmount = 0L;
                _schEvent.IsOddWeeksOnly = false;
                _schEvent.IsEstimatedAmount = false;
                _schEvent.IsReminder = true;
                _schEvent.LastPosting = DateTime.Now.AddHours(-24);
                _schEvent.MonthOfYear = 1;
                _schEvent.WeekOfMonth = 1;
                _schEvent.Period = SchedulePeriod.Monthly;
                _schEvent.Memo = "";
                tabControl.SelectedTab = tabPageMonthly;
            }
        }

        /// <summary>
        /// Did the user request that this event be deleted?
        /// </summary>
        public bool RequestedDelete
        {
            get
            {
                return _userRequestedDelete;
            }
        }

        /// <summary>
        /// Assemble and return the resultant scheduled event.
        /// </summary>
        /// <returns>The populated event, null on error</returns>
        public ScheduledEvent GetEvent()
        {
            if (!ValidateControls())
            {
                return null;
            }
            bool isReminder = true;
            DateTime finalDate = ScheduledEvent.Eternity;
            if (radioButtonFinal.Checked)
            {
                finalDate = dateTimePickerFinal.Value;
            }
            _schEvent.MonthOfYear = listBoxMonth2.SelectedIndex;
            DateTime currentLastPosting = _schEvent.LastPosting;
            if (tabControl.SelectedTab == tabPageMonthly)
            {
                _schEvent.SetDueMonthly(isReminder,
                    GetBitMask(listBoxDaysOfMonth1), finalDate);
            }
            else if (tabControl.SelectedTab == tabPageAnnually)
            {
                _schEvent.SetDueAnnually(isReminder,
                    GetFirstIndex(listBoxDaysOfMonth2), listBoxMonth2.SelectedIndex + 1, finalDate);
            }
            else if (tabControl.SelectedTab == tabPageWeekly)
            {
                if (listBoxDaysOfWeek3.SelectedIndices.Count == 1)
                {
                    _schEvent.SetDueWeekly(isReminder,
                        listBoxDaysOfWeek3.SelectedIndex, finalDate);
                }
            }
            else if (tabControl.SelectedTab == tabPageMonthlySsa)
            {
                if (listBoxDayOfWeek4.SelectedIndices.Count == 1 && listBoxNthOccurrence.SelectedIndices.Count == 1)
                {
                    _schEvent.SetDueMonthlySsa(isReminder,
                        listBoxDayOfWeek4.SelectedIndex, listBoxNthOccurrence.SelectedIndex, finalDate);
                }
            }
            else if (tabControl.SelectedTab == tabPageBiWeekly)
            {
                if (listBoxDayOfWeek5.SelectedIndices.Count == 1)
                {
                    _schEvent.SetDueBiWeekly(isReminder,
                        listBoxDayOfWeek5.SelectedIndex, dateTimePickerNextOccurrence5.Value, finalDate);
                }
            }
            else if (tabControl.SelectedTab == tabPageDaysApart)
            {
                    _schEvent.SetDueDaysApart(isReminder,
                        (int)numericUpDownDaysApart6.Value, dateTimePickerNextOccurrence6.Value, finalDate);
            };
            if (!_isNewEvent)
            {
                _schEvent.LastPosting = currentLastPosting; // preserved during a mod
            }
            if (radioButtonOccurrences.Checked)
            {
                _schEvent.SetRepeatCount((int)numericUpDownOccurrences.Value);
            }
            _schEvent.IsCredit = (comboBoxDebitCredit.SelectedIndex > 0);
            _schEvent.Payee = comboBoxPayee.Text.Trim();
            _schEvent.CategoryName = comboBoxCategory.Text.Trim();
            _schEvent.Amount = UtilityMethods.ParseCurrency(textBoxAmount.Text);
            _schEvent.IsEstimatedAmount = checkBoxEstimate.Checked;
            _schEvent.Memo = textBoxMemo.Text.Trim();
            _schEvent.IsReminder = checkBoxReminder.Checked;
            if (textBoxFinalPayment.Text.Trim().Length < 3)
            {
                _schEvent.FinalPaymentAmount = 0;
            }
            else
            {
                _schEvent.FinalPaymentAmount = UtilityMethods.ParseCurrency(textBoxFinalPayment.Text);
            }
            if(radioButtonDisabled.Checked)
            {
                _schEvent.EndingDate = new DateTime(0L);
            }
            return _schEvent;
        }

        /// <summary>
        /// Original event.
        /// </summary>
        /// <returns>The original event, null on error</returns>
        public ScheduledEvent GetEventBeforeEdit()
        {
            if(_rowEvent == null)
            {
                return null;
            }
            return _rowEvent.GetScheduledEventBeforeEdit();
        }


        /// <summary>
        /// Check the UI to ensure that user selections are valid, updating labelNotice.Text  in the process.
        /// </summary>
        /// <returns>true if all is well, false otherwise</returns>
        private bool ValidateControls()
        {
            labelNotice.Text = "";
            if (tabControl.SelectedTab == tabPageMonthly)
            {
                if (listBoxDaysOfMonth1.SelectedIndices.Count < 1)
                {
                    labelNotice.Text = Strings.Get("Select Day(s) of Month");
                    return false;
                }
                labelMultipleDays.Visible = (listBoxDaysOfMonth1.SelectedIndices.Count > 1);
            }
            else if (tabControl.SelectedTab == tabPageAnnually)
            {
                if (listBoxMonth2.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Month");
                    return false;
                }
                if (listBoxDaysOfMonth2.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Day of Month");
                    return false;
                }
            }
            else if (tabControl.SelectedTab == tabPageWeekly)
            {
                if (listBoxDaysOfWeek3.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Day of Week");
                    return false;
                }
            }
            else if (tabControl.SelectedTab == tabPageMonthlySsa)
            {
                if (listBoxDayOfWeek4.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Day of Week");
                    return false;
                }
                if (listBoxNthOccurrence.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Week of Month");
                    return false;
                }
            }
            else if (tabControl.SelectedTab == tabPageBiWeekly)
            {
                if (listBoxDayOfWeek5.SelectedIndices.Count != 1)
                {
                    labelNotice.Text = Strings.Get("Select Day of Week");
                    return false;
                }
                if (dateTimePickerNextOccurrence5.Value.Date < DateTime.Now.Date)
                {
                    labelNotice.Text = Strings.Get("Invalid Next Date");
                    return false;
                }
                if (dateTimePickerNextOccurrence5.Value.Date.Subtract(DateTime.Now.Date).Days > 14)
                {
                    labelNotice.Text = Strings.Get("Must be Within 2 Wks");
                    return false;
                }
                int dayOfWeek = Array.IndexOf(Enum.GetNames(typeof(DayOfWeek)), dateTimePickerNextOccurrence5.Value.Date.DayOfWeek.ToString());
                if (dayOfWeek != listBoxDayOfWeek5.SelectedIndex)
                {
                    labelNotice.Text = Strings.Get("Next Date Not ") + listBoxDayOfWeek5.SelectedItem.ToString();
                    return false;
                }
            }
            else if (tabControl.SelectedTab == tabPageDaysApart)
            {
                if (dateTimePickerNextOccurrence6.Value.Date < DateTime.Now.Date)
                {
                    labelNotice.Text = Strings.Get("Invalid Next Date");
                    return false;
                }
                if (dateTimePickerNextOccurrence5.Value.Date.Subtract(DateTime.Now.Date).Days > 364)
                {
                    labelNotice.Text = Strings.Get("Must be Within 1 Year");
                    return false;
                }
            };
            if (comboBoxPayee.Text.Trim().Length < 1)
            {
                labelNotice.Text = Strings.Get("Must Specify Payee");
                return false;
            }
            if (comboBoxCategory.Text.Trim().Length < 1)
            {
                labelNotice.Text = Strings.Get("Category Required");
                return false;
            }
            if (comboBoxDebitCredit.Text.Trim().Length < 1)
            {
                labelNotice.Text = Strings.Get("Set Debit/Credit");
                return false;
            }
            if(textBoxAmount.Text.Trim().Length == 0)
            {
                labelNotice.Text = Strings.Get("Amount is Needed");
                return false;
            }
            if (radioButtonOccurrences.Checked)
            {
                int occurrences = (int)numericUpDownOccurrences.Value;
                if(occurrences < 1 || occurrences > ScheduledEvent.MaxOccurrences)
                {
                    labelNotice.Text = Strings.Get("Set Occurrences Count");
                    return false;
                }
            }
            if(radioButtonFinal.Checked)
            {
                if(dateTimePickerFinal.Value.Date < DateTime.Now.Date)
                {
                    labelNotice.Text = Strings.Get("Invalid Final Date");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Update the UI controls.
        /// </summary>
        private void UpdateVisibilities()
        {
            numericUpDownOccurrences.Enabled = radioButtonOccurrences.Checked;
            dateTimePickerFinal.Enabled = radioButtonFinal.Checked;
            radioButtonDisabled.ForeColor = radioButtonDisabled.Checked ? Color.Red : SystemColors.ControlText;
        }

        /// <summary>
        /// Set up UI per _schEvent.
        /// </summary>
        private void PopulateControls()
        {
            int maxOccurrences = 360;
            switch(_schEvent.Period)
            {
                case SchedulePeriod.Monthly:
                    tabControl.SelectedTab = tabPageMonthly;
                    break;
                case SchedulePeriod.Annually:
                    tabControl.SelectedTab = tabPageAnnually;
                    maxOccurrences = 30;
                    break;
                case SchedulePeriod.Weekly:
                    tabControl.SelectedTab = tabPageWeekly;
                    break;
                case SchedulePeriod.MonthlySsa:
                    tabControl.SelectedTab = tabPageMonthlySsa;
                    break;
                case SchedulePeriod.BiWeekly:
                    tabControl.SelectedTab = tabPageBiWeekly;
                    break;
                case SchedulePeriod.DaysApart:
                    tabControl.SelectedTab = tabPageDaysApart;
                    break;
            }
            SetSelections(listBoxDaysOfMonth1, _schEvent.DayOfMonthBits);
            SetSelections(listBoxDaysOfMonth2, _schEvent.DayOfMonthBits);
            SetSelections(listBoxDaysOfWeek3, _schEvent.DayOfWeekBits);
            SetSelections(listBoxDayOfWeek4, _schEvent.DayOfWeekBits);
            SetSelections(listBoxDayOfWeek5, _schEvent.DayOfWeekBits);
            listBoxNthOccurrence.SelectedIndex = _schEvent.WeekOfMonth;
            listBoxMonth2.SelectedIndex = _schEvent.MonthOfYear;
            numericUpDownDaysApart6.Value = _schEvent.DaysApart;
            DateTime dateTime = _schEvent.DueNext();
            if (dateTime < new DateTime(1970, 1, 1))
            {
                dateTime = DateTime.Now;
            }
            dateTimePickerNextOccurrence5.Value = dateTime;
            dateTimePickerNextOccurrence6.Value = dateTime;
            string[] kinds = new string[]
            {
                Strings.Get("Payment"),
                Strings.Get("Deposit"),
            };
            comboBoxDebitCredit.DataSource = kinds;
            comboBoxPayee.DataSource = _backend.Payees;
            comboBoxCategory.DataSource = _backend.Categories;
            textBoxAmount.Text = UtilityMethods.FormatCurrency(_schEvent.Amount);
            textBoxMemo.Text = _schEvent.Memo;
            if (_schEvent.FinalPaymentAmount == 0)
            {
                textBoxFinalPayment.Text = "";
            }
            else
            {
                textBoxFinalPayment.Text = UtilityMethods.FormatCurrency(_schEvent.FinalPaymentAmount);
            }
            checkBoxEstimate.Checked = _schEvent.IsEstimatedAmount;
            radioButtonForever.Checked = true;
            radioButtonOccurrences.Checked = false;
            radioButtonFinal.Checked = false;
            dateTimePickerFinal.Value = _schEvent.EndingDate.Date.Ticks <= 0 ? DateTime.Now : _schEvent.EndingDate;
            numericUpDownOccurrences.Value = 0;
            int occurrences = _schEvent.GetRepeatCount(DateTime.Now);
            occurrences = UtilityMethods.Clamp(0, occurrences, maxOccurrences);
            if (occurrences > 0)
            {
                numericUpDownOccurrences.Minimum = 0;
                numericUpDownOccurrences.Maximum = maxOccurrences;
                numericUpDownOccurrences.Value = occurrences;
                radioButtonForever.Checked = false;
                radioButtonFinal.Checked = true;
            }
            radioButtonDisabled.Checked = _schEvent.EndingDate.Date.Ticks <= 0L;
            comboBoxCategory.Text = "";
            comboBoxDebitCredit.Text = "";
            if (_schEvent.Payee.Trim().Length < 1)
            {
                comboBoxPayee.Text = "";
            }
            else
            {
                comboBoxPayee.SelectedItem = _schEvent.Payee;
                comboBoxPayee.Text = _schEvent.Payee;
            }
            comboBoxCategory.SelectedItem = _schEvent.CategoryName;
            comboBoxCategory.Text = _schEvent.CategoryName;
            comboBoxDebitCredit.SelectedItem = comboBoxDebitCredit.Text  = kinds[_schEvent.IsCredit ? 1 : 0];
            comboBoxDebitCredit.SelectedIndex = _schEvent.IsCredit ? 1 : 0;
            checkBoxReminder.Checked = _schEvent.IsReminder;
            dateTimePickerFinal.ShowUpDown = !Configuration.Instance.ShowCalendars;
            dateTimePickerNextOccurrence5.ShowUpDown = !Configuration.Instance.ShowCalendars;
            ValidateControls();
        }

        /// <summary>
        /// Set/clear the selected items in a list box based on a bit-map.
        /// </summary>
        /// <param name="listBox">To be updated</param>
        /// <param name="bits">bit-map</param>
        private void SetSelections(ListBox listBox, long bits)
        {
            listBox.SelectedIndices.Clear();
            long mask = 1L;
            for(int index = 0; index < listBox.Items.Count; ++index)
            {
                if((bits & mask) != 0)
                {
                    listBox.SelectedIndices.Add(index);
                }
                mask = mask << 1;
            }
        }

        /// <summary>
        /// Return the selected one-based index of a list box.
        /// </summary>
        /// <param name="listBox">To be analyzed</param>
        /// <returns>corresponding index 1...n, 0 if none</returns>
        private int GetFirstIndex(ListBox listBox)
        {
            for (int index = 0; index < listBox.Items.Count; ++index)
            {
                if (listBox.SelectedIndices.Contains(index))
                {
                    return index + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Return a bit-mask based on the selected indices of a list box.
        /// </summary>
        /// <param name="listBox">To be analyzed</param>
        /// <returns>corresponding bit-map</returns>
        private long GetBitMask(ListBox listBox)
        {
            long bitmap = 0L;
            long mask = 1L;
            for (int index = 0; index < listBox.Items.Count; ++index)
            {
                if (listBox.SelectedIndices.Contains(index))
                {
                    bitmap = bitmap | mask;
                }
                mask = mask << 1;
            }
            return bitmap;
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
        /// Allow the user to type in a new item into a combo box - call this upon the Leave event.
        /// </summary>
        /// <param name="sender">ComboBox</param>
        private void AllowTypedInComboItem(object sender)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
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
        /// Adjust the signs of the amounts, based on the corresponding debit/credit.
        /// </summary>
        private void AdjustAmounts()
        {
            long amount = Math.Abs(UtilityMethods.ParseCurrency(textBoxAmount.Text));
            if (comboBoxDebitCredit.SelectedIndex < 1)
            {
                amount = -Math.Abs(amount);
            }
            if (amount == 0L)
            {
                textBoxAmount.Text = "0";
            }
            else
            {
                textBoxAmount.Text = UtilityMethods.FormatCurrency(amount, 3);
            }
            textBoxAmount.ForeColor = amount == 0 ? Color.Blue : (amount < 0 ? Color.Red : Color.Green);
            amount = Math.Abs(UtilityMethods.ParseCurrency(textBoxFinalPayment.Text));
            if (comboBoxDebitCredit.SelectedIndex < 1)
            {
                amount = -Math.Abs(amount);
            }
            if (amount == 0L)
            {
                textBoxFinalPayment.Text = "";
            }
            else
            {
                textBoxFinalPayment.Text = UtilityMethods.FormatCurrency(amount, 3);
            }
            textBoxFinalPayment.ForeColor = amount == 0 ? Color.Blue : (amount < 0 ? Color.Red : Color.Green);
            ValidateControls();
        }

        /// <summary>
        /// Load event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScheduledEventEditForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Edit Scheduled Event");
            labelExample1.Text = Strings.Get("i.e. Pay bill on the 17th of each month");
            tabPageAnnually.Text = Strings.Get("Yearly");
            labelExample2.Text = Strings.Get("i.e. Pay bill on April 25 each year");
            tabPageWeekly.Text = Strings.Get("Weekly");
            labelExample3.Text = Strings.Get("i.e. Paycheck deposit every Thursday");
            tabPageMonthlySsa.Text = Strings.Get("Nth Wkday");
            labelExample4.Text = Strings.Get("i.e. SSA check deposit 2nd Thursday of each month");
            tabPageBiWeekly.Text = Strings.Get("Biweekly");
            labelExample5.Text = Strings.Get("i.e. Mortgage payment every other Friday");
            labelNextOccurrence5.Text = Strings.Get("Next occurrence date...");
            checkBoxEstimate.Text = Strings.Get("Estimate");
            groupBoxOccurrences.Text = Strings.Get("Duration");
            radioButtonFinal.Text = Strings.Get("Final:");
            radioButtonOccurrences.Text = Strings.Get("Occurrences:");
            radioButtonForever.Text = Strings.Get("Forever");
            radioButtonDisabled.Text = Strings.Get("Disabled");
            checkBoxReminder.Text = Strings.Get("Highlight Entry as a Reminder");
            labelPayee.Text = Strings.Get("Payee");
            labelCategory.Text = Strings.Get("Category");
            labelKind.Text = Strings.Get("Trans Kind");
            labelAmount.Text = Strings.Get("Amount");
            labelFinalPayment.Text = Strings.Get("Final Payment if Different");
            labelNotice.Text = "";
            labelMemo.Text = Strings.Get("Memo");
            labelMultipleDays.Text = Strings.Get("FYI: Multiple days per month selected.");
            labelMultipleDays.Visible = false;
            buttonOk.Text = Strings.Get("OK");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonDelete.Text = Strings.Get("Delete");
            buttonDelete.Enabled = !_isNewEvent;
            listBoxNthOccurrence.Items.Clear();
            listBoxNthOccurrence.Items.AddRange(new object[] {
                Strings.Get("1st occurrence in month"),
                Strings.Get("2nd occurrence in month"),
                Strings.Get("3rd occurence in month"),
                Strings.Get("4th occurrence in month")});
            listBoxDaysOfWeek3.Items.Clear();
            listBoxDaysOfWeek3.Items.AddRange(new object[] {
                Enum.GetNames(typeof(DayOfWeek))[0].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[1].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[2].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[3].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[4].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[5].ToString(),
                Enum.GetNames(typeof(DayOfWeek))[6].ToString()});
            listBoxDayOfWeek4.Items.AddRange(listBoxDaysOfWeek3.Items);
            listBoxDayOfWeek5.Items.AddRange(listBoxDaysOfWeek3.Items);
            listBoxMonth2.Items.Clear();
            listBoxMonth2.Items.AddRange(new object[] {
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[0],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[1],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[2],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[3],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[4],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[5],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[6],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[7],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[8],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[9],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[10],
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[11]});
            listBoxDaysOfMonth1.Items.Clear();
            listBoxDaysOfMonth1.Items.AddRange(new object[] {
                Strings.Get("1st (first day of month)"),
                Strings.Get("2nd"),
                Strings.Get("3rd"),
                Strings.Get("4th"),
                Strings.Get("5th"),
                Strings.Get("6th"),
                Strings.Get("7th"),
                Strings.Get("8th"),
                Strings.Get("9th"),
                Strings.Get("10th"),
                Strings.Get("11th"),
                Strings.Get("12th"),
                Strings.Get("13th"),
                Strings.Get("14th"),
                Strings.Get("15th"),
                Strings.Get("16th"),
                Strings.Get("17th"),
                Strings.Get("18th"),
                Strings.Get("19th"),
                Strings.Get("20th"),
                Strings.Get("21st"),
                Strings.Get("22nd"),
                Strings.Get("23rd"),
                Strings.Get("24th"),
                Strings.Get("25th"),
                Strings.Get("26th"),
                Strings.Get("27th"),
                Strings.Get("28th"),
                Strings.Get("29th (or 28th in short Feb)"),
                Strings.Get("30th (or last day in Feb)"),
                Strings.Get("31st (or last day of month)")});
            listBoxDaysOfMonth2.Items.AddRange(listBoxDaysOfMonth1.Items);
            labelDaysApart6.Text = Strings.Get("Days Apart");
            labelNextOccurence6.Text = Strings.Get("Next occurrence date...");
            labelDaysApartExample6.Text = Strings.Get("i.e. Every 7 days would occur weekly");
            numericUpDownDaysApart6.Minimum = 1;
            numericUpDownDaysApart6.Maximum = 365;
            PopulateControls();
            UpdateVisibilities();
            ValidateControls();
        }

        /////////////////////////// Event Handlers ///////////////////////////

        private void comboBoxPayee_Leave(object sender, EventArgs e)
        {
            AllowTypedInComboItem(comboBoxPayee);
            ValidateControls();
        }

        private void comboBoxCategory_Leave(object sender, EventArgs e)
        {
            AllowTypedInComboItem(comboBoxCategory);
            ValidateControls();
        }

        private void comboBoxDebitCredit_Leave(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void textBoxAmount_Leave(object sender, EventArgs e)
        {
            AdjustAmounts();
            UpdateVisibilities();
            ValidateControls();
        }

        private void textBoxFinalPayment_Leave(object sender, EventArgs e)
        {
            AdjustAmounts();
            UpdateVisibilities();
            ValidateControls();
        }

        private void textBoxAmount_TextChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
        }

        private void textBoxFinalPayment_TextChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
        }

        private void radioButtonForever_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateControls();
        }

        private void radioButtonOccurrences_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateControls();
        }

        private void radioButtonFinal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateControls();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateControls();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            _userRequestedDelete = !_isNewEvent;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if(!ValidateControls())
            {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listBoxDaysOfMonth1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxMonth2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxDaysOfMonth2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxDaysOfWeek3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxDayOfWeek4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxNthOccurrence_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void listBoxDayOfWeek5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void dateTimePickerNextOccurrence5_ValueChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void comboBoxPayee_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void comboBoxDebitCredit_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustAmounts();
            ValidateControls();
        }

        private void dateTimePickerFinal_ValueChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void numericUpDownOccurrences_ValueChanged(object sender, EventArgs e)
        {
            ValidateControls();
        }

        private void textBoxAmount_Enter(object sender, EventArgs e)
        {
            textBoxAmount.SelectAll();
        }

        private void textBoxFinalPayment_Enter(object sender, EventArgs e)
        {
            textBoxFinalPayment.SelectAll();
        }

        private void radioButtonDisabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVisibilities();
            ValidateControls();
            if(radioButtonDisabled.Checked)
            {
                _schEvent.UpdateLastPostingToNow();
            }
        }
    }

}
