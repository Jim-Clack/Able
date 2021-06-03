using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;

namespace AbleCheckbook.Logic
{
    /// <summary>
    /// A collection of these is used for tracking unreconciled transactions.
    /// </summary>
    public class OpenEntry
    {

        /// <summary>
        /// The checkbook entry that is affected.
        /// </summary>
        private CheckbookEntry _checkbookEntry = null;

        /// <summary>
        /// Has it beem tentatively cleared during the current reconciliation?
        /// </summary>
        private bool _isChecked = false;

        // Getters/Setter
        public CheckbookEntry CheckbookEntry { get => _checkbookEntry; set => _checkbookEntry = value; }
        public bool IsChecked { get => _isChecked; set => _isChecked = value; }

    }

    /// <summary>
    /// What issue might lead one to think that these checkbook entries might be improperly cleared?
    /// </summary>
    public enum CandidateIssue
    {
        NoIssue =           0,
        SumsToDifference =  1,
        TransposedCents =   2,
        TransposedDollars = 3,
    }

    /// <summary>
    /// Caondidate for a reconciliation issue.
    /// </summary>
    public class CandidateEntry
    {
        CandidateIssue _issue = CandidateIssue.NoIssue;
        List<OpenEntry> _openEntries = new List<OpenEntry>();
        public CandidateEntry(CandidateIssue issue)
        {
            _issue = issue;
        }
        public void Add(OpenEntry openEntry)
        {
            _openEntries.Add(openEntry);
        }
        public CandidateIssue Issue
        {
            get
            {
                return _issue;
            }
        }
        public List<OpenEntry> OpenEntries
        {
            get
            {
                return _openEntries;
            }
        }
    }

    /// <summary>
    /// Support for bank account reconciliation.
    /// </summary>
    public class ReconciliationHelper
    {

        /// <summary>
        /// Access to the data.
        /// </summary>
        private IDbAccess _db = null;

        /// <summary>
        /// Here's where we track the uncleared/unreconciled checkbook entries.
        /// </summary>
        private Dictionary<Guid, OpenEntry> _openEntries = new Dictionary<Guid, OpenEntry>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Checkbook database.</param>
        public ReconciliationHelper(IDbAccess db)
        {
            _db = db;
            UpdateOpenEntries();
        }

        /// <summary>
        /// Reload open entries.
        /// </summary>
        public void UpdateOpenEntries()
        {
            _openEntries = new Dictionary<Guid, OpenEntry>();
            List<CheckbookEntry> entries = new CheckbookSorter().GetSortedEntries(_db, SortEntriesBy.TranDate);
            foreach (CheckbookEntry entry in entries)
            {
                if (!entry.IsCleared)
                {
                    OpenEntry openEntry = new OpenEntry();
                    openEntry.CheckbookEntry = entry;
                    openEntry.IsChecked = false;
                    _openEntries.Add(entry.Id, openEntry);
                }
            }
        }

        /// <summary>
        /// Is reconciliation due? (More than 8 days into the month after most of the uncleared entries.)
        /// </summary>
        public bool IsTimeToReconcile
        {
            get
            {
                int numUncleared = 0;
                int numOldEnough = 0;
                foreach(KeyValuePair<Guid, OpenEntry> pair in _openEntries)
                {
                    ++numUncleared;
                    CheckbookEntry entry = pair.Value.CheckbookEntry;
                    DateTime transDate = entry.DateOfTransaction;
                    DateTime eighthOfNextMonth = 
                        (new DateTime(transDate.Year, transDate.Month, 1)).AddMonths(1).AddDays(8);
                    if(eighthOfNextMonth.CompareTo(DateTime.Now) < 0)
                    {
                        ++numOldEnough;
                    }
                }
                return (numOldEnough * 3 > numUncleared);
            }
        }

        /// <summary>
        /// Get the open entries, including those that are tentatively closed.
        /// </summary>
        public Dictionary<Guid, OpenEntry> OpenEntries
        {
            get
            {
                return _openEntries;
            }
        }

        /// <summary>
        /// Get an entry by its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The open entry, null if not found</returns>
        public OpenEntry CheckbookEntry(Guid id)
        {
            OpenEntry openEntry = null;
            if (_openEntries.TryGetValue(id, out openEntry))
            {
                return openEntry;
            }
            return null;
        }

        /// <summary>
        /// Flag a checkbook entry as cleared (in this session, but not yet in the DB)
        /// </summary>
        /// <param name="id">checkbook entry ID</param>
        public void CheckIt(Guid id)
        {
            OpenEntry openEntry = null;
            if(_openEntries.TryGetValue(id, out openEntry))
            {
                openEntry.IsChecked = true;
            }
        }

        /// <summary>
        /// Flag a checkbook entry as NOT cleared (in this session, not in the DB)
        /// </summary>
        /// <param name="id">checkbook entry ID</param>
        public void UnCheckIt(Guid id)
        {
            OpenEntry openEntry = null;
            if (_openEntries.TryGetValue(id, out openEntry))
            {
                openEntry.IsChecked = false;
            }
        }

        /// <summary>
        /// Return the reconciliation disparity.
        /// </summary>
        /// <param name="closingBalance">Closing balance, 0 to return the sum of checked entries</param>
        /// <returns></returns>
        public long GetDisparity(long closingBalance = 0L)
        {
            long monthlyStatementCredit = closingBalance - _db.GetReconciliationValues().Balance;
            long checkedEntriesCredit = 0L;
            foreach (KeyValuePair<Guid, OpenEntry> pair in _openEntries)
            {
                OpenEntry openEntry = pair.Value;
                if (openEntry.IsChecked)
                {
                    checkedEntriesCredit += openEntry.CheckbookEntry.Amount;
                }
            }
            return checkedEntriesCredit - monthlyStatementCredit;
        }

        /// <summary>
        /// Find potential remaining reconcile candidates that add up to a given amount (disparity).
        /// </summary>
        /// <param name="disparity">The expected sum of the entries yet to be cleared.</param>
        /// <param name="closingDate">Closing date of statement to be reconciled</param>
        /// <param name="maxMatches">The maximum number of entries to be paired-up: 3, 2, or 1</param>
        /// <returns>List of "List of candidates that may explain the disparity."</returns>
        public List<CandidateEntry> FindTipCandidates(long disparity, DateTime closingDate, int maxMatches = 3)
        {
            List<CandidateEntry> candidates = new List<CandidateEntry>();
            if(disparity == 0)
            {
                return candidates;
            }
            int nbrChecked = 0;
            int nbrUnchecked = 0;
            foreach (KeyValuePair<Guid, OpenEntry> pair in _openEntries)
            {
                CheckbookEntry entry = pair.Value.CheckbookEntry;
                if(entry.DateOfTransaction.Date <= closingDate.Date)
                {
                    if(pair.Value.IsChecked)
                    {
                        ++nbrChecked;
                    }
                    else
                    {
                        ++nbrUnchecked;
                    }
                }
            }
            // don't bother looking for tips if there are still too many unchecked entries
            if (nbrUnchecked > nbrChecked && nbrUnchecked > 6)
            {
                return candidates;
            }
            // Find candidates
            candidates.AddRange(FindSumCandidates(disparity, maxMatches));
            candidates.AddRange(FindTransposeCandidates(disparity));
            return candidates;
        }

        /// <summary>
        /// Find potential entries that add up to a given amount (disparity).
        /// </summary>
        /// <param name="disparity">The expected sum of the entries yet to be cleared.</param>
        /// <param name="maxMatches">The maximum number of entries to be paired-up: 3, 2, or 1</param>
        /// <returns>List of "List of candidates that sum up to the desire disparity."</returns>
        public List<CandidateEntry> FindSumCandidates(long disparity, int maxMatches)
        {
            List<CandidateEntry> candidates = new List<CandidateEntry>();
            FindSumCandidatesRecursively(disparity, candidates, null, null, null, maxMatches);
            return candidates;
        }

        /// <summary>
        /// Search for creconciliation candidates recursively.
        /// </summary>
        /// <param name="disparity">The expected sum of the entries yet to be cleared.</param>
        /// <param name="candidates">List of candidate matches to be populated</param>
        /// <param name="entry1">entry to consider, passed from the outermost level of recursion</param>
        /// <param name="entry2">entry to consider, passed from the second level of recursion</param>
        /// <param name="entryPrior">entry to consider, passed from the just prior level of recursion</param>
        /// <param name="maxMatches">The maximum number of entries in each sum: 3, 2, or 1</param>
        private void FindSumCandidatesRecursively(long disparity, List<CandidateEntry> candidates,
            OpenEntry entry1, OpenEntry entry2, OpenEntry entryPrior, int maxMatches)
        {
            long sum = Amount(entry1) + Amount(entry2);
            bool skipPastPrior = entryPrior != null;
            foreach (KeyValuePair<Guid, OpenEntry> pair in _openEntries)
            {
                OpenEntry openEntry = pair.Value;
                if(skipPastPrior) // skip past prior entry
                {
                    skipPastPrior = openEntry.CheckbookEntry.Id != entryPrior.CheckbookEntry.Id;
                    continue;
                }
                if (sum + Amount(openEntry) == disparity) // did we find a candidate match?
                {
                    CandidateEntry candidate = new CandidateEntry(CandidateIssue.SumsToDifference);
                    if(entry1 != null)
                    {
                        candidate.Add(entry1);
                    }
                    if (entry2 != null)
                    {
                        candidate.Add(entry2);
                    }
                    candidate.Add(openEntry);
                    candidates.Add(candidate);
                    continue;
                }
                switch(maxMatches) // recurse, decrementing maxMatches from 3 to 2 to 1
                {
                    case 3:
                        FindSumCandidatesRecursively(disparity, 
                            candidates, openEntry, null, openEntry, 2);
                        break;
                    case 2:
                        FindSumCandidatesRecursively(disparity, 
                            candidates, entry1, openEntry, openEntry, 1);
                        break;
                    case 1:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Return the "Amount" of a checkbookEntry, negated if it has been cleared.
        /// </summary>
        /// <param name="entry">the open entry to consider</param>
        /// <returns>It's signed amount, negated if it has been cleared</returns>
        private long Amount(OpenEntry entry)
        {
            if (entry == null)
            {
                return 0L;
            }
            OpenEntry openEntry = null;
            if(_openEntries.TryGetValue(entry.CheckbookEntry.Id, out openEntry))
            {
                if(openEntry.IsChecked)
                {
                    return -entry.CheckbookEntry.Amount;
                }
            }
            return entry.CheckbookEntry.Amount;
        }

        /// <summary>
        /// Find checkbook "amount" digit transpositions that would account for a disparity.
        /// </summary>
        /// <param name="disparity">The expected delta for corrected amounts.</param>
        /// <returns>List of potential candidates</returns>
        public List<CandidateEntry> FindTransposeCandidates(long disparity)
        {
            List<CandidateEntry> candidates = new List<CandidateEntry>();
            // Check each split for a tens<->units transpose in cents and in dollars
            foreach (KeyValuePair<Guid, OpenEntry> pair in _openEntries)
            {
                OpenEntry openEntry = pair.Value;
                CheckbookEntry entryTest = new CheckbookEntry();
                foreach (SplitEntry split in openEntry.CheckbookEntry.Splits)
                {
                    entryTest.AddSplit(split.CategoryId, split.Kind, split.Amount);
                }
                foreach (SplitEntry split in entryTest.Splits)
                {
                    split.Amount = TransposeCents(split.Amount);
                    if(openEntry.CheckbookEntry.Amount - entryTest.Amount == disparity)
                    {
                        CandidateEntry candidate = new CandidateEntry(CandidateIssue.TransposedCents);
                        candidate.Add(openEntry);
                        candidates.Add(candidate);
                        break;
                    }
                    split.Amount = TransposeCents(split.Amount); // restore
                    split.Amount = TransposeDollars(split.Amount);
                    if (openEntry.CheckbookEntry.Amount - entryTest.Amount == disparity)
                    {
                        CandidateEntry candidate = new CandidateEntry(CandidateIssue.TransposedDollars);
                        candidate.Add(openEntry);
                        candidates.Add(candidate);
                        break;
                    }
                }
            }
            return candidates;
        }

        /// <summary>
        /// Transpose the units and tens digits.
        /// </summary>
        /// <param name="amount">original value</param>
        /// <returns>input value with digits transposed</returns>
        private long TransposeCents(long amount)
        {
            long baseAmount = amount / 100L;
            long tensAmount = (amount % 100L) / 10L;
            long unitsAmount = (amount % 10L);
            return baseAmount * 100L + unitsAmount * 10L + tensAmount;
        }

        /// <summary>
        /// Transpose the hundreds and thousands digits.
        /// </summary>
        /// <param name="amount">original value</param>
        /// <returns>input value with digits transposed</returns>
        private long TransposeDollars(long amount)
        {
            long baseAmount = amount / 10000L;
            long thousandsAmount = (amount / 1000L) % 10L;
            long hundredsAmount = (amount / 100L) % 10L;
            long remainder = amount % 100L;
            return baseAmount * 10000L + hundredsAmount * 1000L + thousandsAmount * 100L + remainder;
        }

    }

}
