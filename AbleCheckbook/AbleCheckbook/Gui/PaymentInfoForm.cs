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
        /// Ctor.
        /// </summary>
        /// <param name="cart">multi-line (2-7 lines) cart description</param>
        public PaymentInfoForm(string[] cart)
        {
            InitializeComponent();
            textBoxShoppingCart.Lines = cart;
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
            this.labelInstructions.Text = Strings.Get("Entries must agree with credit card company data, or your purchase may not go through...");
            this.buttonOk.Text = Strings.Get("Purchase");
            this.buttonCancel.Text = Strings.Get("Cancel");
        }

        private void labelCcCvv2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.Get("CVV2: Amex is 4 digits, Visa/MC/etc are 3 digits"), Strings.Get("Notice"), MessageBoxButtons.OK);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            textBoxErrorMessage.Text = "";
            string custFirstName = textBoxFirstName.Text.Trim();
            string custLastName = textBoxLastName.Text.Trim();
            string custAddress = textBoxStreetAddress.Text.Trim();
            string custApt = textBoxAptNumber.Text.Trim();
            string custCity = textBoxCity.Text.Trim();
            string custState = textBoxState.Text.Trim();
            string custZip = textBoxZip.Text.Trim();
            string custCountry = comboBoxCountryCode.Text.Trim();
            string custPhone = UtilityMethods.OnlyDigits(textBoxPhone.Text.Trim());
            string custEmail = textBoxEmail.Text.Trim();
            string ccNumber = UtilityMethods.OnlyDigits(textBoxCcNumber.Text);
            string ccType = UtilityMethods.CreditCardType(ccNumber);
            string ccExpMonth = comboBoxCcExpMonth.Text.Trim();
            string ccExpYear = comboBoxCcExpYear.Text.Trim();
            string ccCvc2 = textBoxCcCvv2.Text.Trim();
            if(ccExpMonth.Length == 1 && char.IsDigit(ccExpMonth[0]))
            {
                ccExpMonth = "0" + ccExpMonth;
            }
            if (custFirstName.Length < 1 || custLastName.Length < 3 || custAddress.Length < 6 || custCity.Length < 2 ||
                custState.Length < 2 || custZip.Length < 5 || custCountry.Length < 2 || custPhone.Length < 10 || 
                custEmail.Length < 9 || ccType.Length < 1 || ccCvc2.Length < 3 || ccExpMonth.Length < 1 || ccExpYear.Length < 4)
            {
                textBoxErrorMessage.Text = Strings.Get("Please fill out all applicable fields");
                return;
            }
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
