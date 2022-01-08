using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
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
        JustModified = 11,        // Just Now Changed by User      Typ Blue on Cyan
        JustModCredit = 12,       // Just Now Changed Credit       Typ Red on Aqua
        NonDebitCredit = 13,      // Such as Balance Adjustment    Typ Blue on Chartr
        NewEntryRow = 14,         // Click to Insert New Entry     Typ Yellow on Green
        Reminder = 15,            // Reminder to do                Typ White on Red
        CheckedOff = 16,          // Check-Mark Clicked            Typ Slate on Grey
        CheckedAuto = 17,         // Checked Off Automatically     Typ Slate on White
        CheckedMaybe = 18,        // Awaiting Poss Merge Tran      Typ White on Black
        Unknown = 19,             // Something Went Wrong          Typ Purp on Yellow
        Count = 20,               // Number of entries above
    }

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
                return _entry.DateOfTransaction.Date;
            }
            set
            {
                _entry.DateOfTransaction = value;
            }
        }

        public DateTime DateModified
        {
            get
            {
                return _entry.DateModified.Date;
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
                    return _entry.DateCleared.Date.ToShortDateString();
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
                if(_entry.BankPayee.Trim().Length < 1)
                {
                    return "";
                }
                return _entry.BankPayee + " " + _entry.BankTransaction + " " + 
                    _entry.BankTranDate.ToShortDateString() + " " + UtilityMethods.FormatCurrency(_entry.BankAmount) +
                    (_entry.BankMergeAccepted ? " X" : "");
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
    }
}
