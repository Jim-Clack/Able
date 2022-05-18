using System;
using System.Collections.Generic;

namespace AbleStrategiesServices.Support
{
    public class DbContent
    {

        public string _dbName = "";

        private string authority = Environment.UserName;

        private string server = Environment.MachineName;

        private DateTime lastAccess = DateTime.Now;

        private Dictionary<Guid, LicenseRecord> licenseRecords = new Dictionary<Guid, LicenseRecord>();

        private Dictionary<Guid, DeviceRecord> deviceRecords = new Dictionary<Guid, DeviceRecord>();

        private Dictionary<Guid, PurchaseRecord> purchaseRecords = new Dictionary<Guid, PurchaseRecord>();

        private Dictionary<Guid, InteractivityRecord> interactivityRecords = new Dictionary<Guid, InteractivityRecord>();

        public string DbName { get => _dbName; set => _dbName = value; }
        public string Authority { get => authority; set => authority = value; }
        public string Server { get => server; set => server = value; }
        public DateTime LastAccess { get => lastAccess; set => lastAccess = value; }

        public Dictionary<Guid, LicenseRecord> LicenseRecords { get => licenseRecords; set => licenseRecords = value; }
        public Dictionary<Guid, DeviceRecord> DeviceRecords { get => deviceRecords; set => deviceRecords = value; }
        public Dictionary<Guid, PurchaseRecord> PurchaseRecords { get => purchaseRecords; set => purchaseRecords = value; }
        public Dictionary<Guid, InteractivityRecord> InteractivityRecords { get => interactivityRecords; set => interactivityRecords = value; }
    }
}
