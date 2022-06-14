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
        private List<PurchaseRecord> purchaseRecords = null;

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        private List<DeviceRecord> deviceRecords = null;

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        private List<InteractivityRecord> interactivityRecords = null;

        /// <summary>
        /// License data.
        /// </summary>
        public LicenseRecord LicenseRecord
        {
            get
            {
                if(licenseRecord ==  null)
                {
                    licenseRecord = new LicenseRecord(); 
                }
                return licenseRecord;
            }
            set
            {
                licenseRecord = value;
            }
        }

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        public List<PurchaseRecord> PurchaseRecords
        {
            get
            {
                if (purchaseRecords == null)
                {
                    purchaseRecords = new List<PurchaseRecord>() { };
                }
                return purchaseRecords;
            }
            set
            {
                purchaseRecords = value;
            }
        }

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        public List<DeviceRecord> DeviceRecords
        {
            get
            {
                if (deviceRecords == null)
                {
                    deviceRecords = new List<DeviceRecord>() { };
                }
                return deviceRecords;
            }
            set
            {
                deviceRecords = value;
            }
        }

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        public List<InteractivityRecord> InteractivityRecords
        {
            get
            {
                if (interactivityRecords == null)
                {
                    interactivityRecords = new List<InteractivityRecord>() { };
                }
                return interactivityRecords;
            }
            set
            {
                interactivityRecords = value;
            }
        }
    }
}
