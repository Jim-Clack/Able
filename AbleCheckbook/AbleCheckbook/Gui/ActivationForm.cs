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

        private bool criticalValueChanged = false;

        /// <summary>
        /// Ctor.
        /// </summary>
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
            buttonClose.Text = Strings.Get("Close");
            buttonReset.Text = Strings.Get("Reset");
            labelLicenseCode.Text = Strings.Get("Assigned License Code");
            labelPin.Text = Strings.Get("Coded Activation PIN");
            labelPurchase.Text = Strings.Get("Purchase designator (if known)");
            labelSaveTheseNotice.Text = Strings.Get("Record the following values in a safe place as your proof of purchase and activation numbers");
            labelAlreadyPurchased.Text = Strings.Get("If you have already paid and are licensed, you may fill out your codes in the spaces below");
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
            criticalValueChanged = false;
        }

        /// <summary>
        /// Activate button event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            if(criticalValueChanged)
            {
                if(MessageBox.Show(
                    Strings.Get("You changed a greyed-out critical entry, if you did not wish to do this, click Cancel"),
                    Strings.Get("Verify"), MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    SetToInitialValues();
                    return;
                }
            }
            criticalValueChanged = false;
            if (textBoxPin.Text.Trim().Length > 3)
            {
                if (textBoxLicenseCode.Text.Trim().Length != 12)
                {
                    MessageBox.Show(this, Strings.Get("Improper License Code"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                if (textBoxPin.Text.Trim().Length != 4)
                {
                    MessageBox.Show(this, Strings.Get("Improper Activation PIN"), Strings.Get("Sorry"), MessageBoxButtons.OK);
                    return;
                }
                SaveTextboxValues();
                Configuration.Instance.Save();
                ActivateManually();
            }
            else
            {
                SaveTextboxValues();
                Configuration.Instance.Save();
                if (textBoxPurchase.Text.Trim().Length > 6)
                {
                    ActivateOnline();
                }
                else
                {
                    PurchaseAndActivateOnline();
                }
            }
            SaveTextboxValues();
            Configuration.Instance.Save();
            criticalValueChanged = false;
            bool isLicensed = Activation.Instance.IsLicensed;
            if (textBoxPin.Text.Trim().Length > 3 && isLicensed)
            {
                labelSaveTheseNotice.Visible = true;
                labelAlreadyPurchased.Visible = false;
            }
            MessageBox.Show("------------ " + Strings.Get(isLicensed ? "(Licensed)" : "(Unlicensed)") + " ------------", 
                Strings.Get("Notice"), MessageBoxButtons.OK);
        }

        /// <summary>
        /// Calose buttone event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            OnlineActivation.Instance.AdjustTimeout(false);
            Close();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            SetToInitialValues();
        }

        private void linkLabelEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new EulaForm().ShowDialog();
        }

        private void criticalEntry_TextChanged(object sender, EventArgs e)
        {
            criticalValueChanged = true;
        }

        //////////////////////////////// Support /////////////////////////////

        /// <summary>
        /// If a PIN is specified, call this to activate without a call to the server.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void ActivateOnline()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void PurchaseAndActivateOnline()
        {

        }

        /// <summary>
        /// Report a problem to customer support.
        /// </summary>
        /// <param name="message">Error message, containing HTTP response code if not 200</param>
        /// <param name="userInfoResponse">optional WS response, possibly null</param>
        /// <returns>Error message concatenated with intended next step.</returns>
        private string SendMessageToSupport(string message, UserInfoResponse userInfoResponse)
        {
            string action = "Activating Online";
            if(textBoxPin.Text.Trim().Length > 3)
            {
                action = "Activating Manually";
                if(textBoxPurchase.Text.Trim().Length > 10)
                {
                    action = "Activating an Existing Prior Purchase";
                }
            }
            string apiState = "(null response)";
            string serverMsg = "(none)"; 
            if(userInfoResponse != null)
            {
                apiState = "" + userInfoResponse.ApiState;
                serverMsg = userInfoResponse.Message;
            }
            DateTime now = DateTime.Now;
            string body = "Problem " + action + " - " + now.ToShortDateString() + " " + now.ToShortTimeString() + "...\r\n" +
                " Error:    " + message + "\r\n" +
                " Response: " + serverMsg + "\r\n" +
                " ApiState: " + apiState + "\r\n" +
                " SiteId:   " + textBoxSiteId.Text + "\r\n" +
                " Host:     " + textBoxIpAddress.Text + "\r\n" +
                " UserId:   " + textBoxUserId.Text + "\r\n" +
                " EMail:    " + textBoxEmailAddress.Text + "\r\n" +
                " Address:  " + textBoxStreetAddress.Text + "\r\n" +
                " CitySt:   " + textBoxCityState.Text + "\r\n" +
                " Postal:   " + textBoxPostalCode.Text + "\r\n" +
                " Phone:    " + textBoxPhoneNumber.Text + "\r\n" +
                " PurchDes: " + textBoxPurchase.Text + "\r\n" +
                " License:  " + textBoxLicenseCode.Text + "\r\n" +
                " PIN:      " + textBoxPin.Text + "\r\n";
            string jpgPath = null;
            try
            {
                jpgPath = UiHelperMethods.FormCapture(this);
            }
            catch (Exception)
            {
                jpgPath = null;
            }
            if (jpgPath != null)
            {
                body += " Attached: Activation Form Capture\r\n";
            }
            if(Emailer.SendEmail("ATTENTION - CUSTOMER SUPPORT ISSUE !!!", body, jpgPath))
            {
                message += " NOTE: Problem report was sent to customer support. Please check your email in the next business day or two for a response.";
            }
            else
            {
                message += " NOTE: Cannot contact server at this time. Please contact support by email or phone with the info on the activation screen.";
            }
            return message;
        }

        /// <summary>
        /// Set the textboxes to their initial values.
        /// </summary>
        private void SetToInitialValues()
        {
            labelSaveTheseNotice.Visible = false;
            labelAlreadyPurchased.Visible = true;
            LoadTextboxValues();
            string licCode = Activation.Instance.LicenseCode.Trim();
            if (string.IsNullOrEmpty(licCode) || licCode.Length != 12)
            {
                licCode = textBoxUserId.Text.Trim() + Activation.Instance.SiteIdentification + DateTime.Now.Millisecond.ToString() + "QGVXY";
                string location = textBoxPostalCode.Text.Trim() + textBoxCityState.Text.Trim() + DateTime.Now.Millisecond.ToString() + "QGVX";
                licCode = Regex.Replace(licCode.Trim().ToUpper(), "[^A-Z0-9]", "X").Substring(0, 6) + (char)(int)UserLevelPunct.Unknown +
                    Regex.Replace(location.Trim().ToUpper(), "[^A-Z0-9-]", "9").Substring(0, 5);
            }
            textBoxLicenseCode.Text = licCode;
            PollAndSetTextboxValues();
            textBoxSiteId.Text = Activation.Instance.SiteIdentification;
            if (textBoxUserId.Text.Trim().Length < 6)
            {
                textBoxUserId.Text = textBoxUserId.Text + "X" + System.Environment.UserName.Trim();
            }
            textBoxIpAddress.Text = System.Environment.UserDomainName;
        }

        /// <summary>
        /// Try to call the server, then set the textbox values accordingly.
        /// </summary>
        private void PollAndSetTextboxValues()
        {
            UserInfoResponse userInfoResponse = new Poller().Poll(textBoxLicenseCode.Text.Trim(), textBoxSiteId.Text.Trim());
            if (userInfoResponse != null && userInfoResponse.UserInfos != null && userInfoResponse.UserInfos.Count > 0)
            {
                textBoxLicenseCode.Text = userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode;
                LoadTexboxValuesFrom(userInfoResponse.UserInfos.First(), textBoxSiteId.Text);
            }
        }

        /// <summary>
        /// Load the textbox values from the server response.
        /// </summary>
        /// <param name="userInfo">server response</param>
        /// <param name="siteId">this host's calculated site ID</param>
        private void LoadTexboxValuesFrom(UserInfo userInfo, string siteId)
        {
            DeviceRecord deviceRecord = null;
            if (!string.IsNullOrEmpty(siteId) && userInfo.DeviceRecords != null)
            {
                foreach (DeviceRecord testDeviceRecord in userInfo.DeviceRecords)
                {
                    if (testDeviceRecord.DeviceSiteId.Trim() == siteId.Trim())
                    {
                        deviceRecord = testDeviceRecord;
                    }
                }
            }
            if(deviceRecord != null)
            {
                textBoxSiteId.Text = deviceRecord.DeviceSiteId;
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
                textBoxPurchase.Text = userInfo.PurchaseRecords[0].PurchaseDesignator;
            }
            criticalValueChanged = false;
        }

        /// <summary>
        /// Save the textbox values to a UserInfo object.
        /// </summary>
        /// <param name="userInfo">Must exist - will be populated</param>
        private void SaveTexboxValuesTo(UserInfo userInfo)
        {
            DeviceRecord deviceRecord = null;
            if (userInfo.DeviceRecords == null)
            {
                userInfo.DeviceRecords = new List<DeviceRecord>();
            }
            foreach (DeviceRecord testDeviceRecord in userInfo.DeviceRecords)
            {
                if (testDeviceRecord.DeviceSiteId.Trim() == textBoxSiteId.Text.Trim())
                {
                    deviceRecord = testDeviceRecord;
                }
            }
            if(deviceRecord == null)
            {
                deviceRecord = new DeviceRecord();
                deviceRecord.CodesAndPin = textBoxPin.Text.Trim();
                deviceRecord.DeviceSiteId = textBoxSiteId.Text.Trim();
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
            if(textBoxPurchase.Text.Trim().Length > 10)
            {
                userInfo.PurchaseRecords[0].PurchaseDesignator = textBoxPurchase.Text.Trim();
            }
        }

        /// <summary>
        /// Set the textbox values to what they were the last time the user set them on this machine. 
        /// </summary>
        private void LoadTextboxValues()
        {
            string[] textboxValues = Configuration.Instance.LicenseTextboxValues;
            if (textboxValues != null && textboxValues.Length >= 11)
            {
                textBoxSiteId.Text = textboxValues[0];
                textBoxIpAddress.Text = textboxValues[1];
                textBoxUserId.Text = textboxValues[2];
                textBoxEmailAddress.Text = textboxValues[3];
                textBoxStreetAddress.Text = textboxValues[4];
                textBoxCityState.Text = textboxValues[5];
                textBoxPostalCode.Text = textboxValues[6];
                textBoxPhoneNumber.Text = textboxValues[7];
                textBoxPurchase.Text = textboxValues[8];
                textBoxLicenseCode.Text = textboxValues[9];
                textBoxPin.Text = textboxValues[10];
            }
            criticalValueChanged = false;
        }

        /// <summary>
        /// Save the textbox values to the configuration file so they can be reloaded at a later date.
        /// </summary>
        private void SaveTextboxValues()
        {
            string[] textboxValues = new string[11];
            textboxValues[0] = textBoxSiteId.Text.Trim();
            textboxValues[1] = textBoxIpAddress.Text.Trim();
            textboxValues[2] = textBoxUserId.Text.Trim();
            textboxValues[3] = textBoxEmailAddress.Text.Trim();
            textboxValues[4] = textBoxStreetAddress.Text.Trim();
            textboxValues[5] = textBoxCityState.Text.Trim();
            textboxValues[6] = textBoxPostalCode.Text.Trim();
            textboxValues[7] = textBoxPhoneNumber.Text.Trim();
            textboxValues[8] = textBoxPurchase.Text.Trim();
            textboxValues[9] = textBoxLicenseCode.Text.Trim();
            textboxValues[10] = textBoxPin.Text.Trim();
            Configuration.Instance.LicenseTextboxValues = textboxValues;
        }
    }

}
