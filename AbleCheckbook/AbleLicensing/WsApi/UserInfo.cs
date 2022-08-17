using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{
    public class UserInfo
    {
        /// <summary>
        /// License data.
        /// </summary>
        public LicenseRecord LicenseRecord;

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        public List<PurchaseRecord> PurchaseRecords;

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        public List<DeviceRecord> DeviceRecords;

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        public List<InteractivityRecord> InteractivityRecords;

    }
}
