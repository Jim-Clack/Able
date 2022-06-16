using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using AbleStrategiesServices.Support;
using Newtonsoft.Json;

/// <summary>                                      MASTER
/// https://domain:port/as/master
/// </summary>
namespace AbleStrategiesServices.Controllers
{
    [Route("as/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        // GET as/master
        /// <summary>
        /// Simple APi to verify the connection.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            Logger.Diag(ipAddress, "Get API Called");
            return new string[] {
                AbleStrategiesServices.Support.Version.ToString(), 
                now.ToString("o", CultureInfo.GetCultureInfo("en-US")),
                ipAddress,
                (DateTime.Now.Ticks / (DateTime.Now.Millisecond + 173L)).ToString(), // future
                "X" // future use
            };
        }

        // GET as/master/license?pattern=.*
        /// <summary>
        /// Return licenses by various lookups.
        /// </summary>
        /// <param name="by">literal: id, license, site, name, address, city, phone, email, interactivity, daterange</param>
        /// <param name="pattern">Lookup value, often a regular expression (for daterange it must include a "->" delimiter)</param>
        /// <remarks>For dates, if the date is missing or improperly formatted, the call defaults to "the past 60 days"</remarks>
        /// <returns>List of matching licenses, null if not authorized</returns>
        [HttpGet("{by}")]
        public JsonResult Get([FromRoute] string by, [FromQuery]string pattern)
        {
            UserInfo[] userInfo = null;
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            by = by.Trim().ToLower();
            if (SupportMethods.HasWildcard(pattern) && !Configuration.Instance.IsSuperSuperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress, "Attempted unauthorized access [" + pattern + "]");
                return null;
            }
            try
            {
                Logger.Diag(ipAddress, "Lookup " + by + " [" + pattern + "]");
                userInfo = LookupUserInfo(by, pattern);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Failed to process", ex);
            }
            return AsJsonResult(userInfo);
        }

        /*
         TODO
          - Remove EditFlag from JSON output !!!
         */

        // POST as/master
        /// <summary>
        /// Create a new user info.
        /// </summary>
        /// <remarks>Client request header: Content-Type: application/json; charset=UTF-8</remarks>
        /// <remarks>Unlike Put, this (Post) method ignores the ID of each record.</remarks>
        /// <param name="value">Populated JSON representing the UserInfo (FkLiceseId and Id are ignored)</param>
        /// <returns>the ID of the license record (not the license code), "" on failure or if the licenseCode already exists</returns>
        [HttpPost]
        public string Post([FromBody] UserInfo userInfo)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (!Configuration.Instance.IsSuperSuperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress, "Attempted unauthorized access");
                return "";
            }
            userInfo.SetIdsAllNew();
            Logger.Diag(ipAddress, "Post - insert new UserInfo...\n" + userInfo.ToString());
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(userInfo.LicenseRecord.LicenseCode.Trim());
            if(userInfos != null && userInfos.Count != 1)
            {
                Logger.Warn(null, "License Code already exists");
                return "";
            }
            try
            {
                UserInfoDbo.Instance.Update(userInfo);
            }
            catch(Exception ex)
            {
                Logger.Warn(null, "Problem writing to DB " + ex);
                return "";
            }
            return userInfo.LicenseRecord.Id.ToString();
        }

        // PUT as/master/5
        /// <summary>
        /// Update an existing user info, given the ID (not the license code)
        /// </summary>
        /// <param name="id">the license record ID (not the license code)</param>
        /// <param name="value">Populated JSON UserInfo (FkLiceseId is ignored, Id is significant)</param>
        /// <remarks>Unlike Post, this (Put) method requires that the ID of each record be correct.</remarks>
        /// <returns>the ID of the license record (not the license code)</returns>
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] UserInfo userInfo)
        {


            return "";
        }

        // DELETE as/master/5
        /// <summary>
        /// Delete a user info, given the ID (not the license code)
        /// </summary>
        /// <param name="id">the license record ID (not the license code)</param>
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            UserInfo[] userInfo = null;
            int recordCount = 0;
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            Guid guid = new Guid(id.Trim());
            try
            {
                Logger.Diag(ipAddress, "Delete " + id + "]");
                userInfo = LookupUserInfo("id", id);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Failed to process", ex);
            }
            if(userInfo != null && userInfo.Length > 0)
            {
                UserInfoBuilder builder = new UserInfoBuilder(userInfo[0]);
                recordCount = UserInfoDbo.Instance.Delete(userInfo[0]);
            }
            Logger.Warn(ipAddress, "Deleted " + recordCount + " Records from DB, ID=" + userInfo[0].LicenseRecord.Id.ToString());
        }

        /// <summary>
        /// Converft a JSON string to a UserInfo object.
        /// </summary>
        /// <param name="value">JSON string</param>
        /// <returns>The user info object, possibly null on error</returns>
        private static UserInfo JsonToUserInfo(string value)
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
        private JsonResult AsJsonResult(UserInfo[] userInfo)
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
        /// <param name="by">literal: id, license, site, name, address, city, phone, email, interactivity, daterange</param>
        /// <param name="pattern">Lookup value, often a regular expression (for daterange it must include a hyphen)</param>
        /// <returns>Array of matching UserInfos, null if not authorized</returns>
        /// <remarks>For dates, if the date is missing or improperly formatted, the call defaults to "the past 60 days"</remarks>
        private UserInfo[] LookupUserInfo(string by, string pattern)
        {
            List<UserInfo> userInfo = null;
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            switch (by)
            {
                case "id":
                    userInfo = UserInfoDbo.Instance.GetById(new Guid(pattern));
                    break;
                case "license":
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
                case "daterange":
                    DateTime fromDate = DateTime.Now.AddDays(-60);
                    DateTime thruDate = DateTime.Now;
                    string[] dateStrings = pattern.Trim().Split("->");
                    if(dateStrings.Length == 2)
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
            return userInfo.ToArray();
        }

    }

}
