using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleLicensing;

namespace AbleStrategiesServices.Support
{
    public class UserInfo
    {

        /// <summary>
        /// License data. 
        /// </summary>
        private LicenseRecord licenseRecord = null;

        /// <summary>
        /// List of PurchaseRecords. (THere can only be 0 or 1, never more than one purchase record)
        /// </summary>
        private List<PurchaseRecord> purchaseRecords = new List<PurchaseRecord>();

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        private List<DeviceRecord> deviceRecords = new List<DeviceRecord>();

        /// <summary>
        /// List of InteractivityRecords. (Very old ones may be deleted except for the oldest three)
        /// </summary>
        private List<InteractivityRecord> interactivityRecords = new List<InteractivityRecord>();

        /// <summary>
        /// Default ctor.
        /// </summary>
        public UserInfo()
        {
        }

        /// <summary>
        /// Initialized ctor.
        /// </summary>
        /// <param name="name">name of contact</param>
        /// <param name="addr">street address</param>
        /// <param name="city">city and state</param>
        /// <param name="zip">postal code</param>
        /// <param name="phone">phone number</param>
        /// <param name="email">email address</param>
        /// <param name="siteId">host/device ID</param>
        /// <param name="punct">UserLevelPunct</param>
        public UserInfo(string name, string addr, string city, string zip, string phone, string email, string siteId, UserLevelPunct punct)
        {
            LicenseRecord = new LicenseRecord();
            LicenseRecord.ContactName = name;
            LicenseRecord.ContactAddress = addr;
            LicenseRecord.ContactCity = city;
            LicenseRecord.ContactZip = zip;
            LicenseRecord.ContactPhone = phone;
            LicenseRecord.ContactEMail = email;
            LicenseRecord.LicenseFeatures = "";
            LicenseRecord.LicenseCode = "";
            DeviceRecord deviceRecord = new DeviceRecord();
            deviceRecord.DeviceSiteId = siteId;
            deviceRecord.UserLevelPunct = (int)punct;
            DeviceRecords.Add(deviceRecord);
        }

        /// <summary>
        /// License data.
        /// </summary>
        public LicenseRecord LicenseRecord
        {
            get
            {
                if (licenseRecord == null)
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

        /// <summary>
        /// Set the IDs for an all-new UserInfo to be added to the Dbo.
        /// </summary>
        public void SetIdsAllNew()
        {
            // Adjust Id, EditFlag, and FkLicenseId in all records 
            LicenseRecord.Id = Guid.NewGuid();
            LicenseRecord.EditFlag = EditFlag.New;
            foreach (PurchaseRecord record in PurchaseRecords)
            {
                record.FkLicenseId = LicenseRecord.Id;
                record.Id = Guid.NewGuid();
                record.EditFlag = EditFlag.New;
            }
            foreach (DeviceRecord record in DeviceRecords)
            {
                record.FkLicenseId = LicenseRecord.Id;
                record.Id = Guid.NewGuid();
                record.EditFlag = EditFlag.New;
            }
            foreach (InteractivityRecord record in InteractivityRecords)
            {
                record.FkLicenseId = LicenseRecord.Id;
                record.Id = Guid.NewGuid();
                record.EditFlag = EditFlag.New;
            }
        }

        /// <summary>
        /// Set the IDs for a modified UserInfo to be updated in the Dbo.
        /// </summary>
        /// <param name="licenseId">The top level license ID, typically from LicenseRecord.Id</param>
        /// <param name="mayBeNew">true to make this an upsert, false to fail if the license ID is not found in the DBO</param>
        /// <remarks>note: will not delete records that are missing, that have vanished from DBO</remarks>
        /// <returns>Update success (if mayBeNew then returns false if a new UserInfo was created)</returns>
        public bool SetIdsAllModified(Guid licenseId, bool mayBeNew)
        {
            // find the old/existing UserInfo in the DBO
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetById(licenseId); 
            if (userInfos == null || userInfos.Count < 1 || !userInfos.First().LicenseRecord.Id.Equals(licenseId))
            {
                if (mayBeNew)
                {
                    SetIdsAllNew();
                }
                return false;
            }
            // Adjust Id, EditFlag, and FkLicenseId in all records 
            LicenseRecord.Id = licenseId;
            foreach (PurchaseRecord thisRecord in PurchaseRecords)
            {
                thisRecord.EditFlag = EditFlag.New;
                foreach (PurchaseRecord oldRecord in userInfos[0].PurchaseRecords)
                {
                    if(oldRecord.Id == thisRecord.Id)
                    {
                        thisRecord.EditFlag = EditFlag.Modified;
                    }
                }
                thisRecord.FkLicenseId = licenseId;
                if (thisRecord.EditFlag == EditFlag.New)
                {
                    thisRecord.Id = Guid.NewGuid();
                }
            }
            foreach (DeviceRecord thisRecord in DeviceRecords)
            {
                thisRecord.EditFlag = EditFlag.New;
                foreach (DeviceRecord oldRecord in userInfos[0].DeviceRecords)
                {
                    if (oldRecord.Id == thisRecord.Id)
                    {
                        thisRecord.EditFlag = EditFlag.Modified;
                    }
                }
                thisRecord.FkLicenseId = licenseId;
                if (thisRecord.EditFlag == EditFlag.New)
                {
                    thisRecord.Id = Guid.NewGuid();
                }
            }
            foreach (InteractivityRecord thisRecord in InteractivityRecords)
            {
                thisRecord.EditFlag = EditFlag.New;
                foreach (InteractivityRecord oldRecord in userInfos[0].InteractivityRecords)
                {
                    if (oldRecord.Id == thisRecord.Id)
                    {
                        thisRecord.EditFlag = EditFlag.Modified;
                    }
                }
                thisRecord.FkLicenseId = licenseId;
                if (thisRecord.EditFlag == EditFlag.New)
                {
                    thisRecord.Id = Guid.NewGuid();
                }
            }
            return true;
        }

        /// <summary>
        /// Find a particular interactivity record by ClientKind
        /// </summary>
        /// <param name="clientKind">to look for</param>
        /// <param name="createIfNecessary">true to create it if it doesn't exist</param>
        /// <returns>the found or created record, else null</returns>
        public InteractivityRecord GetInteractivityByKind(InteractivityKind clientKind, bool createIfNecessary)
        {
            InteractivityRecord interactivityRecord = null;
            foreach (InteractivityRecord interRecord in InteractivityRecords)
            {
                if (interRecord.InteractivityKind == clientKind)
                {
                    interactivityRecord = interRecord;
                    break;
                }
            }
            if (interactivityRecord == null && createIfNecessary)
            {
                interactivityRecord = new InteractivityRecord();
                InteractivityRecords.Add(interactivityRecord);
            }
            if (interactivityRecord != null)
            {
                interactivityRecord.InteractivityKind = clientKind;
            }
            return interactivityRecord;
        }

        /// <summary>
        /// Find a particular device record by SiteId
        /// </summary>
        /// <param name="clientKind">to look for</param>
        /// <param name="createIfNecessary">true to create it if it doesn't exist</param>
        /// <returns>the found or created record, else null</returns>
        public DeviceRecord GetDeviceBySiteId(string siteId, bool createIfNecessary)
        {
            siteId = siteId.Trim();
            DeviceRecord deviceRecord = null;
            foreach (DeviceRecord devRecord in DeviceRecords)
            {
                if (devRecord.DeviceSiteId == siteId)
                {
                    deviceRecord = devRecord;
                    break;
                }
            }
            if (deviceRecord == null && createIfNecessary)
            {
                deviceRecord = new DeviceRecord();
                DeviceRecords.Add(deviceRecord);
            }
            if (deviceRecord != null)
            {
                deviceRecord.DeviceSiteId = siteId;
            }
            return deviceRecord;
        }

        /// <summary>
        /// Return purchase record indicated by designator; if not found replace the record and put the history in an interactiviry record.
        /// </summary>
        /// <param name="designator">to look for</param>
        /// <param name="overwriteIfNecessary">true to overwrite it if it doesn't exist</param>
        /// <returns>the found or created record, else null</returns>
        public PurchaseRecord GetPurchaseRecord(string designator, bool overwriteIfNecessary)
        {
            designator = designator.Trim();
            PurchaseRecord purchaseRecord = null;
            // should never be more than one purchase record
            foreach (PurchaseRecord purchRecord in PurchaseRecords)
            {
                if (purchRecord.PurchaseDesignator == designator)
                {
                    if (overwriteIfNecessary)
                    {
                        InteractivityRecord interactivityRecord = GetInteractivityByKind(InteractivityKind.PurchaseHistory, true);
                        interactivityRecord.InteractivityKind = InteractivityKind.PurchaseHistory;
                        interactivityRecord.ClientInfo = "Purchase History Tracking";
                        interactivityRecord.Conversation = "Overwrite old purchase record: " + purchaseRecord.ToString();
                    }
                    purchaseRecord = purchRecord;
                    break;
                }
            }
            if (purchaseRecord == null && overwriteIfNecessary)
            {
                purchaseRecord = new PurchaseRecord();
                PurchaseRecords.Add(purchaseRecord);
            }
            if (purchaseRecord != null)
            {
                purchaseRecord.PurchaseDesignator = designator;
            }
            return purchaseRecord;
        }

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("UserInfo> ");
            if (licenseRecord == null)
            {
                builder.Append("(no LicenseRecord)");
            }
            else
            {
                builder.Append("\n  " + licenseRecord.ToString());
            }
            foreach (PurchaseRecord record in purchaseRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            foreach (DeviceRecord record in deviceRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            foreach (InteractivityRecord record in interactivityRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            return builder.ToString();
        }

    }

}
