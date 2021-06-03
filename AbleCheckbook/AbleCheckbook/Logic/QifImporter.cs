using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.w3.org/2000/10/swap/pim/qif-doc/QIF-doc.htm
// C:\Users\jimcl\source\repos\AbleCheckbook\2020.QIF

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// This imports a QIF file.
    /// </summary>
    public class QifImporter : IDisposable
    {

        /// <summary>
        /// These are the kinds of records we get from a QIF file.
        /// </summary>
        private enum RecordType
        {
            None =      0,
            Bank =      1,   // <-- Bank, Cash, and CCard are all the same
            Memorized = 2,
            Category =  3,
            Account =   4,
        }

        /// <summary>
        /// Read from this stream.
        /// </summary>
        private StreamReader _reader = null;

        /// <summary>
        /// Here's the DB we'll be importing to.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// Buffer for the current line as read from the QIF file.
        /// </summary>
        private string _currentLine = "";

        /// <summary>
        /// The first character of each line of a QIF file is the field "type".
        /// </summary>
        private char _firstCh = ' ';

        /// <summary>
        /// This is the field content after each field type that gets read.
        /// </summary>
        private string _currentTail = "";

        /// <summary>
        /// Keep track of the last line read.
        /// </summary>
        private int _lineNumber = 0;

        /// <summary>
        /// Keep track of the number of entries added.
        /// </summary>
        private int _numEntries = 0;

        /// <summary>
        /// Error message.
        /// </summary>
        private string _errorMessage = "";

        /// <summary>
        /// Warning messages.
        /// </summary>
        private string _warningMessages = "";

        /// <summary>
        /// As we parse, this tracks the kind of records we are reading.
        /// </summary>
        private RecordType _recordType = RecordType.None;

        /// <summary>
        /// Buffer for data fields as they are read in.
        /// </summary>
        private Dictionary<char, string> _fields = new Dictionary<char, string>();

        // Getters/Setters
        public string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }
        public string WarningMessages { get => _warningMessages; set => _warningMessages = value; }
        public int LineNumber { get => _lineNumber; set => _lineNumber = value; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">To be updated.</param>
        public QifImporter(IDbAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Import the QIF file in two passes, first to get categories then to get the remainder.
        /// </summary>
        /// <param name="fullPath">full path and filename of QIF</param>
        /// <returns>Number of lines read, 0 on error</returns>
        public int Import(string fullPath)
        {
            _numEntries = 0;
            _errorMessage = "";
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(Configuration.Instance.DirectoryImportExport, Path.GetFileName(fullPath));
            }
            try
            {
                _reader = new StreamReader(fullPath);
                _lineNumber = 0;
                _firstCh = ' ';
                ImportStream();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                return 0;
            }
            finally
            {
                _reader.Close();
            }
            if(_lineNumber < 15)
            {
                _errorMessage = "Empty QIF file";
                return 0;
            }
            return _numEntries;
        }

        /// <summary>
        /// Read in the file and process it one line at a time.
        /// </summary>
        private void ImportStream()
        { 
            bool skipToNextBang = false;
            while (!_reader.EndOfStream)
            {
                _firstCh = Readline();
                if (skipToNextBang && _firstCh != '!')
                {
                    continue;
                }
                skipToNextBang = false;
                switch (_firstCh)
                {
                    case '!':
                        SavePending();
                        skipToNextBang = !HandleBang();
                        break;
                    case '^':
                        SavePending();
                        skipToNextBang = false;
                        break;
                    default:
                        HandleDataField();
                        break;
                }
            }
            SavePending();
        }

        /// <summary>
        /// Read one line of the QIF file.
        /// </summary>
        /// <returns>first character, [space] to ignore line</returns>
        private char Readline()
        {
            _currentLine = _reader.ReadLine();
            _lineNumber++;
            if (_currentLine.Length < 1)
            {
                return ' ';
            }
            char ch = _currentLine[0];
            _currentTail = _currentLine.Substring(1).Trim();
            return ch;
        }

        /// <summary>
        /// Save any pending records to db.
        /// </summary>
        private void SavePending()
        {
            switch(_recordType)
            {
                case RecordType.Bank:
                    HandleBankRecord();
                    break;
                case RecordType.Memorized:
                    HandleMemorizedRecord();
                    break;
                default:
                    break;
            }
            _fields = new Dictionary<char, string>();
        }

        /// <summary>
        /// This is the big guy that does the heavy lifting.
        /// </summary>
        private void HandleBankRecord()
        {
            TransactionKind kind = TransactionKind.Deposit;
            CheckbookEntry entry = new CheckbookEntry();
            entry.MadeBy = EntryMadeBy.Importer;
            if(GetField('T').TrimStart().StartsWith("-"))
            {
                kind = TransactionKind.Payment;
            }
            entry.MadeBy = EntryMadeBy.Importer;
            entry.CheckNumber = GetNumericField('N') == 0 ? "" : "" + GetNumericField('N');
            entry.Payee = GetField('P');
            entry.Memo = GetField('M');
            string dateString = GetField('D');
            if(dateString.Length > 5)
            {
                dateString = dateString.Replace("'", "/20").Replace("/ ", "/").Trim(); // Weird QIF hack
                entry.DateOfTransaction = UtilityMethods.StringToDateTime(dateString);
            }
            string[] categories = GetField('S').Length == 0 ? new string[0] : GetField('S').Split('|');
            string[] amounts = GetField('$').Length == 0 ? new string[0] : GetField('$', "0").Split('|');
            if(GetField('C').ToUpper().Contains("X"))
            {
                entry.IsCleared = true;
                entry.DateCleared = entry.DateOfTransaction;
            }
            if (categories.Length != amounts.Length)
            {
                entry.AppendMemo("Bad QIF record " + entry.DateOfTransaction + " " + entry.Payee);
                _warningMessages += "Bad QIF record " + entry.DateOfTransaction + " " + entry.Payee + " L" + _lineNumber + ".\n";
            }
            int splitCount = Math.Min(categories.Length, amounts.Length);
            for (int index = 0; index < splitCount; ++index)
            {
                AddSplit(entry, categories[index], kind, amounts[index]);
            }
            if (splitCount < 1 && entry.Payee.Length > 1)
            {
                AddSplit(entry, GetField('L', "Unknown"), kind, GetField('T', "0"));
            }
            if (entry.Payee.Length > 1 && entry.Splits.Length > 0)
            {
                if(entry.Payee.ToLower().Equals("opening balance"))
                {
                    entry = CreateStartingBalance(entry);
                }
                if (entry != null)
                {
                    _numEntries++;
                    _db.InsertEntry(entry);
                }
            }
        }

        /// <summary>
        /// Create or update the starting balance.
        /// </summary>
        /// <param name="entry">balance adjustment</param>
        /// <returns>The same entry passed in, null if it should not be saved to the DB</returns>
        private CheckbookEntry CreateStartingBalance(CheckbookEntry entry)
        {
            // If we are importing to an existing acct, zero-out the opening balance
            CheckbookEntryIterator iterator = _db.CheckbookEntryIterator;
            if (iterator.HasNextEntry())
            {
                CheckbookEntry startingBalance = iterator.GetNextEntry();
                if (startingBalance.Amount == 0 && startingBalance.Splits.Length == 1 && startingBalance.Splits[0].Kind == TransactionKind.Adjustment)
                {
                    _db.DeleteEntry(startingBalance);
                    startingBalance.DateOfTransaction = entry.DateOfTransaction;
                    startingBalance.Splits[0].Amount = entry.Amount;
                    startingBalance.Memo = "QIF OPENING BALANCE WAS " + UtilityMethods.FormatCurrency(entry.Amount);
                    _db.InsertEntry(startingBalance);
                    entry = null;
                }
                else
                {
                    _warningMessages += "Zeroed out opening balance [" + entry.Amount + "] L" + _lineNumber + ".\n";
                    entry.Memo = "QIF OPENING BALANCE WAS " + UtilityMethods.FormatCurrency(entry.Amount);
                    entry.Splits[0].Amount = 0;
                }
            }
            return entry;
        }

        /// <summary>
        /// Add a split to a checkbook entry.
        /// </summary>
        /// <param name="entry">To be updated</param>
        /// <param name="catName">Category name</param>
        /// <param name="kind">payment, deposit, etc.</param>
        /// <param name="amt">amount of entry - money</param>
        private void AddSplit(CheckbookEntry entry, string catName, TransactionKind kind, string amt)
        {
            Guid id = UtilityMethods.GetOrCreateCategory(_db, catName, (kind == TransactionKind.Deposit)).Id;
            entry.AddSplit(id, kind, StringToMoney(amt));
        }

        /// <summary>
        /// Import a memorized payee.
        /// </summary>
        private void HandleMemorizedRecord()
        {
            string amount = GetField('T');
            bool isCredit = !amount.StartsWith("-");
            string payee = GetField('P');
            string categoryName = GetField('L');
            Guid catId = UtilityMethods.GetOrCreateCategory(_db, categoryName, isCredit).Id;
            TransactionKind kind = isCredit ? TransactionKind.Deposit : TransactionKind.Payment;
            MemorizedPayee oldPayee = _db.GetMemorizedPayeeByName(payee);
            MemorizedPayee newPayee = new MemorizedPayee(payee, catId, kind, StringToMoney(amount));
            if (oldPayee == null)
            {
                _db.InsertEntry(newPayee);
            }
            else
            {
                _db.UpdateEntry(newPayee, oldPayee);
            }
        }

        /// <summary>
        /// Convert a string to a monetary amount.
        /// </summary>
        /// <param name="amtString">String containing money amount.</param>
        /// <returns>The amount of money, numerically parsed, or 0 on error.</returns>
        private int StringToMoney(string amtString)
        {
            amtString = amtString.Replace(" ", "").Replace(",", "");
            if(!amtString.Contains("."))
            {
                amtString += "00";
            }
            int amt;
            if(int.TryParse(amtString.Replace(".", ""), out amt))
            {
                return amt;
            }
            _warningMessages += "Trouble parsing money from [" + amtString + "] in QIF file L" + _lineNumber + ".\n";
            return 0;
        }

        /// <summary>
        /// Handle a command that begins with a bang (!).
        /// </summary>
        /// <returns>Was it recognized and handled?</returns>
        private bool HandleBang()
        {
            if (_currentTail.Equals("Type:Bank")) // || _currentTail.Equals("Type:Cash") || _currentTail.Equals("Type:CCard"))
            {
                _recordType = RecordType.Bank;
            }
            else if (_currentTail.Equals("Type:Memorized"))
            {
                _recordType = RecordType.Memorized;
            }
            else if (_currentTail.Equals("Type:Cat"))
            {
                _recordType = RecordType.None;
                return false; // We're gleaning category information from check (bank) records
            }
            else if (_currentTail.Equals("Account"))  
            {
                _recordType = RecordType.None;
                return false; // Do we really care about account? (bank name, acct type, etc.)
            }
            else // unimportant bang command
            {
                _recordType = RecordType.None;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Keep track of a field in a record. (For multiple values in one field uses a | separator)
        /// </summary>
        private void HandleDataField()
        {
            if(_fields.ContainsKey(_firstCh))
            {
                _fields[_firstCh] = _fields[_firstCh] + "|" + _currentTail.Replace('|', ':');
            }
            else
            {
                _fields[_firstCh] = _currentTail.Replace('|', ':');
            }
        }

        /// <summary>
        /// Fetch a field from the _fields[] buffer.
        /// </summary>
        /// <param name="key">first character - field type</param>
        /// <param name="defaultValue">To be returned if the field is not found</param>
        /// <returns></returns>
        private string GetField(char key, string defaultValue = "")
        {
            if(!_fields.ContainsKey(key))
            {
                return defaultValue;
            }
            return _fields[key];
        }

        /// <summary>
        /// Fetch a numeric field from the _fields[] buffer.
        /// </summary>
        /// <param name="key">first character - field type</param>
        /// <param name="defaultValue">To be returned if the field is not found</param>
        /// <returns></returns>
        private int GetNumericField(char key, int defaultValue = 0)
        {
            if (!_fields.ContainsKey(key))
            {
                return defaultValue;
            }
            int result = 0;
            if(int.TryParse(_fields[key], out result))
            {
                return result;
            }
            return defaultValue;
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
