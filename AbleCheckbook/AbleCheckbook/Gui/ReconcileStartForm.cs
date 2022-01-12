using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class ReconcileStartForm : Form
    {

        private IDbAccess _db = null;

        private UriBuilder _uri = null;

        public ReconcileStartForm(IDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        public DateTime PrevReconDate
        {
            get
            {
                return dateTimePickerPrevRecon.Value;
            }
            set
            {
                dateTimePickerPrevRecon.Value = value;
            }
        }

        public DateTime ThisReconDate
        {
            get
            {
                return dateTimePickerThisRecon.Value;
            }
            set
            {
                dateTimePickerThisRecon.Value = value;
            }
        }

        public string PrevReconBalance
        {
            get
            {
                return textBoxPrevBalance.Text;
            }
            set
            {
                textBoxPrevBalance.Text = value;
            }
        }

        public string ThisReconBalance
        {
            get
            {
                return textBoxThisBalance.Text;
            }
            set
            {
                textBoxThisBalance.Text = value;
            }
        }

        private void ReconcileSourceForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Start Reconciliation");
            groupBoxSource.Text = Strings.Get("Source");
            radioButtonManual.Text = Strings.Get("Manual - Clear one entry at a time per a bank statement");
            radioButtonWeb.Text = Strings.Get("Web: Financial Institution:");
            radioButtonCsv.Text = Strings.Get("CSV File:");
            textBoxCsvFile.Text = Strings.Get("");
            textBoxWebConnection.Text = Strings.Get("");
            buttonBrowse.Text = Strings.Get("Browse");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("Ok");
            labelPrompt.Text = Strings.Get("Start/Prev Date is Inclusive, End/This date is Exclusive (after)");
            labelPrevRecon.Text = Strings.Get("Prev Reconcile");
            labelThisRecon.Text = Strings.Get("This Reconcile");
            dateTimePickerPrevRecon.ShowUpDown = !Configuration.Instance.ShowCalendars;
            dateTimePickerThisRecon.ShowUpDown = !Configuration.Instance.ShowCalendars;
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
            radioButtonCsv.Select();
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
            if (textBoxThisBalance.Text.Trim().Length < 3)
            {
                labelPrompt.Text = Strings.Get("Fill in bottom/left from your bank statement");
                return;
            }

            if (radioButtonCsv.Checked && String.IsNullOrEmpty(textBoxCsvFile.Text))
            {
                labelPrompt.Text = Strings.Get("First select the CSV file");
                return;
            }

            // TODO... (web import)

            if (radioButtonCsv.Checked)
            {
                ReconcileFromCsv();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ReconcileFromCsv()
        {
            JsonDbAccess db = new JsonDbAccess("csvtemp", null, true);
            CsvImporter importer = new CsvImporter(db);
            if (importer.Import(textBoxCsvFile.Text.Trim()) > 0)
            {
                AutoReconciler reconciler = new AutoReconciler(_db, db);
                int countChanges = reconciler.Reconcile(
                    dateTimePickerPrevRecon.Value, dateTimePickerThisRecon.Value, false, _db.Account.OnlineBankingAggressive);
                int countDateWrong = reconciler.CountDateWrong;
                string message = "" + countChanges + Strings.Get(" entries changed or added");
                if (countDateWrong > 0)
                {
                    message = "" + countDateWrong + Strings.Get(" entries ignored as out of date range");
                }
                int countDuplicates = reconciler.CountDuplicates;
                if (countDuplicates > 0)
                {
                    message = Strings.Get("Cannot Proceed:") + " " + countDuplicates +
                        Strings.Get(" entries already reconciled in range:\n  ") +
                        reconciler.FirstDuplicate + "\n        ... \n  " + reconciler.LastDuplicate +
                        Strings.Get("\nRecommended action: [Abandon Reconcile]");
                }
                MessageBox.Show(message);
            }
            db.CloseWithoutSync();
        }
    }
}
