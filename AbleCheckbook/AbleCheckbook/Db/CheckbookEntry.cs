
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// How as this particular entry originally created?
    /// </summary>
    public enum EntryMadeBy
    {
        User =         0,    // user entry
        Reminder =     1,    // entered by scheduler, awaiting user interaction
        Scheduler =    2,    // auto-entered by scheduler
        IndirectXfer = 3,    // transfer via a different acct 
        Importer =     4,    // imported from spreadsheet
        Reconciler =   5,    // forced adjustment at reconciliation
        Mobile =       6,    // Pushed from mobile app
        YearEnd =      7,    // Balance forward
        Process =      8,    // Rename Payee, etc.
    }

    /// <summary>
    /// Time slot, expected to be used for setting the background color in the GUI.
    /// </summary>
    public enum TimeSlot
    {
        Past =         0,    // Made (date tran) in the past or today
        PastMod =      1,    // Made (date tran) in the past/today and changed (date mod) today
        Future =       2,    // Made (date tran) in the future
        FutureMod =    3,    // Made (date tran) in the future and changed (date mod) today
    }

    /// <summary>
    /// Each entry can be highlighted (until the next repaint) in a color based on...
    /// </summary>
    public enum Highlight
    {
        None =         0,    // No highlighting  
        Modified =     1,    // Mark as just modified by user
        UndoRedo =     2,    // Mark as just now undone or redone
        Processed =    3,    // Mark as just now imported or mass-edited
    }

    /// <summary>
    /// An entry in the checkbook register.
    /// </summary>
    public class CheckbookEntry
    {

        /// <summary>
        /// This uniquely identifies a checkbook entry.
        /// </summary>
        private Guid _id = new Guid();

        /// <summary>
        /// Check number, or "" if unused.
        /// </summary>
        private string _checkNumber = "";

        /// <summary>
        /// When was this entered?
        /// </summary>
        private DateTime _dateOfTransaction = DateTime.Now;

        /// <summary>
        /// When was this last modified?
        /// </summary>
        private DateTime _dateModified = DateTime.Now;

        /// <summary>
        /// When was this cleared/reconciled?
        /// </summary>
        private DateTime _dateCleared = DateTime.Now;

        /// <summary>
        /// Who modified this last?
        /// </summary>
        private string _modifiedBy = System.Environment.UserName;

        /// <summary>
        /// Has this entry been cleared/reconciled/closed?
        /// </summary>
        private bool _isCleared = false;

        /// <summary>
        /// Has this entry been checked for further processing?
        /// </summary>
        private bool _isChecked = false;

        /// <summary>
        /// How was this entered. i.e. by user, imported, xfered, automatic
        /// </summary>
        private EntryMadeBy _madeBy = EntryMadeBy.User;

        /// <summary>
        /// Name of party.
        /// </summary>
        private string _payee = "";

        /// <summary>
        /// Cumulative memo.
        /// </summary>
        private string _memo = "";

        /// <summary>
        /// Balance gets persisted but not trusted, instead recalculated.
        /// </summary>
        private long _balance = 0;

        /// <summary>
        /// For GUI usage. NOT PERSISTED.
        /// </summary>
        private Highlight _highlight = Highlight.None;

        /// <summary>
        /// Amounts and categories. 
        /// </summary>
        /// <remarks>Array instead of list because 99% of access is read only</remarks>
        private SplitEntry[] _splits = new SplitEntry[0];

        // Getters/Setters
        public Guid Id { get => _id; set => _id = value; }
        public string CheckNumber { get => _checkNumber; set => _checkNumber = value; }
        public DateTime DateOfTransaction { get => _dateOfTransaction; set => _dateOfTransaction = value; }
        public DateTime DateModified { get => _dateModified; set => _dateModified = value; }
        public DateTime DateCleared { get => _dateCleared; set => _dateCleared = value; }
        public string ModifiedBy { get => _modifiedBy; set => _modifiedBy = value; }
        public bool IsChecked { get => _isChecked; set => _isChecked = value; }
        public bool IsCleared { get => _isCleared; set => _isCleared = value; }
        public EntryMadeBy MadeBy { get => _madeBy; set => _madeBy = value; }
        public string Payee { get => _payee; set => _payee = value; }
        public long Balance { get => _balance; set => _balance = value; }
        public SplitEntry[] Splits { get => _splits; set => _splits = value; }
        public Highlight Highlight { get => _highlight; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public CheckbookEntry()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Create a deep duplicate with its own id. (may update destinDb if a category is missing)
        /// </summary>
        /// <param name="withDifferentId">True to assign the clone a different Guid</param>
        /// <returns>The clone.</returns>
        public CheckbookEntry Clone(bool withDifferentId = true)
        {
            CheckbookEntry clonedEntry = new CheckbookEntry();
            if (!withDifferentId)
            {
                clonedEntry.Id = this.Id;
            }
            clonedEntry.ResetMemo();
            clonedEntry._memo = this._memo;
            clonedEntry.CheckNumber = this.CheckNumber;
            clonedEntry.DateCleared = this.DateCleared;
            clonedEntry.DateModified = this.DateModified;
            clonedEntry.DateOfTransaction = this.DateOfTransaction;
            clonedEntry.MadeBy = this.MadeBy;
            clonedEntry.ModifiedBy = this.ModifiedBy;
            clonedEntry.Payee = this.Payee;
            clonedEntry.IsChecked = this.IsChecked;
            clonedEntry.IsCleared = this.IsCleared;
            clonedEntry.Balance = this.Balance;
            clonedEntry._highlight = this._highlight;
            foreach (SplitEntry split in this.Splits)
            {
                clonedEntry.AddSplit(split.CategoryId, split.Kind, split.Amount);
            }
            return clonedEntry;
        }

        /// <summary>
        /// Signed amount where payments are negative and deposits are positive.
        /// </summary>
        public long Amount
        {
            get
            {
                long sum = 0L;
                foreach(SplitEntry split in _splits)
                {
                    sum += split.Amount;
                }
                return sum;
            }
        }

        /// <summary>
        /// Is this a credit? (i.e. deposit, xfer in, refund, or pos adjustment)
        /// </summary>
        /// <returns>true if it is a credit, false if it is a debit/payment</returns>
        public bool IsCredit
        {
            get
            {
                return Amount > 0;
            }
        }

        /// <summary>
        /// Append to the memo field or fetch a (possibly multi-line) memo.
        /// </summary>
        public string Memo
        {
            get
            {
                return _memo.Replace("|", "\x0d\x0a");
            }
            set
            {
                AppendMemo(value);
            }
        }

        /// <summary>
        /// Append to the memo field.
        /// </summary>
        /// <param name="memo">To be appended.</param>
        public void AppendMemo(string memo)
        {
            if(_memo.Length > 1)
            {
                _memo = _memo + "|"; // entries are separated by "|"
            }
            _memo += memo.Replace("\x0d", "|").Replace("\x0a", "");
        }

        /// <summary>
        /// Clear out the Memo field.
        /// </summary>
        public void ResetMemo()
        {
            _memo = "";
        }

        /// <summary>
        /// Add a new split entry.
        /// </summary>
        /// <param name="categoryId">The split category Guid</param>
        /// <param name="kind">payment, deposit, etc.</param>
        /// <param name="amount">money</param>
        /// <returns>Guid of the new entry.</returns>
        public Guid AddSplit(Guid categoryId, TransactionKind kind, long amount)
        {
            SplitEntry split = new SplitEntry(categoryId, kind, amount);
            SplitEntry[] oldSplits = _splits;
            _splits = new SplitEntry[oldSplits.Length + 1];
            Array.Copy(oldSplits, _splits, oldSplits.Length);
            _splits[oldSplits.Length] = split;
            return split.Id;
        }

        /// <summary>
        /// Delete all splits. (dangerous state - add splits immediately)
        /// </summary>
        public void DeleteSplits()
        {
            _splits = new SplitEntry[0];
        }

        /// <summary>
        /// Call this to delete a split entry.
        /// </summary>
        /// <param name="split">TO be deleted.</param>
        /// <returns>success - entry was found and deleted</returns>
        public bool DeleteSplit(SplitEntry split)
        {
            for(int index = 0; index < _splits.Length; ++index)
            {
                SplitEntry match = _splits[index];
                if (split.Id == match.Id)
                {
                    SplitEntry[] oldSplits = _splits;
                    _splits = new SplitEntry[oldSplits.Length - 1];
                    Array.Copy(oldSplits, _splits, index);
                    Array.Copy(oldSplits, index + 1, _splits, index, oldSplits.Length - (index + 1));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Time slot: whether this entry occurred in the past or future and whether is was modified today.
        /// </summary>
        public TimeSlot TimeSlot
        {
            get
            {
                DateTime midnight = DateTime.Now; // actually 11:59:59 tonight
                midnight = new DateTime(midnight.Year, midnight.Month, midnight.Day, 23, 59, 59);
                bool modifiedToday = Math.Abs(DateTime.Now.Subtract(_dateModified).TotalMinutes) < 24 * 60; 
                if (_dateOfTransaction.CompareTo(midnight) < 0)
                {
                    if(modifiedToday)
                    {
                        return TimeSlot.PastMod;
                    }
                    else
                    {
                        return TimeSlot.Past;
                    }
                }
                if (modifiedToday)
                {
                    return TimeSlot.FutureMod;
                }
                else
                {
                    return TimeSlot.Future;
                }
            }
        }

        /// <summary>
        /// This is not a setter in order to avoid having .NET persist it.
        /// </summary>
        /// <param name="highlight"></param>
        public void SetHighlight(Highlight highlight)
        {
            _highlight = highlight;
        }

        /// <summary>
        /// Assemble a diagnostic man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "CheckbookEntry " + Id.ToString() + " - " + DateOfTransaction.ToShortDateString() + " " +
                Payee + (IsCleared ? " X " : " ") +
                ((Splits.Length > 0) ? ("- " + Splits[0].CategoryId + ": " + Splits[0].Amount) : "") +
                ((Splits.Length > 1) ? ("- " + Splits[1].CategoryId + ": " + Splits[1].Amount) : "");
        }

        /// <summary>
        /// Assemble a brief man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            return DateOfTransaction.ToShortDateString() + " " + Payee + " " + UtilityMethods.FormatCurrency(Amount);
        }

        /// <summary>
        /// Return a key that's a string but collates by ascending treansaction date.
        /// </summary>
        /// <returns>Collatable key.</returns>
        public string UniqueKey()
        {
            return ((ulong)_dateOfTransaction.ToBinary()).ToString("D22") + "-" + _id;
        }

    }

}
