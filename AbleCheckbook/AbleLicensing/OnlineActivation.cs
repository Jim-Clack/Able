using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AbleLicensing
{
    /// <summary>
    /// Web service API states (both requests and responses, not really a state any more)
    /// </summary>
    public enum ApiState
    {
        Unknown = 0,
        // Request that passes minimal info and does not update DB
        LookupLicense = 2,           // Find my info by license code
        // Requests that expect info, as is known, to be populated
        RegisterLicense = 5,         // May alter/update licenseCode in returned UserInfo
        UpdateInfo = 6,              // Change addr, phone, email, etc
        ChangeFeature = 7,           // Change the feature mask
        ChangeLevel = 8,             // Change permission level (and licenseCode punctuation))
        AddlDevice = 9,              // Activate add'l device on same license, no charge
        // Requests that license host devices
        MakePurchase = 11,           // Complete the purchase
        // Successful Non-Purchase Responses
        ReturnOk = 20,               // Completed non-purchase okay 
        ReturnOkAddlDev = 21,        // Purchase ok, no charge, existing lic, return PinNumber
        ReturnNotActivated = 22,     // Not activated, no paid license found, return Message
        ReturnDeactivate = 23,       // Too many devices, deactivate, return Message
        // Failed Non-Purchase Responses
        ReturnBadArg = 31,           // Invalid city, phone, email, etc, return Message
        ReturnNotFound = 32,         // License not found, return Message
        ReturnNotMatched = 33,       // Name or other info incorrect, return Message
        ReturnLCodeTaken = 34,       // License code already in use by a different user
        ReturnError = 35,            // Internal error, typically all similar license codes in use
        ReturnDenied = 36,           // Caller does not have permission, return Message
        ReturnTimeout = 37,          // Set by Activation when the ws call times out
        // Purchase Responses
        PurchaseOk = 50,             // Purchase went thru, return PinNumber, new LicCode
        PurchaseOkUpgrade = 51,      // Purchase ok, upgrade existing, return PinNumber, new LicCode
        PurchaseFailed = 52,         // Purchase failed, return Message, LicCode
        PurchaseIncomplete = 53,     // Purchase went thru but something else failed, return Message
    }

    public class OnlineActivation
    {
        /// <summary>
        /// Test connection timeout in millis.
        /// </summary>
        private const int TestTimeout = 4000;

        /// <summary>
        /// Poll timeout in millis.
        /// </summary>
        private const int PollTimeout = 8000;

        /// <summary>
        /// Web service call timeout in millis.
        /// </summary>
        private const int DbAccessTimeout = 20000;

        /// <summary>
        /// Singleton instancem, null initially.
        /// </summary>
        private static OnlineActivation instance = null;

        /// <summary>
        /// Each activation attempt will increase the timeout allowances.
        /// </summary>
        private int addlTimeout = 0;

        /// <summary>
        /// Singleton - get our one and only instance.
        /// </summary>
        public static OnlineActivation Instance
        {
            get
            {
                Console.Out.WriteLine("Ctor");
                if (instance == null)
                {
                    instance = new OnlineActivation();
                }
                return instance;
            }
        }

        /// <summary>
        /// Default Ctor.
        /// </summary>
        private OnlineActivation()
        {
        }

        /// <summary>
        /// Adjust timeout allowance on web service calls.
        /// </summary>
        /// <param name="increase">true to cumulatively increase timeouts by 2 seconds, false to reset to initial timeouts</param>
        public void AdjustTimeout(bool increase)
        {
            if(increase)
            {
                addlTimeout += 2000;
                return;
            }
            addlTimeout = 0;
        }

        public bool TestConnection()
        {
            Activation.Instance.LoggerHook("-----------------");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Activation.Instance.WsUrlOverride);
                request.Timeout = TestTimeout + addlTimeout;
                request.Method = "GET";
                request.Accept = "application/json";
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                Activation.Instance.LoggerHook(response);
                responseReader.Close();
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook(e.Message);
            }
            return true;
        }

        /// <summary>
        /// Poll.
        /// </summary>
        /// <param name="licenseCode"></param>
        /// <param name="siteId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns>user info response, populated with user info if licensed</returns>
        public UserInfoResponse Poll(string licenseCode, string siteId, int majorVersion, int minorVersion)
        {
            Activation.Instance.LoggerHook("-----------------");
            UserInfoResponse userInfoResponse = null;
            try
            {
                string url = Activation.Instance.WsUrlOverride +
                    "/poll/" + licenseCode.Trim() + "/" + siteId.Trim() + "/" + majorVersion + "-" + minorVersion + "/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = PollTimeout + addlTimeout;
                request.Method = "GET";
                request.Accept = "application/json";
                //request.ContentType = ""
                //request.ContentLength = DATA.Length;
                Activation.Instance.LoggerHook("XXX 1 " + url);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Activation.Instance.LoggerHook("XXX 2 ");
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        Activation.Instance.LoggerHook("XXX 3 " + json);
                        userInfoResponse = (UserInfoResponse)JsonSerializer.Deserialize(json, typeof(UserInfoResponse));
                        Activation.Instance.LoggerHook("XXX 4 " + userInfoResponse.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook(e.Message);
            }
            Activation.Instance.LicenseCode = licenseCode;
            if (userInfoResponse != null)
            {
                // TODO
                // update or delete device per siteId
                // update or delete license per punct
                return userInfoResponse;
                
            }
            userInfoResponse = new UserInfoResponse();
            userInfoResponse.ApiState = (int)ApiState.ReturnNotFound;
            userInfoResponse.PinNumber = "";
            userInfoResponse.Message = "Not Found";
            userInfoResponse.UserInfos = new List<UserInfo>();
            return userInfoResponse;
        }

        public void Poll()
        {

        }

        public bool IsAlreadyLicensed()
        {

            return true;
        }

        public bool Register()
        {

            return true;
        }

        public string Purchase()
        {

            return null;
        }

        //////////////////////////////// Support /////////////////////////////
        
   }
}
