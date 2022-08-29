using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{
    public class ReconfigurationRecord
    {
        /// <summary>
        /// For tracking specific reconfigurations.
        /// </summary>
        public Guid AuditId = Guid.Empty;

        /// <summary>
        /// Selects what to reconfigure.
        /// </summary>
        public char ReconfigureSelector = ' ';

        /// <summary>
        /// One or more new values.
        /// </summary>
        public List<string> NewValues = new List<string>();

        /// <summary>
        /// Man-readable,
        /// </summary>
        /// <returns>Textual rendition of content of this record</returns>
        public override string ToString()
        {
            string result = "ReConf{" + ReconfigureSelector;
            foreach(string val in NewValues)
            {
                result += ", " + val;
            }
            return result + "}";
        }

    }

}
