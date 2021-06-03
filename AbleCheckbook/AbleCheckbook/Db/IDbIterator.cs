using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// Interface for DB API to return collections.
    /// </summary>
    /// <typeparam name="T">Type of collection</typeparam>
    public interface IDbIterator<T>
    {

        /// <summary>
        /// Restart at the beginning. (default)
        /// </summary>
        void Rewind();

        /// <summary>
        /// Check cursor for more entries.
        /// </summary>
        /// <returns>True if there are more entries, false if at end</returns>
        bool HasNextEntry();

        /// <summary>
        /// Get the next entry.
        /// </summary>
        /// <returns>The next entry, null on error or on attempt to read past end</returns>
        T GetNextEntry();

    }
}
