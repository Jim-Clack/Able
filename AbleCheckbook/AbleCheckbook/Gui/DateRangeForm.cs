using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class DateRangeForm : Form
    {

        private PrintDocument _printDocument = null;
        private bool _defaultToLastMonth;
        private bool _showDetailedCheckbox;

        public DateRangeForm(bool defaultToLastMonth = true, bool showDetailedCheckbox = true)
        {
            InitializeComponent();
            _defaultToLastMonth = defaultToLastMonth;
            _showDetailedCheckbox = showDetailedCheckbox;
        }

        public DateTime FirstDate
        {
            get
            {
                return dateFirst.Value;
            }
            set
            {
                dateFirst.Value = value;
            }
        }

        public DateTime LastDate
        {
            get
            {
                return dateLast.Value;
            }
            set
            {
                dateLast.Value = value;
            }
        }

        public bool Detailed
        {
            get
            {
                return checkDetailed.Checked;
            }
            set
            {
                checkDetailed.Checked = value;
            }
        }

        public PrintDocument Document
        {
            get
            {
                return _printDocument;
            }
        }

        private void DateRangeForm_Load(object sender, EventArgs e)
        {
            // Workaround for Bug in Visual Studio 
            // When DateRangeForm.cs(Design) gets closed in VS, this form gets scrambled
            this.SuspendLayout();
            dateFirst.Location = new Point(126, 18);
            dateFirst.Size = new Size(260, 22);
            dateLast.Location = new Point(126, 52);
            dateLast.Size = new Size(260, 22);
            buttonSelectPrinter.Location = new Point(87, 120);
            buttonSelectPrinter.Size = new Size(297, 32);
            buttonCancel.Location = new Point(87, 165);
            buttonCancel.Size = new Size(120, 32);
            buttonPrint.Location = new Point(264, 165);
            buttonPrint.Size = new Size(120, 32);
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 215);
            this.MaximumSize = new System.Drawing.Size(432, 262);
            this.MinimumSize = new System.Drawing.Size(432, 262);
            this.Size = new System.Drawing.Size(432, 262);
            this.ResumeLayout(false);
            this.PerformLayout();
            // end Workaround

            label1.Text = Strings.Get("Start First Date");
            label2.Text = Strings.Get("End Last Date");
            checkDetailed.Text = Strings.Get("Detailed Report");
            buttonPrint.Text = Strings.Get("Go Print");
            buttonCancel.Text = Strings.Get("Cancel");
            buttonSelectPrinter.Text = Strings.Get("Select Printer and Settings");
            Text = Strings.Get("Date Range");
            _printDocument = new PrintDocument();
            _printDocument.DocumentName = "Checkbook Report";
            if (!_showDetailedCheckbox)
            {
                checkDetailed.Visible = false;
                dateLast.Location = new Point(dateLast.Location.X, dateLast.Location.Y + 4);
                buttonSelectPrinter.Location = new Point(buttonSelectPrinter.Location.X, buttonSelectPrinter.Location.Y - 16);
                buttonCancel.Location = new Point(buttonCancel.Location.X, buttonCancel.Location.Y - 16);
                buttonPrint.Location = new Point(buttonPrint.Location.X, buttonPrint.Location.Y - 16);
            }
            if (_defaultToLastMonth)
            {
                DateTime startMonth = DateTime.Now.AddMonths(-1);
                dateFirst.Value = new DateTime(startMonth.Year, startMonth.Month, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
                dateLast.Value = endDate;
            }
            else
            {
                dateFirst.Value = DateTime.Now.AddMonths(-1).AddDays(1);
                dateLast.Value = DateTime.Now;
            }
            dateFirst.MaxDate = DateTime.Now.AddYears(1);
            dateFirst.MinDate = DateTime.Now.AddYears(-20);
            dateLast.MaxDate = DateTime.Now.AddYears(1);
            dateLast.MinDate = DateTime.Now.AddYears(-20);
            dateLast.Format = DateTimePickerFormat.Long;
            dateFirst.Format = DateTimePickerFormat.Long;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSelectPrinter_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.AllowCurrentPage = false;
            printDialog.AllowSelection = false;
            printDialog.AllowSomePages = false;
            DialogResult result = printDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                _printDocument.PrinterSettings = printDialog.PrinterSettings;
            }
        }
    }

}
