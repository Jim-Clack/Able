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
    public partial class RenamePayeeForm : Form
    {

        private MainScreen _mainScreen = null;

        private bool _changesMade = false;

        public RenamePayeeForm(MainScreen mainScreen)
        {
            _mainScreen = mainScreen;
            InitializeComponent();
        }

        private void RenamePayeeFormcs_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Rename Payee");
            label1.Text = Strings.Get("Old Payee Name");
            label2.Text = Strings.Get("New Payee Name");
            label3.Text = Strings.Get("Number of Entries Changed");
            buttonClose.Text = Strings.Get("Close");
            buttonGo.Text = Strings.Get("Go...");
            comboBoxOldName.DataSource = _mainScreen.Backend.Payees;
            comboBoxOldName.Text = "";
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if(comboBoxOldName.Text == null || comboBoxOldName.Text.Length < 1)
            {
                return;
            }
            if (textBoxNewName.Text == null || textBoxNewName.Text.Length < 1)
            {
                return;
            }
            int numChanges = _mainScreen.Backend.RenamePayee(comboBoxOldName.Text, textBoxNewName.Text);
            textBoxNumChanges.Text = "" + numChanges;
            _changesMade = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RenamePayeeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_changesMade)
            {
                _mainScreen.Backend.ReloadTransactions(SortEntriesBy.TranDate);
            }
        }
    }
}
