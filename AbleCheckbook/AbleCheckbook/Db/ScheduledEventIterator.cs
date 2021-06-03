using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{
    
    public class ScheduledEventIterator : IDbIterator<ScheduledEvent>
    {

        /// <summary>
        /// This is used to iterate over the entries.
        /// </summary>
        IEnumerator<KeyValuePair<string, ScheduledEvent>> _enumerator = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="enumerator">The collection enumerator.</param>
        public ScheduledEventIterator(IEnumerator<KeyValuePair<string, ScheduledEvent>> enumerator)
        {
            _enumerator = enumerator;
        }

        /// <summary>
        /// Restart at the beginning. (default)
        /// </summary>
        public void Rewind()
        {
            _enumerator.Reset();
        }

        /// <summary>
        /// Check cursor for more entries.
        /// </summary>
        /// <returns>True if there are more entries, false if at end. (don't call twice!)</returns>
        public bool HasNextEntry()
        {
            bool ok = false;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                ok = _enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                throw new AppException("Error in ScheduledEvent HasNextEntry()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return ok;
        }

        /// <summary>
        /// Get the next entry.
        /// </summary>
        /// <returns>The next entry, null on error or on attempt to read past end</returns>
        public ScheduledEvent GetNextEntry()
        {
            ScheduledEvent entry = null;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                entry = _enumerator.Current.Value;
            }
            catch (Exception ex)
            {
                throw new AppException("Error in ScheduledEvent GetNextEntry()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return entry;
        }

    }

}
