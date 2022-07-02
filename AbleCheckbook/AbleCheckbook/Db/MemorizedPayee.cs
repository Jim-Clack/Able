using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{
    public class MemorizedPayee
    {
        /// <summary>
        /// Keep track of payees and their last transaction per category
        /// </summary>
        private string _payee = "";

        /// <summary>
        /// Category of purchase.
        /// </summary>
        private Guid _categoryId = default(Guid);

        /// <summary>
        /// Payment or deposit.
        /// </summary>
        private TransactionKind _kind = TransactionKind.Payment; 

        /// <summary>
        /// Monetary amount.
        /// </summary>
        private long _amount = 0L;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="payee">2nd/3rd party entity.</param>
        /// <param name="categoryId">Category Guid.</param>
        /// <param name="amount">Monetary amt.</param>
        public MemorizedPayee(string payee, Guid categoryId, TransactionKind kind, long amount)
        {
            _payee = payee;
            _categoryId = categoryId;
            _kind = kind;
            _amount = amount;
        }

        // Getters/Setters
        public string Payee { get => _payee; set => _payee = value; }
        public Guid CategoryId { get => _categoryId; set => _categoryId = value; }
        public long Amount { get => _amount; set => _amount = value; }
        public TransactionKind Kind { get => _kind; set => _kind = value; }

        /// <summary>
        /// Create a deep duplicate with its own Id.
        /// </summary>
        /// <returns>The duplicate</returns>
        public MemorizedPayee Clone()
        {
            MemorizedPayee payee = new MemorizedPayee(this.Payee, this.CategoryId, this.Kind, this.Amount);
            return payee;
        }

        /// <summary>
        /// Assemble a man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "MemorizedPayee" + " - " + Payee;
        }

        /// <summary>
        /// Return a key that's a string but collates by ascending due date.
        /// </summary>
        /// <returns>Collatable key.</returns>
        public string UniqueKey()
        {
            return _categoryId.ToString();
        }

    }


}
