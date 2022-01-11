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
    public partial class PreferencesForm : Form
    {

        private string _originalDirectoryPath = null;

        public PreferencesForm()
        {
            InitializeComponent();
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            Logger.LogLevel level = Configuration.Instance.LogLevel;
            this.Text = Strings.Get("Preferences");
            labelBaseDir.Text = Strings.Get("Base Directory:");
            labelBackupsDir.Text = Strings.Get("Weekly Backups:");
            labelLogLevel.Text = Strings.Get("Log Level (Trace = Detailed, Diag = Normal, Warn = Smaller Logs)");
            labelSchedEventDays.Text = Strings.Get("Number of days in advance to post scheduled events to checkbook:");
            labelBadDirectory.Text = Strings.Get("Illegal Directory Path Specified, Reverted...");
            checkBoxDisableSanity.Text = Strings.Get("Disable sanity-checks for wild dates and amounts during data-entry");
            checkBoxCalendars.Text = Strings.Get("Edit dates via calendar instead of day/month/year spinners");
            checkBoxReconcileNote.Text = Strings.Get("Display the Reconcile Overdue notification when appropriate");
            checkBoxYearEndNote.Text = Strings.Get("Display the Year-End Wrap-Up Due notification When appropriate");
            checkBoxTwoColumns.Text = Strings.Get("Display amounts in two columns (Debit/Credit) instead of one (Amount)");
            checkBoxHighVisibility.Text = Strings.Get("High Visibility - Larger Fonts");
            buttonBrowseDb.Text = Strings.Get("Change");
            buttonClose.Text = Strings.Get("Close");
            comboBoxLogLevel.DataSource = Enum.GetNames(typeof(Logger.LogLevel));
            _originalDirectoryPath = Configuration.Instance.DirectoryDatabase;
            textBoxBaseDir.Text = _originalDirectoryPath;
            textBoxBackupsDir.Text = Configuration.Instance.DirectoryBackup2;
            numericUpDownDays.Value = Configuration.Instance.PostEventAdvanceDays;
            comboBoxLogLevel.Text = Enum.GetNames(typeof(Logger.LogLevel))[(int)level];
            comboBoxLogLevel.SelectedIndex = (int)level;
            checkBoxDisableSanity.Checked = Configuration.Instance.DisableSanityChecks;
            checkBoxCalendars.Checked = !Configuration.Instance.ShowCalendars;
            checkBoxReconcileNote.Checked = !Configuration.Instance.SuppressReconcileAlert;
            checkBoxYearEndNote.Checked = !Configuration.Instance.SuppressYearEndAlert;
            checkBoxTwoColumns.Checked = Configuration.Instance.TwoAmountColumns;
            checkBoxHighVisibility.Checked = Configuration.Instance.HighVisibility;
            labelBadDirectory.Visible = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxBaseDir_Leave(object sender, EventArgs e)
        {
            SetBaseDirectory();
        }

        private void textBoxBaseDir_Validating(object sender, CancelEventArgs e)
        {
            SetBaseDirectory();
        }

        private void ChangeBaseDirectory()
        {
            FolderBrowserDialog form = new FolderBrowserDialog();
            form.Description = Strings.Get("Base DB Directory");
            form.ShowNewFolderButton = true;
            form.SelectedPath = textBoxBaseDir.Text.Trim();
            DialogResult result = form.ShowDialog();
            if (result != DialogResult.OK || form.SelectedPath.Trim().Length < 1)
            {
                return;
            }
            string path = form.SelectedPath.Trim();
            textBoxBaseDir.Text = path;
            SetBaseDirectory();
        }

        private bool SetBaseDirectory()
        {
            labelBadDirectory.Visible = false;
            if (!Configuration.Instance.SetBaseDirectory(textBoxBaseDir.Text))
            {
                textBoxBaseDir.Text = _originalDirectoryPath;
                labelBadDirectory.Visible = true;
                return false;
            }
            textBoxBackupsDir.Text = Configuration.Instance.DirectoryBackup2; // just in case it was changed too
            return true;
        }

        private void ChangeBackupsDirectory()
        {
            FolderBrowserDialog form = new FolderBrowserDialog();
            form.Description = Strings.Get("Weekly Backups Directory");
            form.ShowNewFolderButton = true;
            DialogResult result = form.ShowDialog();
            if (result != DialogResult.OK || form.SelectedPath.Trim().Length < 1)
            {
                return;
            }
            string path = form.SelectedPath.Trim();
            textBoxBackupsDir.Text = path;
            SetBackupsDirectory();
        }

        private void SetBackupsDirectory()
        {
            Configuration.Instance.DirectoryBackup2 = textBoxBackupsDir.Text.Trim();
        }

        private void PreferencesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SetBaseDirectory())
            {
                Configuration.Instance.Save();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void numericUpDownDays_ValueChanged(object sender, EventArgs e)
        {
            if((int)numericUpDownDays.Value < 0)
            {
                numericUpDownDays.Value = 0;
            }
            Configuration.Instance.PostEventAdvanceDays = (int)numericUpDownDays.Value;
        }

        private void comboBoxLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.LogLevel level = Logger.LogLevel.Diag;
            if (Enum.TryParse<Logger.LogLevel>(comboBoxLogLevel.Text, out level))
            {
                Configuration.Instance.LogLevel = level;
            }
        }

        private void checkBoxReconcileNote_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.SuppressReconcileAlert = !checkBoxReconcileNote.Checked;
        }

        private void buttonBrowseDb_Click(object sender, EventArgs e)
        {
            ChangeBaseDirectory();
        }

        private void checkBoxTwoColumns_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.TwoAmountColumns = checkBoxTwoColumns.Checked;
        }

        private void checkBoxHighVisibility_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.HighVisibility = checkBoxHighVisibility.Checked;
        }

        private void textBoxBackupsDir_Leave(object sender, EventArgs e)
        {
            SetBackupsDirectory();
        }

        private void textBoxBackupsDir_Validating(object sender, CancelEventArgs e)
        {
            SetBackupsDirectory();
        }

        private void buttonBrowseBackups_Click(object sender, EventArgs e)
        {
            ChangeBackupsDirectory();
        }

        private void checkBoxCalendars_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.ShowCalendars = checkBoxCalendars.Checked;
        }

        private void checkBoxDisableSanity_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.DisableSanityChecks = checkBoxDisableSanity.Checked;
        }
    }
}
