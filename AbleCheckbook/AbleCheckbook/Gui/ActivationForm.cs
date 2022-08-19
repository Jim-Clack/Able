using AbleCheckbook.Logic;
using AbleLicensing;
using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            labelUserId.Text = Strings.Get("Contact/User Name");
            labelStreetAddress.Text = Strings.Get("Address and Street");
            labelCityState.Text = Strings.Get("City and State/Province");
            labelPostalCode.Text = Strings.Get("Postal/ZIP Code or CC");
            labelPhoneNumber.Text = Strings.Get("10-12 Digit Phone Nbr");
            labelEmailAddress.Text = Strings.Get("Contact Email Address");
            labelIpAddress.Text = Strings.Get("Computer Identification");
            checkBoxAcceptTerms.Text = Strings.Get("I have read and accept the terms of the EULA");
            linkLabelEula.Text = Strings.Get("EULA - End User License Agreement");
            buttonActivate.Text = Strings.Get("Activate");
            buttonCancel.Text = Strings.Get("Close");
            buttonReset.Text = Strings.Get("Reset");
            labelLicenseCode.Text = Strings.Get("Assigned License Code");
            labelPin.Text = Strings.Get("Coded Activation PIN");
            labelPurchase.Text = Strings.Get("Purchase designation (if known)");
            labelNotice.Text = Strings.Get("Record the following values in a safe place as your proof of purchase and activation numbers");
            SetToInitialValues();
            UserLevel userLevel = Configuration.Instance.GetUserLevel();
            if (userLevel != UserLevel.Unlicensed)
            {
                if (MessageBox.Show(this, Strings.Get("Already activated - Do you wish to continue?"),
                    Strings.Get("Warning"), MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    Close();
                }
            }
        }

        private void buttonActivate_Click(object sender, EventArgs e)
        {
            if(textBoxUserId.Text.Trim().Length < 6)
            {
                MessageBox.Show(this, Strings.Get("User name too short"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxStreetAddress.Text.Trim().Length < 6 || textBoxCityState.Text.Trim().Length < 6)
            {
                MessageBox.Show(this, Strings.Get("Address and City Needed"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxPostalCode.Text.Trim().Length < 5)
            {
                MessageBox.Show(this, Strings.Get("Invalid postal/CC code"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            if (textBoxPhoneNumber.Text.Trim().Length + textBoxEmailAddress.Text.Trim().Length < 16)
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
                if (textBoxLicenseCode.Text.Trim().Length != 12)
                {
                    MessageBox.Show(this, Strings.Get("Contact support for License Code"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                if (textBoxPin.Text.Trim().Length != 4)
                {
                    MessageBox.Show(this, Strings.Get("Contact support for Activation PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                SaveContactValues();
                Configuration.Instance.Save();
                ActivateManually();
            }
            else
            {
                SaveContactValues();
                Configuration.Instance.Save();
                ActivateOnline();
            }
            Configuration.Instance.Save();
            if (textBoxPin.Text.Trim().Length > 3 && Activation.Instance.IsLicensed)
            {
                labelNotice.Visible = true;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OnlineActivation.Instance.AdjustTimeout(false);
            Close();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            SetToInitialValues();
        }

        //////////////////////////////// Support /////////////////////////////

        private void ActivateOnline()
        {
            // TODO
        }

        private void ActivateManually()
        {
            string originalLicensedTo = Configuration.Instance.LicenseCode;
            string originalPin = Configuration.Instance.ActivationPin;
            Configuration.Instance.LicenseCode = textBoxLicenseCode.Text.Trim();
            Configuration.Instance.ActivationPin = textBoxPin.Text.Trim();
            UserLevel userLevel = Configuration.Instance.GetUserLevel();
            if (userLevel == UserLevel.Unlicensed)
            {
                Configuration.Instance.LicenseCode = originalLicensedTo;
                Configuration.Instance.ActivationPin = originalPin;
                MessageBox.Show(this, Strings.Get("Invalid License Code and/or PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                return;
            }
            else
            {
                MessageBox.Show(this, Strings.Get("Activated") + " " + userLevel, Strings.Get("Success"), MessageBoxButtons.OK);
            }
        }

        private void SetToInitialValues()
        {
            LoadContactValues();
            string licCode = Activation.Instance.LicenseCode.Trim();
            if (string.IsNullOrEmpty(licCode) || licCode.Length != 12)
            {
                licCode = textBoxUserId.Text.Trim() + Activation.Instance.SiteIdentification + DateTime.Now.Millisecond.ToString() + "QGVXY";
                string location = textBoxPostalCode.Text.Trim() + textBoxCityState.Text.Trim() + DateTime.Now.Millisecond.ToString() + "QGVX";
                licCode = Regex.Replace(licCode.Trim().ToUpper(), "[^A-Z0-9]", "X").Substring(0, 6) + (char)(int)UserLevelPunct.Unknown +
                    Regex.Replace(location.Trim().ToUpper(), "[^A-Z0-9-]", "9").Substring(0, 5);
            }
            textBoxLicenseCode.Text = licCode;
            PollAndSetContactValues();
            textBoxSiteId.Text = Activation.Instance.SiteIdentification;
            textBoxUserId.Text = System.Environment.UserName;
            textBoxIpAddress.Text = System.Environment.UserDomainName;
        }

        private void PollAndSetContactValues()
        {
            UserInfoResponse userInfoResponse = 
                OnlineActivation.Instance.Poll(textBoxLicenseCode.Text.Trim(), textBoxSiteId.Text.Trim(), Logic.Version.AppMajor, Logic.Version.AppMinor);
            if(userInfoResponse == null)
            {
                return;
            }
            if(userInfoResponse.ApiState == (int)ApiState.ReturnTimeout)
            {
                OnlineActivation.Instance.AdjustTimeout(true);
            }
            if(userInfoResponse.UserInfos != null && userInfoResponse.UserInfos.Count > 0)
            {
                LoadContactValuesFrom(userInfoResponse.UserInfos.First(), textBoxSiteId.Text);
            }

        }

        private void LoadContactValuesFrom(UserInfo userInfo, string siteId)
        {
            DeviceRecord deviceRecord = null;
            if (!string.IsNullOrEmpty(siteId) && userInfo.DeviceRecords != null)
            {
                foreach (DeviceRecord testDeviceRecord in userInfo.DeviceRecords)
                {
                    if (testDeviceRecord.DeviceSite.Trim() == siteId.Trim())
                    {
                        deviceRecord = testDeviceRecord;
                    }
                }
            }
            if(deviceRecord != null)
            {
                textBoxSiteId.Text = deviceRecord.DeviceSite;
                textBoxPin.Text = deviceRecord.CodesAndPin;
            }
            textBoxUserId.Text = userInfo.LicenseRecord.ContactName;
            textBoxEmailAddress.Text = userInfo.LicenseRecord.ContactEMail;
            textBoxStreetAddress.Text = userInfo.LicenseRecord.ContactAddress;
            textBoxCityState.Text = userInfo.LicenseRecord.ContactCity;
            textBoxPostalCode.Text = userInfo.LicenseRecord.ContactZip;
            textBoxPhoneNumber.Text = userInfo.LicenseRecord.ContactPhone;
            textBoxLicenseCode.Text = userInfo.LicenseRecord.LicenseCode;
            if (userInfo.PurchaseRecords != null && userInfo.PurchaseRecords.Count > 0)
            {
                textBoxPurchase.Text = "P" + userInfo.PurchaseRecords[0].PurchaseTransaction + "|" + userInfo.PurchaseRecords[0].PurchaseVerification;
            }
        }

        private void SaveContactValuesTo(UserInfo userInfo)
        {
            DeviceRecord deviceRecord = null;
            if (userInfo.DeviceRecords == null)
            {
                userInfo.DeviceRecords = new List<DeviceRecord>();
            }
            foreach (DeviceRecord testDeviceRecord in userInfo.DeviceRecords)
            {
                if (testDeviceRecord.DeviceSite.Trim() == textBoxSiteId.Text.Trim())
                {
                    deviceRecord = testDeviceRecord;
                }
            }
            if(deviceRecord == null)
            {
                deviceRecord = new DeviceRecord();
                deviceRecord.CodesAndPin = textBoxPin.Text.Trim();
                deviceRecord.DeviceSite = textBoxSiteId.Text.Trim();
                deviceRecord.UserLevelPunct = 0;
                userInfo.DeviceRecords.Add(deviceRecord);
            }
            if(userInfo.LicenseRecord == null)
            {
                userInfo.LicenseRecord = new LicenseRecord();
            }
            userInfo.LicenseRecord.ContactName = textBoxUserId.Text.Trim();
            userInfo.LicenseRecord.ContactEMail = textBoxEmailAddress.Text.Trim();
            userInfo.LicenseRecord.ContactAddress = textBoxStreetAddress.Text.Trim();
            userInfo.LicenseRecord.ContactCity = textBoxCityState.Text.Trim();
            userInfo.LicenseRecord.ContactZip = textBoxPostalCode.Text.Trim();
            userInfo.LicenseRecord.ContactPhone = textBoxPhoneNumber.Text.Trim();
            userInfo.LicenseRecord.LicenseCode = textBoxLicenseCode.Text.Trim();
            if(textBoxPurchase.Text.Trim().Length > 10 && textBoxPurchase.Text.Contains("|") && textBoxPurchase.Text.StartsWith("P"))
            {
                string[] purchaseFields = textBoxPurchase.Text.Trim().Substring(1).Split('|');
                userInfo.PurchaseRecords[0].PurchaseTransaction = purchaseFields[0];
                userInfo.PurchaseRecords[0].PurchaseVerification = purchaseFields[1];
            }
        }

        private void LoadContactValues()
        {
            string[] contactValues = Configuration.Instance.ContactValues;
            if (contactValues != null && contactValues.Length >= 11)
            {
                textBoxSiteId.Text = contactValues[0];
                textBoxIpAddress.Text = contactValues[1];
                textBoxUserId.Text = contactValues[2];
                textBoxEmailAddress.Text = contactValues[3];
                textBoxStreetAddress.Text = contactValues[4];
                textBoxCityState.Text = contactValues[5];
                textBoxPostalCode.Text = contactValues[6];
                textBoxPhoneNumber.Text = contactValues[7];
                textBoxPurchase.Text = contactValues[8];
                textBoxLicenseCode.Text = contactValues[9];
                textBoxPin.Text = contactValues[10];
            }
        }

        private void SaveContactValues()
        {
            string[] contactValues = new string[11];
            contactValues[0] = textBoxSiteId.Text.Trim();
            contactValues[1] = textBoxIpAddress.Text.Trim();
            contactValues[2] = textBoxUserId.Text.Trim();
            contactValues[3] = textBoxEmailAddress.Text.Trim();
            contactValues[4] = textBoxStreetAddress.Text.Trim();
            contactValues[5] = textBoxCityState.Text.Trim();
            contactValues[6] = textBoxPostalCode.Text.Trim();
            contactValues[7] = textBoxPhoneNumber.Text.Trim();
            contactValues[8] = textBoxPurchase.Text.Trim();
            contactValues[9] = textBoxLicenseCode.Text.Trim();
            contactValues[10] = textBoxPin.Text.Trim();
            Configuration.Instance.ContactValues = contactValues;
        }

        private void linkLabelEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new EulaForm().ShowDialog();
        }
    }

}
