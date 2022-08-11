using System;
using System.Collections.Generic;
using System.Text;

namespace AbleStrategies.CheckbookWsApi
{
    public interface UserInfo
    {
        /// <summary>
        /// License data.
        /// </summary>
        LicenseRecord LicenseRecord { get; set; }

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        List<PurchaseRecord> PurchaseRecords { get; set; }

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        List<DeviceRecord> DeviceRecords { get; set; }

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        List<InteractivityRecord> InteractivityRecords { get; set; }

    }
}
