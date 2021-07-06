using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class AccountSettingsForm : Form
    {
        private IDbAccess _db;

        private bool _testedOk = false;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Acct Db to be read and possibly updated</param>
        public AccountSettingsForm(IDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        private void AccountSettingsForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Account Settings - ") + _db.Name;
            checkBoxLiveSync.Text = Strings.Get("Live sync to bank acct online (instead of only for monthly reconcile)");
            checkBoxAggressive.Text = Strings.Get("Aggressively merge transactions (you can still un-merge if desired)");
            labelBank.Text = Strings.Get("Select Bank");
            labelAccount.Text = Strings.Get("Account");
            labelUser.Text = Strings.Get("Your Login");
            labelPwd.Text = Strings.Get("Password");
            buttonTest.Text = Strings.Get("Test");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("Ok");
            checkBoxAggressive.Checked = _db.Account.OnlineBankingAggressive;
            checkBoxLiveSync.Checked = _db.Account.OnlineBankingLive;
            // Todo: bank & acct
            textBoxUser.Text = _db.Account.OnlineBankingUser;
            textBoxPwd.Text = _db.Account.OnlineBankingPwd;
            AdjustVisibilties();
        }

        private void AdjustVisibilties()
        {
            buttonTest.Enabled = false;
            buttonOk.Enabled = false;
            if (comboBoxBank.Text.Trim().Length < 1)
            {
                labelAssistance.Text = Strings.Get("Bank/branch req'd");
            }
            else if (comboBoxAcct.Text.Trim().Length < 1)
            {
                labelAssistance.Text = Strings.Get("Acct type required");
            }
            else if (textBoxUser.Text.Trim().Length < 1)
            {
                labelAssistance.Text = Strings.Get("Bank login required");
            }
            else if (textBoxPwd.Text.Trim().Length < 1)
            {
                labelAssistance.Text = Strings.Get("Bank password req'd");
            }
            else if (!_testedOk)
            {
                labelAssistance.Text = Strings.Get("Click Test...");
                buttonTest.Enabled = true;
            }
            else
            {
                labelAssistance.Text = "";
                buttonOk.Enabled = true;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            // Todo...
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _db.Account.OnlineBankingAggressive = checkBoxAggressive.Checked;
            _db.Account.OnlineBankingLive = checkBoxLiveSync.Checked;
            // Todo: bank & acct
            _db.Account.OnlineBankingUser = textBoxUser.Text.Trim();
            _db.Account.OnlineBankingPwd = textBoxPwd.Text.Trim();
            DialogResult = DialogResult.OK;
            this.Hide();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Close();
        }

        private void comboBoxBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustVisibilties();
        }

        private void comboBoxAcct_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustVisibilties();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            AdjustVisibilties();
        }

        private void textBoxPwd_TextChanged(object sender, EventArgs e)
        {
            AdjustVisibilties();
        }
    }

}
