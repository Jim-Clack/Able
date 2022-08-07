using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AbleLicensing;
using AbleStrategiesServices.Support;
using Microsoft.AspNetCore.Mvc;

/// <summary>                                      CHECKBOOK
/// https://domain:port/as/checkbook
/// </summary>
namespace AbleStrategiesServices.Controllers
{
    /// <summary>
    /// These APIs only to be called from an end-user AbleCheckbook client regarding its own license.
    /// </summary>
    /// <remarks>
    /// These calls are expected to occur in the following sequence:
    ///  1. PostDb(RegisterLicense...)
    ///  2. PostDb(MakePurchase...)
    /// Other calls may occur in any sequence
    /// </remarks>
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
            string ipAddress;
            if (!ClientCallFilter.Instance.Validate(HttpContext.Connection.RemoteIpAddress, false, out ipAddress))
            {
                HttpContext.Abort();
                return null;
            }
            Logger.Diag(ipAddress, "Get API Called");
            return new string[] {
                AbleStrategiesServices.Support.Version.ToString(),
                now.ToString("o", CultureInfo.GetCultureInfo("en-US")),
                ipAddress,
                (DateTime.Now.Ticks / (DateTime.Now.Millisecond + 173L)).ToString(), // future
                "X" // future use
            };
        }

        // GET as/checkbook/user/xxxx/vv-vv
        /// <summary>
        /// Return user license info. - Polling call
        /// </summary>
        /// <param name="lCode">expected license code (will be verified - must be correct)</param>
        /// <param name="version">Major-minor - from version of client software</param>
        /// <returns>List of 0-1 user info records, empty if not found, null if not verified, message may have important info</returns>
        /// <remarks>
        /// This call is uniquely for use by unlicensed as well as licensed clients
        /// </remarks>
        [HttpGet("user/{lCode}/{version}")]
        public JsonResult GetUser([FromRoute] string lCode, [FromRoute] string version)
        {
            string ipAddress;
            if (!ClientCallFilter.Instance.Validate(HttpContext.Connection.RemoteIpAddress, false, out ipAddress))
            {
                HttpContext.Abort();
                return null;
            }
            lCode = lCode.ToUpper().Trim();
            // TODO: update ipAdress in a new table named "UniqueClients"
            UserInfo[] userInfos = ApiSupport.GetUserInfoBy(ipAddress, "license", lCode, lCode, false);
            string message = ApiSupport.GetVersionSpecificMessage(version);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnOk, userInfos.ToList(), message));
        }

        // PUT as/checkbook/lll/sss (upload file, content from http body) text/plain
        /// <summary>
        /// Upload log file content
        /// </summary>
        /// <param name="lCode">license code</param>
        /// <param name="siteId">site/device code</param>
        /// <returns>success</returns>
        /// <remarks>Body of request contain the log text to be uploaded</remarks>
        [HttpPut("log/{lCode}/{siteId}")]
        public bool PutLog([FromRoute] string lCode, [FromRoute] string siteId /*, [FromBody] string logContent */)
        {
            string ipAddress;
            if (!ClientCallFilter.Instance.Validate(HttpContext.Connection.RemoteIpAddress, false, out ipAddress))
            {
                HttpContext.Abort();
                return false;
            }
            if (lCode.Length < 1 || siteId.Length < 1)
            {
                Logger.Warn(ipAddress, "Failed request to upload logfile");
                return false;
            }
            ApiSupport.PurgeOldUploads();
            // filename uses id fields, but replaces non-alphanumerics with a hyphen
            string filePath = Path.GetFullPath(Configuration.Instance.UploadPath + 
                Regex.Replace(lCode + "-" + siteId, "[^A-Za-z0-9]", "-") + "-" + DateTime.Now.Ticks + ".log");
            Logger.Diag(ipAddress, "Uploading " + filePath);
            try
            {
                Directory.CreateDirectory(Configuration.Instance.UploadPath);
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
            return true;
        }

        // POST as/checkbook/2?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&purchase=P12345%7C67890
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
        /// <param name="lCode">installation assigned license code</param>
        /// <param name="siteId">installation host device code</param>
        /// <param name="purchase">Ptran|val codes from the purchase ("" ok, if this is not a purchase)</param>
        /// <returns>UserInfoResponse with populated PIN, PinNumber = "" on error - see Message</returns>
        /// <remarks>
        /// * Caution: Multiple devices per user license, each with their own site id, may be returned. 
        ///   (caller must search the returned device records for the correct one per the site id)
        /// </remarks>
        [HttpPost("db/{apiState}")]
        public JsonResult PostDb([FromRoute] int apiState, 
            [FromQuery] string name, [FromQuery] string addr, [FromQuery] string city, [FromQuery] string zip, [FromQuery] string phone, 
            [FromQuery] string email, [FromQuery] string feature, [FromQuery] string lCode, [FromQuery] string siteId, [FromQuery] string purchase)
        {
            string ipAddress;
            if (!ClientCallFilter.Instance.Validate(HttpContext.Connection.RemoteIpAddress, false, out ipAddress))
            {
                HttpContext.Abort();
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnDenied, null, "Denied"));
            }
            if (name == null || addr == null || city == null || zip == null || phone == null || email == null || 
                feature == null || lCode == null || siteId == null || purchase == null)
            {
                Logger.Warn(ipAddress, "All args must be specified, even if empty " + apiState + " [" + HttpContext.Request.QueryString + "]");
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnBadArg, null,
                    "Missing arg, poss internal error - contact cupport for help"));
            }
            UserInfo userInfo = new UserInfo(name, addr, city, zip, phone, email, siteId, UserLevelPunct.Standard);
            userInfo.LicenseRecord.LicenseFeatures = feature;
            userInfo.LicenseRecord.LicenseCode = lCode;
            // validate args
            if (apiState >= (int)AbleLicensing.ApiState.RegisterLicense && 
                (apiState >= (int)AbleLicensing.ApiState.ReturnOk || siteId.Length < 4 || name.Length < 3 || addr.Length < 5 || city.Length < 3 || 
                zip.Length < 5 || phone.Length < 10 || email.Length < 9 || !email.Contains('@') || !email.Contains('.')))
            {
                Logger.Warn(ipAddress, "Invalid arg for state " + apiState + " [" + HttpContext.Request.QueryString + "]");
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnBadArg, null, 
                    "Invalid name, address, city, zip, phone, or email"));
            }
            // get existing user info, if any
            UserInfo existingUserInfo = null;
            List<UserInfo> userInfos = UserInfoDbo.Instance.GetByLicenseCode(lCode); // NOTE: Future: Punctuation should match any via regex
            if(userInfos.Count > 0)
            {
                existingUserInfo = userInfos.First();
            }
            else if(apiState >= (int)AbleLicensing.ApiState.UpdateInfo && apiState != (int)AbleLicensing.ApiState.MakePurchase)
            {
                Logger.Warn(ipAddress, "PostDb " + apiState + " called, but license code not found in DB");
                return ApiSupport.AsJsonResult(
                    new UserInfoResponse((int)ApiState.ReturnNotMatched, null, "Cannot find specified license"));
            }
            // handle the API state
            switch (apiState)
            {
                case (int)ApiState.LookupLicense:
                    return LookupLicense(ipAddress, userInfo, existingUserInfo, purchase);
                case (int)ApiState.RegisterLicense:
                    return RegisterLicense(ipAddress, userInfo, existingUserInfo, purchase);
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
        /// Handle the API call to LookupLicense.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        private JsonResult LookupLicense(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            Logger.Diag(ipAddress, "LookupLicense API Call - " + userInfo);
            List<UserInfo> userInfos = new List<UserInfo>();
            bool found = false;
            if (existingUserInfo != null)
            {
                found = true;
                userInfos.Add(existingUserInfo);
            }
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)(found ? ApiState.ReturnOk : ApiState.ReturnNotFound), userInfos));
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
            Logger.Diag(ipAddress, "UpdateInfo API Call - " + userInfo);
            List<UserInfo> userInfos = new List<UserInfo>();
            bool found = false;
            if (existingUserInfo != null)
            {
                found = true;
                userInfo.LicenseRecord.Id = existingUserInfo.LicenseRecord.Id;
                userInfo.LicenseRecord.LicenseFeatures = existingUserInfo.LicenseRecord.LicenseFeatures;
                userInfo.SetIdsAllModified(userInfo.LicenseRecord.Id, true);
                userInfos.Add(userInfo);
            }
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)(found ? ApiState.ReturnOk : ApiState.ReturnNotMatched), userInfos));
        }

        /// <summary>
        /// Handle the API call to ChangeFeature.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        /// <remarks>CAUTION!!! Currently this does no confirmation, and no purchase necessary</remarks>
        private JsonResult ChangeFeature(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // See CAUTION!!! above - No big deal at this time because the PIN will be wrong, so this is unsupported
            Logger.Diag(ipAddress, "ChangeFeature API Call - " + userInfo);
            List<UserInfo> userInfos = new List<UserInfo>();
            bool found = false;
            if (existingUserInfo != null)
            {
                found = true;
                userInfo.LicenseRecord.Id = existingUserInfo.LicenseRecord.Id;
                userInfo.SetIdsAllModified(userInfo.LicenseRecord.Id, true);
                userInfos.Add(userInfo);
            }
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)(found ? ApiState.ReturnOk : ApiState.ReturnNotMatched), userInfos));
        }

        /// <summary>
        /// Handle the API call to ChangeLevel.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse</returns>
        /// <remarks>CAUTION!!!  Currently this does no confirmation, and no purchase necessary</remarks>
        private JsonResult ChangeLevel(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // See CAUTION!!! above - No big deal at this time because the PIN will be wrong, so this is unsupported
            Logger.Diag(ipAddress, "ChangeLevel API Call - " + userInfo);
            List<UserInfo> userInfos = new List<UserInfo>();
            bool found = false;
            if (existingUserInfo != null)
            {
                found = true;
                userInfo.SetIdsAllModified(userInfo.LicenseRecord.Id, true);
                userInfos.Add(userInfo);
            }
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)(found ? ApiState.ReturnOk : ApiState.ReturnNotMatched), userInfos));
        }

        /// <summary>
        /// Handle the API call to RegisterLicense.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call - contact info, lCode, and siteId are necessary</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse - NOTE: LicenseCode may be different from that passed in</returns>
        private JsonResult RegisterLicense(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            Logger.Diag(ipAddress, "RegisterLicense API Call - " + userInfo);
            string lCode = ApiSupport.CreateLicenseCodeBasedOn(userInfo.LicenseRecord.LicenseCode);
            if(string.IsNullOrEmpty(lCode))
            {
                Logger.Warn(ipAddress, "Cannot Generate License Code for " + userInfo);
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnError,
                    null, "Internal Error - Cannot Generate License Code"));
            }
            ApiSupport.AddInteractivity(userInfo, InteractivityClient.RegistrationWs, ipAddress,
                "RegisterLicense - License Code rqst: " + userInfo.LicenseRecord.LicenseCode + " - generated as: " + lCode);
            userInfo.LicenseRecord.LicenseCode = lCode;
            userInfo.LicenseRecord.LicenseFeatures = "0";
            userInfo.SetIdsAllNew();
            UserInfoDbo.Instance.Update(userInfo);
            UserInfoDbo.Instance.Sync();
            List<UserInfo> userInfos = new List<UserInfo>();
            userInfos.Add(userInfo);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnOk, userInfos));
        }

        /// <summary>
        /// Handle the API call to MakePurchase.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns>JSON rendition of UserInfoResponse with PIN populated on success</returns>
        private JsonResult MakePurchase(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            Logger.Diag(ipAddress, "MakePurchase API Call (" + purchase + ") " + userInfo);
            if (existingUserInfo == null)
            {
                Logger.Warn(ipAddress, "Cannot complete purchase for unregistered user " + userInfo);
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.PurchaseIncomplete,
                    null, "License not yet registered, poss internal error - purchase out of sequence"));
            }
            long purchAmount = 0L;
            if (!ApiSupport.VerifyPurchase(existingUserInfo, purchase, out purchAmount))
            {
                Logger.Warn(ipAddress, "Purchase not verified for (" + purchase + ") " + userInfo);
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.PurchaseFailed,
                    null, "Cannot verify purchase through pay underwriter service - contact support for help"));
            }
            string pinNumber = "";
            foreach (DeviceRecord regDeviceRecord in existingUserInfo.DeviceRecords)
            {
                foreach (DeviceRecord apiDeviceRecord in existingUserInfo.DeviceRecords)
                {
                    if (regDeviceRecord.DeviceSite.Trim().CompareTo(apiDeviceRecord.DeviceSite.Trim()) == 0)
                    {
                        pinNumber = ApiSupport.CalculatePin(existingUserInfo, regDeviceRecord.DeviceSite.Trim());
                    }
                }
            }
            if (string.IsNullOrEmpty(pinNumber))
            {
                Logger.Warn(ipAddress, "Purchase not completed, cannot calculate PIN");
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.PurchaseIncomplete,
                    null, "License code or device/site ID is corrupt - contact support for help"));
            }
            ApiSupport.AddInteractivity(userInfo, InteractivityClient.ActivationWs, ipAddress,
                "MakePurchase - License Code: " + existingUserInfo.LicenseRecord.LicenseCode + " - purchase: " + purchase);
            ApiSupport.AddPurchase(existingUserInfo, purchase, purchAmount);
            List<UserInfo> userInfos = new List<UserInfo>();
            userInfos.Add(existingUserInfo);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.PurchaseOk, userInfos, null, pinNumber));
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
            Logger.Diag(ipAddress, "AddlDevice API Call " + userInfo);
            if (existingUserInfo == null)
            {
                Logger.Warn(ipAddress, "AddlDevice failed, unlicensed " + userInfo);
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnNotActivated,
                    null, "Not a registered license, cannot add a device to unlicensed site"));
            }
            if (existingUserInfo.PurchaseRecords.Count < 1)
            {
                Logger.Warn(ipAddress, "AddlDevice failed, no purchase record " + userInfo);
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnNotActivated,
                    null, "Site registered but no license has been purchased, cannot add device"));
            }
            string pinNumber = ApiSupport.ActivateSiteGetPin(userInfo, existingUserInfo);
            if (string.IsNullOrEmpty(pinNumber))
            {
                Logger.Warn(ipAddress, "AddlDevice not completed, cannot calculate PIN");
                return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.PurchaseIncomplete,
                    null, "License code or device/site ID is corrupt - contact support for help"));
            }
            ApiSupport.AddInteractivity(userInfo, InteractivityClient.OtherWs, ipAddress,
                "AddlDevice - License Code: " + existingUserInfo.LicenseRecord.LicenseCode);
            List<UserInfo> userInfos = new List<UserInfo>();
            userInfos.Add(existingUserInfo);
            return ApiSupport.AsJsonResult(new UserInfoResponse((int)ApiState.ReturnOk, userInfos, null, pinNumber));
        }

    }

}