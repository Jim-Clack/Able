using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    /// <summary>
    /// This divides the current db into two portions at the end of last year. Entries
    /// that occur before then but that have not been reconciled by the split date will
    /// be left in both the older and the newer file. Note that "older/_oldDb" and 
    /// "new/_newDb" refers to the dates of the transactions within them, not they date
    /// of DB creation, as this code newly creates _oldDb.  It is expected that the 
    /// current DB is named per LAST year but, if that is not the case, it will rename
    /// it before proceding, but only if force = true.
    /// </summary>
    public class YearEndWrapup
    {

        /// <summary>
        /// Here's the DB where the older entries will be moved.
        /// </summary>
        private IDbAccess _oldDb = null;

        /// <summary>
        /// Here's the original DB that will become the new one.
        /// </summary>
        private IDbAccess _newDb = null;

        /// <summary>
        /// Description of final status after SPlitDbsAt().
        /// </summary>
        private string _message = "";

        /// <summary>
        /// Path and name for archival file.
        /// </summary>
        string _oldFilename = "";

        /// <summary>
        /// Path and name for new DB file.
        /// </summary>
        string _newFilename = "";

        /// <summary>
        /// Keep track of the starting balance for the new checkbook.
        /// </summary>
        long _newStartingBalance = 0L;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="db">Current DB to be split</param>
        /// <remarks>
        /// Resultant output files will be baseName-YYYY.db and baseName-YYYY.db, where YYYY
        /// is the year. Original file may or may not be overwitten, depending on baseName.
        /// and year
        /// </remarks>
        public YearEndWrapup(IDbAccess db)
        {
            _newDb = db;
            _oldFilename = UtilityMethods.GetDbFilename(db.Name, true, true);
            _newFilename = UtilityMethods.GetDbFilename(db.Name, true, false);
            _message = "";
        }

        /// <summary>
        /// Is it time to do a year-end wrap-up
        /// </summary>
        public bool IsTimeToWrapUp
        {
            get
            {
                string dbName = _newFilename + ".acb";
                if(File.Exists(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName))) // already wrapped-up
                {
                    return false;
                }
                DateTime now = DateTime.Now;
                if(now.Month < 2 || (now.Month == 2 && now.Day < 12)) // wait until Feb 12, at least
                {
                    return false;
                }
                int year = DateTime.Now.Year;
                int numOld = 0;
                int numNew = 0;
                int numOldUncleared = 0;
                int numNewUncleared = 0;
                CheckbookEntryIterator iterator = _newDb.CheckbookEntryIterator;
                while(iterator.HasNextEntry())
                {
                    CheckbookEntry entry = iterator.GetNextEntry();
                    if(entry.DateOfTransaction.Year == year)
                    {
                        ++numNew;
                        if (!entry.IsCleared)
                        {
                            ++numNewUncleared;
                        }
                    }
                    else if(entry.DateOfTransaction.Year < year)
                    {
                        ++numOld;
                        if (!entry.IsCleared)
                        {
                            ++numOldUncleared;
                        }
                    }
                }
                if (numOldUncleared * 3 + 1 < numNewUncleared && numOldUncleared * 20 < numOld) // last year mostly reconciled?
                {
                    return true;
                }
                return false; // otehrwise
            }
        }

        /// <summary>
        /// Split DB's so that the current one is renamed to the current year and
        /// contains current or active entries only. Archive older entries in a db
        /// renamed per the previous year.
        /// </summary>
        /// <param name="force">true to force the operation even though it is not recommended</param>
        /// <returns>success, populates message on failure</returns>
        public bool SplitDbsAtDec31(bool force)
        {
            if (!force)
            {
                if(!VerifyDates())
                {
                    return false;
                }
            }
            string dbName = _oldFilename + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            _newStartingBalance = 0L;
            _oldDb = new JsonDbAccess(_oldFilename, null); // do not put bank creds in old db
            _newDb.Name = Path.GetFileNameWithoutExtension(_newFilename);
            try
            {
                bool okay = MoveInactiveEntries();
                _oldDb.Sync();
                InsertStartingBalance();
                _newDb.Sync(); // JBC added 7-5-2021 (correct?)
            }
            catch (Exception ex)
            {
                Logger.Error("Problem with Year-End Wrap-Up.", ex);
                _message = Strings.Get("Problem with Year-End Wrap-Up (Suggest Undo) ") + ex.Message;
                return false;
            }
            _message = Strings.Get("Success");
            return true;
        }

        /// <summary>
        /// Get the last (error?) message regarding the function of this class. 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Here's where all the work is done.
        /// </summary>
        /// <returns>success, populates message on failure</returns>
        private bool MoveInactiveEntries()
        {
            Logger.Info("Year-End Wrap-Up " + _newFilename);
            DateTime startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            List<Guid> toArchive = new List<Guid>();
            CheckbookEntryIterator iterator = _newDb.CheckbookEntryIterator;
            while (iterator.HasNextEntry())
            {
                bool putIntoOld = false;
                bool keepInNew = false;
                CheckbookEntry entry = iterator.GetNextEntry();
                if(entry.DateOfTransaction < startOfYear)
                {
                    putIntoOld = true;
                }
                else
                {
                    keepInNew = true;
                }
                if (!entry.IsCleared || entry.DateCleared > startOfYear)
                {
                    keepInNew = true;
                }
                if(putIntoOld)
                {
                    Logger.Diag("Archiving into Last Year " + entry.Payee + " " + entry.Amount);
                    CreateDuplicate(entry);
                }
                if (!keepInNew)
                {
                    _newStartingBalance += entry.Amount;
                    toArchive.Add(entry.Id);
                }
            }
            foreach(Guid id in toArchive)
            {
                CheckbookEntry entry = _newDb.GetCheckbookEntryById(id);
                Logger.Diag("Discarding from This Year " + entry.Payee + " " + entry.Amount);
                _newDb.DeleteEntry(entry);
            }
            return true;
        }

        /// <summary>
        /// Insert a new balance-forwaard prior to all transactions in the new db.
        /// </summary>
        private void InsertStartingBalance()
        {
            CheckbookEntryIterator iterator = _newDb.CheckbookEntryIterator;
            DateTime oldest = new DateTime(DateTime.Now.Year, 1, 1);
            // oldest entry should be fist, but just in case...
            while(iterator.HasNextEntry())
            {
                CheckbookEntry entry = iterator.GetNextEntry();
                if(entry.MadeBy == EntryMadeBy.YearEnd)
                {
                    _newStartingBalance += entry.Amount;
                    _newDb.DeleteEntry(entry);
                    continue;
                }
                if(entry.DateOfTransaction < oldest)
                {
                    oldest = entry.DateOfTransaction;
                }
            }
            FinancialCategory category = UtilityMethods.GetOrCreateCategory(
                _newDb, Strings.Get("Adjustment"), _newStartingBalance > 0);
            CheckbookEntry carryOver = new CheckbookEntry();
            carryOver.DateCleared = carryOver.DateOfTransaction = oldest.AddDays(-1);
            carryOver.IsCleared = true;
            carryOver.AddSplit(category.Id, TransactionKind.Adjustment, _newStartingBalance);
            carryOver.AppendMemo(_oldFilename + " --> " + _newFilename);
            carryOver.MadeBy = EntryMadeBy.YearEnd;
            carryOver.Payee = Strings.Get("Balance Forward");
            _newDb.InsertEntry(carryOver);
        }

        /// <summary>
        /// Create a duplicate of a checkbook entry and put it into the old DB.
        /// </summary>
        /// <param name="entry">To be duplicated.</param>
        /// <returns>The duplicate</returns>
        private CheckbookEntry CreateDuplicate(CheckbookEntry entry)
        {
            CheckbookEntry clonedEntry = entry.Clone(true);
            foreach (SplitEntry split in entry.Splits)
            {
                Guid categoryId = split.CategoryId;
                if (_newDb != null && _oldDb != null)
                {
                    FinancialCategory category = _newDb.GetFinancialCategoryById(split.CategoryId);
                    if (category == null)
                    {
                        category = UtilityMethods.GetCategoryOrUnknown(_newDb, null);
                    }
                    categoryId = UtilityMethods.GetOrCreateCategory(_oldDb, category.Name, category.IsCredit).Id;
                }
                clonedEntry.AddSplit(categoryId, split.Kind, split.Amount);
            }
            if (_oldDb.GetCheckbookEntryById(entry.Id) == null)
            {
                _oldDb.InsertEntry(clonedEntry);
            }
            else
            {
                _oldDb.UpdateEntry(clonedEntry, entry, true);
            }
            return clonedEntry;
        }

        /// <summary>
        /// Verify that all is well before continuing.
        /// </summary>
        /// <returns>success, populates message on failure</returns>
        private bool VerifyDates()
        {
            if (_newDb.Name.Contains("-" + DateTime.Now.Year))
            {
                _message = Strings.Get("Active DB name indicates that it is already current.");
                return false;
            }
            DateTime loDate = new DateTime(DateTime.Now.Year - 1, 12, 1); // Start of Dec
            DateTime hiDate = new DateTime(DateTime.Now.Year, 1, 1); // Start of Jan
            int loDateCount = 0;
            int hiDateCount = 0;
            int totalCleared = 0;
            CheckbookEntryIterator iterator = _newDb.CheckbookEntryIterator;
            while (iterator.HasNextEntry())
            {
                CheckbookEntry entry = iterator.GetNextEntry();
                if (entry.IsCleared && entry.DateCleared >= hiDate)
                {
                    hiDateCount++;
                }
                if (!entry.IsCleared && entry.DateOfTransaction >= loDate)
                {
                    loDateCount++;
                }
                if (entry.IsCleared)
                {
                    totalCleared++;
                }
            }
            if (totalCleared <= 1)
            {
                _message = Strings.Get("Cannot perform year-end with large proportion unreconciled.");
                return false;
            }
            if (loDateCount > hiDateCount) // more old uncleared than recently cleared
            {
                _message = Strings.Get("Too soon for year-end: many old entries not yet been cleared.");
                return false;
            }
            return true;
        }

    }

}
