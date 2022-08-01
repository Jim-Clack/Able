using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        // GET as/checkbook/XXXX
        /// <summary>
        /// Return user license info.
        /// </summary>
        /// <param name="licenseCode">expected license code (will be verified - must be correct)</param>
        /// <returns>List of 0-1 user info records, empty if not found, null if not verified</returns>
        [HttpGet("{licenseCode}")]
        public JsonResult Get([FromRoute] string licenseCode)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            return ApiSupport.AsJsonResult(ApiSupport.GetUserInfoBy(ipAddress, "license", licenseCode, licenseCode, false));
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
        /// <param name="feature">installation edition/features/etc to be purchased ("" if new or unchanged)</param>
        /// <param name="lCode">installation assigned license code ("" if unknown)</param>
        /// <param name="purchase">validation code from the purchase ("" if unknown)</param>
        /// <returns>UserInfo with populated PIN, PinNumber = "" on error - see Message</returns>
        /// <returns>Results of the attempt to activate</returns>
        /// <remarks>
        /// * Caution: Multiple devices, each with their own site id, may be returned. (caller must search for the correct one)
        /// * If both lCode and purchase are passed in as null, it will trigger a PayPal purchase.
        ///   If either are passed in as null, the system will first attempt to find the transaction
        ///   and then, only if it cannot, will it trigger a PayPal purchase.
        /// </remarks>
        [HttpPost("{apiState}")]
        public JsonResult ProcessLicense([FromRoute] int apiState, 
            [FromQuery] string name, [FromQuery] string addr, [FromQuery] string city, [FromQuery] string zip, [FromQuery] string phone, 
            [FromQuery] string email, [FromQuery] string feature, [FromQuery] string lCode, [FromQuery] string purchase)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            UserInfo userInfo = new UserInfo(name, addr, city, zip, phone, email);
            userInfo.LicenseRecord.LicenseFeatures = feature;
            userInfo.LicenseRecord.LicenseCode = lCode;
            // validate args
            if (apiState >= (int)AbleLicensing.ApiState.UpdateInfo && (apiState >= (int)AbleLicensing.ApiState.ReturnOk ||
                name.Length < 3 || addr.Length < 5 || city.Length < 3 || zip.Length < 5 ||
                phone.Length < 10 || email.Length < 9 || !email.Contains('@') || !email.Contains('.')))
            {
                return ReturnError(ipAddress, ApiState.ReturnBadArg, userInfo,
                    "ProcessLicense called with missing or incorrect arg", "Invalid state, name, address, city, zip, phone, or email");
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
                return ReturnError(ipAddress, ApiState.ReturnNotMatched, userInfo,
                    "ProcessLicense " + apiState + " called but license code not found in DB", "Cannot find specified license in DB");
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
                    return ReturnError(ipAddress, ApiState.ReturnBadArg, userInfo,
                        "ProcessLicense called with missing or incorrect arg [" + (int)apiState + "}", "Invalid API state passed to call [" + apiState + "]");
            }
        }

        ////////////////////////// API Call Handlers /////////////////////////
        
        /// <summary>
        /// Return an error to the caller.
        /// </summary>
        /// <param name="ipAddress">textual host id</param>
        /// <param name="apiState">error return code</param>
        /// <param name="userInfo">populated from args</param>
        /// <param name="logMsg">message to be logged</param>
        /// <param name="ErrorMessage">message to be returned</param>
        /// <returns></returns>
        private JsonResult ReturnError(string ipAddress, ApiState apiState, UserInfo userInfo, string logMsg, string errorMessage)
        {
            Logger.Warn(ipAddress, logMsg);
            userInfo.ApiState = (int)apiState;
            userInfo.Message = errorMessage;
            return ApiSupport.AsJsonResult(new UserInfo[] { userInfo });
        }

        /// <summary>
        /// Handle the API call to FuzzyLookup.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult FuzzyLookup(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to LookupLicense.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult LookupLicense(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to UpdateInfo.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult UpdateInfo(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to ChangeFeature.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult ChangeFeature(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to ChangeLevel.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult ChangeLevel(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to MakePurchase.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult MakePurchase(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }

        /// <summary>
        /// Handle the API call to AddlDevice.
        /// </summary>
        /// <param name="ipAddress">textual host id, for logging and security</param>
        /// <param name="userInfo">passed into the call</param>
        /// <param name="existingUserInfo">as found in the database, null if not found</param>
        /// <param name="purchase">purchase code, "" if none</param>
        /// <returns></returns>
        private JsonResult AddlDevice(string ipAddress, UserInfo userInfo, UserInfo existingUserInfo, string purchase)
        {
            // TODO
            return ApiSupport.AsJsonResult(null);
        }


    }
}