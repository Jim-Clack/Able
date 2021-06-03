using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class NewDbForm : Form
    {

        private string _filename = null;

        public NewDbForm()
        {
            InitializeComponent();
        }

        private void NewDbForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("New DB Acct");
            label1.Text = Strings.Get("Select Acct");
            comboBoxAcctNames.DataSource = Configuration.Instance.GetLegalFilenames();
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("OK");
            labelErrorExists.Text = Strings.Get("Sorry - File Already Exists");
            labelErrorIllegal.Text = Strings.Get("Sorry - Illegal Account Name");
            labelErrorExists.Visible = false;
            labelErrorIllegal.Visible = false;
        }

        public string AccountName
        {
            get
            {
                return _filename;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            labelErrorIllegal.Visible = false;
            labelErrorExists.Visible = false;
            if (Configuration.Instance.GetLegalFilenames().Contains(comboBoxAcctNames.Text.Trim()))
            {
                _filename = comboBoxAcctNames.Text.Trim();
                string filepath = UtilityMethods.GetDbFilename(_filename, false, false);
                if (File.Exists(filepath))
                {
                    labelErrorExists.Visible = true;
                    _filename = null;
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            labelErrorIllegal.Visible = true;

        }

        private void comboBoxAcctNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelErrorExists.Visible = false;
            labelErrorIllegal.Visible = false;
        }

        private void comboBoxAcctNames_Enter(object sender, EventArgs e)
        {
            labelErrorExists.Visible = false;
            labelErrorIllegal.Visible = false;
        }
    }
}
