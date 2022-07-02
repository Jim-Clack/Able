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
        /// Here's where they're stored.
        /// </summary>
        private Dictionary<string, MemorizedPayee> _payees = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="checkbookDb">To fetch memorized payees from - already populated.</param>
        public AutofillPayee(IDbAccess checkbookDb)
        {
            _checkbookDb = checkbookDb;
            _payees = _checkbookDb.MemorizedPayees;
            if(_payees == null)
            {
                _payees = new Dictionary<string, MemorizedPayee>();
            }
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
                if(_payees.ContainsKey(entry.Payee))
                {
                    _payees.Remove(entry.Payee);
                }
                _payees.Add(entry.Payee, payee);
            }
        }

        public List<string> Payees()
        {
            SortedSet<string> matches = new SortedSet<string>();
            foreach(MemorizedPayee payee in _payees.Values)
            {
                matches.Add(payee.Payee);
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
            foreach (MemorizedPayee payee in _payees.Values)
            {
                if(payee.Payee.ToLower().StartsWith(pattern))
                {
                    matches.Add(payee);
                }
            }
            if(matches.Count > 1)
            {
                // 12/18/2021 - changed - was longest
                int shortestLgt = 999;
                int shortestIndex = 0;
                for(int index = 0; index < matches.Count; ++index)
                {
                    if(matches[index].Payee.Length < shortestLgt)
                    {
                        shortestLgt = matches[index].Payee.Length;
                        shortestIndex = index; 
                    }
                }
                // shorted one should be first
                if(shortestIndex > 0)
                {
                    MemorizedPayee shortest = matches[shortestIndex];
                    matches.RemoveAt(shortestIndex);
                    matches.Insert(0, shortest);
                }
            }
            return matches;
        }

    }

}
