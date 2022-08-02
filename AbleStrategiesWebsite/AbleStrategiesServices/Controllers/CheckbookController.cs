using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AbleLicensing;
using AbleStrategiesServices.Support;
using Microsoft.AspNetCore.Mvc;

/// <summary>                                      CHECKBOOK
/// https://domain:port/as/checkbook
/// </summary>
namespace AbleStrategiesServices.Controllers
{
    /// <summary>
    /// These APIs only to be called from an end-user Able Checkbook client regarding its own license.
    /// </summary>
    [Route("as/[controller]")]
    [ApiController]
    public class CheckbookController : ControllerBase
    {

        /////////////////////////////// APIs /////////////////////////////////

        // GET as/checkbook
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

        // WRONG - MAY WORK OKAY, BUT NEEDS TO BE REFACTORED
        // GET as/checkbook/xxxx
        /// <summary>
        /// Return user license info.
        /// </summary>
        /// <param name="licenseCode">expected license code (will be verified - must be correct)</param>
        /// <returns>List of 0-1 user info records, empty if not found, null if not verified</returns>
        [HttpGet("{licenseCode}")]
        public JsonResult Get([FromRoute] string licenseCode)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            UserInfo[] userInfos = ApiSupport.GetUserInfoBy(ipAddress, "license", licenseCode, licenseCode, false);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnOk, userInfos.ToList()));
        }

        // PUT as/checkbook/lll/sss (upload file, content from http body) text/plain
        /// <summary>
        /// Upload log file content
        /// </summary>
        /// <param name="lCode">license code</param>
        /// <param name="siteId">site/device code</param>
        /// <returns>success</returns>
        /// <remarks>Body of request contain the log text to be uploaded</remarks>
        [HttpPut("{lCode}/{siteId}")]
        public bool Put([FromRoute] string lCode, [FromRoute] string siteId /*, [FromBody] string logContent */)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (lCode.Length < 1 || siteId.Length < 1)
            {
                Logger.Warn(ipAddress, "Failed request to upload logfile");
                return false;
            }
            // filename uses id fields, but replacing non-alphanumerics with a hyphen
            string filePath = Path.GetFullPath(ApiSupport.UploadPath + 
                Regex.Replace(lCode + siteId, "[^A-Za-z0-9]", "-") + "-" + DateTime.Now.Ticks + ".log");
            Logger.Diag(ipAddress, "Uploading " + filePath);
            try
            {
                Directory.CreateDirectory(ApiSupport.UploadPath);
                System.IO.File.Delete(filePath);
                FileStream outFile = new FileStream(filePath, FileMode.Create);
                HttpContext.Request.Body.CopyTo(outFile);
                outFile.Close();
            }
            catch (IOException ex)
            {
                Logger.Error(ipAddress, "Problem uploading " + filePath, ex);
                return false;
            }
            PurgeOldUploads();
            return true;
        }

        // POST as/checkbook/1?name=nnn&addr=aaa&city=ccc&zip=zzz&phone=ppp&email=eee&feature=fff&lCode=lll&purchase=ppp
        /// <summary>
        /// Process an activation state.
        /// </summary>
        /// <param name="apiState">From ApiState enum in Activation</param>
        /// <param name="name">installation contact name</param>
        /// <param name="addr">installation street address</param>
        /// <param name="city">installation city</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased ("" ok, if unchanged)</param>
        /// <param name="lCode">installation assigned license code ("" ok, if new)</param>
        /// <param name="purchase">validation(???) code from the purchase ("" ok, if this is not a purchase)</param>
        /// <returns>UserInfoResponse with populated PIN, PinNumber = "" on error - see Message</returns>
        /// <returns>Results of the attempt to purchase, activate, or lookup</returns>
        /// <remarks>
        /// * Caution: Multiple devices per user license, each with their own site id, may be returned. 
        ///   (caller must search for the correct one per the site id)
        /// </remarks>
        [HttpPost("{apiState}")]
        public JsonResult Post([FromRoute] int apiState, 
            [FromQuery] string name, [FromQuery] string addr, [FromQuery] string city, [FromQuery] string zip, [FromQuery] string phone, 
            [FromQuery] string email, [FromQuery] string feature, [FromQuery] string lCode, [FromQuery] string purchase)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            UserInfo userInfo = new UserInfo(name, addr, city, zip, phone, email);
            userInfo.LicenseRecord.LicenseFeatures = feature;
            userInfo.LicenseRecord.LicenseCode = lCode;
            // validate args
            if (apiState >= (int)AbleLicensing.ApiState.UpdateInfo && 
                (apiState >= (int)AbleLicensing.ApiState.ReturnOk || name.Length < 3 || addr.Length < 5 || city.Length < 3 || 
                zip.Length < 5 || phone.Length < 10 || email.Length < 9 || !email.Contains('@') || !email.Contains('.')))
            {
                Logger.Warn(ipAddress, "Invalid arg for state " + apiState + " [" + HttpContext.Request.QueryString + "]");
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnBadArg, null, 
                    "Invalid name, address, city, zip, phone, or email"));
            }
            // get existing user info, if any
            UserInfo existingUserInfo = null;
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(lCode); 
            if(userInfos.Count > 0)
            {
                existingUserInfo = userInfos.First();
            }
            else if(apiState >= (int)AbleLicensing.ApiState.UpdateInfo)
            {
                Logger.Warn(ipAddress, "ProcessLicense " + apiState + " called but license code not found in DB");
                return ApiSupport.AsJsonResult(
                    new UserInfoResponse((int)ApiState.ReturnNotMatched, null, "Cannot find specified license"));
            }
            // handle the API state
            switch (apiState)
            {
                case (int)ApiState.FuzzyLookup:
                    return FuzzyLookup(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.LookupLicense:
                    return LookupLicense(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.UpdateInfo:
                    return UpdateInfo(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.ChangeFeature:
                    return ChangeFeature(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.ChangeLevel:
                    return ChangeLevel(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.MakePurchase:
                    return MakePurchase(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.AddlDevice:
                    return AddlDevice(ipAddress, userInfo, existingUserInfo, purchase);
                default:
                    Logger.Warn(ipAddress, "ProcessLicense called with missing or incorrect arg [" + (int)apiState + "}");
                    return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnBadArg, null, 
                        "Invalid API state passed to call [" + apiState + "]"));
            }
        }

        ////////////////////////////// Support ///////////////////////////////

        /// <summary>
        /// If there are too many uploaded files, delete the old ones
        /// </summary>
        private void PurgeOldUploads()
        {
            IEnumerable<string> enumerator = Directory.EnumerateFiles(ApiSupport.UploadPath);
            long totalDaysOld = 0;
            int numFiles = 0;
            foreach (string filePath in enumerator)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                totalDaysOld += Math.Abs((DateTime.Now - fileInfo.CreationTime).Duration().Days);
                ++numFiles;
            }
            if (numFiles < 5)
            {
                return;
            }
            long numDaysToKeep = Math.Max(4, totalDaysOld / numFiles);
            enumerator = Directory.EnumerateFiles(ApiSupport.UploadPath);
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

        /// <summary>
        /// Handle the API call to FuzzyLookup.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult FuzzyLookup(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to LookupLicense.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult LookupLicense(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to UpdateInfo.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult UpdateInfo(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to ChangeFeature.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult ChangeFeature(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to ChangeLevel.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult ChangeLevel(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to MakePurchase.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult MakePurchase(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to AddlDevice.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult AddlDevice(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            Logger.Diag(ipAddress, "???");
            return ApiSupport.AsJsonResult(null);
        }

    }

}