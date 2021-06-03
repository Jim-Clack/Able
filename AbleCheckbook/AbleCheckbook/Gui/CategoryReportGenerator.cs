using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{

    ///////////////////////////// ReportCategory /////////////////////////////

    /// <summary>
    /// Used only within this package to track categories.
    /// </summary>
    /// <remarks>
    /// Usage:
    ///   CategoryReport.RenderReport(...)        Creates the first page of the report
    ///   CategoryReport.RenderCategoryList(...)  Creates subsequent pages of a multi-page report
    /// </remarks>
    public class ReportCategory
    {
        /// <summary>
        /// Category that gets accumulated herein.
        /// </summary>
        private FinancialCategory _category = null;

        /// <summary>
        /// Sum of all amounts in category.
        /// </summary>
        private long _amount = 0;

        /// <summary>
        /// List of entries in this category.
        /// </summary>
        private List<CheckbookEntry> _entries = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="category">Category to be tracked.</param>
        public ReportCategory(FinancialCategory category)
        {
            _category = category;
            _amount = 0;
            _entries = new List<CheckbookEntry>();
        }

        /// <summary>
        /// Add a new entry to the category.
        /// </summary>
        /// <param name="entry">Entry to be added.</param>
        /// <param name="split">Which split is in this category.</param>
        public void Add(CheckbookEntry entry, SplitEntry split)
        {
            _entries.Add(entry);
            _amount += split.Amount;
        }

        /// <summary>
        /// The category name.
        /// </summary>
        public string CategoryName
        {
            get
            {
                return _category.Name;
            }
        }

        /// <summary>
        /// Sum of all entry amounts in this category. (pos=deposits, neg=expenses)
        /// </summary>
        public long Amount
        {
            get
            {
                return _amount;
            }
        }

        /// <summary>
        /// Checkbook entries that have a split in this category.
        /// </summary>
        public List<CheckbookEntry> Entries
        {
            get
            {
                return _entries;
            }
        }
    }

    ///////////////////////////// CategoryReport /////////////////////////////

    /// <summary>
    /// Here's the report generator. 
    /// </summary>
    public class CategoryReportGenerator : BaseReportGenerator
    {

        /// <summary>
        /// Original graphics as passed-in.
        /// </summary>
        private Graphics _origGraphics;

        /// <summary>
        /// For printing to other-than-the-current page.
        /// </summary>
        private Graphics _nullGraphics = Graphics.FromImage(new Bitmap(10, 10));

        // Internal private state...
        private FontDesc _companyText = null;
        private FontDesc _emphasisText = null;
        private FontDesc _titleText = null;
        private FontDesc _headingText = null;
        private FontDesc _bodyText = null;
        private FontDesc _categoryText = null;
        private FontDesc _entryText = null;
        private FontDesc _monoText = null;
        private bool _detailed = false;
        private bool _hasMorePages = true;
        private bool _skippingPage = false;
        private float _xLeft = 0;
        private float _yTop = 0;
        private float _width = 100;
        private float _height = 100;
        private float _xChart = 0;
        private float _yChart = 0;
        private float _widthChart = 0;
        private float _xHeadings = 0;
        private float _xValues = 0;
        private float _xBox = 0;
        private float _widthBox = 0;
        private float _heightBox = 0;
        private float _xCatName = 0;
        private float _xCatAmount = 0;
        private float _xChkDate = 0;
        private float _xChkNbr = 0;
        private float _xChkPayee = 0;
        private float _xChkAmt = 0;
        private float _yOnPage = 0;
        private int _pageNumber = 0;
        private int _countEntriesPrinted = 0;
        private int _countEntriesSkipped = 0;
        private string _licensedTo = null;
        private string _logoFilePath = "";
        private DateTime _startDate;
        private DateTime _endDate;
        private IDbAccess _db = null;
        private Brush _boxDebitBrush = null;
        private Brush _boxCreditBrush = null;
        private Dictionary<Guid, ReportCategory> _reportCategories = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">DB to be reported on.</param>
        /// <param name="startDate">Starting date, inclusive.</param>
        /// <param name="endDate">Ending date, inclusive.</param>
        /// <param name="detailed">True to list all checkbook entries as well.</param>
        public CategoryReportGenerator(IDbAccess db, DateTime startDate, DateTime endDate, bool detailed)
        {
            _pageNumber = 0;
            _db = db;
            _startDate = startDate;
            _endDate = endDate;
            _detailed = detailed;
            _reportCategories = GetCategories();
        }

        /// <summary>
        /// Generate a page of the report.
        /// </summary>
        /// <param name="graphics">To render to.</param>
        /// <param name="margins">Margins, typically same as or within graphics visible clip bounds.</param>
        /// <returns>true if more pages follow</returns>
        public override bool PrintPage(Graphics graphics, Rectangle margins)
        {
            _graphics = graphics;
            _xLeft = margins != null ? margins.Left : graphics.VisibleClipBounds.X;
            _yTop = margins != null ? margins.Top : graphics.VisibleClipBounds.Y;
            _width = margins != null ? margins.Width : graphics.VisibleClipBounds.Width;
            _height = margins != null ? margins.Height : graphics.VisibleClipBounds.Height;
            if (_pageNumber == 0)
            {
                if (_reportCategories.Count < 1)
                {
                    return false; // empty db
                }
                RenderTopOfPage1(graphics, _db, _startDate, _endDate, _width, _height, _reportCategories);
            }
            return RenderCategoryList(graphics);
        }

        /// <summary>
        /// Render Heading and Pie Chart
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="db"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="yTop"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="reportCategories"></param>
        public void RenderTopOfPage1(Graphics graphics, IDbAccess db, 
            DateTime startDate, DateTime endDate, float width, float height, Dictionary<Guid, ReportCategory> reportCategories)
        {
            _origGraphics = graphics;
            _width = width;     // <-- redundant but necessary to support unit testing
            _height = height;   // <--  "
            _graphics = graphics;
            InitializeVariables();
            _graphics.FillRectangle(Brushes.White, new RectangleF(_xLeft, _yTop, width, height));
            long[] amountsArray;
            string[] captionsArray;
            long income = BuildPieChartArrays(reportCategories, out amountsArray, out captionsArray);
            int yBottom = UiHelperMethods.DrawPieChart(
                _graphics, (int)_xChart, (int)_yChart, (int)_widthChart, amountsArray, captionsArray);
            DrawLeftSideHeaderBlock(db, startDate, endDate, income);
            _yOnPage = Math.Max(_yOnPage, yBottom);
        }

        /// <summary>
        /// Render everything after heading, incuding subsequent pages. (First page must be printed first)
        /// </summary>
        /// <param name="graphics">When printing, this is different for each page!</param>
        /// <returns>True if there are more pages to follow.</returns>
        public bool RenderCategoryList(Graphics graphics)
        {
            _origGraphics = graphics;
            _pageNumber++;
            SelectGraphicsByY(_yOnPage);
            foreach (KeyValuePair<Guid, ReportCategory> pair in _reportCategories)
            {
                ReportCategory reportCategory = pair.Value;
                Brush boxBrush = reportCategory.Amount > 0 ? _boxCreditBrush: _boxDebitBrush;
                SelectGraphicsByY(_yOnPage);
                _graphics.FillRectangle(boxBrush, new RectangleF(_xBox, _yOnPage + 7, _widthBox, _heightBox + 1));
                DrawTextReturnNewY(reportCategory.CategoryName, _categoryText, _xCatName, _yOnPage + 8);
                _yOnPage = DrawTextReturnNewY(UtilityMethods.FormatCurrency(reportCategory.Amount, 3), _categoryText, _xCatAmount, _yOnPage + 8);
                if (_detailed)
                {
                    foreach (CheckbookEntry entry in reportCategory.Entries)
                    {
                        _graphics = SelectGraphicsByY(_yOnPage);
                        foreach (SplitEntry split in entry.Splits)
                        {
                            if (split.CategoryId == pair.Key)
                            {
                                _graphics.FillRectangle(boxBrush, new RectangleF(_xBox, _yOnPage - 1, _widthBox, _heightBox));
                                DrawTextReturnNewY(UtilityMethods.DateTimeToString(entry.DateOfTransaction), _entryText, _xChkDate, _yOnPage);
                                DrawTextReturnNewY("" + entry.CheckNumber, _entryText, _xChkNbr, _yOnPage);
                                DrawTextReturnNewY(entry.Payee, _entryText, _xChkPayee, _yOnPage);
                                _yOnPage = DrawTextReturnNewY(UtilityMethods.FormatCurrency(Math.Abs(split.Amount), 10), _monoText, _xChkAmt, _yOnPage);
                            }
                        }
                    }
                }
            }
            _yOnPage = _yTop;
            return _hasMorePages;
        }

        /////////////////////////////// Support //////////////////////////////

        /// <summary>
        /// Determine whether the cursor is on the current page or to divert printing to nullGraphics.
        /// </summary>
        /// <param name="yOnPage">Next anticipated print y</param>
        /// <returns>current graphics object, may be _nullGraphics if rendering a prior or future page.</returns>
        private Graphics SelectGraphicsByY(float yOnPage)
        {            
            if (yOnPage < _yTop + _height - _categoryText.Font.Height)
            {
                if (_skippingPage && _countEntriesSkipped < _countEntriesPrinted)
                {
                    _countEntriesSkipped++;
                    _yOnPage = _yTop;
                    _graphics = _nullGraphics; // Prior page
                }
                else
                {
                    _countEntriesPrinted++;
                    _skippingPage = false;
                    _hasMorePages = false;
                    _graphics = _origGraphics; // Current page
                }
            }
            else 
            {
                _countEntriesSkipped = 0;
                _skippingPage = true;
                _hasMorePages = true;
                _graphics = _nullGraphics; // Future page
            }
            return _graphics;
        }

        /// <summary>
        /// Create a dictionary of all ReportCategories by analyzing the db.
        /// </summary>
        /// <returns>Dictionary of populated ReportCategories</returns>
        public Dictionary<Guid, ReportCategory> GetCategories()
        {
            Dictionary<Guid, ReportCategory> reportCategories = new Dictionary<Guid, ReportCategory>();
            CheckbookEntryIterator iterator = _db.CheckbookEntryIterator;
            while (iterator.HasNextEntry())
            {
                CheckbookEntry entry = iterator.GetNextEntry();
                if (entry.DateOfTransaction.Date.CompareTo(_startDate.Date) < 0 ||
                    entry.DateOfTransaction.Date.CompareTo(_endDate.Date) > 0)
                {
                    continue;
                }
                foreach (SplitEntry split in entry.Splits)
                {
                    ReportCategory reportCategory = null;
                    if (reportCategories.Keys.Contains(split.CategoryId))
                    {
                        reportCategory = reportCategories[split.CategoryId];
                    }
                    else
                    {
                        FinancialCategory category = _db.GetFinancialCategoryById(split.CategoryId);
                        if(category == null || category.Name == null || category.Name.Length < 1)
                        {
                            continue;
                        }
                        reportCategory = new ReportCategory(category);
                        reportCategories.Add(split.CategoryId, reportCategory);
                    }
                    reportCategory.Add(entry, split);
                }
            }
            return reportCategories;
        }

        /// <summary>
        /// Populate the arrays to be passed to DrawPieChart.
        /// </summary>
        /// <param name="reportCategories">Populated.</param>
        /// <param name="amountsArray">To be filled with total amounts per category.</param>
        /// <param name="captionsArray">To be filled with corresponding category names.</param>
        /// <returns>Sum of all deposits (income/credits).</returns>
        public long BuildPieChartArrays(Dictionary<Guid, ReportCategory> reportCategories, out long[] amountsArray, out string[] captionsArray)
        {
            List<long> amounts = null;
            List<string> captions = null;
            long income = 0L;
            long sum = 0L;
            // assemble amounts/captions lists with everything
            amounts = new List<long>();
            captions = new List<string>();
            foreach (KeyValuePair<Guid, ReportCategory> pair in reportCategories)
            {
                ReportCategory reportCategory = pair.Value;
                amounts.Add(reportCategory.Amount);
                captions.Add(reportCategory.CategoryName);
                if (reportCategory.Amount < 0)
                {
                    sum -= reportCategory.Amount;
                }
                else
                {
                    income += reportCategory.Amount;
                }
            }
            // assemble largestAmounts/largestCaptions lists with largest ones
            long otherAmount = 0L; // small categories to be collapsed into one
            List<long> largestAmounts = new List<long>();
            List<string> largestCaptions = new List<string>();
            for (int index = 0; index < amounts.Count; ++index)
            {
                if (-amounts[index] * 36 > sum)
                {
                    largestAmounts.Add(Math.Abs(amounts[index]));
                    largestCaptions.Add(captions[index]);
                }
                else if (amounts[index] < 0)
                {
                    otherAmount += Math.Abs(amounts[index]);
                }
            }
            if (otherAmount > 0)
            {
                largestAmounts.Add(otherAmount);
                largestCaptions.Add(Strings.Get("Other"));
            }
            // populate arrays with amounts/captions
            amountsArray = largestAmounts.ToArray<long>();
            captionsArray = largestCaptions.ToArray<string>();
            return income;
        }

        /// <summary>
        /// Render the headr area to the left of the pie chart.
        /// </summary>
        /// <param name="db">To be analyzed.</param>
        /// <param name="startDate">Starting date, inclusive.</param>
        /// <param name="endDate">Ending date, inclusive.</param>
        /// <param name="income">Total income (account credits)</param>
        private void DrawLeftSideHeaderBlock(IDbAccess db, DateTime startDate, DateTime endDate, long income)
        {
            _yOnPage = _yChart;
            if (File.Exists(_logoFilePath))
            {
                Image logo = Image.FromFile(_logoFilePath);
                float logoWidth = logo.Width;
                float logoScale = _width * 0.36F / logoWidth;
                Image scaledLogo = UiHelperMethods.ResizeImage(
                    logo, (int)(logo.Width * logoScale), (int)(logo.Height * logoScale));
                _graphics.DrawImage(scaledLogo, _xHeadings, _yChart);
                _yOnPage = _yChart + (int)(logo.Height * logoScale + 10);
            }
            _yOnPage = DrawTextReturnNewY(Strings.Get("Able Strategies AbleCheckbook"), _companyText, _xHeadings, _yOnPage) + 10;
            _yOnPage = DrawTextReturnNewY(_licensedTo, _emphasisText, _xHeadings, _yOnPage) + 30;
            _yOnPage = DrawTextReturnNewY(Strings.Get("Category Report"), _titleText, _xHeadings, _yOnPage) + 18;
            DrawTextReturnNewY(Strings.Get("User:"), _headingText, _xHeadings, _yOnPage);
            _yOnPage = DrawTextReturnNewY(Environment.UserName, _headingText, _xValues, _yOnPage);
            DrawTextReturnNewY(Strings.Get("Account:"), _headingText, _xHeadings, _yOnPage);
            _yOnPage = DrawTextReturnNewY(db.Name, _headingText, _xValues, _yOnPage);
            DrawTextReturnNewY(Strings.Get("Date:"), _headingText, _xHeadings, _yOnPage);
            _yOnPage = DrawTextReturnNewY(UtilityMethods.DateTimeToString(DateTime.Now, true), _headingText, _xValues, _yOnPage);
            DrawTextReturnNewY(Strings.Get("From:"), _headingText, _xHeadings, _yOnPage);
            _yOnPage = DrawTextReturnNewY(UtilityMethods.DateTimeToString(startDate, false), _headingText, _xValues, _yOnPage);
            DrawTextReturnNewY(Strings.Get("Thru:"), _headingText, _xHeadings, _yOnPage);
            _yOnPage = DrawTextReturnNewY(UtilityMethods.DateTimeToString(endDate, false), _headingText, _xValues, _yOnPage);
            DrawTextReturnNewY(Strings.Get("Deposits:"), _emphasisText, _xHeadings, _yOnPage + 20);
            _yOnPage = DrawTextReturnNewY(UtilityMethods.FormatCurrency(income, 3), _emphasisText, _xValues, _yOnPage + 20);
        }

        /// <summary>
        /// Build the text layouts / fonts / locations / sizes based on _xLeft and _width.
        /// </summary>
        private void InitializeVariables()
        {
            // fonts
            _companyText = new FontDesc(Brushes.RoyalBlue, new Font(FontFamily.GenericSansSerif, _width / 44, FontStyle.Bold));
            _emphasisText = new FontDesc(Brushes.DarkRed, new Font(FontFamily.GenericSansSerif, _width / 58, FontStyle.Bold));
            _titleText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 44, FontStyle.Bold));
            _headingText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 56, FontStyle.Bold));
            _bodyText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 60, FontStyle.Regular));
            _categoryText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 60, FontStyle.Bold));
            _entryText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericSansSerif, _width / 66, FontStyle.Regular));
            _monoText = new FontDesc(Brushes.Black, new Font(FontFamily.GenericMonospace, _width / 64, FontStyle.Bold));
            // x/y locations
            _xChart = _xLeft + _width * 0.5F + 20;
            _yChart = _yTop + 10;
            _xHeadings = _xLeft + 20;
            _xValues = _xLeft + _headingText.Font.Height * 6;
            _xBox = _xLeft + 20;
            _widthBox = _width - 40;
            _heightBox = _categoryText.Font.Height + 1;
            _xCatName = _xLeft + _categoryText.Font.Height * 2;
            _xCatAmount = _width - (_categoryText.Font.Height * 13);
            _xChkDate = _xLeft + _entryText.Font.Height * 6;
            _xChkNbr = _xLeft + _entryText.Font.Height * 12;
            _xChkPayee = _xLeft + _entryText.Font.Height * 19;
            _xChkAmt = _xLeft + Math.Min(_entryText.Font.Height * 34, _width - (_entryText.Font.Height * 8));
            // other...
            _countEntriesPrinted = 0;
            _countEntriesSkipped = 0;
            _boxDebitBrush = new SolidBrush(Color.FromArgb(255, 255, 230, 230));
            _boxCreditBrush = new SolidBrush(Color.FromArgb(255, 230, 255, 230));
            _logoFilePath = Configuration.Instance.FindSupportFilePath("logo-###.png");
            _widthChart = _width * 0.49F - 50;
            if (_width > _height)
            {
                _widthChart = _height * 0.49F - 40;
                _xChart = _width - (_widthChart + 40);
            }
            _licensedTo = Strings.Get("UNLICENSED_VERSION");
            if (Configuration.Instance.GetIsLicensedVersion())
            {
                _licensedTo = Configuration.Instance.SiteDescription;
            }
        }

    }

}
