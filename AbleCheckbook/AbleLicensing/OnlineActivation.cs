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
        private const int TestTimeout = 6000;

        /// <summary>
        /// Web service call timeout in millis.
        /// </summary>
        private const int CallTimeout = 30000;

        /// <summary>
        /// SIngleton instancem, null initially.
        /// </summary>
        private static OnlineActivation instance = null;

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

        public bool CheckConnection()
        {
            Activation.Instance.LoggerHook("-----------------");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Activation.Instance.WsUrlOverride);
                request.Timeout = 8000;
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
        /// <returns></returns>
        public bool Poll(string licenseCode, string siteId, int majorVersion, int minorVersion)
        {
            Activation.Instance.LoggerHook("-----------------");
            bool success = false;
            try
            {
                string url = Activation.Instance.WsUrlOverride +
                    "/poll/" + licenseCode.Trim() + "/" + siteId.Trim() + "/" + majorVersion + "-" + minorVersion + "/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 8000;
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
                        UserInfoResponse userInfoResponse = (UserInfoResponse)JsonSerializer.Deserialize(json, typeof(UserInfoResponse));
                        Activation.Instance.LoggerHook("XXX 4 " + userInfoResponse.ToString());
                    }
                }
                success = true;
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook(e.Message);
            }
            Activation.Instance.LicenseCode = licenseCode;
            if (success)
            {
                // update or delete device per siteId
                // update or delete license per punct
            }
            return true;
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
