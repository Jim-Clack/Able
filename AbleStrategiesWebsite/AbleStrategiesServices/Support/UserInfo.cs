using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public class UserInfo
    {

        /// <summary>
        /// License data.
        /// </summary>
        private LicenseRecord licenseRecord = null;

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        private List<PurchaseRecord> purchaseRecords = new List<PurchaseRecord>();

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        private List<DeviceRecord> deviceRecords = new List<DeviceRecord>();

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        private List<InteractivityRecord> interactivityRecords = new List<InteractivityRecord>();

        /// <summary>
        /// License data.
        /// </summary>
        public LicenseRecord LicenseRecord { get => licenseRecord; set => licenseRecord = value; }

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        public List<PurchaseRecord> PurchaseRecords { get => purchaseRecords; set => purchaseRecords = value; }

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        public List<DeviceRecord> DeviceRecords { get => deviceRecords; set => deviceRecords = value; }

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        public List<InteractivityRecord> InteractivityRecords { get => interactivityRecords; set => interactivityRecords = value; }
    }
}
