using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AbleLicensing
{
    public class OnlineActivation
    {

        private static OnlineActivation instance = null;

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

        private OnlineActivation()
        {
        }

        public void DoNothing()
        {
            Activation.Instance.LoggerHook("NADA");
        }

        public bool CheckConnection()
        {
            Activation.Instance.LoggerHook("-----------------");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Activation.Instance.WsUrlOverride);
                request.Method = "GET";
                //request.ContentType = ""
                //request.ContentLength = DATA.Length;
                //StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                //requestWriter.Write(DATA);
                //requestWriter.Close();
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
