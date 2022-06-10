using AbleCheckbook.Logic;
using AbleLicensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Able Strategies");
            textBoxHeading.Text = Strings.Get("Able Strategies AbleCheckbook - See Terms of License");
            textBoxIpAddress.Text = System.Environment.UserDomainName;
            textBoxUserLevel.Text = Configuration.Instance.GetUserLevel().ToString();
            textBoxSiteId.Text = Activation.Instance.SiteIdentification;
            textBoxSiteDesc.Text = Activation.Instance.LicenseCode;
            textBoxUserId.Text = System.Environment.UserName;
            Assembly assembly = Assembly.GetExecutingAssembly();
            textBoxVersion.Text = Strings.Get("Version: ") + AbleCheckbook.Logic.Version.AppVersion;
            linkLabelEula.Text = Strings.Get("EULA - End User License Agreement");
            int expDays = Activation.Instance.GetExpirationDays();
            if (expDays < 0)
            {
                textBoxSiteDesc.Text = Strings.Get("Expired") + " " + Math.Abs(expDays) + Strings.Get(" days");
            }
            textBoxHeading.Select(0, 0);

            // JBC - NO, for debugging only...
   OnlineActivationClient onlineClient = new OnlineActivationClient();
   bool ok = onlineClient.CallServerToCheckConnection();

        }

        private void linkLabelEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
                "This is an Alpha test version to be used only for that purpose and remains the " +
                "property of Able Strategies. No warranties are expressed or implied and it is " +
                "expected that there will be bugs and inconsistencies as the product evolves. " +
                "Any attempt to engineer or thwart the security of the application is prohibited. " +
                "We thank you for your participation in the Alpha Test program.", "EULA", MessageBoxButtons.OK);
        }
    }
}
