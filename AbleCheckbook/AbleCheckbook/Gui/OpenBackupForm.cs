using AbleCheckbook.Db;
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
    public partial class OpenBackupForm : Form
    {

        private UndoableDbAccess _db = null;

        private List<RowOfBackup> _backups = null;

        private string _filepath = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Current DB.</param>
        public OpenBackupForm(UndoableDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        /// <summary>
        /// Get the full path to the DB to open.
        /// </summary>
        public string Filepath
        {
            get
            {
                return _filepath;
            }
        }

        /// <summary>
        /// Load event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenBackupForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Open Backup of DB File: ") + _db.Name;
            string dbName = _db.Name;
            label1.Text = Strings.Get("Save the current DB as ") + dbName + ".bu1" + 
                Strings.Get("; Open the selected backup as ") + dbName + ".acb";
            buttonOk.Text = Strings.Get("OK");
            buttonCancel.Text = Strings.Get("Cancel");
            _backups = new List<RowOfBackup>();
            List<FileInfo> infos = UtilityMethods.PotentialBackups(_db.Name);
            foreach(FileInfo fileInfo in infos)
            {
                _backups.Add(new RowOfBackup(fileInfo));
            }
            _backups.Sort();
            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = _backups;
            dataGridView.DataSource = bindingSource1;
            dataGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            dataGridView.Columns["FileName"].DisplayIndex = 0;
            dataGridView.Columns["Ok"].DisplayIndex = 1;
            dataGridView.Columns["SaveDate"].DisplayIndex = 2;
            dataGridView.Columns["ModifDate"].DisplayIndex = 3;
            dataGridView.Columns["LastModPayee"].DisplayIndex = 4;
            dataGridView.Columns["LastModAmount"].DisplayIndex = 5;
            dataGridView.Columns["NumEntries"].DisplayIndex = 6;
            dataGridView.Columns["Last30Days"].DisplayIndex = 7;
            dataGridView.Columns["Last90Days"].DisplayIndex = 8;
            dataGridView.Columns["ThisYear"].DisplayIndex = 9;
            dataGridView.Columns["SchedEvnts"].DisplayIndex = 10;
            dataGridView.Columns["Bytes"].DisplayIndex = 11;
            dataGridView.Columns["Score"].DisplayIndex = 12;
            dataGridView.Columns["Path"].DisplayIndex = 13;
            dataGridView.Columns["Path"].Width = 360;
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            dataGridView.ResumeLayout();
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if(dataGridView.SelectedRows.Count != 1)
            {
                return;
            }
            _filepath = ((RowOfBackup)dataGridView.Rows[dataGridView.SelectedRows[0].Index].DataBoundItem).Path;
            if(_filepath == null || _filepath.Length < 1)
            {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
