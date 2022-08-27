using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AbleLicensing
{
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
            UserInfoResponse userInfoResponse = null;
            try
            {
                string url = Activation.Instance.WsUrlOverride +
                    "/poll/" + licenseCode.Trim() + "/" + siteId.Trim() + "/" + majorVersion + "-" + minorVersion + "/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = PollTimeout + addlTimeout;
                request.Method = "GET";
                request.Accept = "application/json";
                Activation.Instance.LoggerHook("Poll " + licenseCode + " " + siteId);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        userInfoResponse = (UserInfoResponse)JsonSerializer.Deserialize(json, typeof(UserInfoResponse));
                    }
                }
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook("Exception" + e.Message);
            }
            Activation.Instance.LicenseCode = licenseCode;
            if (userInfoResponse != null)
            {
                Activation.Instance.LoggerHook(userInfoResponse.ToString());
                return userInfoResponse;
            }
            Activation.Instance.LoggerHook("Response is null");
            userInfoResponse = new UserInfoResponse();
            userInfoResponse.ApiState = (int)ApiState.ReturnTimeout;
            userInfoResponse.PinNumber = "";
            userInfoResponse.Message = "Not Found";
            userInfoResponse.UserInfos = new List<UserInfo>();
            return userInfoResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiState">ApiState.LookupLicense, RegisterLicense, or MakePurchase</param>
        /// <param name="name">Contact name</param>
        /// <param name="addr">Contact address</param>
        /// <param name="city">Contact city and state</param>
        /// <param name="zip">Contact postal code</param>
        /// <param name="phone">Contact phone number</param>
        /// <param name="email">Contact Email</param>
        /// <param name="feature">License feature mask requested</param>
        /// <param name="lCode">License code including punct for user level</param>
        /// <param name="siteId">Host device code (license may be used for more than one device)</param>
        /// <param name="purchDes">Trans/Verification codes from purcahse provider</param>
        /// <returns>
        /// User license information. Note that there may be multiple devices under a license, so you will
        /// need to search the DeviceRecords for the one with the corresponding siteId. Only certain args need
        /// to be populated and only certain return fields will be correct, depending on the passed apiState.
        /// </returns>
        public UserInfoResponse DbCall(int apiState, string name, string addr, string city, 
            string zip, string phone, string email, string feature, string lCode, string siteId, string purchDes)
        {
            UserInfoResponse userInfoResponse = null;
            try
            {
                UriBuilder builder = new UriBuilder(Activation.Instance.WsUrlOverride + "/db/" + apiState);
                AppendQueryArg(builder, "name", name);
                AppendQueryArg(builder, "addr", addr);
                AppendQueryArg(builder, "city", city);
                AppendQueryArg(builder, "zip", zip);
                AppendQueryArg(builder, "phone", phone);
                AppendQueryArg(builder, "email", email);
                AppendQueryArg(builder, "feature", feature);
                AppendQueryArg(builder, "lCode", lCode);
                AppendQueryArg(builder, "siteId", siteId);
                AppendQueryArg(builder, "purchDes", purchDes);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(builder.Uri);
                request.Timeout = PollTimeout + addlTimeout;
                request.Method = "POST";
                request.Accept = "application/json";
                //request.ContentType = ""
                //request.ContentLength = DATA.Length;
                Activation.Instance.LoggerHook("DB lookup " + lCode + " " + siteId);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        userInfoResponse = (UserInfoResponse)JsonSerializer.Deserialize(json, typeof(UserInfoResponse));
                    }
                }
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook("Exception" + e.Message);
            }
            Activation.Instance.LicenseCode = lCode;
            if (userInfoResponse != null)
            {
                Activation.Instance.LoggerHook(userInfoResponse.ToString());
                return userInfoResponse;
            }
            Activation.Instance.LoggerHook("Response is null");
            userInfoResponse = new UserInfoResponse();
            userInfoResponse.ApiState = (int)ApiState.ReturnTimeout;
            userInfoResponse.PinNumber = "";
            userInfoResponse.Message = "Not Found";
            userInfoResponse.UserInfos = new List<UserInfo>();
            return userInfoResponse;
        }

        //////////////////////////////// Support /////////////////////////////

        /// <summary>
        /// Append a query to a URL
        /// </summary>
        /// <param name="builder">URL to be updated</param>
        /// <param name="argName">name of query arg</param>
        /// <param name="argValue">value of query arg</param>
        private void AppendQueryArg(UriBuilder builder, string argName, string argValue)
        {
            string queryToAppend = argName + "=" + Uri.EscapeDataString(argValue);
            // UriBuilder.Query get/set are not strict complimentary - set prepends a question mark
            if (builder.Query != null && builder.Query.Length > 1)
            {
                builder.Query = builder.Query.Substring(1) + "&" + queryToAppend;
            }
            else
            {
                builder.Query = queryToAppend;
            }
        }

        private bool PurchaseViaPaypal(ref UserInfo userInfo, string descript, long amount)
        {
            // TODO
      //      IPurchaseProvider purchaseProvider = new PayPalPurchaseProvider();
            purchaseProvider.CompletePurchase(ref userInfo, amount, descript);
            return true;
        }
        
   }
}
