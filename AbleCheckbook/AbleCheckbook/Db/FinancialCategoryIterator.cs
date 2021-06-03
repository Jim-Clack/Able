using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{
   
    public class FinancialCategoryIterator : IDbIterator<FinancialCategory>
    {

        /// <summary>
        /// This is used to iterate over the entries.
        /// </summary>
        IEnumerator<KeyValuePair<string, FinancialCategory>> _enumerator = null;

        /// <summary>
        /// Substring to match, "" for all.
        /// </summary>
        private string _startsWith = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="enumerator">The collection enumerator.</param>
        /// <param name="startsWith">Category name substring to match, "" for all</param>
        public FinancialCategoryIterator(IEnumerator<KeyValuePair<string, FinancialCategory>> enumerator, string startsWith)
        {
            _enumerator = enumerator;
            _startsWith = startsWith.ToLower();
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
                do
                {
                    ok = _enumerator.MoveNext();
                    if (_startsWith.Length == 0)
                    {
                        break;
                    }
                    if (ok)
                    {
                        FinancialCategory entry = _enumerator.Current.Value;
                        if (entry.Name.ToLower().StartsWith(_startsWith))
                        {
                            break;
                        }
                    }
                }
                while (true);
            }
            catch (Exception ex)
            {
                throw new AppException("Error in FinancialCategory HasNextEntry()", ex, ExceptionHandling.NoSaveCleanupContinue);
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
        public FinancialCategory GetNextEntry()
        {
            FinancialCategory entry = null;
            JsonDbAccess.Mutex.WaitOne();
            try
            {
                entry = _enumerator.Current.Value;
            }
            catch (Exception ex)
            {
                throw new AppException("Error in FinancialCategory GetNextEntry()", ex, ExceptionHandling.NoSaveCleanupContinue);
            }
            finally
            {
                JsonDbAccess.Mutex.ReleaseMutex();
            }
            return entry;
        }

    }

}
