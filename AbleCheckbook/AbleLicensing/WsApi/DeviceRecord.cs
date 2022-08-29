using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{

    /// <summary>
    /// Host/device info.
    /// </summary>
    public class DeviceRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        public Guid Id = Guid.Empty;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated = DateTime.Now;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified = DateTime.Now;

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId = Guid.Empty;

        /// <summary>
        /// Site ID (device) abbreviation.
        /// </summary>
        public string DeviceSiteId = "";

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        public int UserLevelPunct = 0;

        /// <summary>
        /// Binary authentication/authorization info.
        /// </summary>
        public string CodesAndPin = "";

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DevRec{" + UserLevelPunct + ", " + DeviceSiteId + "}";
        }

    }
}
