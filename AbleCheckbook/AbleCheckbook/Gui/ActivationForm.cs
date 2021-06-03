using AbleCheckbook.Logic;
using AbleLicensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Activation");
            labelSiteId.Text = Strings.Get("Site Identification Code");
            labelUserId.Text = Strings.Get("Contact / User Name");
            labelStreetAddress.Text = Strings.Get("Address and Street");
            labelCityState.Text = Strings.Get("City and State/ Province");
            labelPostalCode.Text = Strings.Get("Postal / ZIP Code or CC");
            labelPhoneNumber.Text = Strings.Get("10-12 Digit Phone Nbr");
            labelEmailAddress.Text = Strings.Get("Contact Email Address");
            labelIpAddress.Text = Strings.Get("Computer Identification");
            checkBoxAcceptTerms.Text = Strings.Get("I have read and accept the terms of the EULA");
            linkLabelEula.Text = Strings.Get("EULA - End User License Agreement");
            buttonActivate.Text = Strings.Get("Activate");
            labelSiteDescription.Text = Strings.Get("Assigned site description");
            labelPin.Text = Strings.Get("Offline / Manual Activation PIN");
            textBoxSiteId.Text = Activation.Instance.SiteIdentification;
            textBoxUserId.Text = System.Environment.UserName;
            textBoxIpAddress.Text = System.Environment.UserDomainName;
            UserLevel userLevel = Configuration.Instance.GetUserLevel();
            if(userLevel != UserLevel.Unlicensed)
            {
                if(MessageBox.Show(this, Strings.Get("Already activated - Do you wish to continue?"), 
                    Strings.Get("Warning"), MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    Close();
                }
            }
        }

        private void OnlineActivation()
        {
            MessageBox.Show(Strings.Get("Use manual activation - Call support for a PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
            textBoxSiteDescription.Enabled = true;
            textBoxPin.Enabled = true;
        }

        private void ManualActivation()
        {
            string prevLicensedTo = Configuration.Instance.SiteDescription;
            string prevPin = Configuration.Instance.ActivationPin;
            Configuration.Instance.SiteDescription = textBoxSiteDescription.Text.Trim();
            Configuration.Instance.ActivationPin = textBoxPin.Text.Trim();
            UserLevel userLevel = Configuration.Instance.GetUserLevel();
            if (userLevel == UserLevel.Unlicensed)
            {
                Configuration.Instance.SiteDescription = prevLicensedTo;
                Configuration.Instance.ActivationPin = prevPin;
                MessageBox.Show(this, Strings.Get("Invalid Description and/or PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            else
            {
                MessageBox.Show(this, Strings.Get("Activated") + " " + userLevel, Strings.Get("Success"), MessageBoxButtons.OK);
                Configuration.Instance.Save();
                Close();
            }
        }

        private void buttonActivate_Click(object sender, EventArgs e)
        {
            if(textBoxUserId.Text.Trim().Length < 8)
            {
                MessageBox.Show(this, Strings.Get("User name too short"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxStreetAddress.Text.Trim().Length < 5 || textBoxCityState.Text.Trim().Length < 5)
            {
                MessageBox.Show(this, Strings.Get("Address and City Needed"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxPostalCode.Text.Trim().Length < 5)
            {
                MessageBox.Show(this, Strings.Get("Invalid postal/CC code"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxPhoneNumber.Text.Trim().Length + textBoxEmailAddress.Text.Trim().Length < 10)
            {
                MessageBox.Show(this, Strings.Get("Phone and email required"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (!checkBoxAcceptTerms.Checked)
            {
                MessageBox.Show(this, Strings.Get("You must accept the EULA"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxPin.Text.Trim().Length > 3)
            {
                if (textBoxSiteDescription.Text.Trim().Length != 12)
                {
                    MessageBox.Show(this, Strings.Get("Call support for Description"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                if (textBoxPin.Text.Trim().Length != 4)
                {
                    MessageBox.Show(this, Strings.Get("Call for an Activation PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                ManualActivation();
            }
            else
            {
                OnlineActivation();
            }
        }
    }
}
