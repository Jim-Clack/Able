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
    public partial class CategoryChartForm : Form
    {

        private IDbAccess _db = null;

        private DateTime _month = DateTime.Now;

        private DateTime _startDate = DateTime.Now;

        private DateTime _endDate = DateTime.Now;

        public CategoryChartForm(IDbAccess db)
        {
            _db = db;
            InitializeComponent();
        }

        private void CategoryChart_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Category Report");
            buttonPrint.Text = Strings.Get("Print");
            buttonClose.Text = Strings.Get("Close");
            AdjustMonth(-1);
        }

        /// <summary>
        /// Draw the chart and update the Month/Year box as well.
        /// </summary>
        /// <param name="graphics">Where to draw it.</param>
        private void DrawChart(Graphics graphics)
        {
            long[] amountsArray;
            string[] captionsArray;
            graphics.FillRectangle(Brushes.White, 0, 0, pictureBox1.Width, pictureBox1.Height);
            textBoxMonthYear.Lines = new string[]
            {
                _startDate.ToString("MMMM"),
                _startDate.ToString("yyyy")
            };
            CategoryReportGenerator categoryReport = new CategoryReportGenerator(
                _db, _startDate, _endDate, true);
            Dictionary<Guid, ReportCategory> reportCategories = categoryReport.GetCategories();
            long income = categoryReport.BuildPieChartArrays(
                reportCategories, out amountsArray, out captionsArray);
            if (amountsArray.Length < 1)
            {
                graphics.DrawString(Strings.Get("No Activity"), SystemFonts.DialogFont, Brushes.Red, 10, 10);
            }
            else
            {
                UiHelperMethods.DrawPieChart(
                    graphics, 10, 10, pictureBox1.Width - 20, amountsArray, captionsArray);
            }
        }

        /// <summary>
        /// Adjust the current month upward or downward.
        /// </summary>
        /// <param name="delta">Signed value for how many months to move.</param>
        private void AdjustMonth(int delta)
        {
            _month = _month.AddMonths(delta);
            _startDate = new DateTime(_month.Year, _month.Month, 1);
            DateTime nextMonth = _startDate.AddMonths(1);
            _endDate = new DateTime(nextMonth.Year, nextMonth.Month, 1).AddDays(-1);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            ReportPrinter printer = new ReportPrinter();
            printer.PrintCategoryReport(_db);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawChart(e.Graphics);
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            AdjustMonth(1);
            pictureBox1.Invalidate();
        }

        private void buttonBackward_Click(object sender, EventArgs e)
        {
            AdjustMonth(-1);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Graphics graphics = pictureBox1.CreateGraphics();
            DrawChart(graphics);
        }
    }
}
