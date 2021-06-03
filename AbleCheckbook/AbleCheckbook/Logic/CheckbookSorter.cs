using AbleCheckbook.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    /// <summary>
    /// How to sort the data grid view
    /// </summary>
    public enum SortEntriesBy
    {
        NoChange =      0,   // Same as before
        TranDate =      1,   // Ascending transaction date
        Payee =         2,   // Case-insensitive payee
        Category =      3,   // Case-insensitive category
        CheckNumber =   4,   // Check numbers numerically
        SearchResults = 5,   // List search results first
        CheckBox =      6,   // Similar to SearchResults
    }

    /// <summary>
    /// For sorting checkbook entries.
    /// </summary>
    public class CheckbookSorter
    {

        /// <summary>
        /// Matches, used only for CompareEntriesByMatch
        /// </summary>
        private List<Guid> _matches = null;

        /// <summary>
        /// Get a list of sorted checkbook entries.
        /// </summary>
        /// <param name="db">Data to be sorted</param>
        /// <param name="sortedBy">How to sort it.</param>
        /// <param name="matches">List of GUids to match, used only for CompareEntriesByMatch</param>
        /// <returns>The sorted list</returns>
        public List<CheckbookEntry> GetSortedEntries(IDbAccess db, SortEntriesBy sortedBy, List<Guid> matches = null)
        {
            _matches = matches;
            List<CheckbookEntry> entries = new List<CheckbookEntry>();
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            while (iterator.HasNextEntry())
            {
                CheckbookEntry entry = iterator.GetNextEntry();
                entries.Add(entry);
            }
            switch (sortedBy)
            {
                case SortEntriesBy.TranDate:
                    entries.Sort(CompareEntriesByTranDate);
                    break;
                case SortEntriesBy.CheckNumber:
                    entries.Sort(CompareEntriesByCheckNumber);
                    break;
                case SortEntriesBy.Category:
                    entries.Sort(CompareEntriesByCategory);
                    break;
                case SortEntriesBy.Payee:
                    entries.Sort(CompareEntriesByPayee);
                    break;
                case SortEntriesBy.SearchResults:
                    entries.Sort(CompareEntriesByMatch);
                    break;
                case SortEntriesBy.CheckBox:
                    entries.Sort(CompareEntriesByMatch);
                    break;
            }
            return entries;
        }

        /// <summary>
        /// Comparison method for sorting checkbook entries by transaction date, then Credit/Debit, then Id.
        /// </summary>
        /// <param name="leftArg">First entry top compare</param>
        /// <param name="rightArg">Second entry</param>
        /// <returns>Sign() of value after subtracting leftArg-rightArg</returns>
        /// <note>In order to align balances, this sorts exactly the same way as db AdjustBalances()</note>
        private int CompareEntriesByTranDate(CheckbookEntry leftArg, CheckbookEntry rightArg)
        {
            if (leftArg.DateOfTransaction == null)
            {
                leftArg.DateOfTransaction = DateTime.Now;
            }
            if (rightArg.DateOfTransaction == null)
            {
                rightArg.DateOfTransaction = DateTime.Now;
            }
            int result = leftArg.DateOfTransaction.Date.CompareTo(rightArg.DateOfTransaction.Date);
            if (result == 0)
            {
                result = -leftArg.IsCredit.CompareTo(rightArg.IsCredit); // list credits before debits
            }
            if (result == 0)
            {
                result = leftArg.Id.CompareTo(rightArg.Id); // consistent discriminator
            }
            return result;
        }

        /// <summary>
        /// Comparison method for sorting checkbook entries by payee.
        /// </summary>
        /// <param name="leftArg">First entry top compare</param>
        /// <param name="rightArg">Second entry</param>
        /// <returns>Sign() of value after subtracting leftArg-rightArg</returns>
        private int CompareEntriesByPayee(CheckbookEntry leftArg, CheckbookEntry rightArg)
        {
            if (leftArg.Payee == null)
            {
                leftArg.Payee = "";
            }
            if (rightArg.Payee == null)
            {
                rightArg.Payee = "";
            }
            int result = leftArg.Payee.Trim().ToUpper().CompareTo(rightArg.Payee.Trim().ToUpper());
            if (result == 0)
            {
                result = CompareEntriesByTranDate(leftArg, rightArg);
            }
            return result;
        }

        /// <summary>
        /// Comparison method for sorting checkbook entries by category.
        /// </summary>
        /// <param name="leftArg">First entry top compare</param>
        /// <param name="rightArg">Second entry</param>
        /// <returns>Sign() of value after subtracting leftArg-rightArg</returns>
        private int CompareEntriesByCategory(CheckbookEntry leftArg, CheckbookEntry rightArg)
        {
            if (leftArg.Splits == null || leftArg.Splits.Length < 1)
            {
                return -1;
            }
            if (rightArg.Splits == null || rightArg.Splits.Length < 1)
            {
                return 1;
            }
            if (leftArg.Splits.Length > 1 && rightArg.Splits.Length > 1)
            {
                return 0;
            }
            if (leftArg.Splits.Length > 1)
            {
                return -1;
            }
            if (rightArg.Splits.Length > 1)
            {
                return 1;
            }
            int result = leftArg.Splits[0].CategoryId.CompareTo(rightArg.Splits[0].CategoryId);
            if (result == 0)
            {
                result = CompareEntriesByTranDate(leftArg, rightArg);
            }
            return result;
        }

        /// <summary>
        /// Comparison method for sorting checkbook entries by check number.
        /// </summary>
        /// <param name="leftArg">First entry top compare</param>
        /// <param name="rightArg">Second entry</param>
        /// <returns>Sign() of value after subtracting leftArg-rightArg</returns>
        private int CompareEntriesByCheckNumber(CheckbookEntry leftArg, CheckbookEntry rightArg)
        {
            if (leftArg.CheckNumber == null)
            {
                leftArg.CheckNumber = "";
            }
            if (rightArg.CheckNumber == null)
            {
                rightArg.CheckNumber = "";
            }
            int result = leftArg.CheckNumber.Trim().ToUpper().CompareTo(rightArg.CheckNumber.Trim().ToUpper());
            if (result == 0)
            {
                result = CompareEntriesByTranDate(leftArg, rightArg);
            }
            else
            {
                if (leftArg.CheckNumber.Length < 1)
                {
                    return 1;
                }
                if (rightArg.CheckNumber.Length < 1)
                {
                    return -1;
                }
            }
            return result;
        }

        /// <summary>
        /// Comparison method for sorting checkbook entries by search match results.
        /// </summary>
        /// <param name="leftArg">First entry top compare</param>
        /// <param name="rightArg">Second entry</param>
        /// <returns>Sign() of value after subtracting leftArg-rightArg</returns>
        private int CompareEntriesByMatch(CheckbookEntry leftArg, CheckbookEntry rightArg)
        {
            bool leftMatch = _matches.Contains(leftArg.Id);
            bool rightMatch = _matches.Contains(rightArg.Id);
            if (leftMatch == rightMatch)
            {
                return CompareEntriesByTranDate(leftArg, rightArg);
            }
            if (leftMatch)
            {
                return -1;
            }
            return 1;
        }

    }
}
