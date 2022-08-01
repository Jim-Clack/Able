using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AbleStrategiesServices.Support;
using System.Globalization;

namespace AbleStrategiesServices.Support
{
    public static class ApiSupport
    {
        /// <summary>
        /// Return licenses by various lookups.
        /// </summary>
        /// <param name="ipAddress">Remote IP Address</param>
        /// <param name="by">literal: id, license, site, name, address, city, phone, email, interactivity, daterange</param>
        /// <param name="pattern">Lookup value, often a regular expression (for daterange it must include a "->")</param>
        /// <param name="licenseCode">null to find all matches, else expected license code</param>
        /// <param name="withPurchAndInter">true to also add purchase and interactivity records</param>
        /// <remarks>For dates, if the date is missing or improperly formatted, the call defaults to "the past 60 days"</remarks>
        /// <returns>List of matching licenses, null if not verified</returns>
        public static UserInfo[] GetUserInfoBy(string ipAddress, string by, string pattern, string licenseCode, bool withPurchAndInter)
        {
            UserInfo[] userInfos = null;
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
                    !userInfos[0].LicenseRecord.Id.Equals(licenseCode.Trim()))
                {
                    return null;
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
        /// <param name="userInfo">To be represented as JSON, null okay</param>
        /// <returns>The JsonResult</returns>
        public static JsonResult AsJsonResult(UserInfo[] userInfo)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.MaxDepth = 20;
            settings.NullValueHandling = NullValueHandling.Include;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.CheckAdditionalContent = true;
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            settings.Culture = new CultureInfo("en-US");
            return new JsonResult(userInfo, settings);
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
                UserInfoDbo.Instance.UpdateListWithPurchAndInter(userInfo);
            }
            return userInfo.ToArray();
        }

    }

}
