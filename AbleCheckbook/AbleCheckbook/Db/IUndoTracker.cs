using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Interface for tracking actions that may be undo-able.
    /// </summary>
    public interface IUndoTracker
    {

        /// <summary>
        /// Mark the start of an Undo block, also celaring ant pending redo's.
        /// </summary>
        /// <param name="description">Very brief man-readable.</param>
        void MarkUndoBlock(string description);

        /// <summary>
        /// Track the deletion of a checkbook entry.
        /// </summary>
        /// <param name="values">entry that was deleted</param>
        void TrackDeletion(ReconciliationValues values);

        /// <summary>
        /// Track the insertion of a new entry.
        /// </summary>
        /// <param name="values">entry that was inserted</param>
        void TrackInsertion(ReconciliationValues values);

        /// <summary>
        /// Track the deletion of a checkbook entry.
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        void TrackDeletion(CheckbookEntry entry);

        /// <summary>
        /// Track the insertion of a new entry.
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        void TrackInsertion(CheckbookEntry entry);

        /// <summary>
        /// Track the deletion of a financial category.
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        void TrackDeletion(FinancialCategory entry);

        /// <summary>
        /// Track the insertion of a new entry.
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        void TrackInsertion(FinancialCategory entry);

        /// <summary>
        /// Track the deletion of a scheduled event.
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        void TrackDeletion(ScheduledEvent entry);

        /// <summary>
        /// Track the insertion of a new entry.
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        void TrackInsertion(ScheduledEvent entry);

        /// <summary>
        /// Track the deletion of a Memorized Payee.
        /// </summary>
        /// <param name="entry">entry that was deleted</param>
        void TrackDeletion(MemorizedPayee entry);

        /// <summary>
        /// Track the insertion of a new entry.
        /// </summary>
        /// <param name="entry">entry that was inserted</param>
        void TrackInsertion(MemorizedPayee entry);

    }
}
