using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Is this a credit or debit, and what kind?
    /// </summary>
    public enum TransactionKind
    {
        Payment = 0,        // Debit (-): Normal check or EFT 
        Deposit = 1,        // Credit (+): Normal paycheck/gain/etc.
        Refund = 2,         // Credit (+): Refund to Payment
        XferOut = 3,        // Debit (-): Went to a different acct
        XferIn = 4,         // Credit (+): From a different acct
        Adjustment = 5,     // Signed (+/-): Initial or adjustment to balance
    }

    /// <summary>
    /// Node for a list of split entries, each with a category and amount.
    /// </summary>
    public class SplitEntry
    {

        /// <summary>
        /// This uniquely identifies a category.
        /// </summary>
        private Guid _id = new Guid();

        /// <summary>
        /// FinancialCategory id.
        /// </summary>
        private Guid _categoryId = Guid.Empty;

        /// <summary>
        /// Amount, always positive.
        /// </summary>
        private long _amount = 0L;

        /// <summary>
        /// Is this a debit or credit and what kind?
        /// </summary>
        private TransactionKind _kind = TransactionKind.Payment;

        // Getters/Setters
        public Guid Id { get => _id; set => _id = value; }
        public Guid CategoryId { get => _categoryId; set => _categoryId = value; }
        public TransactionKind Kind { get => _kind; set => _kind = value; }
        public long Amount { get => _amount; set => _amount = value; }

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public SplitEntry()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="categoryId">The split category Guid</param>
        /// <param name="kind">Payment, deposit, etc.</param>
        /// <param name="amount">money</param>
        /// <returns>Guid of the new entry.</returns>
        public SplitEntry(Guid categoryId, TransactionKind kind, long amount)
        {
            _id = Guid.NewGuid();
            _categoryId = categoryId;
            _kind = kind;
            _amount = IsDebit((int)kind, amount) ? -Math.Abs(amount) : Math.Abs(amount);
        }

        /// <summary>
        /// Static method to determine if the attributes of an entry indicate a debit transaction.
        /// </summary>
        /// <param name="kind">The TransactionKind</param>
        /// <param name="amount">The transaciton amount, only used for Adjustments</param>
        /// <returns></returns>
        public static bool IsDebit(int kind, long amount)
        {
            if (kind == (int)TransactionKind.Deposit || kind == (int)TransactionKind.Refund || kind == (int)TransactionKind.XferIn)
            {
                return false;
            }
            if (kind == (int)TransactionKind.Adjustment && amount >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Assemble a man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SplitEntry" + Id.ToString() + " " + Kind + " " + Amount + " Cat:" + this.CategoryId;
        }

    }
}
