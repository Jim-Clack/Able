using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class PaymentInfoForm : Form
    {

        /// <summary>
        /// Details of transaction.
        /// </summary>
        private string details = "";

        private string itemName = "";

        private long itemCost = 0L;

        private string productDesignator = "";

        /// <summary>
        /// Member scratch variables.
        /// </summary>
        private string custFirstName = "";
        private string custLastName = "";
        private string custAddress = "";
        private string custApt = "";
        private string custCity = "";
        private string custState = "";
        private string custZip = "";
        private string custCountry = "";
        private string custPhone = "";
        private string custEmail = "";
        private string ccNumber = "";
        private string ccType = "";
        private string ccExpMonth = "";
        private string ccExpYear = "";
        private string ccCvc2 = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="itemName">Name of item to purchase</param>
        /// <param name="itemCost">Cost of item to purchase</param>
        public PaymentInfoForm(string itemName, long itemCost)
        {
            this.itemName = itemName;
            this.itemCost = itemCost;
            string cost = UtilityMethods.FormatCurrency(itemCost, 11);
            InitializeComponent();
            textBoxShoppingCart.Lines = new string[]
            {
                Strings.Get("Purchase") + " " + Strings.Get("Activation"),
                Strings.Get("Able Strategies AbleCheckbook"),
                string.Format("{0,-22}{1,11}", itemName, cost),
                string.Format("{0,-22}{1,11}", "", "-----------"),
                string.Format("{0,-22}{1,11}", "Total", cost)
            };
        }

        public void SetDefaults(string first, string last, string address, string city, string state, string zip, string phone, string email)
        {
            textBoxFirstName.Text = first.Trim();
            textBoxLastName.Text = last.Trim();
            textBoxStreetAddress.Text = address.Trim();
            textBoxCity.Text = city.Trim();
            textBoxState.Text = state.Trim();
            textBoxZip.Text = zip.Trim();
            textBoxPhone.Text = UtilityMethods.FormatPhoneNumber(phone.Trim());
            textBoxEmail.Text = email.Trim();
        }

        /// <summary>
        /// Details of transaction.
        /// </summary>
        public string Details
        {
            get
            {
                return details;
            }
        }

        ////////////////////////////// Support ///////////////////////////////

        private string LoadAndCheckScratchVars()
        {
            textBoxErrorMessage.Text = "";
            custFirstName = textBoxFirstName.Text.Trim();
            custLastName = textBoxLastName.Text.Trim();
            custAddress = textBoxStreetAddress.Text.Trim();
            custApt = textBoxAptNumber.Text.Trim();
            custCity = textBoxCity.Text.Trim();
            custState = textBoxState.Text.Trim();
            custZip = textBoxZip.Text.Trim();
            custCountry = comboBoxCountryCode.Text.Trim();
            custPhone = UtilityMethods.OnlyDigits(textBoxPhone.Text.Trim());
            custEmail = textBoxEmail.Text.Trim();
            ccNumber = UtilityMethods.OnlyDigits(textBoxCcNumber.Text);
            ccType = UtilityMethods.CreditCardType(ccNumber);
            ccExpMonth = comboBoxCcExpMonth.Text.Trim();
            ccExpYear = comboBoxCcExpYear.Text.Trim();
            ccCvc2 = textBoxCcCvv2.Text.Trim();
            if (ccExpMonth.Length == 1 && char.IsDigit(ccExpMonth[0]))
            {
                ccExpMonth = "0" + ccExpMonth;
            }
            string badFields =
                UtilityMethods.LengthNotBetween(labelFirstName.Text, custFirstName, 1, 100) +
                UtilityMethods.LengthNotBetween(labelLastName.Text, custLastName, 3, 100) +
                UtilityMethods.LengthNotBetween(labelStreetAddress.Text, custAddress, 6, 100) +
                UtilityMethods.LengthNotBetween(labelCity.Text, custCity, 2, 100) +
                UtilityMethods.LengthNotBetween(labelState.Text, custState, 2, 12) +
                UtilityMethods.LengthNotBetween(labelZip.Text, custZip, 5, 10) +
                UtilityMethods.LengthNotBetween(labelCountryCode.Text, custCountry, 2, 12) +
                UtilityMethods.LengthNotBetween(labelPhone.Text, custPhone, 10, 20) +
                UtilityMethods.LengthNotBetween(labelEmail.Text, custEmail, 8, 40) +
                UtilityMethods.LengthNotBetween(labelCcNumber.Text, ccType, 1, 20) +
                UtilityMethods.LengthNotBetween(labelCcCvv2.Text, ccCvc2, 1, 4) +
                UtilityMethods.LengthNotBetween(labelCcExpMonth.Text, ccExpMonth, 2, 2) +
                UtilityMethods.LengthNotBetween(labelCcExpYear.Text, ccExpYear, 4, 4);
            return badFields;
        }

        /////////////////////////// Event Handlers ///////////////////////////

        private void PaymentInfoForm_Load(object sender, EventArgs e)
        {
            int thisYear = DateTime.Now.Year;
            comboBoxCcExpYear.Items.Clear();
            for (int year = 0; year < 20; ++year)
            {
                comboBoxCcExpYear.Items.Add("" + (thisYear + year));
            }
            comboBoxCountryCode.Select(0, 1);
            this.Text = Strings.Get("Payment Info");
            this.labelFirstName.Text = Strings.Get("First Name on CC");
            this.labelLastName.Text = Strings.Get("Last Name on CC");
            this.labelStreetAddress.Text = Strings.Get("Street Address");
            this.labelAptNumber.Text = Strings.Get("Apt Number");
            this.labelCity.Text = Strings.Get("City (per USPS)");
            this.labelState.Text = Strings.Get("State (2-letters)");
            this.labelZip.Text = Strings.Get("Zip/Postal Code");
            this.labelCountryCode.Text = Strings.Get("Country Code");
            this.labelPhone.Text = Strings.Get("Phone (10+ digits)");
            this.labelEmail.Text = Strings.Get("Email Address");
            this.labelCcNumber.Text = Strings.Get("Credit Card Num");
            this.labelCcCvv2.Text = Strings.Get("CVV2 (Help)");
            this.labelCcExpMonth.Text = Strings.Get("Expiration Month");
            this.labelCcExpYear.Text = Strings.Get("Expiration Year");
            this.labelInstructions.Text = Strings.Get("Entries must agree with credit card billing info, or your purchase may not go through...");
            Image image = buttonOk.BackgroundImage;
            if (image == null || image.Width < 10) // just in case
            {
                this.buttonOk.Text = Strings.Get("Purchase");
            }
            this.buttonCancel.Text = Strings.Get("Cancel");
            this.toolTipPopups.SetToolTip(buttonOk, Strings.Get("Click here to make the Purchase"));
        }

        private void labelCcCvv2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.Get("CVV2: Amex is 4 digits, Visa/MC/etc are 3 digits"), Strings.Get("Notice"), MessageBoxButtons.OK);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            string badFields = LoadAndCheckScratchVars();
            if (badFields.Length > 0)
            {
                textBoxErrorMessage.Text = Strings.Get("Please fill out applicable fields:") + "   " + badFields;
                return;
            }
            details = DateTime.Now.ToString() + " " + itemName + " " + UtilityMethods.FormatCurrency(itemCost) + "\n " +
                custFirstName + " " + custLastName + ", " + UtilityMethods.FormatPhoneNumber(custPhone) + ", " + custEmail + "\n " +
                custAddress + " " + custApt + ", " + custCity + " " + custState + " " + custZip + " " + custCountry + "\n " +
                ccType + " xxxx" + ccNumber.Substring(11) + ", " + ccExpMonth + "/" + ccExpYear;

            // TODO - make purchase

            details += " " + productDesignator;
        }

        private void textBoxAptNumber_Leave(object sender, EventArgs e)
        {
            string apt = textBoxAptNumber.Text.Trim();
            if(apt.Length < 1)
            {
                return;
            }
            if(char.IsDigit(apt[0]))
            {
                textBoxAptNumber.Text = Strings.Get("Apt") + " " + apt;
            }
        }

        private void textBoxPhone_TextChanged(object sender, EventArgs e)
        {
            textBoxPhone.Text = UtilityMethods.FormatPhoneNumber(textBoxPhone.Text);
        }

        private void textBoxCcNumber_TextChanged(object sender, EventArgs e)
        {
            this.labelCcNumber.Text = Strings.Get("Credit Card Num");
            string ccNum = UtilityMethods.OnlyDigits(textBoxCcNumber.Text);
            textBoxCcNumber.Text = UtilityMethods.FormatCreditCardNumber(ccNum);
            string cardType = UtilityMethods.CreditCardType(ccNum);
            if (cardType.Length > 0)
            {
                this.labelCcNumber.Text = Strings.Get(UtilityMethods.CreditCardType(ccNum));
            }
        }

    }

}
