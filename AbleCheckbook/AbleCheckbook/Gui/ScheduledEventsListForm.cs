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
    public partial class ScheduledEventsListForm : Form
    {

        private List<RowOfSchEvents> _schEvents = null;

        private UiBackend _backend = null;

        public ScheduledEventsListForm(UiBackend backend)
        {
            _backend = backend;
            InitializeComponent();
        }

        private void ReloadGrid()
        {
            _schEvents = new List<RowOfSchEvents>();
            ScheduledEventIterator iterator = _backend.Db.ScheduledEventIterator;
            while (iterator.HasNextEntry())
            {
                _schEvents.Add(new RowOfSchEvents(iterator.GetNextEntry()));
            }
            _schEvents.Sort();
            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = _schEvents;
            dataGridViewEvents.DataSource = bindingSource1;
            dataGridViewEvents.SuspendLayout();
            buttonAddNew.Text = Strings.Get("Add New");
            buttonClose.Text = Strings.Get("Close");
            ((System.ComponentModel.ISupportInitialize)(dataGridViewEvents)).BeginInit();
            dataGridViewEvents.Columns["Payee"].ToolTipText = dataGridViewEvents.Columns["Payee"].HeaderText = 
                Strings.Get("Payee");
            dataGridViewEvents.Columns["Amount"].ToolTipText = dataGridViewEvents.Columns["Amount"].HeaderText =
                Strings.Get("Amount");
            dataGridViewEvents.Columns["Status"].ToolTipText = dataGridViewEvents.Columns["Status"].HeaderText =
                Strings.Get("Status");
            dataGridViewEvents.Columns["Due"].ToolTipText = dataGridViewEvents.Columns["Due"].HeaderText =
                Strings.Get("Due");
            dataGridViewEvents.Columns["Payee"].DisplayIndex = 0;
            dataGridViewEvents.Columns["Amount"].DisplayIndex = 1;
            dataGridViewEvents.Columns["Status"].DisplayIndex = 2;
            dataGridViewEvents.Columns["Due"].DisplayIndex = 3;
            dataGridViewEvents.Columns["Memo"].DisplayIndex = 4;
            ((System.ComponentModel.ISupportInitialize)(dataGridViewEvents)).EndInit();
            dataGridViewEvents.ResumeLayout();
            foreach (DataGridViewColumn column in dataGridViewEvents.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void ScheduledEventsListForm_Load(object sender, EventArgs e)
        {
            ReloadGrid();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridViewEvents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0)
            {
                return;
            }
            object obj = dataGridViewEvents.Rows[e.RowIndex].DataBoundItem;
            if(obj == null)
            {
                return;
            }
            ScheduledEventEditForm form = new ScheduledEventEditForm(_backend, (RowOfSchEvents)obj);
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            if (form.RequestedDelete)
            {
                _backend.Db.DeleteEntry(form.GetEventBeforeEdit());
                ReloadGrid();
            }
            else
            {
                _backend.Db.UpdateEntry(form.GetEvent(), form.GetEventBeforeEdit());
                ReloadGrid();
            }
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            ScheduledEventEditForm form = new ScheduledEventEditForm(_backend, null);
            if(form.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            _backend.Db.DeleteEntry(form.GetEvent()); // just in case
            _backend.Db.InsertEntry(form.GetEvent());
            ReloadGrid();
        }

        private void ScheduledEventsListForm_Resize(object sender, EventArgs e)
        {
            int width = 32;
            int height = 32;
            if(dataGridViewEvents.RowCount < 10)
            {
                return;
            }
            foreach(DataGridViewColumn col in dataGridViewEvents.Columns)
            {
                if(col.Visible)
                {
                    width += col.Width + 4;
                }
            }
            foreach (DataGridViewRow row in dataGridViewEvents.Rows)
            {
                if (row.Visible)
                {
                    height += row.Height + 4;
                }
            }
            this.MaximumSize = new Size(width, height);
        }
    }
}
