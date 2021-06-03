using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Iterator for a database table.
    /// </summary>
    /// <typeparam name="T">Database record type.</typeparam>
    public class JsonDbIterator<T> : IDbIterator<T>
    {

        /// <summary>
        /// Restart at the beginning. (default)
        /// </summary>
        public void Rewind()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check cursor for more entries.
        /// </summary>
        /// <returns>True if there are more entries, false if at end</returns>
        public bool HasNextEntry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the next entry.
        /// </summary>
        /// <returns>The next entry, null on error or on attempt to read past end</returns>
        public T GetNextEntry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the next entry.
        /// </summary>
        /// <param name="id">The GUID to look-up</param>
        /// <returns>The entry with that GUID, null on error or on attempt to read past end</returns>
        public T GetEntryById(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public string LastMessage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }


}
