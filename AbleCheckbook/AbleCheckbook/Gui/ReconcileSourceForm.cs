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
    public partial class ReconcileSourceForm : Form
    {

        private IDbAccess _db = null;

        private UriBuilder _uri = null;

        public ReconcileSourceForm(IDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        private void ReconcileSourceForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Reconciliation Source");
            groupBoxSource.Text = Strings.Get("Source");
            radioButtonManual.Text = Strings.Get("Manual - Clear one entry at a time per a bank statement");
            radioButtonWeb.Text = Strings.Get("Web: Financial Institution:");
            radioButtonCsv.Text = Strings.Get("CSV File:");
            textBoxCsvFile.Text = Strings.Get("");
            textBoxWebConnection.Text = Strings.Get("");
            buttonBrowse.Text = Strings.Get("Browse");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("Ok");
            labelPrompt.Text = Strings.Get("");
            try
            {
                _uri = new UriBuilder(_db.Account.OnlineBankingUrl);
                textBoxWebConnection.Text = _uri.Host;
                radioButtonWeb.Enabled = !String.IsNullOrEmpty(_uri.Host);
            }
            catch(Exception ex)
            {
                Logger.Diag("Not reconcilable via web ", ex);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            labelPrompt.Text = "";
            OpenFileDialog form = new OpenFileDialog();
            form.AddExtension = true;
            form.CheckFileExists = true;
            form.CheckPathExists = true;
            form.DefaultExt = "csv";
            form.DereferenceLinks = true;
            form.Filter = "CSV/Excel|*.csv|All Files|*.*";
            form.InitialDirectory = Configuration.Instance.DirectoryImportExport;
            form.Multiselect = false;
            form.RestoreDirectory = true;
            form.Title = Strings.Get("Import CSV");
            if (form.ShowDialog() == DialogResult.OK)
            {
                string filepath = form.FileName.Trim();
                if (filepath.Length > 0)
                {
                    textBoxCsvFile.Text = filepath;
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if(radioButtonCsv.Checked && String.IsNullOrEmpty(textBoxCsvFile.Text))
            {
                labelPrompt.Text = Strings.Get("First select the CSV file");
                return;
            }

            // TODO... (web/csv import)

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
