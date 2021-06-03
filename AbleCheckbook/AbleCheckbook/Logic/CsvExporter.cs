using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://central.xero.com/s/article/Import-a-CSV-bank-statement-US#Preparethedatainthefile
// C:\Users\jimcl\source\repos\AbleCheckbook\Suntrust_History.csv

namespace AbleCheckbook.Logic
{
    /// <summary>
    /// Note that this collapses splits into a single entry.
    /// </summary>
    public class CsvExporter : IDisposable
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
        public CsvExporter(IDbAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Export the DB content.
        /// </summary>
        /// <param name="fullPath">Full path and filename of the output CSV.</param>
        /// <returns>Success</returns>
        public bool Export(string fullPath)
        {
            if(!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(Configuration.Instance.DirectoryImportExport, Path.GetFileName(fullPath));
            }
            try
            {
                _writer = new StreamWriter(fullPath, false);
                _writer.WriteLine("\"Date\",\"Check#\",\"Payee\",\"Category\",\"Memo\",\"Debit\",\"Credit\",\"XCleared\"");
                CheckbookEntryIterator iterator = _db.CheckbookEntryIterator;
                while(iterator.HasNextEntry())
                {
                    CheckbookEntry entry = iterator.GetNextEntry();
                    FinancialCategory category = _db.GetFinancialCategoryById(entry.Splits[0].CategoryId);
                    if (category == null)
                    {
                        category = UtilityMethods.GetCategoryOrUnknown(_db, null);
                    }
                    long amount = entry.Amount;
                    string csvAmount = Math.Abs(amount / 100.0).ToString("F2");
                    StringBuilder buffer = new StringBuilder();
                    buffer.Append("\"" + UtilityMethods.DateTimeToString(entry.DateOfTransaction) + "\",");
                    buffer.Append("\"" + entry.CheckNumber.Replace("\"", "'") + "\",");
                    buffer.Append("\"" + entry.Payee.Replace("\"", "'") + "\",");
                    buffer.Append("\"" + category.Name.Replace("\"", "'") + "\",");
                    buffer.Append("\"" + entry.Memo.Replace("\x0d", ";").Replace("\x0a", "").Replace("\"", "'") + "\",");
                    buffer.Append("\"" + ((amount < 0L) ? ("-" + csvAmount) : "0") + "\",");
                    buffer.Append("\"" + ((amount > 0L) ? csvAmount : "0") + "\",");
                    buffer.Append("\"" + (entry.IsCleared ? "X" : "") + "\"");
                    _writer.WriteLine(buffer.ToString());
                }
                _writer.Close();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                return false;
            }
            return true;
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
