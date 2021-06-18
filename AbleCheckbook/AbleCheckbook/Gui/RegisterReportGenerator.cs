using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{
    public class RegisterReportGenerator : BaseReportGenerator
    {

        /// <summary>
        /// Source of data.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// For traversing DB.
        /// </summary>
        private List<CheckbookEntry> _entries = null;
        private IEnumerator<CheckbookEntry> _enumerator = null;

        // Internal private state...
        private CheckbookSorter _sorter = null;
        private FontDesc _companyText = null;
        private FontDesc _emphasisText = null;
        private FontDesc _columnText = null;
        private FontDesc _headingText = null;
        private FontDesc _bodyText = null;
        private FontDesc _monoText = null;
        private bool _hasMorePages = true;
        private float _xLeft = 0;
        private float _yTop = 0;
        private float _width = 100;
        private float _height = 100;
        private float _xBox = 0;
        private float _widthBox = 0;
        private float _heightBox = 0;
        private float _xChkDate = 0;
        private float _xChkNbr = 0;
        private float _xChkPayee = 0;
        private float _xChkCateg = 0;
        private float _xChkAmt = 0;
        private float _xChkBal = 0;
        private float _xChkCleared = 0;
        private float _xOnPage = 0;
        private float _yOnPage = 0;
        private int _pageNumber = 0;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _licensedTo = null;
        private string _logoFilePath = "";
        private Brush _boxDebitBrush = null;
        private Brush _boxCreditBrush = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">DB to be reported on.</param>
        /// <param name="startDate">Starting date, inclusive.</param>
        /// <param name="endDate">Ending date, inclusive.</param>
        public RegisterReportGenerator(IDbAccess db, DateTime startDate, DateTime endDate)
        {
            _pageNumber = 0;
            _db = db;
            _startDate = startDate;
            _endDate = endDate;
            _sorter = new CheckbookSorter();
            _sorter.SetDateRange(startDate, endDate);
        }

        /// <summary>
        /// Generate a page of the report.
        /// </summary>
        /// <param name="graphics">To render to.</param>
        /// <param name="margins">Margins, typically same as or within graphics visible clip bounds.</param>
        /// <returns>true if more pages follow</returns>
        public override bool PrintPage(Graphics graphics, Rectangle margins)
        {
            _xLeft = margins != null ? margins.Left : graphics.VisibleClipBounds.X;
            _yTop = margins != null ? margins.Top : graphics.VisibleClipBounds.Y;
            _width = margins != null ? margins.Width : graphics.VisibleClipBounds.Width;
            _height = margins != null ? margins.Height : graphics.VisibleClipBounds.Height;
            _graphics = graphics;
            if (_pageNumber == 0)
            {
                if (!_db.CheckbookEntryIterator.HasNextEntry())
                {
                    return false; // empty db
                }
                InitializeVariables();
                _graphics.FillRectangle(Brushes.White, new RectangleF(_xLeft, _yTop, _width, _height));
                _entries = _sorter.GetSortedEntries(_db, SortEntriesBy.TranDate);
                _enumerator = _entries.GetEnumerator();
                _yOnPage = DrawHeaderBlock(_db, _startDate, _endDate);
            }
            return RenderNextPage(graphics);
        }

        /// <summary>
        /// Render everything after heading, incuding subsequent pages. (First page must be printed first)
        /// </summary>
        /// <param name="graphics">When printing, this is different for each page!</param>
        /// <returns>True if there are more pages to follow.</returns>
        public bool RenderNextPage(Graphics graphics)
        {
            _graphics = graphics;
            _pageNumber++;
            _hasMorePages = false;
            _graphics.FillRectangle(Brushes.WhiteSmoke, new RectangleF(_xBox, _yOnPage - 1, _widthBox, _heightBox));
            DrawTextReturnNewX(Strings.Get("Date"), _columnText, _xChkDate, _yOnPage);
            DrawTextReturnNewX(Strings.Get("Chk#"), _columnText, _xChkNbr, _yOnPage);
            DrawTextReturnNewX(Strings.Get("Payee"), _columnText, _xChkPayee, _yOnPage);
            DrawTextReturnNewX(Strings.Get("Category"), _columnText, _xChkCateg, _yOnPage);
            DrawTextReturnNewX("    " + Strings.Get("Amount"), _monoText, _xChkAmt, _yOnPage);
            DrawTextReturnNewX("   " + Strings.Get("Balance"), _monoText, _xChkBal, _yOnPage);
            _yOnPage = DrawTextReturnNewY("X", _columnText, _xChkCleared, _yOnPage) + 3; 
            while(_enumerator.MoveNext())
            {
                CheckbookEntry entry = _enumerator.Current;
                int places = entry.Amount > 0 ? 10 : 11;
                Brush boxBrush = entry.Amount > 0 ? _boxCreditBrush : _boxDebitBrush;
                _graphics.FillRectangle(boxBrush, new RectangleF(_xBox, _yOnPage - 1, _widthBox, _heightBox));
                string dateString = UtilityMethods.DateTimeToString(entry.DateOfTransaction);
                string numberString = entry.CheckNumber;
                string amountString = UtilityMethods.FormatCurrency(entry.Amount, places);
                string balanceString = UtilityMethods.FormatCurrency(entry.Balance, 10);
                string clearedString = entry.IsCleared ? "X" : "";
                string categoryName = Strings.Get("(Split)");
                if(entry.Splits.Length < 2)
                {
                    FinancialCategory finCateg = _db.GetFinancialCategoryById(entry.Splits[0].CategoryId);
                    if(finCateg != null)
                    {
                        categoryName = finCateg.Name;
                    }
                }
                DrawTextReturnNewX(dateString, _bodyText, _xChkDate, _yOnPage);
                DrawTextReturnNewX(numberString, _bodyText, _xChkNbr, _yOnPage);
                DrawTextReturnNewX(entry.Payee, _bodyText, _xChkPayee, _yOnPage);
                DrawTextReturnNewX(categoryName, _bodyText, _xChkCateg, _yOnPage);
                DrawTextReturnNewX(amountString, _monoText, _xChkAmt, _yOnPage);
                DrawTextReturnNewX(balanceString, _monoText, _xChkBal, _yOnPage);
                _yOnPage = DrawTextReturnNewY(clearedString, _bodyText, _xChkCleared, _yOnPage) + 2;
                if (_yOnPage >= _yTop + _height - (_bodyText.Font.Height + 3))
                {
                    _hasMorePages = true;
                    break; // we hit end of the page
                }
            }
            _yOnPage = _yTop;
            return _hasMorePages;
        }

        /////////////////////////////// Support //////////////////////////////

        /// <summary>
        /// Render the headr area to the left of the pie chart.
        /// </summary>
        /// <param name="db">To be analyzed.</param>
        /// <param name="startDate">Starting date, inclusive.</param>
        /// <param name="endDate">Ending date, inclusive.</param>
        /// <returns>Y on page.</returns>
        private float DrawHeaderBlock(IDbAccess db, DateTime startDate, DateTime endDate)
        {
            _yOnPage = _yTop + _companyText.Font.Height + 20;
            if (File.Exists(_logoFilePath))
            {
                Image logo = Image.FromFile(_logoFilePath);
                float logoWidth = logo.Width;
                float logoScale = _width * 0.36F / logoWidth;
                Image scaledLogo = UiHelperMethods.ResizeImage(
                    logo, (int)(logo.Width * logoScale), (int)(logo.Height * logoScale));
                _graphics.DrawImage(scaledLogo, _xLeft, _yTop);
                _xOnPage = _xLeft + logo.Width * logoScale + 28;
                _yOnPage = _yTop + logo.Height * logoScale + 10;
            }
            float xSpacing = _headingText.Font.Height;
            _xOnPage = DrawTextReturnNewX(Strings.Get("Able Strategies AbleCheckbook"), _companyText, _xOnPage, _yTop + 6) + xSpacing;
            DrawTextReturnNewX(Strings.Get("Licensed To"), _emphasisText, _xOnPage, _yTop + 6);
            DrawTextReturnNewY(_licensedTo, _emphasisText, _xOnPage, _yTop + _emphasisText.Font.Height + 6);
            _xOnPage = DrawTextReturnNewX(Strings.Get("From:"), _headingText, _xLeft, _yOnPage);
            _xOnPage = DrawTextReturnNewX(UtilityMethods.DateTimeToString(startDate, false), _headingText, _xOnPage, _yOnPage) + xSpacing;
            _xOnPage = DrawTextReturnNewX(Strings.Get("Thru:"), _headingText, _xOnPage, _yOnPage);
            _xOnPage = DrawTextReturnNewX(UtilityMethods.DateTimeToString(endDate, false), _headingText, _xOnPage, _yOnPage) + xSpacing;
            _xOnPage = DrawTextReturnNewX(Strings.Get("Acct:"), _headingText, _xOnPage, _yOnPage);
            _xOnPage = DrawTextReturnNewX(db.Name, _headingText, _xOnPage, _yOnPage) + xSpacing;
            _xOnPage = DrawTextReturnNewX(Strings.Get("Date:"), _headingText, _xOnPage, _yOnPage);
            _yOnPage = DrawTextReturnNewY(UtilityMethods.DateTimeToString(DateTime.Now, true), _headingText, _xOnPage, _yOnPage);
            return _yOnPage + 18;
        }

        /// Build the text layouts / fonts / locations / sizes based on _xLeft and _width.
        /// </summary>
        private void InitializeVariables()
        {
            // fonts
            _companyText = new FontDesc(Brushes.RoyalBlue, new Font(FontFamily.GenericSansSerif, _width / 48, FontStyle.Bold));
            _emphasisText = new FontDesc(Brushes.DarkRed, new Font(FontFamily.GenericSansSerif, _width / 70, FontStyle.Bold));
            _headingText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 66, FontStyle.Bold));
            _bodyText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 68, FontStyle.Regular));
            _columnText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 66, FontStyle.Bold));
            _monoText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericMonospace, _width / 74, FontStyle.Bold));
            // x/y locations
            _xOnPage = _xLeft;
            _yOnPage = _yTop;
            _xBox = _xLeft;
            _widthBox = _width;
            _heightBox = _bodyText.Font.Height + 2;
            _xChkDate = _xLeft + 10;
            _xChkNbr = _xLeft + _width * 0.12F;
            _xChkPayee = _xLeft + _width * 0.20F;
            _xChkCateg = _xLeft + _width * 0.48F;
            _xChkAmt = _xLeft + _width * 0.69F;
            _xChkBal = _xLeft + _width * 0.83F;
            _xChkCleared = _xLeft + _width - _bodyText.Font.Height;
            // other...
            _boxDebitBrush = new SolidBrush(Color.FromArgb(255, 255, 230, 230));
            _boxCreditBrush = new SolidBrush(Color.FromArgb(255, 230, 255, 230));
            _logoFilePath = Configuration.Instance.FindSupportFilePath("logo-###.png");
            _licensedTo = Strings.Get("UNLICENSED_VERSION");
            if (Configuration.Instance.GetIsLicensedVersion())
            {
                _licensedTo = Configuration.Instance.SiteDescription;
            }

        }
    }

}
