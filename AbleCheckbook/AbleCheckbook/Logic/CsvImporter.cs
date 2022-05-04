using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class CsvImporter : IDisposable
    {

        /// <summary>
        /// CSV column identifiers.
        /// </summary>
        private enum ColumnMap
        {
            Date = 0,  
            CheckNum = 1,
            Payee = 2,
            Category = 3,
            Memo = 4,
            Debit = 5,
            Credit = 6,
            Amount = 7,
            Cleared = 8,
        }

        /// <summary>
        /// Flags a missing column.
        /// </summary>
        private static int NotPresent = 100;

        /// <summary>
        /// DB into which the data will be imported.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// CSV file to be processed.
        /// </summary>
        private StreamReader _reader = null;

        /// <summary>
        /// Error message.
        /// </summary>
        private string _errorMessage = Strings.Get("Note: No Importable Entries Read");

        /// <summary>
        /// Track line numbers in CSV file for diagnostics.
        /// </summary>
        private int _lineNumber = 0;

        /// <summary>
        /// How many entries were imported?
        /// </summary>
        private int _entryCount = 0;

        /// <summary>
        /// Are tabs used instead of commas?
        /// </summary>
        private bool _useTabs = false;

        /// <summary>
        /// Was a header present?
        /// </summary>
        private bool _foundHeader = false;

        /// <summary>
        /// How many columns are significant?
        /// </summary>
        private int _numColumns = 8;

        /// <summary>
        /// Column mappings.
        /// </summary>
        private Dictionary<ColumnMap, int> _columnMap = new Dictionary<ColumnMap, int>
        {
            { ColumnMap.Date, 0 },
            { ColumnMap.CheckNum, 1 },
            { ColumnMap.Payee, 2 },
            { ColumnMap.Category, 3 },
            { ColumnMap.Memo, 4 },
            { ColumnMap.Debit, 5 },
            { ColumnMap.Credit, 6 },
            { ColumnMap.Amount, NotPresent },
            { ColumnMap.Cleared, NotPresent },
        };

        /// <summary>
        /// Did an error occur?.
        /// </summary>
        public bool ErrorOccurred { get => _errorMessage.Length > 0; }

        /// <summary>
        /// Description of any error.
        /// </summary>
        public string ErrorMessage { get => _errorMessage; }

        /// <summary>
        /// Path to CSV file.
        /// </summary>
        private string _fullPath = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">DB into which the data will be imported.</param>
        public CsvImporter(IDbAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Import a CSV file.
        /// </summary>
        /// <param name="fullPath">Full path and filename of CSV file.</param>
        /// <returns>How many entries were imported</returns>
        public int Import(string fullPath)
        {
            _fullPath = fullPath;
            if (!Path.IsPathRooted(_fullPath))
            {
                _fullPath = Path.Combine(Configuration.Instance.DirectoryImportExport, Path.GetFileName(_fullPath));
            }
            try
            {
                _reader = new StreamReader(_fullPath);
                if (!IdentifyColumnsUsingHeaders())
                {
                    _reader.Close();
                    _reader = new StreamReader(_fullPath);
                    IdentifyColumnsByGuessing();
                }
                _reader.Close();
                _reader = new StreamReader(_fullPath);
                if (_foundHeader)
                {
                    GetNextLine(); // discard header line
                }
                ImportEntries();
                _reader.Close();
            }
            catch(Exception ex)
            {
                _errorMessage = ex.Message;
                _entryCount = 0;
            }
            return _entryCount;
        }

        /// <summary>
        /// Read headers from the CSV file and populate _columnMap authoritatively.
        /// </summary>
        /// <returns>success</returns>
        private bool IdentifyColumnsUsingHeaders()
        {
            string buffer = _reader.ReadLine().ToLower();
            char delimiter = ',';
            int numTabs = 0, numCommas = 0;
            bool sawDate = false, sawPayee = false, sawDebit = false, sawCredit = false, sawAmount = false;
            foreach (char ch in buffer)
            {
                if (ch == '\x09')
                {
                    ++numTabs;
                }
                else if (ch == ',')
                {
                    ++numCommas;
                }
            }
            if (numTabs > numCommas)
            {
                _useTabs = true;
                delimiter = '\x09';
            }
            _columnMap[ColumnMap.Date] = 0;
            _columnMap[ColumnMap.Payee] = 2;
            _columnMap[ColumnMap.Category] = NotPresent;
            _columnMap[ColumnMap.CheckNum] = NotPresent;
            _columnMap[ColumnMap.Memo] = NotPresent;
            _numColumns = 0;
            int colNum = 0;
            string[] headers = buffer.ToLower().Split(delimiter);
            foreach(string header in headers)
            {
                string hdr = header.Trim();
                if(hdr.StartsWith("\"") && hdr.EndsWith("\""))
                {
                    hdr = hdr.Substring(1, hdr.Length - 2);
                }
                switch(hdr)
                {
                    case "date":
                    case "transaction date":
                    case "date of tran":
                    case "date of transaction":
                        sawDate = true;
                        _columnMap[ColumnMap.Date] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                    case "check #":
                    case "checknum":
                    case "check number":
                        _columnMap[ColumnMap.CheckNum] = colNum;
                        break;
                    case "amount":
                        sawAmount = true;
                        _columnMap[ColumnMap.Amount] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                    case "debit":
                    case "payment":
                        sawDebit = true;
                        _columnMap[ColumnMap.Debit] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                    case "credit":
                    case "deposit":
                        sawCredit = true;
                        _columnMap[ColumnMap.Credit] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                    case "cleared":
                        _columnMap[ColumnMap.Cleared] = colNum;
                        break;
                    case "payee":
                        sawPayee = true;
                        _columnMap[ColumnMap.Payee] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                    case "memo":
                        _columnMap[ColumnMap.Memo] = colNum;
                        break;
                    case "category":
                    case "description":
                        _columnMap[ColumnMap.Category] = colNum;
                        if (!sawPayee)
                        {
                            sawPayee = true; // default to this column as payee for now
                        }
                        _columnMap[ColumnMap.Payee] = colNum;
                        _numColumns = Math.Max(_numColumns, colNum + 1);
                        break;
                }
                ++colNum;
            }
            if (sawDate && sawPayee)
            {
                if(sawDebit && sawCredit)
                {
                    _columnMap[ColumnMap.Amount] = NotPresent;
                    _foundHeader = true;
                    return true;
                }
                if (sawAmount)
                {
                    _columnMap[ColumnMap.Debit] = NotPresent;
                    _columnMap[ColumnMap.Credit] = NotPresent;
                    _foundHeader = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Prescan the CSV file and populate _columnMap by making an educated guess.
        /// </summary>
        private void IdentifyColumnsByGuessing()
        {
            // Brute force - not doing any sophisticated mapping...
            string buffer = _reader.ReadLine().ToLower();
            _columnMap[ColumnMap.Date] = 0;
            _columnMap[ColumnMap.CheckNum] = 1;
            _columnMap[ColumnMap.Payee] = 2;
            _columnMap[ColumnMap.Category] = 3;
            if (buffer.Contains("debit") || buffer.Contains("payment") ||
                buffer.Contains("credit") || buffer.Contains("deposit"))
            {
                _columnMap[ColumnMap.Debit] = 4;
                _columnMap[ColumnMap.Credit] = 5;
                _columnMap[ColumnMap.Cleared] = 6;
                _numColumns = 7;
                _foundHeader = true;
                _columnMap[ColumnMap.Amount] = NotPresent;
                if (buffer.Contains("memo") && buffer.Contains("debit") &&
                    buffer.IndexOf("memo") < buffer.IndexOf("debit"))
                {
                    _columnMap[ColumnMap.Debit] = 5;
                    _columnMap[ColumnMap.Credit] = 6;
                    _columnMap[ColumnMap.Cleared] = 7;
                    _numColumns = 8;
                }
            }
            else
            {
                _columnMap[ColumnMap.Amount] = 4;
                _columnMap[ColumnMap.Cleared] = 5;
                _numColumns = 6;
                _columnMap[ColumnMap.Debit] = NotPresent;
                _columnMap[ColumnMap.Credit] = NotPresent;
            }
            if (buffer.Contains("category") || buffer.Contains("reference"))
            {
                // Suntrust
            }
            else if(buffer.Contains("transaction"))
            {
                // Truist
                _columnMap[ColumnMap.Payee]++;
                _columnMap[ColumnMap.CheckNum]++;
            }
            else
            {
                _columnMap[ColumnMap.Debit]--;
                _columnMap[ColumnMap.Credit]--;
                _columnMap[ColumnMap.Amount]--;
                _columnMap[ColumnMap.Cleared]--;
                _numColumns--;
                _columnMap[ColumnMap.Category] = NotPresent;
            }
            if (buffer.Contains("cleared") || buffer.Contains("reconcile"))
            {
                if (buffer.Contains("balance"))
                {
                    _columnMap[ColumnMap.Cleared]++;
                    _numColumns++;
                }
            }
            else
            {
                _columnMap[ColumnMap.Cleared] = NotPresent;
                _numColumns--;
            }
        }

        /// <summary>
        /// Here's where the heavy lifting is done.
        /// </summary>
        private void ImportEntries()
        {
            while (!_reader.EndOfStream)
            {
                string currentLine = GetNextLine();
                string[] columns = SplitColumns(currentLine);
                if (columns.Length < _numColumns)
                {
                    continue;
                }
                string dateString = columns[_columnMap[ColumnMap.Date]];
                string checkNum = "";
                if (_columnMap[ColumnMap.CheckNum] < NotPresent)
                {
                    checkNum = columns[_columnMap[ColumnMap.CheckNum]];
                    if (!Regex.IsMatch(checkNum, ".*[1-9].*", System.Text.RegularExpressions.RegexOptions.None))
                    {
                        checkNum = "";
                    }
                }
                string payee = columns[_columnMap[ColumnMap.Payee]].Replace("\\x09", " ").Replace("   ", " ").Replace("  ", " ").Trim();
                string category = "";
                if (_columnMap[ColumnMap.Category] < NotPresent)
                {
                    category = columns[_columnMap[ColumnMap.Category]].Replace("   ", " ").Replace ("  ", " ").Trim();
                }
                string memo = "";
                if (_columnMap[ColumnMap.Memo] < NotPresent)
                {
                    memo = columns[_columnMap[ColumnMap.Memo]];
                }
                bool cleared = true;
                if(_columnMap[ColumnMap.Cleared] < NotPresent)
                {
                    cleared = columns[_columnMap[ColumnMap.Cleared]].ToLower().Equals("x");
                }
                int amount = GetAmount(columns);
                TransactionKind kind = (amount > 0) ? TransactionKind.Deposit : TransactionKind.Payment;
                CheckbookEntry entry = new CheckbookEntry();
                entry.DateOfTransaction = UtilityMethods.StringToDateTime(dateString);
                int result = 0;
                int.TryParse(checkNum, out result);
                entry.MadeBy = EntryMadeBy.Importer;
                entry.CheckNumber = result == 0 ? "" : "" + result;
                entry.Payee = payee;
                entry.IsCleared = cleared;
                Guid categoryId = UtilityMethods.GetOrCreateCategory(_db, category, amount > 0).Id;
                entry.AddSplit(categoryId, kind, Math.Abs(amount));
                _entryCount++;
                _db.InsertEntry(entry);
            }
        }

        /// <summary>
        /// Get the tran amount from either the amount or the debit/credit columns.
        /// </summary>
        /// <param name="columns">populated columns</param>
        /// <returns>Amount, positive if deposit, negative if payment.</returns>
        private int GetAmount(string[] columns)
        {
            string amountString;
            int amount = 0;
            if (_columnMap[ColumnMap.Amount] < _numColumns)               // Use "amount" column
            {
                amountString = columns[_columnMap[ColumnMap.Amount]];
                if (!ParseValue(amountString, out amount))
                {
                    _errorMessage = "Cannot parse amount " + amountString + " at L" + _lineNumber + ". ";
                    throw new IOException(_errorMessage);
                }
            }
            else                                                          // Use "debit" column
            {
                amountString = columns[_columnMap[ColumnMap.Debit]];
                if (!ParseValue(amountString, out amount))
                {
                    _errorMessage = "Cannot parse debit " + amountString + " at L" + _lineNumber + ". ";
                    throw new IOException(_errorMessage);
                }
                amount = -Math.Abs(amount);
                if (amount == 0)                                         // Use "credit" column
                {
                    amountString = columns[_columnMap[ColumnMap.Credit]];
                    if (!ParseValue(amountString, out amount))
                    {
                        _errorMessage = "Cannot parse credit " + amountString + " at L" + _lineNumber + ". ";
                        throw new IOException(_errorMessage);
                    }
                    amount = Math.Abs(amount); 
                }
            }
            return amount;
        }

        /// <summary>
        /// Parse a monetary amount from a string.
        /// </summary>
        /// <param name="textAmt">String containing currency in cents with a decimal point</param>
        /// <returns>true if valid numeric, even if empty</returns>
        private bool ParseValue(string textAmt, out int amount)
        {
            const string currencyRegex = "^[0-9\\$\\.\\,\\(\\)\\-\\+]*$"; // should be precompiled
            if (textAmt == null || !Regex.IsMatch(textAmt, currencyRegex, System.Text.RegularExpressions.RegexOptions.None))
            {
                amount = 0;
                return false;
            }
            amount = (int)UtilityMethods.ParseCurrency(textAmt);
            return true;

        }

        /// <summary>
        /// Split a line into multiple fields.
        /// </summary>
        /// <param name="oneLine">the CSV line</param>
        /// <returns>array of fields as delimited by commas or tabs</returns>
        private string[] SplitColumns(string oneLine)
        {
            string[] columns = oneLine.Split(_useTabs ? '\x09' : ',');
            for(int columnNumber = 0; columnNumber < columns.Length; ++columnNumber)
            {
                if(columns[columnNumber].StartsWith("\"") && columns[columnNumber].EndsWith("\""))
                {
                    int length = columns[columnNumber].Length;
                    columns[columnNumber] = columns[columnNumber].Substring(1, length - 2);
                }
                columns[columnNumber] = columns[columnNumber].Trim();
            }
            return columns;
        }

        /// <summary>
        /// Read a line from the CSV file.
        /// </summary>
        /// <returns></returns>
        private string GetNextLine()
        {
            string buffer = _reader.ReadLine();
            ++_lineNumber;
            if (_useTabs)
            {
                if (buffer.Contains("\""))
                {
                    buffer.Replace("\x09", ",");
                }
                else
                {
                    buffer = "\"" + buffer.Replace("\x09", "\",\"") + "\"";
                }
            }
            return buffer;
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
