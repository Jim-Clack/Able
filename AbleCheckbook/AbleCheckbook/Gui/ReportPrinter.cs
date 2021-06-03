using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{

    /// <summary>
    /// Printer output.
    /// </summary>
    public class ReportPrinter
    {

        /// <summary>
        /// Printer selection and settings.
        /// </summary>
        private PrintDocument _printDoc = null;

        /// <summary>
        /// What kind of report is to be printed?
        /// </summary>
        private BaseReportGenerator _reportGenerator = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        public ReportPrinter()
        {
            _printDoc = null;
        }

        //////////////////////////// RegisterReport //////////////////////////

        /// <summary>
        /// Print a Register Report, first prompting the user with a dialog.
        /// </summary
        /// <param name="db">Source fo report data.</param>
        /// <returns>Successful enqueuing the print job?</returns>
        public bool PrintRegisterReport(IDbAccess db)
        {
            DateRangeForm form = new DateRangeForm(true, false);
            if(form.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            _reportGenerator = new RegisterReportGenerator(db, form.FirstDate, form.LastDate);
            try
            {
                if (_printDoc == null)
                {
                    _printDoc = form.Document;
                }
                _printDoc.PrintPage += new PrintPageEventHandler(PrintReportCallback);
                _printDoc.Print();
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(true, "Printing Problem", ex.Message, false);
                alert.Show();
                Logger.Error("Printing Failure", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Print a Register Report to the default printer.
        /// </summary
        /// <param name="db">Source fo report data.</param>
        /// <returns>Successful enqueuing the print job?</returns>
        public bool PrintRegisterReport(IDbAccess db, DateTime startDate, DateTime endDate)
        {
            _reportGenerator = new RegisterReportGenerator(db, startDate, endDate);
            try
            {
                if (_printDoc == null)
                {
                    _printDoc = new PrintDocument();
                }
                _printDoc.PrintPage += new PrintPageEventHandler(PrintReportCallback);
                _printDoc.Print();
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(true, "Printing Problem", ex.Message, false);
                alert.Show();
                Logger.Error("Printing Failure", ex);
                return false;
            }
            return true;
        }

        //////////////////////////// CategoryReport //////////////////////////

        /// <summary>
        /// Print a Category Report, first prompting the user with a dialog.
        /// </summary
        /// <param name="db">Source fo report data.</param>
        /// <returns>Successful enqueuing the print job?</returns>
        public bool PrintCategoryReport(IDbAccess db)
        {
            DateRangeForm form = new DateRangeForm(true, true);
            if (form.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            _reportGenerator = new CategoryReportGenerator(db, form.FirstDate, form.LastDate, form.Detailed);
            try
            {
                if (_printDoc == null)
                {
                    _printDoc = form.Document;
                }
                _printDoc.PrintPage += new PrintPageEventHandler(PrintReportCallback);
                _printDoc.Print();
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(true, "Printing Problem", ex.Message, false);
                alert.Show();
                Logger.Error("Printing Failure", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Print a Category Report to the default printer.
        /// </summary
        /// <param name="db">Source fo report data.</param>
        /// <returns>Successful enqueuing the print job?</returns>
        public bool PrintCategoryReport(IDbAccess db, DateTime startDate, DateTime endDate, bool detailed)
        {
            _reportGenerator = new CategoryReportGenerator(db, startDate, endDate, detailed);
            try
            {
                if (_printDoc == null)
                {
                    _printDoc = new PrintDocument();
                }
                _printDoc.PrintPage += new PrintPageEventHandler(PrintReportCallback);
                _printDoc.Print();
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(true, "Printing Problem", ex.Message, false);
                alert.Show();
                Logger.Error("Printing Failure", ex);
                return false;
            }
            return true;
        }

        /////////////////////////////// Callback /////////////////////////////

        /// <summary>
        /// This dispatches a one-page print operation to the appropriate report generator. 
        /// </summary>
        /// <param name="sender">per .NET callback</param>
        /// <param name="ev">per .NET callback</param>
        private void PrintReportCallback(object sender, PrintPageEventArgs ev)
        {
            Logger.Diag("Printing report to " + ev.PageSettings.PrinterSettings.PrinterName);
            ev.HasMorePages = _reportGenerator.PrintPage(ev.Graphics, ev.MarginBounds);
        }

    }

}
