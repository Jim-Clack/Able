using AbleLicensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    /// <summary>
    /// This is a JSON record but NOT a DB record - never persisted.
    /// </summary>
    public class ReconfigurationRecord
    {
        /// <summary>
        /// For tracking specific reconfigurations.
        /// </summary>
        private Guid auditId = Guid.NewGuid();

        /// <summary>
        /// Selects what to reconfigure.
        /// </summary>
        private ReconfigurationSelection reconfigureSelector = ReconfigurationSelection.Unknown;

        /// <summary>
        /// One or more new values.
        /// </summary>
        private List<string> newValues = new List<string>();

        /// <summary>
        /// For tracking specific reconfigurations.
        /// </summary>
        public Guid AuditId { get => auditId; set => auditId = value; }

        /// <summary>
        /// Selects what to reconfigure.
        /// </summary>
        public ReconfigurationSelection ReconfigureSelector { get => reconfigureSelector; set => reconfigureSelector = value; }

        /// <summary>
        /// One or more new values.
        /// </summary>
        public List<string> NewValues { get => newValues; set => newValues = value; }

        /// <summary>
        /// Man-readable,
        /// </summary>
        /// <returns>Textual rendition of content of this record</returns>
        public override string ToString()
        {
            string result = "ReConf{" + ReconfigureSelector;
            foreach (string val in NewValues)
            {
                result += "," + val;
            }
            return result + "}";
        }
    }
}
