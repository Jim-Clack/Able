using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AbleStrategiesServices.Support;
using Newtonsoft.Json;
using AbleLicensing;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>                                      MASTER
/// https://domain:port/as/master
/// </summary>
namespace AbleStrategiesServices.Controllers
{

    /// <summary>
    /// Super User API for DB maintenance and diagnostics.
    /// </summary>
    [Route("as/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        /////////////////////////////// APIs /////////////////////////////////

        // GET as/master
        /// <summary>
        /// Simple API to verify the connection.
        /// </summary>
        /// <returns>array of useful string values</returns>
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
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (SupportMethods.HasWildcard(pattern) && !Configuration.Instance.IsHyperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress.ToString(), "Attempted unauthorized access [" + pattern + "]");
                return null;
            }
            UserInfo[] userInfos = ApiSupport.GetUserInfoBy(ipAddress, by, pattern, null, true);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnOk, userInfos.ToList()));
        }

        // GET as/master/log/lll/sss/0
        /// <summary>
        /// Download a client logfile that has been uploaded via as/checkbook.
        /// </summary>
        /// <param name="lCode">license code</param>
        /// <param name="siteId">device site ID</param>
        /// <param name="countBack">0=latest, 1=previous, 2=previous to that, etc.</param>
        /// <returns>File contents, message if no more files, "???" on error</returns>
        [HttpGet("log/{lCode}/{siteId}/{countBack}")]
        public string Get([FromRoute] string lCode, [FromRoute] string siteId, [FromRoute] int countBack)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            // filename uses id fields, but replacing non-alphanumerics with a hyphen
            string filePattern = Regex.Replace(lCode + siteId, "[^A-Za-z0-9]", "-") + "-*.log";
            SortedList<string, string> filePaths = new SortedList<string, string>();
            IEnumerable<string> enumerator = Directory.EnumerateFiles(ApiSupport.UploadPath, filePattern);
            foreach (string testFilePath in enumerator)
            {
                FileInfo fileInfo = new FileInfo(testFilePath);
                filePaths.Add(testFilePath, testFilePath);
            }
            if(filePaths.Count <= countBack)
            {
                return "(No more files for " + lCode + " " + siteId + ")\n";
            }
            string filePath = filePaths.Values[filePaths.Count - (countBack + 1)];
            string fileContent = "";
            Logger.Diag(ipAddress, "Downloading " + filePath);
            try
            {
                StreamReader reader = new StreamReader(filePath);
                fileContent = reader.ReadToEnd();
                reader.Close();
            }
            catch (IOException ex)
            {
                Logger.Error(ipAddress, "Problem downloading " + filePath, ex);
                return "???";
            }
            return fileContent;
        }

        // POST as/master (JSON Body)
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
            Logger.Diag(ipAddress, "Post - insert new UserInfo...\n" + userInfo.ToString());
            if (!Configuration.Instance.IsHyperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress, "Attempted unauthorized access");
                return "";
            }
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(userInfo.LicenseRecord.LicenseCode.Trim());
            userInfo.SetIdsAllNew();
            if (userInfos != null && userInfos.Count != 1)
            {
                Logger.Warn(null, "License Code already exists, cannot create");
                return "";
            }
            try
            {
                UserInfoDbo.Instance.Update(userInfo);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Problem writing to DB " + ex);
                return "";
            }
            return userInfo.LicenseRecord.Id.ToString();
        }

        // PUT as/master (JSON Body)
        /// <summary>
        /// Update an existing user info, based on the License ID and the license code
        /// </summary>
        /// <param name="value">Populated JSON UserInfo (License Id and LicenseCode are significant)</param>
        /// <remarks>Unlike Post, this (Put) method requires that the ID of each record be correct.</remarks>
        /// <returns>the ID of the license record (not the license code)</returns>
        [HttpPut("{id}")]
        public string Put([FromBody] UserInfo userInfo)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            Logger.Diag(ipAddress, "Put - update existing UserInfo...\n" + userInfo.ToString());
            if (!Configuration.Instance.IsHyperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress, "Attempted unauthorized access");
                return "";
            }
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(userInfo.LicenseRecord.LicenseCode.Trim());
            if (userInfos == null || userInfos.Count != 1)
            {
                Logger.Warn(null, "Cannot find existing License Code. Cannot update");
                return "";
            }
            if (userInfos[0].LicenseRecord.Id != userInfo.LicenseRecord.Id)
            {
                Logger.Warn(null, "Id does not match License. Cannot update");
                return "";
            }
            try
            {
                UserInfoDbo.Instance.Update(userInfo);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Problem writing to DB " + ex);
                return "";
            }
            return userInfo.LicenseRecord.Id.ToString();
        }

        // DELETE as/master/aab54ae5-5329-4807-9878-a1b262c15bff
        /// <summary>
        /// Delete a user info, given the ID (not the license code)
        /// </summary>
        /// <param name="id">regex that matches the license record ID(s) (not the license code)</param>
        /// <returns>0 if not found, else number of records deleted</returns>
        [HttpDelete("{id}")]
        public int Delete([FromRoute] string id)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            Logger.Diag(ipAddress, "Delete - delete existing UserInfo(s)...\n" + id);
            if (!Configuration.Instance.IsHyperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(ipAddress, "Attempted unauthorized access");
                return 0;
            }
            UserInfo[] userInfos = null;
            int recordCount = 0;
            Guid guid = new Guid(id.Trim());
            try
            {
                userInfos = ApiSupport.LookupUserInfo(HttpContext.Connection.RemoteIpAddress.ToString(), "id", id, false);
            }
            catch (Exception ex)
            {
                Logger.Warn(null, "Failed to process", ex);
            }
            if(userInfos != null && userInfos.Length == 1)
            {
                UserInfoBuilder builder = new UserInfoBuilder(userInfos[0]);
                recordCount = UserInfoDbo.Instance.Delete(userInfos[0]);
            }
            else
            {
                Logger.Warn(ipAddress, "Cannot delete records from DB, no single match, ID=" + id);
                return 0;
            }
            Logger.Diag(ipAddress, "Deleted " + recordCount + " Records from DB, ID=" + userInfos[0].LicenseRecord.Id.ToString());
            return recordCount;
        }

    }

}
