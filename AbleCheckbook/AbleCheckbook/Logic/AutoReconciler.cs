using AbleCheckbook.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class AutoReconciler
    {

        /// <summary>
        /// Man-readable enum for CheckbookEntry match scores
        /// </summary>
        public enum ThresholdScore
        {
            Mismatch =  0, 
            Unlikely = 25,   // Only Amounts Match (generally not acceptable)
            Possible = 50,   // Questionable Match (acceptable after exhaustive comparison)
            Probable = 75,   // But Some Differences (acceptable for aggressive matching)
            Matched = 100,   // Only Minor Differences (acceptable in all cases)
        }

        private IDbAccess _userDb = null;

        private IDbAccess _statementDb = null;

        private int _countDateWrong = 0;

        private int _countDuplicates = 0;

        private string _firstDuplicate = null;

        private string _lastDuplicate = null;

        public int CountDateWrong { get => _countDateWrong; }

        public int CountDuplicates { get => _countDuplicates; }

        public string FirstDuplicate { get => _firstDuplicate; }

        public string LastDuplicate { get => _lastDuplicate; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="userDb">For accessing user DB, already partially populated</param>
        /// <param name="statementDb">Bank statement</param>
        public AutoReconciler(IDbAccess userDb, IDbAccess statementDb)
        {
            _userDb = userDb;
            _statementDb = statementDb;
        }

        /// <summary>
        /// Perform the automatic reconcile.
        /// </summary>
        /// <param name="prevReconDate">Start of the period to process</param>
        /// <param name="thisReconDate">One day past end of the period</param>
        /// <param name="useBankInfo">true to use bankXxx instead of regular properties from bankDb</param>
        /// <param name="aggressive">true to perform a more aggressive match</param>
        /// <returns>number of CheckbookEntries affected - if >0 then call reconHelper.UpdateOpenEntries()</returns>
        public int Reconcile(DateTime prevReconDate, DateTime thisReconDate, bool useBankInfo, bool aggressive)
        {
            if(_userDb == null || _statementDb == null)
            {
                return 0; // should never occur, but...
            }
            _countDateWrong = 0;
            int countAffected = 0;
            double targetScore = (int)(aggressive ? AutoReconciler.ThresholdScore.Possible : AutoReconciler.ThresholdScore.Probable);
            CheckbookEntryIterator bankIterator = _statementDb.CheckbookEntryIterator;
            while (bankIterator.HasNextEntry())
            {
                CheckbookEntry bankEntry = bankIterator.GetNextEntry();
                double bestScore = 0.0;
                DateTime bankDate = bankEntry.DateOfTransaction;
                if (Math.Abs(DateTime.Now.Subtract(bankDate).Days) > 400 && useBankInfo)
                {
                    bankDate = bankEntry.BankTranDate;
                }
                if(bankDate.Date < prevReconDate.Date || bankDate.Date >= thisReconDate.Date)
                {
                    ++_countDateWrong;
                    continue;
                }
                CheckbookEntry bestEntry = null;
                CheckbookEntryIterator userIterator = _userDb.CheckbookEntryIterator;
                while (userIterator.HasNextEntry())
                {
                    CheckbookEntry userEntry = userIterator.GetNextEntry();
                    // Future: we shouldn't iterate over ALL entries, just the ones to be reconciled
                    if (userEntry.IsChecked || !string.IsNullOrEmpty(userEntry.BankPayee))
                    {
                        continue;
                    }
                    double score = Score(userEntry, bankEntry, useBankInfo);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestEntry = userEntry;
                    }
                }
                UpdateUserDb(targetScore, bankEntry, bestScore, bestEntry, useBankInfo);
                ++countAffected;
            }
            return countAffected;
        }

        /// <summary>
        /// See if any of the bank entries have already been processed/cleared in the user DB (deletes dups in _statementDb)
        /// </summary>
        /// <param name="prevReconDate">Start of the period to process</param>
        /// <param name="thisReconDate">One day past end of the period</param>
        /// <param name="message">description of duplicate occurrences</param>
        /// <returns>number of duplicates</returns>
        public int CheckForDuplicates(DateTime prevReconDate, DateTime thisReconDate, out string message)
        {
            message = "";
            _countDuplicates = 0;
            if (_userDb == null || _statementDb == null)
            {
                return 0; // should never occur, but...
            }
            LinkedList<CheckbookEntry> duplicates = new LinkedList<CheckbookEntry>();
            DateTime firstDupDate = prevReconDate.Date;
            DateTime lastDupDate = thisReconDate.Date;
            CheckbookEntryIterator bankIterator = _statementDb.CheckbookEntryIterator;
            while (bankIterator.HasNextEntry())
            {
                CheckbookEntry bankEntry = bankIterator.GetNextEntry();
                CheckbookEntryIterator userIterator = _userDb.CheckbookEntryIterator;
                string bankPayee = bankEntry.Payee.Trim();
                long bankAmount = Math.Abs(bankEntry.Amount);
                DateTime bankDate = bankEntry.DateOfTransaction;
                long bankCheckNumber = 0;
                long.TryParse(bankEntry.CheckNumber, out bankCheckNumber);
                if (string.IsNullOrEmpty(bankPayee))
                {
                    bankPayee = bankEntry.BankPayee;
                    bankAmount = Math.Abs(bankEntry.BankAmount);
                    bankCheckNumber = bankEntry.BankCheckNumber;
                    bankDate = bankEntry.BankTranDate;
                }
                while (userIterator.HasNextEntry())
                {
                    CheckbookEntry userEntry = userIterator.GetNextEntry();
                    if(!string.IsNullOrEmpty(userEntry.BankPayee.Trim()) &&
                        Math.Abs(userEntry.BankAmount) == bankAmount &&
                        userEntry.BankPayee.Replace(" ", "").Equals(bankPayee.Replace(" ", "")) &&
                        userEntry.BankCheckNumber == bankCheckNumber &&
                        userEntry.BankTranDate.Date == bankDate.Date)
                    {
                        string duplicateString = "[" + bankDate.ToShortDateString() + " (" +
                            UtilityMethods.FormatCurrency(bankAmount) + ") " +
                            bankPayee + "]";
                        if(_firstDuplicate == null)
                        {
                            _firstDuplicate = duplicateString;
                            firstDupDate = bankDate.Date;
                            _lastDuplicate = duplicateString;
                            lastDupDate = bankDate.Date;
                        }
                        else if(bankDate.Date > lastDupDate)
                        {
                            _lastDuplicate = duplicateString;
                            lastDupDate = bankDate.Date;
                        }
                        else if (bankDate.Date < firstDupDate)
                        {
                            _firstDuplicate = duplicateString;
                            firstDupDate = bankDate.Date;
                        }
                        duplicates.AddLast(bankEntry);
                        ++_countDuplicates;
                    }
                }
            }
            foreach(CheckbookEntry entry in duplicates)
            {
                _statementDb.DeleteEntry(entry); // delete duplicates in the statement
            }
            if(_countDuplicates > 0)
            {
                message = "" + _countDuplicates + Strings.Get(" duplicates ignored ") + firstDupDate.ToShortDateString();
                if (_countDuplicates > 1)
                {
                    message = message + " - " + lastDupDate.ToShortDateString();
                }
            }
            Logger.Diag("CheckForDuplicates " + _countDuplicates);
            return _countDuplicates;
        }

        /// <summary>
        /// Update the user DB with the new/changed checkbook entry.
        /// </summary>
        /// <param name="targetScore">Score to beat in order to merge</param>
        /// <param name="bankEntry">From bank statement</param>
        /// <param name="bestScore">Best match score</param>
        /// <param name="bestEntry">User DB entry that matches bank entry with bestScore</param>
        /// <param name="useBankInfo">true to use bankXxx instead of regular properties from bankDb</param>
        private void UpdateUserDb(double targetScore, 
            CheckbookEntry bankEntry, double bestScore, CheckbookEntry bestEntry, bool useBankInfo)
        {
            CheckbookEntry newEntry;
            if (bestScore < targetScore)
            {
                newEntry = new CheckbookEntry();
            }
            else
            {
                newEntry = bestEntry.Clone();
            }
            // populate newEntry BankXxxx properties in all cases
            newEntry.BankPayee = bankEntry.BankPayee;
            newEntry.BankTranDate = bankEntry.BankTranDate;
            newEntry.BankAmount = bankEntry.BankAmount;
            newEntry.BankCheckNumber = bankEntry.BankCheckNumber;
            newEntry.BankTransaction = bankEntry.BankTransaction;
            // if not useBankInfo the overwrite those same BankXxxx properties
            if (!useBankInfo)
            {
                newEntry.BankPayee = bankEntry.Payee;
                newEntry.BankTranDate = bankEntry.DateOfTransaction;
                newEntry.BankAmount = bankEntry.Amount;
                long checkNumber = 0L;
                long.TryParse(bankEntry.CheckNumber, out checkNumber);
                newEntry.BankCheckNumber = checkNumber;
            }
            if(newEntry.BankPayee == null || newEntry.BankPayee.Length < 4)
            {
                newEntry.BankPayee = "----"; // flags BankXxxx proerties as filled-in
            }
            newEntry.IsChecked = true;
            if (newEntry.BankTranDate > DateTime.Now.AddDays(-400))
            {
                newEntry.DateCleared = newEntry.BankTranDate;
            }
            if (bestScore < targetScore)
            {
                // if this is to be a NEW entry, populate its primary properties
                string catName = UtilityMethods.GuessAtCategory(newEntry.BankPayee);
                Guid catId = UtilityMethods.GetCategoryOrUnknown(_userDb, catName).Id;
                TransactionKind tranKind = (newEntry.BankAmount < 0 ? TransactionKind.Payment : TransactionKind.Deposit);
                newEntry.AddSplit(catId, tranKind, Math.Abs(newEntry.BankAmount));
                newEntry.CheckNumber = (newEntry.BankCheckNumber == 0 ? "" : "" + newEntry.BankCheckNumber);
                newEntry.Payee = newEntry.BankPayee;
                newEntry.DateOfTransaction = newEntry.BankTranDate;
                newEntry.MadeBy = EntryMadeBy.Reconciler;
                newEntry.ModifiedBy = System.Environment.UserName;
                _userDb.InsertEntry(newEntry);
                Logger.Diag("AutoReconciler - Inserted Entry " + newEntry.ToShortString());
            }
            else
            {
                newEntry.BankMergeAccepted = true;
                _userDb.UpdateEntry(newEntry, bestEntry, true);
                Logger.Diag("AutoReconciler - Updated Entry " + newEntry.ToShortString());
            }
        }

        /// <summary>
        /// Compare two checkbook entries and score them for match.
        /// </summary>
        /// <param name="userEntry">Per checkbook, user expectations</param>
        /// <param name="bankEntry">Entry from bank statement</param>
        /// <param name="useBankInfo">True to check bankEntry.bankXxx fields as well</param>
        /// <returns>match score, most significance between 0.0 and 100.0</returns>
        public double Score(CheckbookEntry userEntry, CheckbookEntry bankEntry, bool useBankInfo)
        {
            // if amounts match, start with a score of 30 (may yet be added to or subtracted from)
            double score = 0.0;
            if (userEntry.Amount != bankEntry.Amount)
            {
                if (!useBankInfo || userEntry.Amount != bankEntry.BankAmount)
                {
                    return score;
                }
            }
            score = 30.0;
            // add from 0 to 30 for nearly matching dates, expecting bank to process in one day
            DateTime expectedDate = userEntry.DateOfTransaction.AddDays(1);
            int daysOff = Math.Abs(bankEntry.DateOfTransaction.Subtract(expectedDate).Days);
            if(daysOff > 30 && useBankInfo)
            {
                daysOff = Math.Abs(bankEntry.BankTranDate.Subtract(expectedDate).Days);
            }
            score = score + 30 - Math.Min(30, daysOff);
            // if check numbers are specified on both, add or subtract 40 depending on the match
            long userCheckNumber = 0;
            long bankCheckNumber = 0;
            if (long.TryParse(userEntry.CheckNumber, out userCheckNumber) && userCheckNumber > 0)
            {
                long.TryParse(bankEntry.CheckNumber, out bankCheckNumber);
                if (useBankInfo && bankCheckNumber == 0)
                {
                    bankCheckNumber = bankEntry.BankCheckNumber;
                }
                if(bankCheckNumber > 0)
                {
                    score = score + (userCheckNumber == bankCheckNumber ? 40 : -20);
                }
            }
            // check for substring matches on the payee, adding up to 45
            string bankPayee = bankEntry.Payee;
            if (useBankInfo && bankPayee.Trim().Length < bankEntry.BankPayee.Trim().Length)
            {
                bankPayee = bankEntry.BankPayee;
            }
            score = score + ScoreSubStringMatch(userEntry.Payee, bankPayee, 45);
            return score;
        }

        /// <summary>
        /// Compare strings for matching substrings within them
        /// </summary>
        /// <param name="pattern">Payee to be searched for, may be abbreviated or whatever</param>
        /// <param name="toSearch">String that may contain part of the pattern</param>
        /// <param name="maxScore">to be returned if the full pattern is found</param>
        /// <returns>0 if there is no match, maxScore for a good match, may be even higher</returns>
        private double ScoreSubStringMatch(string pattern, string toSearch, int maxScore)
        {
            // future: this brute-force approach should be rewritten more efficiently
            double score1 = 0, score2 = 0;
            string[] wordsInPattern = pattern.Replace("  ", " ").Split(' ');
            string[] wordsInToSearch = toSearch.Replace("  ", " ").Split(' ');
            score1 = ScoreMatches(wordsInPattern, wordsInToSearch, maxScore);
            if (score1 < 2 * maxScore / 3) // <-- performance compromise
            {
                string[] consanantsPattern = pattern.Replace("a", "").Replace("e", "").Replace("i", "").
                    Replace("o", "").Replace("u", "").Replace("  ", " ").Split(' ');
                string[] consanantsToSearch = toSearch.Replace("a", "").Replace("e", "").Replace("i", "").
                    Replace("o", "").Replace("u", "").Replace("  ", " ").Split(' ');
                score2 = ScoreMatches(consanantsPattern, consanantsToSearch, maxScore);
            }
            return Math.Max(score1, score2);
        }

        /// <summary>
        /// Look for any of several search patterns in any of several search strings
        /// </summary>
        /// <param name="wordsInPattern">array of patterns to look for</param>
        /// <param name="wordsInToSearch">array of strings to be searched</param>
        /// <param name="maxScore">approx max score to be returned on a good match</param>
        /// <returns>0 if there is no match, maxScore for a good match, may be even higher</returns>
        private static double ScoreMatches(string[] wordsInPattern, string[] wordsInToSearch, int maxScore)
        {
            const int MIN_MATCH_LGT = 3;     // don't even consider patterns shorter than this
            const int MIN_MATCH_DIVISOR = 6; // reserve maxScore for a match at least this long
            int maxPatternLgt = 0;           // the longest pattern to be matched
            int maxMatchLgt = 0;             // here's what we are trying to find
            // now find the longest match
            foreach (string patternWord in wordsInPattern)
            {
                int patternLgt = patternWord.Trim().Length;
                maxPatternLgt = Math.Max(maxPatternLgt, patternLgt);
                if (patternLgt < MIN_MATCH_LGT)
                {
                    continue;
                }
                foreach (string toSearchWord in wordsInToSearch)
                {
                    if (toSearchWord.Length < MIN_MATCH_LGT)
                    {
                        continue;
                    }
                    int matchLgt = 0;
                    for (int charNum = 0; charNum < patternLgt && charNum < toSearchWord.Length; ++charNum)
                    {
                        if (Char.ToLower(patternWord[charNum]) == Char.ToLower(toSearchWord[charNum]))
                        {
                            ++matchLgt;
                        }
                        maxMatchLgt = Math.Max(maxMatchLgt, matchLgt);
                    }
                }
            }
            return (maxMatchLgt * maxScore) / Math.Max(maxPatternLgt, MIN_MATCH_DIVISOR);
        }
    }
}
