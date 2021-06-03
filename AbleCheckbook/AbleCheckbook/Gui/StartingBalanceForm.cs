using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class StartingBalanceForm : Form
    {

        private long _amount = 999999L;

        public StartingBalanceForm()
        {
            InitializeComponent();
        }

        public long Amount
        {
            get
            {
                return _amount;
            }
        }

        public DateTime AsOfDate
        {
            get
            {
                return dateTimePickerOpeningDate.Value;
            }
        }

        private void StartingBalanceForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("New Account, Starting Balance");
            textBoxInstructions.Text = Strings.Get("Normally this is the closing date and closing balance from your last bank statement");
            buttonOk.Text = Strings.Get("OK");
            labelBalance.Text = Strings.Get("Initial Account Balance:");
            labelNotice.Text = Strings.Get("Starting Balance for New Account...");
            labelAsOf.Text = Strings.Get("As of:");
            DateTime now = DateTime.Now;
            dateTimePickerOpeningDate.Value = (new DateTime(now.Year, now.Month, 1)).AddDays(-1);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if(_amount == 999999L)
            {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.') && (e.KeyChar != '(') && (e.KeyChar != ')') && (e.KeyChar != '-') &&
                (e.KeyChar != CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol[0]))
            {
                e.Handled = true;
            }
        }

        private void textBoxAmount_Leave(object sender, EventArgs e)
        {
            _amount = UtilityMethods.ParseCurrency(textBoxAmount.Text);
            if (_amount == 0L)
            {
                textBoxAmount.Text = "0";
            }
            else
            {
                textBoxAmount.Text = UtilityMethods.FormatCurrency(_amount, 3);
            }
            textBoxAmount.ForeColor = (_amount == 0) ? Color.Blue : (_amount < 0 ? Color.Red : Color.Green);
        }
    }
}
