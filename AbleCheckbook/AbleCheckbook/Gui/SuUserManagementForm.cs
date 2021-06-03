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
    public partial class SuUserManagementForm : Form
    {

        private SuUserManagement _userManagement = new SuUserManagement();

        private List<SuUserData> _searchResults = null;

        public SuUserManagementForm()
        {
            if(Configuration.Instance.GetUserLevel() != UserLevel.SuperUser)
            {
                throw new Exception("Failed SU Attempt");
            }
            InitializeComponent();
        }

        /// <summary>
        /// Call this immediately AFTER binding a datasource to the grid, even though the error occurs when binding. 
        /// </summary>
        /// <param name="dataGridView">DataGridView immediately AFTER it has been bound to a data source.</param>
        private void LayoutColumns(DataGridView dataGridView)
        { 
            dataGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            dataGridView.Columns["SiteDesc"].DisplayIndex = 0;
            dataGridView.Columns["SiteId"].DisplayIndex = 1;
            dataGridView.Columns["Contact"].DisplayIndex = 2;
            dataGridView.Columns["Company"].DisplayIndex = 3;
            dataGridView.Columns["Important"].DisplayIndex = 4;
            dataGridView.Columns["PhoneNum"].DisplayIndex = 5;
            dataGridView.Columns["EmailAddr"].DisplayIndex = 6;
            dataGridView.Columns["ZipCode"].DisplayIndex = 7;
            dataGridView.Columns["OtherInfo"].DisplayIndex = 8;
            dataGridView.Columns["ActivBy"].DisplayIndex = 9;
            dataGridView.Columns["Notes"].DisplayIndex = 10;
            dataGridView.Columns["IpAddress"].DisplayIndex = 11;
            dataGridView.Columns["DateEnter"].DisplayIndex = 12;
            dataGridView.Columns["DateActiv"].DisplayIndex = 13;
            dataGridView.Columns["DateLastAcc"].DisplayIndex = 14;
            dataGridView.Columns["DateLastWebService"].DisplayIndex = 15;
            dataGridView.Columns["Id"].DisplayIndex = 16;
            dataGridView.Columns["HiddenInfo"].DisplayIndex = 17;
            dataGridView.Columns["SiteDesc"].Frozen = true;
            dataGridView.Columns["SiteId"].Frozen = true;
            dataGridView.AllowUserToDeleteRows = Configuration.Instance.GetAdminMode();
            dataGridView.Columns["SiteDesc"].ReadOnly = !Configuration.Instance.GetAdminMode();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            dataGridView.ResumeLayout();
        }

        private void SuUserManagement_Load(object sender, EventArgs e)
        {
            SetDataSource(_userManagement.Users, false);
            buttonDelete.Visible = Configuration.Instance.GetAdminMode();
            if(Configuration.Instance.GetAdminMode())
            {
                labelCannotDelete.Text = "Caution: Admin Mode allows you to alter critical info.";
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void buttonActivationPin_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show(this, "First click the arrow by the populated user-data row", "Sorry !");
                return;
            }
            SuUserData userData = (SuUserData)dataGridView1.SelectedRows[0].DataBoundItem;
            if (textBoxSiteIdCode.Text.Trim().Length < 4 || textBoxSiteDescriptionAbbrev.Text.Trim().Length != 12)
            {
                MessageBox.Show(this, "IdCode is wrong or Desc is bad", "Sorry !");
                return;
            }
            List<SuUserData> users = _userManagement.FindInDesc(textBoxSiteDescriptionAbbrev.Text.Trim());
            if (users.Count > 1 || (users.Count == 1 && users.First<SuUserData>().Id != userData.Id))
            {
                MessageBox.Show(this, "That site description is already in use", "Sorry !");
                return;
            }
            string pin = SuUserManagement.GetActivationPin(
                textBoxSiteIdCode.Text.Trim(), textBoxSiteDescriptionAbbrev.Text.Trim());
            if (pin.StartsWith("#"))
            {
                MessageBox.Show(this, pin.Substring(1), "Sorry !");
                return;
            }
            textBoxActivationPin.Text = pin;
            userData.SiteDesc = textBoxSiteDescriptionAbbrev.Text.Trim();
            userData.SiteId = textBoxSiteIdCode.Text.Trim();
            userData.DateActiv = DateTime.Now;
            userData.DateLastAcc = DateTime.Now;
            userData.ActivBy = Environment.UserName;
            dataGridView1.Invalidate(true);
            _userManagement.SaveUser(userData);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectRow();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SuUserData userData = (SuUserData)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            if (userData == null)
            {
                return;
            }
            userData.DateLastAcc = DateTime.Now;
            _userManagement.SaveUser(userData);
            buttonDelete.Enabled = (dataGridView1.SelectedRows.Count == 1);
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            buttonDelete.Enabled = (dataGridView1.SelectedRows.Count == 1);
            _userManagement.Sync();
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            SelectRow();
        }

        private void SelectRow()
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                return;
            }
            SuUserData userData = (SuUserData)dataGridView1.SelectedRows[0].DataBoundItem;
            if (userData == null)
            {
                return;
            }
            textBoxSiteDescriptionAbbrev.Text = "";
            textBoxSiteIdCode.Text = "";
            textBoxActivationPin.Text = "";
            if (userData.Contact == null && userData.SiteDesc == null && userData.SiteId == null)
            {
                userData.Id = Guid.NewGuid();
                userData.Contact = "";
                userData.Company = "";
                userData.PhoneNum = "";
                userData.ZipCode = "";
                userData.SiteDesc = "";
                userData.ActivBy = "";
                userData.EmailAddr = "";
                userData.HiddenInfo = "";
                userData.Important = "";
                userData.Notes = "";
                userData.SiteId = "";
                userData.IpAddress = "";
                userData.DateActiv = DateTime.Now;
                userData.DateEnter = DateTime.Now;
                userData.DateLastAcc = DateTime.Now;
                userData.DateLastWebService = DateTime.Now;
            }
            textBoxSiteDescriptionAbbrev.Text = userData.SiteDesc.Trim();
            textBoxSiteIdCode.Text = userData.SiteId.Trim();
            if (textBoxSiteDescriptionAbbrev.Text.Length < 12)
            {
                string abbrev = "";
                if (userData.Company.Length > 2)
                {
                    abbrev += userData.Company.Substring(0, 3);
                }
                if (userData.Contact.Length > 5)
                {
                    abbrev += userData.Contact.Substring(0, 6);
                }
                else if (userData.Contact.Length > 2)
                {
                    abbrev += userData.Contact.Substring(0, 3);
                }
                if (userData.PhoneNum.Length > 2)
                {
                    abbrev += userData.PhoneNum.Substring(userData.PhoneNum.Length - 3);
                }
                if (abbrev.Length > 6)
                {
                    abbrev = abbrev.Substring(0, 6);
                }
                abbrev += "-";
                abbrev += userData.ZipCode;
                if (abbrev.Length > 12)
                {
                    abbrev = abbrev.Substring(0, 12);
                }
                textBoxSiteDescriptionAbbrev.Text = abbrev;
            }
        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            SelectRow();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSearchNotes_Click(object sender, EventArgs e)
        {
            string pattern = textBoxSearchPattern.Text.Trim();
            if (pattern.Length < 1)
            {
                return;
            }
            _searchResults = _userManagement.FindInNotes(pattern);
            SetDataSource(_searchResults, false);
        }

        private void buttonSearchInfo_Click(object sender, EventArgs e)
        {
            string pattern = textBoxSearchPattern.Text.Trim();
            if (pattern.Length < 1)
            {
                return;
            }
            _searchResults = _userManagement.FindInHeader(pattern);
            SetDataSource(_searchResults, false);
        }

        private void textBoxSearchPattern_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBoxSearchPattern.Text.Trim().Length > 0)
            {
                return;
            }
            SetDataSource(_userManagement.Users, false);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                SuUserData userData = (SuUserData)dataGridView1.SelectedRows[0].DataBoundItem;
                if (userData == null)
                {
                    return;
                }
                _userManagement.DeleteUser(userData);
                SetDataSource(_userManagement.Users, false);
                textBoxSearchPattern.Text = "";
            }
        }

        private void SetDataSource(Object dataSource, bool isSearchActive)
        {
            foreach(DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Frozen = false;
            }
            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = dataSource;
            dataGridView1.DataSource = bindingSource1;
            LayoutColumns(dataGridView1);
            if (isSearchActive)
            {
                dataGridView1.AllowUserToAddRows = false;
            }
            else
            {
                _searchResults = null;
                dataGridView1.AllowUserToAddRows = true;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SelectRow();
            buttonDelete.Enabled = (dataGridView1.SelectedRows.Count == 1);
        }

        private void SuUserManagementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _userManagement.Sync();
        }
    }
}
