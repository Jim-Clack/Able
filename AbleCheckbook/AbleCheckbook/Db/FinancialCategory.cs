
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Income/Expense category.
    /// </summary>
    public class FinancialCategory
    {

        /// <summary>
        /// This uniquely identifies a category.
        /// </summary>
        Guid _id = new Guid();

        /// <summary>
        /// If a category isn't used for, say 15 months, get rid of it.
        /// </summary>
        DateTime _dateLastUsed = new DateTime();

        /// <summary>
        /// Name of category, like Groceries or AutoFuel.
        /// </summary>
        string _name = "???";

        /// <summary>
        /// Debit paiod or credit deposited?
        /// </summary>
        bool _isCredit = false;

        // Getters/Setters
        public Guid Id { get => _id; set => _id = value; }
        public DateTime DateLastUsed { get => _dateLastUsed; set => _dateLastUsed = value; }
        public string Name { get => _name; set => _name = value; }
        public bool IsCredit { get => _isCredit; set => _isCredit = value; }

        public FinancialCategory()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Create a deep duplicate with its own id.
        /// </summary>
        /// <returns>The clone.</returns>
        public FinancialCategory Clone()
        {
            FinancialCategory clonedEntry = new FinancialCategory();
            clonedEntry._dateLastUsed = _dateLastUsed;
            clonedEntry._name = _name;
            clonedEntry._isCredit = _isCredit;
            return clonedEntry;
        }
        
        /// <summary>
        /// Assemble a man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "FinancialCategory" + Id.ToString() + " - " + Name;
        }

        /// <summary>
        /// Return a key that's a string but collates by ascending category name.
        /// </summary>
        /// <returns>Collatable key.</returns>
        public string UniqueKey()
        {
            return _name.ToLower() + "-" + _id;
        }

    }

}
