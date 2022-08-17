using System;
using System.Collections.Generic;
using System.Text;

NO - OBSOLETE

namespace AbleStrategies.CheckbookWsApi
{

    /// <summary>
    /// Host/device info.
    /// </summary>
    public class DeviceRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified;

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId;

        /// <summary>
        /// Site ID (device) abbreviation.
        /// </summary>
        public string DeviceSite;

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        public int UserLevelPunct;

        /// <summary>
        /// Binary authentication/authorization info.
        /// </summary>
        public string CodesAndPin;

    }
}
