using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// For exporting a QIF file from the db.
    /// </summary>
    public class QifExporter : IDisposable
    {

        /// <summary>
        /// Here's the source of the data.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// Stream to export.
        /// </summary>
        private StreamWriter _writer = null;

        /// <summary>
        /// If an error occurs...
        /// </summary>
        private string _errorMessage = "";

        // Getters/Setters
        public string ErrorMessage { get => _errorMessage; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Source of the data to export.</param>
        public QifExporter(IDbAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Export the DB content.
        /// </summary>
        /// <param name="fullPath">Full path and filename of the output QIF.</param>
        /// <returns>Success</returns>
        public bool Export(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(Configuration.Instance.DirectoryImportExport, Path.GetFileName(fullPath));
            }
            try
            {
                _writer = new StreamWriter(fullPath, false);
                ExportHeader();
                ExportFinancialCategories();
                ExportCheckbookEntries();
                _writer.Close();
            }
            catch(Exception ex)
            {
                _errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Export the file header.
        /// </summary>
        private void ExportHeader()
        {
            _writer.WriteLine("!Clear:AutoSwitch");
            _writer.WriteLine("!Account");
            _writer.WriteLine("N" + _db.Name);
            _writer.WriteLine("DChecking");
            _writer.WriteLine("TBank");
            _writer.WriteLine("^");
        }

        /// <summary>
        /// Export the categories.
        /// </summary>
        private void ExportFinancialCategories()
        {
            _writer.WriteLine("!Type:Cat");
            FinancialCategoryIterator iterator = _db.FinancialCategoryIterator;
            while (iterator.HasNextEntry())
            {
                FinancialCategory entry = iterator.GetNextEntry();
                _writer.WriteLine("N" + entry.Name.Trim());
                if (entry.IsCredit)
                {
                    _writer.WriteLine("D" + entry.Name + " (Income)");
                    _writer.WriteLine("T");
                    _writer.WriteLine("I");
                }
                else
                {
                    _writer.WriteLine("D" + entry.Name + " (Expense)");
                    _writer.WriteLine("E");
                }
                _writer.WriteLine("^");
            }
        }

        /// <summary>
        /// Export the checkbook entries.
        /// </summary>
        private void ExportCheckbookEntries()
        {
            _writer.WriteLine("!Type:Bank");
            CheckbookEntryIterator iterator = _db.CheckbookEntryIterator;
            while (iterator.HasNextEntry())
            {
                CheckbookEntry entry = iterator.GetNextEntry();
                DateTime date = entry.DateOfTransaction;
                string yearPrefix = (date.Year >= 2000) ?  "'" : "/";
                _writer.WriteLine("D" + date.Month + "/" + date.Day + yearPrefix + (date.Year % 100));
                _writer.WriteLine("T" + (entry.Amount / 100.0).ToString("F2"));
                if(entry.CheckNumber.Length > 0)
                {
                    _writer.WriteLine("N" + entry.CheckNumber);
                }
                _writer.WriteLine("P" + entry.Payee);
                if(entry.IsCleared == true)
                {
                    _writer.WriteLine("CX");
                }
                foreach (SplitEntry split in entry.Splits)
                {
                    FinancialCategory category = _db.GetFinancialCategoryById(split.CategoryId);
                    if(category == null)
                    {
                        category = UtilityMethods.GetCategoryOrUnknown(_db, null);
                    }
                    if(entry.Splits.Length == 1 && category.IsCredit)
                    {
                        _writer.WriteLine("L" + category.Name);
                    }
                    else
                    {
                        _writer.WriteLine("S" + category.Name);
                        _writer.WriteLine("$" + (split.Amount / 100.0).ToString("F2"));
                    }
                }
                _writer.WriteLine("^");
            }
        }

        /// <summary>
        /// IDisposable.
        /// </summary>
        public void Dispose()
        {
            // Doesn't leave files open
        }
    }
}
