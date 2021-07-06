using AbleCheckbook.Db;
using AbleCheckbook.Logic;
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
    public partial class AccountSettingsForm : Form
    {
        private IDbAccess _db;

        public AccountSettingsForm(IDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        private void AccountSettingsForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Account Settings - ") + _db.Name;
            checkBoxLiveSync.Text = Strings.Get("Live sync with bank online (instead of only for statement reconcile)");
            checkBoxAggressive.Text = Strings.Get("Aggressively merge transactions (you can still un-merge if necessary)");
            labelBank.Text = Strings.Get("Select Bank");
            labelAccount.Text = Strings.Get("Account");
            labelUser.Text = Strings.Get("Your Login");
            labelPwd.Text = Strings.Get("Password");
            buttonTest.Text = Strings.Get("Test");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("Ok");
            checkBoxAggressive.Checked = _db.Account.OnlineBankingAggressive;
            checkBoxLiveSync.Checked = _db.Account.OnlineBankingLive;
            textBoxUser.Text = _db.Account.OnlineBankingUser;
            textBoxPwd.Text = _db.Account.OnlineBankingPwd;
        }
    }
}
