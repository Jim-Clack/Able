using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// The database layer is an implementation of this.
    /// </summary>
    public interface IDbAccess : IDisposable
    {

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        void CloseWithoutSync();

        /// <summary>
        /// Update and close the DB.
        /// </summary>
        void SyncAndClose();

        /// <summary>
        /// Full path the current DB account file.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Get account info, i.e. online connection settings.)
        /// </summary>
        IAccount Account { get; }

        /// <summary>
        /// Iterate over the checkbook entries, updating the balance in each.
        /// </summary>
        /// <returns>Final balance.</returns>
        long AdjustBalances();

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        bool Sync();

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        void IdleTimeSync();

        /// <summary>
        /// Get the name of the checkbook (account, etc.). Typically set by the ctor.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        bool IsDirty
        {
            get;
            set;
        }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        bool Successful
        {
            get;
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        string LastMessage
        {
            get;
        }

        /// <summary>
        /// Are we in the middle of something?
        /// </summary>
        InProgress InProgress { get; set; }

        /////////////////////////// Reconciliation ///////////////////////////

        /// <summary>
        /// Get recon values - balance & date.
        /// </summary>
        /// <returns>values</returns>
        ReconciliationValues GetReconciliationValues();

        /// <summary>
        /// Insert reconciliation values.
        /// </summary>
        /// <param name="values">recon values</param>
        /// <returns>success</returns>
        bool InsertEntry(ReconciliationValues values);

        /// <summary>
        /// Delete reconciliation values.
        /// </summary>
        /// <param name="values">ignored - merely stashes old values to the undo queue</param>
        /// <returns>success</returns>
        bool DeleteEntry(ReconciliationValues values);

        /////////////////////////// CheckbookEntry ///////////////////////////

        /// <summary>
        /// Iterator to travese checkbook entries by date entered.
        /// </summary>
        CheckbookEntryIterator CheckbookEntryIterator
        {
            get;
        }

        /// <summary>
        /// Iterator to travese checkbook entries.
        /// </summary>
        CheckbookEntryIterator GetCheckbookEntryIterator(SortEntriesBy sortEntriesBy);

        /// <summary>
        /// Add a new entry into the checkbook.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID may be updated</param>
        /// <param name="highlight">how to highlight the entry, Modified if by default</param>
        /// <returns>True if successful</returns>
        bool InsertEntry(CheckbookEntry entry, Highlight highlight = Highlight.Modified);

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <param name="updateModDate">true to update the modified date</param>
        /// <returns>True if successful</returns>
        bool UpdateEntry(CheckbookEntry newEntry, CheckbookEntry oldEntry, bool updateModDate);

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        CheckbookEntry GetCheckbookEntryById(Guid id);

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        bool DeleteEntry(CheckbookEntry entry);

        /////////////////////////// ScheduledEvent ///////////////////////////

        /// <summary>
        /// Iterator to travese entries by next due date.
        /// </summary>
        ScheduledEventIterator ScheduledEventIterator
        {
            get;
        }

        /// <summary>
        /// Add a new entry into the scheduled events
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID may be updated</param>
        /// <returns>True if successful</returns>
        bool InsertEntry(ScheduledEvent entry);

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        bool UpdateEntry(ScheduledEvent newEntry, ScheduledEvent oldEntry);

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        ScheduledEvent GetScheduledEventById(Guid id);

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        bool DeleteEntry(ScheduledEvent entry);

        ////////////////////////////// Category //////////////////////////////

        /// <summary>
        /// Iterator to travese categories in alphabetic order.
        /// </summary>
        FinancialCategoryIterator FinancialCategoryIterator
        {
            get;
        }

        /// <summary>
        /// Iterator to travese entries by partial match of category name, "" = all.
        /// </summary>
        FinancialCategoryIterator GetFinancialCategoryIterator(string startsWith);

        /// <summary>
        /// Add a new entry into the categories.
        /// </summary>
        /// <param name="entry">To be saved to the DB; note that it's ID may be updated</param>
        /// <returns>True if successful</returns>
        bool InsertEntry(FinancialCategory entry);

        /// <summary>
        /// Flag an entry as "Updated."
        /// </summary>
        /// <param name="newEntry">Modified version; note that it will be updated by Id</param>
        /// <param name="oldEntry">Prior unchanged version; note that it will be updated by Id</param>
        /// <returns>True if successful</returns>
        bool UpdateEntry(FinancialCategory newEntry, FinancialCategory oldEntry);

        /// <summary>
        /// Get an entry based on the ID.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        FinancialCategory GetFinancialCategoryById(Guid id);

        /// <summary>
        /// Get an entry based on the category name.
        /// </summary>
        /// <param name="id">The category name to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        FinancialCategory GetFinancialCategoryByName(string name);

        /// <summary>
        /// Remove an entry from the DB.
        /// </summary>
        /// <param name="entry">To be removed from the DB; note that it will be done by Id</param>
        /// <returns>True if successful</returns>
        bool DeleteEntry(FinancialCategory entry);

    }
}
