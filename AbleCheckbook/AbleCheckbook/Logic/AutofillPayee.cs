using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// This guy maintains the checkbook entry autofill entries.
    /// </summary>
    public class AutofillPayee
    {

        /// <summary>
        /// Where the data is stored.
        /// </summary>
        private IDbAccess _checkbookDb = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="checkbookDb">To fetch memorized payees from - already populated.</param>
        public AutofillPayee(IDbAccess checkbookDb)
        {
            _checkbookDb = checkbookDb;
            IDbIterator<CheckbookEntry> ckbkIterator = _checkbookDb.CheckbookEntryIterator;
            try
            {
                while (ckbkIterator.HasNextEntry())
                {
                    CheckbookEntry entry = ckbkIterator.GetNextEntry();
                    UpdateFromCheckbookEntry(entry);
                }
            }
            catch(Exception ex)
            {
                Logger.Warn("Exception updating memorized payees", ex); // not critical, so ignore
            }
        }

        /// <summary>
        /// Update the autofill cache from a Memorized Payee
        /// </summary>
        /// <param name="oldPayee">Memorized Payee to be added to (or updated) the autofill cache.</param>
        public void UpdateFromMemorizedPayee(MemorizedPayee oldPayee)
        {
            MemorizedPayee newPayee = _checkbookDb.GetMemorizedPayeeByName(oldPayee.Payee);    
            if(newPayee == null)
            {
                newPayee = new MemorizedPayee(oldPayee.Payee, oldPayee.CategoryId, oldPayee.Kind, oldPayee.Amount);
            }
            else
            {
                newPayee.Payee = oldPayee.Payee;
                newPayee.CategoryId = oldPayee.CategoryId;
                newPayee.Kind = oldPayee.Kind;
                newPayee.Amount = oldPayee.Amount;
            }
            _checkbookDb.UpdateEntry(newPayee, oldPayee);
        }

        /// <summary>
        /// Update the autofill cache from a checkbook entry.
        /// </summary>
        /// <param name="entry">Checkbook entry to be added to (or updated) the autofill cache.</param>
        public void UpdateFromCheckbookEntry(CheckbookEntry entry)
        {
            if (entry.Splits.Length < 1)
            {
                return;
            }
            if (entry.Splits[0].Kind == TransactionKind.Payment || entry.Splits[0].Kind == TransactionKind.Deposit)
            {   // only do the first split, in order to avoid incidentals such as cash-back
                MemorizedPayee payee = new MemorizedPayee(entry.Payee, entry.Splits[0].CategoryId, entry.Splits[0].Kind, entry.Splits[0].Amount);
                UpdateFromMemorizedPayee(payee);
            }
        }

        public List<string> Payees()
        {
            SortedSet<string> matches = new SortedSet<string>();
            MemorizedPayeeIterator iterator = _checkbookDb.MemorizedPayeeIterator;
            while (iterator.HasNextEntry())
            {
                MemorizedPayee current = iterator.GetNextEntry();
                matches.Add(current.Payee);
            }
            return matches.ToList();
        }

        /// <summary>
        /// Search for an autofill entry.
        /// </summary>
        /// <param name="payeeSubstring">Beginning of payee name, typically a partial match.</param>
        /// <returns>List of matches, possiobly empty.</returns>
        public List<MemorizedPayee> LookUp(string payeeSubstring)
        {
            List<MemorizedPayee> matches = new List<MemorizedPayee>();
            if (payeeSubstring.Length < 1)
            {
                return matches;
            }
            string pattern = payeeSubstring.ToLower();
            MemorizedPayeeIterator iterator = _checkbookDb.MemorizedPayeeIterator;
            while(iterator.HasNextEntry())
            {
                MemorizedPayee current = iterator.GetNextEntry();
                if(current.Payee.ToLower().StartsWith(pattern))
                {
                    matches.Add(current);
                }
            }
            return matches;
        }

        /// <summary>
        /// Search for an autofill entry.
        /// </summary>
        /// <param name="payeeSubstring">Payee name.</param>
        /// <param name="categoryId">ID of category</param>
        /// <returns>List of matches, possiobly empty.</returns>
        public List<MemorizedPayee> LookUp(string payee, Guid categoryId)
        {
            List<MemorizedPayee> matches = new List<MemorizedPayee>();
            if (payee.Length < 1)
            {
                return matches;
            }
            string pattern = payee.ToLower();
            MemorizedPayeeIterator iterator = _checkbookDb.MemorizedPayeeIterator;
            while (iterator.HasNextEntry())
            {
                MemorizedPayee current = iterator.GetNextEntry();
                if (current.Payee.ToUpper().Equals(pattern) && current.CategoryId == categoryId)
                {
                    matches.Add(current);
                }
            }
            return matches;
        }

    }
}
