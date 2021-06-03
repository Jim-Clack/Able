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
            textBoxSiteDesc.Text = Activation.Instance.SiteDescription;
            textBoxUserId.Text = System.Environment.UserName;
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            textBoxVersion.Text = Strings.Get("Version: ") + version.ToString();
            linkLabelEula.Text = Strings.Get("EULA - End User License Agreement");
            int expDays = Activation.Instance.UpdateSiteSettings();
            if (expDays < 0)
            {
                textBoxSiteDesc.Text = Strings.Get("Expired") + " " + Math.Abs(expDays) + Strings.Get(" days");
            }
            textBoxHeading.Select(0, 0);
        }
    }
}
