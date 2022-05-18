using System;
using System.Collections.Generic;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// Serializable JSON DB Content
    /// </summary>
    public class DbContent
    {

        /// <summary>
        /// Name of database
        /// </summary>
        private string dbName = "";
        public string DbName { get => dbName; set => dbName = value; }

        /// <summary>
        /// Environment.UserName
        /// </summary>
        private string authority = Environment.UserName;
        public string Authority { get => authority; set => authority = value; }

        /// <summary>
        /// Computer name.
        /// </summary>
        private string server = Environment.MachineName;
        public string Server { get => server; set => server = value; }

        /// <summary>
        /// Last access date/time.
        /// </summary>
        private DateTime lastAccess = DateTime.Now;
        public DateTime LastAccess { get => lastAccess; set => lastAccess = value; }

        /// <summary>
        /// Table of LicenseRecords.
        /// </summary>
        private Dictionary<Guid, LicenseRecord> licenseRecords = new Dictionary<Guid, LicenseRecord>();
        public Dictionary<Guid, LicenseRecord> LicenseRecords { get => licenseRecords; set => licenseRecords = value; }

        /// <summary>
        /// Table of DeviceRecords.
        /// </summary>
        private Dictionary<Guid, DeviceRecord> deviceRecords = new Dictionary<Guid, DeviceRecord>();
        public Dictionary<Guid, DeviceRecord> DeviceRecords { get => deviceRecords; set => deviceRecords = value; }

        /// <summary>
        /// Table of PurchaseRecords.
        /// </summary>
        private Dictionary<Guid, PurchaseRecord> purchaseRecords = new Dictionary<Guid, PurchaseRecord>();
        public Dictionary<Guid, PurchaseRecord> PurchaseRecords { get => purchaseRecords; set => purchaseRecords = value; }

        /// <summary>
        /// Table of InteractivityRecords.
        /// </summary>
        private Dictionary<Guid, InteractivityRecord> interactivityRecords = new Dictionary<Guid, InteractivityRecord>();
        public Dictionary<Guid, InteractivityRecord> InteractivityRecords { get => interactivityRecords; set => interactivityRecords = value; }
    }
}
