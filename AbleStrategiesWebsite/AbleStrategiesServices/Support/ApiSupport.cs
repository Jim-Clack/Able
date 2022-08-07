using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using AbleLicensing;
using System.Linq;
using System.Text.RegularExpressions;

namespace AbleStrategiesServices.Support
{
    public static class ApiSupport
    {

        /// <summary>
        /// Only one at a time.
        /// </summary>
        private static Mutex PurgeMutex = new Mutex();

        /// <summary>
        /// Only one at a time.
        /// </summary>
        private static Mutex ServerSettingsMutex = new Mutex();

        /// <summary>
        /// Return users by various lookups.
        /// </summary>
        /// <param name="ipAddress">Remote IP Address</param>
        /// <param name="by">literal: id, license, site, name, address, city, phone, email, interactivity, daterange</param>
        /// <param name="pattern">Lookup value, often a regular expression (for daterange it must include a "->")</param>
        /// <param name="licenseCode">null to find all matches, else expected license code</param>
        /// <param name="withPurchAndInter">true to also add purchase and interactivity records</param>
        /// <remarks>For dates, if the date is missing or improperly formatted, the call defaults to "the past 60 days"</remarks>
        /// <returns>List of matching licenses (0-1)</returns>
        public static UserInfo[] GetUserInfoBy(string ipAddress, string by, string pattern, string licenseCode, bool withPurchAndInter)
        {
            UserInfo[] userInfos = new UserInfo[0];
            by = by.Trim().ToLower();
            try
            {
                Logger.Diag(ipAddress, "Lookup " + by + " [" + pattern + "]");
                userInfos = LookupUserInfo(ipAddress.ToString(), by, pattern, withPurchAndInter);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Failed to process", ex);
            }
            if(licenseCode != null)
            {
                if (userInfos == null || userInfos.Length != 1 || userInfos[0].LicenseRecord == null ||
                    !userInfos[0].LicenseRecord.LicenseCode.Equals(licenseCode.Trim()))
                {
                    return new UserInfo[0];
                }
            }
            return userInfos;
        }

        /// <summary>
        /// Converft a JSON string to a UserInfo object.
        /// </summary>
        /// <param name="value">JSON string</param>
        /// <returns>The user info object, possibly null on error</returns>
        public static UserInfo JsonToUserInfo(string value)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.MaxDepth = 20;
            settings.NullValueHandling = NullValueHandling.Include;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.CheckAdditionalContent = true;
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            settings.Culture = new CultureInfo("en-US");
            UserInfo userInfo = JsonConvert.DeserializeObject(value, settings) as UserInfo;
            return userInfo;
        }

        /// <summary>
        /// Return the JsonResult representing an object.
        /// </summary>
        /// <param name="userInfoResponse">To be represented as JSON, null okay</param>
        /// <returns>The JsonResult</returns>
        public static JsonResult AsJsonResult(UserInfoResponse userInfoResponse)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.MaxDepth = 20;
            settings.NullValueHandling = NullValueHandling.Include;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.CheckAdditionalContent = true;
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            settings.Culture = new CultureInfo("en-US");
            return new JsonResult(userInfoResponse, settings);
        }

        /// <summary>
        /// Perform a lookup of UserInfo
        /// </summary>
        /// <param name="ipAddress">Remote IP Address</param>
        /// <param name="by">literal: id, license, site, name, address, city, phone, email, interactivity, daterange</param>
        /// <param name="pattern">Lookup value, often a regular expression (for daterange it must include "->")</param>
        /// <param name="withPurchAndInter">true to populate returned objects with purchase and interactivity records as well</param>
        /// <returns>Array of matching UserInfos, null if not authorized</returns>
        /// <remarks>For dates, if the date is missing or improperly formatted, the call defaults to "the past 60 days"</remarks>
        public static UserInfo[] LookupUserInfo(string ipAddress, string by, string pattern, bool withPurchAndInter)
        {
            List<UserInfo> userInfo = null;
            switch (by)
            {
                case "id":
                    userInfo = UserInfoDbo.Instance.GetById(new Guid(pattern));
                    break;
                case "license":
                case "licence":
                case "licensecode":
                    userInfo = UserInfoDbo.Instance.GetByLicenseCode(pattern);
                    break;
                case "site":
                case "siteid":
                case "sitecode":
                    userInfo = UserInfoDbo.Instance.GetBySiteCode(pattern);
                    break;
                case "name":
                case "contactname":
                    userInfo = UserInfoDbo.Instance.GetByContactName(pattern);
                    break;
                case "address":
                case "contactaddress":
                    userInfo = UserInfoDbo.Instance.GetByContactAddress(pattern);
                    break;
                case "city":
                case "contactcity":
                    userInfo = UserInfoDbo.Instance.GetByContactCity(pattern);
                    break;
                case "email":
                case "contactemail":
                    userInfo = UserInfoDbo.Instance.GetByContactEmail(pattern);
                    break;
                case "phone":
                case "contactphone":
                    userInfo = UserInfoDbo.Instance.GetByContactPhone(pattern);
                    break;
                case "interactivity":
                case "recentinteractivity":
                    DateTime startDate = DateTime.Now.AddDays(-60);
                    DateTime.TryParse(pattern, out startDate);
                    userInfo = UserInfoDbo.Instance.GetByRecentInteractivity(startDate);
                    break;
                case "originaldate":
                case "initialdate":
                case "daterange":
                    DateTime fromDate = DateTime.Now.AddDays(-60);
                    DateTime thruDate = DateTime.Now;
                    string[] dateStrings = pattern.Trim().Split("->");
                    if (dateStrings.Length == 2)
                    {
                        DateTime.TryParse(dateStrings[0].Trim(), out fromDate);
                        DateTime.TryParse(dateStrings[1].Trim(), out thruDate);
                    }
                    userInfo = UserInfoDbo.Instance.GetByOriginalDate(fromDate, thruDate);
                    break;
                default:
                    Logger.Warn(ipAddress, "Bad lookup by [" + by + "]");
                    break;
            }
            if (userInfo == null)
            {
                return null;
            }
            if (withPurchAndInter)
            {
                UserInfoDbo.Instance.PopulateWithPurchAndInteractivity(userInfo);
            }
            return userInfo.ToArray();
        }

        /// <summary>
        /// Verify that a purchase went through
        /// </summary>
        /// <param name="userInfo">The license user info</param>
        /// <param name="purchase">purchase transaction/verification, dependent on provide (PayPal begins with "P")</param>
        /// <param name="amount">Amount of purchase in smallest currency units, i.e. cents</param>
        /// <returns></returns>
        public static bool VerifyPurchase(UserInfo userInfo, string purchase, out long amount)
        {
            if(string.IsNullOrEmpty(purchase) || purchase.Length < 10 || !purchase.StartsWith("P") || purchase.IndexOf("|") < 1)
            {
                amount = 0;
                return false;
            }

            // TODO

            amount = 2000;
            return true;
        }

        /// <summary>
        /// Generate a PIN for a registered user.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="siteId">host/device id</param>
        /// <returns>activation PIN< "" on error</returns>
        public static string CalculatePin(UserInfo userInfo, string siteId)
        {
            string pin = "";
            ServerSettingsMutex.WaitOne();
            try
            {
                ServerSettings.Instance.LicenseCode = userInfo.LicenseRecord.LicenseCode;
                long features = 0L;
                long.TryParse(userInfo.LicenseRecord.LicenseFeatures, out features);
                ServerSettings.Instance.FeaturesBitMask = features;
                long checkSum = Activation.Instance.ChecksumOfString(siteId);
                pin = Activation.Instance.CalculatePin(checkSum, siteId, userInfo.LicenseRecord.LicenseCode);
            }
            finally
            {
                ServerSettingsMutex.ReleaseMutex();
            }
            return pin;
        }

        /// <summary>
        /// Activate a site, updating or adding a DeviceRecord, and return the PIN number
        /// </summary>
        /// <param name="passedUserInfo">As passed into the API</param>
        /// <param name="existingUserInfo">As registered in the DB - will be updated</param>
        /// <returns></returns>
        public static string ActivateSiteGetPin(UserInfo passedUserInfo, UserInfo existingUserInfo)
        {
            string pinNumber = "";
            // is the site/device already in the DB?
            foreach (DeviceRecord regDeviceRecord in existingUserInfo.DeviceRecords)
            {
                foreach (DeviceRecord apiDeviceRecord in passedUserInfo.DeviceRecords)
                {
                    if (regDeviceRecord.DeviceSite.Trim().CompareTo(apiDeviceRecord.DeviceSite.Trim()) == 0)
                    {
                        pinNumber = ApiSupport.CalculatePin(existingUserInfo, regDeviceRecord.DeviceSite.Trim());
                    }
                }
            }
            // add a new site/device
            if (string.IsNullOrEmpty(pinNumber))
            {
                DeviceRecord deviceRecord = new DeviceRecord();
                deviceRecord.DeviceSite = passedUserInfo.DeviceRecords[0].DeviceSite;
                deviceRecord.UserLevelPunct = passedUserInfo.DeviceRecords[0].UserLevelPunct;
                existingUserInfo.DeviceRecords.Add(deviceRecord);
                pinNumber = ApiSupport.CalculatePin(existingUserInfo, deviceRecord.DeviceSite.Trim());
            }
            // TODO - deal with Configuration.InstanceMaxDevicesPerLicense
            return pinNumber;
        }

        /// <summary>
        /// Add an interactivity record to the user info.
        /// </summary>
        /// <param name="userInfo">To be updated</param>
        /// <param name="clientKind">kind of client</param>
        /// <param name="clientInfo">Client name, ip address, etc.</param>
        /// <param name="conversation">text - context and content</param>
        public static void AddInteractivity(UserInfo userInfo, InteractivityClient clientKind, string clientInfo, string conversation)
        {
            InteractivityRecord interactivity = new InteractivityRecord();
            interactivity.ClientInfo = clientInfo;
            interactivity.InteractivityClient = clientKind;
            interactivity.Conversation = conversation;
            userInfo.InteractivityRecords.Add(interactivity);
        }

        /// <summary>
        /// Add a purchase record to a user info
        /// </summary>
        /// <param name="userInfo">to be updated</param>
        /// <param name="purchase">authorityCharacter + transaction + "|" + validationCode, i.e. P12345678|9090909090909</param>
        /// <param name="purchAmount">amount in smallest currency units, i.e. cents</param>
        public static void AddPurchase(UserInfo userInfo, string purchase, long purchAmount)
        {
            string[] purchaseFields = purchase.Trim().Substring(1).Split("|");
            PurchaseRecord purchaseRecord = new PurchaseRecord();
            purchaseRecord.PurchaseAuthority = PurchaseAuthority.PayPalStd;
            purchaseRecord.PurchaseAmount = purchAmount;
            purchaseRecord.PurchaseDate = DateTime.Now;
            purchaseRecord.PurchaseTransaction = purchaseFields[0];
            purchaseRecord.PurchaseVerification = purchaseFields[1];
            purchaseRecord.Details = "v1";
            userInfo.PurchaseRecords.Add(purchaseRecord);
        }

        /// <summary>
        /// If there are too many uploaded files, delete the old ones
        /// </summary>
        public static void PurgeOldUploads()
        {
            PurgeMutex.WaitOne();
            try
            {
                IEnumerable<string> enumerator = Directory.EnumerateFiles(Configuration.Instance.UploadPath);
                long totalDaysOld = 0;
                int numFiles = 0;
                foreach (string filePath in enumerator)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    totalDaysOld += Math.Abs((DateTime.Now - fileInfo.CreationTime).Duration().Days);
                    ++numFiles;
                }
                if (numFiles >= 5)
                {
                    long numDaysToKeep = Math.Max(8, totalDaysOld / numFiles);
                    enumerator = Directory.EnumerateFiles(Configuration.Instance.UploadPath);
                    foreach (string filePath in enumerator)
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        int fileAge = Math.Abs((DateTime.Now - fileInfo.CreationTime).Duration().Days);
                        if (fileAge > numDaysToKeep)
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }
            finally
            {
                PurgeMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Validate the provided license code and ensure that it is not already in use.
        /// </summary>
        /// <param name="lCode">provided base license code, never null</param>
        /// <returns>generated/adjusted license code, null on error (all searched lCodes are in use)</returns>
        public static string CreateLicenseCodeBasedOn(string lCode)
        {
            lCode = Regex.Replace(lCode.Trim().ToUpper(), "[^A-Z0-9]", "X");
            List<UserInfo> userInfosTest = UserInfoDbo.Instance.GetByLicenseCode(lCode);
            while (lCode.Length < 12)
            {
                lCode = lCode + DateTime.Now.Ticks; // if too short or not provided
            }
            if (lCode.Length > 12)
            {
                lCode = lCode.Substring(0, 12);
            }
            lCode = lCode.Substring(0, 6) + (char)UserLevelPunct.Standard + lCode.Substring(7);
            string initialLCode = lCode;
            while(true)
            {
                if(UserInfoDbo.Instance.GetByLicenseCode(lCode).Count == 0)
                { 
                    break; // found a license code that is not in-use
                }
                if (SpinChar(ref lCode, 5, initialLCode))
                {
                    if (SpinChar(ref lCode, 4, initialLCode))
                    {
                        Logger.Warn(null, "Exhausted all attempts to generate a license code!");
                        return null; // give up!
                    }
                }
            }
            return lCode;
        }

        /// <summary>
        /// Spin a character in a string like an odometer digit 
        /// </summary>
        /// <param name="code">uppercase string with alphanumerics, will be modified</param>
        /// <param name="index">index to character to be spun</param>
        /// <param name="initial">the initial string, before it was spun</param>
        /// <returns>true if it rolled over to its initial value</returns>
        private static bool SpinChar(ref string code, int index, string initial)
        {
            if (char.IsDigit(initial[index]) && code[index] == '9') // 9 rolls over to 0
            {
                code = code.Substring(0, index) + "0" + code.Substring(index + 1);
            }
            else if(code.ToUpper()[index] == 'Z') // Z rolls over to A
            {
                code = code.Substring(0, index) + "A" + code.Substring(index + 1);
            }
            else
            {
                code = code.Substring(0, index) + (char)(code[index] + 1) + code.Substring(index + 1);
            }
            return (code[index] == initial[index]);
        }

        /// <summary>
        /// Get a message based on the specific client software version
        /// </summary>
        /// <param name="version">major-minor</param>
        /// <returns>message, "" if none</returns>
        public static string GetVersionSpecificMessage(string version)
        {
            string message = "";
            string filePath = Configuration.Instance + "ver-" + version + ".msg";
            if (System.IO.File.Exists(filePath))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
                message = reader.ReadToEnd();
                reader.Close();
            }
            return message;
        }

    }

}
