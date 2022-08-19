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
            textBoxLicenseCode.Text = Activation.Instance.LicenseCode;
            textBoxUserId.Text = System.Environment.UserName;
            Assembly assembly = Assembly.GetExecutingAssembly();
            textBoxVersion.Text = Strings.Get("Version: ") + AbleCheckbook.Logic.Version.AppVersion;
            linkLabelEula.Text = Strings.Get("EULA - End User License Agreement");
            int expDays = Activation.Instance.GetExpirationDays();
            if (expDays < 0)
            {
                textBoxLicenseCode.Text = Strings.Get("Expired") + " " + Math.Abs(expDays) + Strings.Get(" days");
            }
            textBoxHeading.Select(0, 0);
        }

        private void linkLabelEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new EulaForm().ShowDialog();
        }
    }

}
