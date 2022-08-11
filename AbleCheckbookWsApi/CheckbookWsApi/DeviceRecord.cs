using System;
using System.Collections.Generic;
using System.Text;

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
        Guid Id;

        /// <summary>
        /// When was this record created?
        /// </summary>
        DateTime DateCreated;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        DateTime DateModified;

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        Guid FkLicenseId;

        /// <summary>
        /// Site ID (device) abbreviation.
        /// </summary>
        string DeviceSite;

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        int UserLevelPunct;

        /// <summary>
        /// Binary authentication/authorization info.
        /// </summary>
        string CodesAndPin;

    }
}
