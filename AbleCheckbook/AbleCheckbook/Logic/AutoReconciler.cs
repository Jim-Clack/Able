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
            Mismatch = 0, 
            Unlikely = 25,   // Only Amounts Match (generally not acceptable)
            Possible = 50,   // Questionable Match (acceptable after exhaustive comparison)
            Probable = 75,   // But Some Differences (acceptable for aggressive matching)
            Matched = 100,   // Only Minor Differences (acceptable in all cases)
        }

        public AutoReconciler()
        {

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
            int userCheckNumber = 0;
            int bankCheckNumber = 0;
            if (int.TryParse(userEntry.CheckNumber, out userCheckNumber) && userCheckNumber > 0)
            {
                int.TryParse(bankEntry.CheckNumber, out bankCheckNumber);
                if (useBankInfo && bankCheckNumber == 0)
                {
                    bankCheckNumber = bankEntry.BankCheckNumber;
                }
                if(bankCheckNumber > 0)
                {
                    score = score + (userCheckNumber == bankCheckNumber ? 40 : -20);
                }
            }
            // check for substring matches on the payee, adding up to 50
            string bankPayee = bankEntry.Payee;
            if (useBankInfo && bankPayee.Trim().Length < bankEntry.BankPayee.Trim().Length)
            {
                bankPayee = bankEntry.BankPayee;
            }
            score = score + ScoreSubStringMatch(userEntry.Payee, bankPayee, 50);
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
