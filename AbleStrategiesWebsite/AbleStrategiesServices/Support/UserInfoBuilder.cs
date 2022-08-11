using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// Usage: 
    ///   UserInfo userInfo = new UserInfoBuilder(...)
    ///       .AddPurchase(...)
    ///       .AddDevice(...)
    ///       .UpdateDevice(...)
    ///       .AddInteractivity(...)
    ///       .UserInfo;
    /// </summary>
    public class UserInfoBuilder
    {

        private UserInfo userInfo = null;

        /// <summary>
        /// Use this Ctor to create a new UserInfo.
        /// </summary>
        /// <param name="licCode">Unique record discriminator.</param>
        /// <param name="name">Contact name.</param>
        /// <param name="address">Street address</param>
        /// <param name="city">City, state/province/zip</param>
        /// <param name="phone">Contact phone</param>
        /// <param name="email">Contact email</param>
        /// <param name="features">Bitmap</param>
        public UserInfoBuilder(string licCode, string name, string address, string city, string phone, string email, string features)
        {
            this.userInfo = new UserInfo();
            this.userInfo.LicenseRecord.ContactName = name;
            this.userInfo.LicenseRecord.ContactAddress = address;
            this.userInfo.LicenseRecord.ContactCity = city;
            this.userInfo.LicenseRecord.ContactPhone = phone;
            this.userInfo.LicenseRecord.ContactEMail = email;
            this.userInfo.LicenseRecord.LicenseFeatures = features;
            this.userInfo.LicenseRecord.LicenseCode = licCode;
        }

        /// <summary>
        /// Use this Ctor to modify/update an existing UserInfo.
        /// </summary>
        /// <param name="userInfo">UserInfo to modify/update.</param>
        public UserInfoBuilder(UserInfo userInfo)
        {
            this.userInfo = userInfo;
        }

        /// <summary>
        /// Use this Ctor to modify/update an existing UserInfo.
        /// </summary>
        /// <param name="id">String representation of license code.</param>
        public UserInfoBuilder(string licCode)
        {
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(licCode.Trim());
            if(userInfos == null || userInfos.Count != 1)
            {
                this.userInfo = null;
            }
            this.userInfo = userInfos.First();
        }

        /// <summary>
        /// Is the UserInfo valid and populated?
        /// </summary>
        public bool Ok
        {
            get
            {
                return userInfo != null && userInfo.LicenseRecord != null;
            }
        }

        /// <summary>
        /// Add a new purchase.
        /// </summary>
        /// <param name="auth">Typically PayPalStd.</param>
        /// <param name="trans">Authority's transaction number.</param>
        /// <param name="verif">Authority's verification code.</param>
        /// <param name="amt">Price paid.</param>
        /// <param name="details">Related data - optional</param>
        /// <param name="date">Date of purchase.</param>
        /// <returns>The updated UserInfoBuilder</returns>
        public UserInfoBuilder AddPurchase(PurchaseAuthority auth, string trans, string verif, long amt, string details, string date)
        {
            PurchaseRecord purchaseRecord = new PurchaseRecord();
            purchaseRecord.PurchaseAuthority = PurchaseAuthority.PayPalStd;
            purchaseRecord.PurchaseTransaction = "";
            purchaseRecord.PurchaseVerification = "";
            purchaseRecord.PurchaseAmount = amt;
            purchaseRecord.Details = details;
            DateTime dateTime = DateTime.Now;
            if (DateTime.TryParse(date, out dateTime))
            {
                purchaseRecord.PurchaseDate = dateTime;
            }
            this.userInfo.PurchaseRecords.Add(purchaseRecord);
            return this;
        }

        /// <summary>
        /// Add a new device.
        /// </summary>
        /// <param name="siteId">Site ID (device) abbreviation.</param>
        /// <param name="punct">Current status of device activation.</param>
        /// <param name="codes">Binary authentication/authorization info.</param>
        /// <returns>The updated UserInfoBuilder</returns>
        public UserInfoBuilder AddDevice(string siteId, AbleLicensing.UserLevelPunct punct, string codes)
        {
            DeviceRecord deviceRecord = new DeviceRecord();
            deviceRecord.DeviceSite = siteId;
            deviceRecord.UserLevelPunct = (int)punct;
            deviceRecord.CodesAndPin = codes;
            this.userInfo.DeviceRecords.Add(deviceRecord);
            return this;
        }

        /// <summary>
        /// Add a new interactivity.
        /// </summary>
        /// <param name="client">Interactivity by phone, web service, or what?</param>
        /// <param name="clientInfo">Client name, email, and/or IP address.</param>
        /// <param name="convers">Content - what occurred during interactivity.</param>
        /// <returns>The updated UserInfoBuilder</returns>
        public UserInfoBuilder AddInteractivity(InteractivityClient client, string clientInfo, string convers)
        {
            InteractivityRecord interactivityRecord = new InteractivityRecord();
            interactivityRecord.InteractivityClient = client;
            interactivityRecord.ClientInfo = clientInfo;
            interactivityRecord.Conversation = convers;
            this.userInfo.InteractivityRecords.Add(interactivityRecord);
            return this;
        }

        /// <summary>
        /// Modify/Upsert the first purchase related to the license, insert new otherwise.
        /// </summary>
        /// <param name="auth">Typically PayPalStd.</param>
        /// <param name="trans">Authority's transaction number.</param>
        /// <param name="verif">Authority's verification code.</param>
        /// <param name="amt">Price paid.</param>
        /// <param name="details">Related data - optional</param>
        /// <param name="date">Date of purchase. null to leave unchanged</param>
        /// <returns>The updated UserInfoBuilder</returns>
        public UserInfoBuilder UpdatePurchase(PurchaseAuthority auth, string trans, string verif, long amt, string details = null, string date = null)
        {
            PurchaseRecord purchaseRecord = null;
            if (this.userInfo.PurchaseRecords.Count < 1) // if not found, create new purchase
            {
                purchaseRecord = new PurchaseRecord();
            }
            purchaseRecord = this.userInfo.PurchaseRecords[0];
            purchaseRecord.PurchaseAuthority = PurchaseAuthority.PayPalStd;
            purchaseRecord.PurchaseTransaction = "";
            purchaseRecord.PurchaseVerification = "";
            purchaseRecord.PurchaseAmount = amt;
            if (details != null)
            {
                purchaseRecord.Details = details;
            }
            if (date != null)
            {
                DateTime dateTime = DateTime.Now;
                if (DateTime.TryParse(date, out dateTime))
                {
                    purchaseRecord.PurchaseDate = dateTime;
                }
            }
            return this;
        }

        /// <summary>
        /// Modify/Upsert a device, selected by the siteId, insert new otherwise.
        /// </summary>
        /// <param name="siteId">Site ID (device) abbreviation. If found, update, else insert new device.</param>
        /// <param name="punct">Current status of device activation.</param>
        /// <param name="codes">Binary authentication/authorization info.</param>
        /// <returns>The updated UserInfoBuilder</returns>
        public UserInfoBuilder UpdateDevice(string siteId, AbleLicensing.UserLevelPunct punct, string codes)
        {
            DeviceRecord deviceRecord = null;
            foreach (DeviceRecord record in this.userInfo.DeviceRecords)
            {
                if(record.DeviceSite == siteId)
                {
                    deviceRecord = record;
                    break;
                }
            }
            if(deviceRecord == null) // if not found, create new device
            {
                deviceRecord = new DeviceRecord();
                deviceRecord.DeviceSite = siteId;
            }
            deviceRecord.UserLevelPunct = (int)punct;
            deviceRecord.CodesAndPin = codes;
            return this;
        }

        /// <summary>
        /// Fetch the resultant UserInfo.
        /// </summary>
        public UserInfo UserInfo
        {
            get
            {
                return userInfo;
            }
        }

    }

}
