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
        public LicenseRecord LicenseRecord = null; // Note: null = no license!

        /// <summary>
        /// List of PurchaseRecords.
        /// </summary>
        public List<PurchaseRecord> PurchaseRecords = new List<PurchaseRecord>();

        /// <summary>
        /// List of DeviceRecords.
        /// </summary>
        public List<DeviceRecord> DeviceRecords = new List<DeviceRecord>();

        /// <summary>
        /// List of InteractivityRecords.
        /// </summary>
        public List<InteractivityRecord> InteractivityRecords = new List<InteractivityRecord>();

        /// <summary>
        /// Find a particular interactivity record by ClientKind
        /// </summary>
        /// <param name="clientKind">to look for</param>
        /// <param name="createIfNecessary">true to create it if it doesn't exist</param>
        /// <returns>the found or created record, else null</returns>
        public InteractivityRecord GetInteractivityByClientKind(int clientKind, bool createIfNecessary)
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
            if(interactivityRecord != null)
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
                    if(overwriteIfNecessary)
                    {
                        InteractivityRecord interactivityRecord = GetInteractivityByClientKind((int)InteractivityKind.PurchaseHistory, true);
                        interactivityRecord.InteractivityKind = (int)InteractivityKind.PurchaseHistory;
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
            if (LicenseRecord == null)
            {
                builder.Append("(no LicenseRecord)");
            }
            else
            {
                builder.Append("\n  " + LicenseRecord.ToString());
            }
            foreach (PurchaseRecord record in PurchaseRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            foreach (DeviceRecord record in DeviceRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            foreach (InteractivityRecord record in InteractivityRecords)
            {
                builder.Append("\n  " + record.ToString());
            }
            return builder.ToString();
        }

    }

}
