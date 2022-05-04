using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook
{

    public enum EntryColor // Override numerically, highest valid one wins
    {
        Past = 0,                 // Past Trans/Modif              Typ Red on White
        Future = 1,               // Impending Trans               Typ Red on Pink
        PastMod = 2,              // Past Trans, Modif             Typ Br Red on White
        FutureMod = 3,            // Impending Trans, Modif        Typ Br Red on Pink 
        Archived = 4,             // Reconciled/Cleared            Typ Red on Grey
        ArchivedCredit = 5,       // Reconciled/Cleared Deposit    Typ Green on Grey
        Credit = 6,               // Deposit                       Typ Green on White
        FutureCredit = 7,         // Impending Deposit             Typ Green on Pink
        Scheduled = 8,            // Scheduled Event               Typ Red on Maize
        ScheduledCredit = 9,      // Scheduled Event Credit        Typ Green on Maize
        JustUndoRedo = 10,        // Just Now Changed by Undo/Redo Typ Blue on Torq
        JustModified = 11,        // Just Now Changed by User      Typ Red on Cyan
        JustModCredit = 12,       // Just Now Changed Credit       Typ Green on Cyan
        NonDebitCredit = 13,      // Such as Balance Adjustment    Typ Blue on Chartr
        NewEntryRow = 14,         // Click to Insert New Entry     Typ Yellow on Green
        Reminder = 15,            // Reminder to do                Typ White on Red
        CheckedOff = 16,          // Check-Mark Clicked            Typ Slate on Grey
        CheckedAuto = 17,         // Checked Off Automatically     Typ Slate on White
        CheckedMaybe = 18,        // Awaiting Poss Merge Tran      Typ White on Black
        Invisible = 19,           // Blank                         Typ White on White
        Unknown = 20,             // Something Went Wrong          Typ Purp on Yellow
        Count = 21,               // Number of entries above
    }

    /// <summary>
    /// Encapsulates and decorates a CheckbookEntry
    /// </summary>
    public class RowOfCheckbook
    {

        /// <summary>
        /// This uniquely identifies a checkbook entry.
        /// </summary>
        private CheckbookEntry _entry = null;

        /// <summary>
        /// Here's where we keep track of the entry BEFORE it was edited.
        /// </summary>
        private CheckbookEntry _entryBeforeEdit = null;

        /// <summary>
        /// Textual name of category.
        /// </summary>
        private string _category = "?";

        /// <summary>
        /// Place holder for "Insert New Entry"
        /// </summary>
        private bool _newEntryRow = false;

        /// <summary>
        /// True to display splits instead of categorry name.
        /// </summary>
        private bool _showSplits = false;

        /// <summary>
        /// Cache for splits from checkbook.
        /// </summary>
        private string _splits = "";

        /// <summary>
        /// Access to DB, etc.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Access to DB, needed to look up splits</param>
        /// <param name="entry">The corresponding checkbook entry</param>
        /// <param name="category">Name of the category</param>
        /// <param name="newEntryRow">true if this is a "new entry" placeholder, not a real checkbook entry</param>
        public RowOfCheckbook(IDbAccess db, CheckbookEntry entry, string category, bool newEntryRow = false)
        {
            _db = db;
            _entryBeforeEdit = entry;
            _entry = entry.Clone(false);
            _category = category;
            _newEntryRow = newEntryRow;
        }

        /// <summary>
        /// Should we show splits instead of just category?
        /// </summary>
        public bool ShowSplits
        {
            get
            {
                return _showSplits;
            }
            set
            {
                _showSplits = value;
            }
        }

        public bool IsChecked
        {
            get
            {
                return _entry.IsChecked;
            }
            set
            {
                _entry.IsChecked = value;
            }
        }

        public string CheckNumber
        {
            get
            {
                return _entry.CheckNumber;
            }
            set
            {
                _entry.CheckNumber = value;
            }
        }

        public DateTime DateOfTransaction
        {
            get
            {
                return _entry.DateOfTransaction.Date <= new DateTime(0L) ? DateTime.Now : _entry.DateOfTransaction.Date;
            }
            set
            {
                _entry.DateOfTransaction = value.Date;
            }
        }

        public DateTime DateModified
        {
            get
            {
                return _entry.DateModified.Date <= new DateTime(0L) ? DateTime.Now : _entry.DateModified.Date;
            }
            set
            {
                // noop
            }
        }

        public string DateCleared
        {
            get
            {
                if (_entry.IsCleared)
                {
                    return (_entry.DateCleared.Date <= new DateTime(0L) ? DateTime.Now : _entry.DateCleared.Date)
                        .ToString();
                }
                return "";
            }
            set
            {
                // noop
            }
        }

        public string ModifiedBy
        {
            get
            {
                if (_newEntryRow)
                {
                    return "";
                }
                return _entry.ModifiedBy;
            }
            set
            {
                // noop
            }
        }

        public string IsCleared
        {
            get
            {
                return _entry.IsCleared ? "X" : "";
            }
            set
            {
                // noop
            }
        }

        public string Payee
        {
            get
            {
                return _entry.Payee;
            }
            set
            {
                _entry.Payee = value;
            }
        }

        public string Memo
        {
            get
            {
                if(_showSplits)
                {
                    return _entry.Memo;
                }
                return _entry.Memo.Replace("\x0d\x0a", "; ");
            }
            set
            {
                _entry.Memo = value;
            }
        }

        public string Balance
        {
            get
            {
                if(_newEntryRow)
                {
                    return "";
                }
                return UtilityMethods.FormatCurrency(_entry.Balance, 9);
            }
            set
            {
                throw new MissingMethodException("Balance.set");
            }
        }

        public string Amount
        {
            get
            {
                if (_newEntryRow)
                {
                    return "";
                }
                return UtilityMethods.FormatCurrency(_entry.Amount, 9);
            }
            set
            {
                throw new MissingMethodException("Amount.set");
            }
        }

        public string Debit
        {
            get
            {
                if (_newEntryRow)
                {
                    return "";
                }
                if (_entry.Amount < 0)
                {
                    return Amount;
                }
                return "";
            }
            set
            {
                throw new MissingMethodException("Debit.set");
            }
        }

        public string Credit
        {
            get
            {
                if (_newEntryRow)
                {
                    return "";
                }
                if (_entry.Amount >= 0)
                {
                    return Amount;
                }
                return "";
            }
            set
            {
                throw new MissingMethodException("Credit.set");
            }
        }

        /// <summary>
        /// Get the bank info.
        /// </summary>
        public string BankInfo
        {
            get
            {
                if(string.IsNullOrEmpty(_entry.BankPayee))
                {
                    return "";
                }
                return UtilityMethods.FormatCurrency(_entry.BankAmount) + " "
                    + (_entry.BankCheckNumber != 0 ? ("#" + _entry.BankCheckNumber + " ") : "")
                    + _entry.BankPayee + " "
                    + _entry.BankTranDate.ToShortDateString() + " "
                    + _entry.BankTransaction + " "
                    + (_entry.BankMergeAccepted ? " X" : "");
            }
        }

        /// <summary>
        /// Fetch the category name or, if _showSplits is true, a multi-line list of splits
        /// </summary>
        public string Category
        {
            get
            {
                if(_showSplits)
                {
                    if (_splits.Length < 1)
                    {
                        string delimiter = "";
                        _splits = "";
                        foreach (SplitEntry split in _entry.Splits)
                        {
                            FinancialCategory categ = _db.GetFinancialCategoryById(split.CategoryId);
                            string categName = "Unknown";
                            if (categ != null)
                            {
                                categName = categ.Name;
                            }
                            _splits += delimiter + categName + " " + UtilityMethods.FormatCurrency(split.Amount);
                            delimiter = "\x0d\x0a";
                        }
                    }
                    return _splits;
                }
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public bool NewEntryRow
        {
            get
            {
                return _newEntryRow;
            }
            set
            {
                throw new MissingMethodException("NewEntryRow.set");
            }
        }

        public Guid Id
        {
            get
            {
                return _entry.Id;
            }
        }

        public CheckbookEntry Entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
            }
        }

        public CheckbookEntry EntryBeforeEdit
        {
            get
            {
                return _entryBeforeEdit;
            }
            set
            {
                _entryBeforeEdit = value;
            }
        }

        /// <summary>
        /// Figure out the row foreground/background color.
        /// </summary>
        public EntryColor Color
        {
            get
            {
                TimeSlot timeSlot = _entry.TimeSlot;
                if (_newEntryRow)
                {
                    return EntryColor.NewEntryRow;
                }
                // TODO - CheckedAuto, CheckedMaybe
                if(IsChecked)
                {
                    return EntryColor.CheckedOff;
                }
                if (_entry.MadeBy == EntryMadeBy.Reminder)
                {
                    return EntryColor.Reminder;
                }
                if (_entry.MadeBy == EntryMadeBy.Scheduler)
                {
                    if (_entry.Amount > 0)
                    {
                        return EntryColor.ScheduledCredit;
                    }
                    else
                    {
                        return EntryColor.Scheduled;
                    }
                }
                if (DateTime.Now.Subtract(_entry.DateModified).TotalSeconds < 60.0) // 60 seconds
                {
                    if (_entry.Highlight == Highlight.UndoRedo)
                    {
                        return EntryColor.JustUndoRedo;
                    }
                    if (_entry.Amount > 0)
                    {
                        return EntryColor.JustModCredit;
                    }
                    else
                    {
                        return EntryColor.JustModified;
                    }
                }
                if(_entry.Splits.Length > 0 && _entry.Splits[0].Kind == TransactionKind.Adjustment)
                {
                    return EntryColor.NonDebitCredit;
                }
                if (_entry.Amount > 0)
                {
                    if (_entry.IsCleared)
                    {
                        return EntryColor.ArchivedCredit;
                    }
                    else
                    {
                        if (timeSlot == TimeSlot.Future || timeSlot == TimeSlot.FutureMod)
                        {
                            return EntryColor.FutureCredit;
                        }
                        else
                        {
                            return EntryColor.Credit;
                        }
                    }
                }
                if (_entry.IsCleared)
                {
                    return EntryColor.Archived;
                }
                switch (timeSlot)
                {
                    case TimeSlot.Past:
                        return EntryColor.Past;
                    case TimeSlot.Future:
                        return EntryColor.Future;
                    case TimeSlot.PastMod:
                        return EntryColor.PastMod;
                    case TimeSlot.FutureMod:
                        return EntryColor.FutureMod;
                    default:
                        return EntryColor.Unknown;
                }
            }
        }

        /// <summary>
        /// Currently hardcoded - needs to change.
        /// </summary>
        /// <param name="colorIndex">Row type per ordinal of EntryColor</param>
        /// <returns>Corresponding foreground color.</returns>
        public static System.Drawing.Color CellFgColor(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0:            // Typ Red on White
                    return System.Drawing.Color.FromArgb(160, 0, 0);
                case 1:            // Typ Red on Pink
                    return System.Drawing.Color.FromArgb(160, 0, 0);
                case 2:            // Typ Br Red on White
                    return System.Drawing.Color.FromArgb(220, 0, 0);
                case 3:            // Typ Br Red on Pink 
                    return System.Drawing.Color.FromArgb(220, 0, 0);
                case 4:            // Typ Red on Grey
                    return System.Drawing.Color.FromArgb(160, 0, 0);
                case 5:            // Typ Green on Grey
                    return System.Drawing.Color.FromArgb(0, 160, 0);
                case 6:            // Typ Green on White
                    return System.Drawing.Color.FromArgb(0, 160, 0);
                case 7:            // Typ Typ Green on Pink
                    return System.Drawing.Color.FromArgb(0, 160, 0);
                case 8:            // Typ Red on Maize
                    return System.Drawing.Color.FromArgb(160, 0, 0);
                case 9:            // Typ Green on Maize
                    return System.Drawing.Color.FromArgb(0, 160, 0);
                case 10:            // Typ Blue on Torq
                    return System.Drawing.Color.FromArgb(0, 0, 160);
                case 11:            // Typ Red on Cyan
                    return System.Drawing.Color.FromArgb(160, 0, 0);
                case 12:           // Typ Green on Cyan
                    return System.Drawing.Color.FromArgb(0, 160, 0);
                case 13:           // Typ Blue on Chartr
                    return System.Drawing.Color.FromArgb(80, 80, 100);
                case 14:           // Typ Yellow on Green 
                    return System.Drawing.Color.FromArgb(255, 255, 80);
                case 15:           //  Typ White on Red
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case 16:           // Typ Slate on Grey
                    return System.Drawing.Color.FromArgb(100, 100, 100);
                case 17:           // Typ Slate on White
                    return System.Drawing.Color.FromArgb(120, 120, 120);
                case 18:           // Typ White on Black
                    return System.Drawing.Color.FromArgb(220, 220, 220);
                case 19:           // Typ White on WHite
                    return System.Drawing.Color.FromArgb(250, 250, 250);
                case 20:           // Typ Purp on Yellow
                    return System.Drawing.Color.FromArgb(80, 0, 80);
                default:           // Unused
                    return System.Drawing.Color.FromArgb(255, 255, 255);
            }
        }

        /// <summary>
        /// Currently hardcoded - needs to change.
        /// </summary>
        /// <param name="colorIndex">Row type per ordinal of EntryColor</param>
        /// <returns>Corresponding background color.</returns>
        public static System.Drawing.Color CellBgColor(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0:            // Typ Red on White
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case 1:            // Typ Red on Pink
                    return System.Drawing.Color.FromArgb(255, 240, 240);
                case 2:            // Typ Br Red on White
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case 3:            // Typ Br Red on Pink 
                    return System.Drawing.Color.FromArgb(255, 240, 240);
                case 4:            // Typ Red on Grey
                    return System.Drawing.Color.FromArgb(230, 230, 230);
                case 5:            // Typ Green on Grey
                    return System.Drawing.Color.FromArgb(230, 230, 230);
                case 6:            // Typ Green on White
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case 7:            // Typ Typ Green on Pink
                    return System.Drawing.Color.FromArgb(255, 240, 240);
                case 8:            // Typ Red on Maize
                    return System.Drawing.Color.FromArgb(250, 240, 200);
                case 9:            // Typ Green on Maize
                    return System.Drawing.Color.FromArgb(250, 240, 200);
                case 10:            // Typ Blue on Torq
                    return System.Drawing.Color.FromArgb(210, 240, 240);
                case 11:            // Typ Red on Cyan
                    return System.Drawing.Color.FromArgb(210, 230, 250);
                case 12:           // Typ Green on Cyan
                    return System.Drawing.Color.FromArgb(210, 230, 250);
                case 13:           // Typ Blue on Chartr
                    return System.Drawing.Color.FromArgb(240, 250, 210);
                case 14:           // Typ Yellow on Green 
                    return System.Drawing.Color.FromArgb(80, 180, 120);
                case 15:           //  Typ White on Red
                    return System.Drawing.Color.FromArgb(120, 20, 20);
                case 16:           // Typ Slate on Grey
                    return System.Drawing.Color.FromArgb(175, 175, 175);
                case 17:           // Typ Slate on White
                    return System.Drawing.Color.FromArgb(200, 200, 200);
                case 18:           // Typ White on Black
                    return System.Drawing.Color.FromArgb(20, 20, 20);
                case 19:           // Typ White on White
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case 20:           // Typ Purp on Yellow
                    return System.Drawing.Color.FromArgb(255, 255, 220);
                default:           // Unused
                    return System.Drawing.Color.FromArgb(0, 0, 0);
            }
        }
    }
}
