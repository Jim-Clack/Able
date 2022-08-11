using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// NOT USED - OBSOLETE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

namespace AbleLicensing
{

    /// <summary>
    /// Activation status to be returned
    /// </summary>
    public enum ActivationStatus
    {
        Activated = 0,        // All is well
        UnknownAsOfYet = 1,   // Do not yet have enough info or processing incomplete - not sure of the status
        NetworkProblems = 2,  // Cannot communicate with server
        NotActivated = 3,     // Unrecognized user, siteID, purchase, or other activation issue
        DeActivated = 4,      // Most latent site deactivation due to more recent activations on same purchase 
        Future1 = 5,
        Future2 = 6,
    }

    /// <summary>
    /// Data container class used to return results from AbleStrategies server calls.
    /// </summary>
    public class ActivationResults
    {

        /// <summary>
        /// Activation status
        /// </summary>
        private ActivationStatus _status = ActivationStatus.NotActivated;

        /// <summary>
        /// Assigned license code.
        /// </summary>
        private string _lCode = null;

        /// <summary>
        /// Returned PIN.
        /// </summary>
        private string _pin = null;

        /// <summary>
        /// Purchase validation code, i.e. from PayPal.
        /// </summary>
        private string _purchase = null;

        /// <summary>
        /// If PIN is null or "", this contains the reason.
        /// </summary>
        private string _errorMessage = null;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="status">Activation status</param>
        /// <param name="lCode">Assigned license code.</param>
        /// <param name="pin">Returned PIN.</param>
        /// <param name="purchase">Purchase validation code.</param>
        /// <param name="errorMessage">If PIN is null or "", this contains the reason.</param>
        public ActivationResults(ActivationStatus status, string lCode, string pin, string purchase, string errorMessage)
        {
            _status = status;
            _lCode = lCode;
            _pin = pin;
            _purchase = purchase;
            _errorMessage = errorMessage;
        }

        public string LicenseCode { get => _lCode; set => _lCode = value; }
        public string Pin { get => _pin; set => _pin = value; }
        public string Purchase { get => _purchase; set => _purchase = value; }
        public string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }
        public ActivationStatus Status { get => _status; set => _status = value; }
    }

    ///////////////////////////// Online Client //////////////////////////////

    /// <summary>
    /// Main class for this module.
    /// </summary>
    public class OnlineActivationClient
    {

        /// <summary>
        /// URL for calling Able Checkbook web services
        /// </summary>
#if DEBUG
        private string _wsUrl = "https://localhost:44363/as/checkbook";
#else
        private string _wsUrl = "http://ablestrategies.com:33333/as/checkbook";
#endif

        /// <summary>
        /// Staging area for a server error.
        /// </summary>
        private string _serverErrorMessage = null;

        /// <summary>
        /// installation site identification.
        /// </summary>
        private string _sid = null;

        /// <summary>
        /// installation user ID.
        /// </summary>
        private string _user = null;

        /// <summary>
        /// installation IP address or host name.
        /// </summary>
        private string _ip = null;

        /// <summary>
        /// Ctor for any kind of usage.
        /// </summary>
        /// <param name="sid">installation site identification</param>
        /// <param name="user">installation user ID</param>
        /// <param name="ip">installation IP address or host name</param>
        public OnlineActivationClient(string sid, string user, string ip)
        {
            _sid = sid; 
            _user = user;
            _ip = ip;
            string wsUrlOverride = Activation.Instance.SiteSettings.WsUrlOverride;
            if(!string.IsNullOrEmpty(wsUrlOverride))
            {
                _wsUrl = wsUrlOverride;
            }
        }

        /// <summary>
        /// Ctor for use when activating on this host.
        /// </summary>
        /// <param name="user">user name, null to use the environment's username</param>
        public OnlineActivationClient(string user = null)
        {
            _sid = Activation.Instance.SiteIdentification;
            _ip = System.Environment.UserDomainName;
            _user = System.Environment.UserName;
            if (user != null && user.Trim().Length > 0)
            {
                _user = user;
            }
            string wsUrlOverride = Activation.Instance.SiteSettings.WsUrlOverride;
            if (!string.IsNullOrEmpty(wsUrlOverride))
            {
                _wsUrl = wsUrlOverride;
            }
        }

        /// <summary>
        /// Ping the server about activity on this licensed site, get user alert, and get activation status.
        /// </summary>
        /// <param name="lCode">installation assigned license code, possibly unknown/null</param>
        /// <param name="userAlert">populated with user alert, if one is pending.</param>
        /// <returns>ActivationStatus</returns>
        public ActivationStatus GetActivationStatus(string lCode, out string userAlert)
        {
            return CallServerToVerifyActivation(lCode, out userAlert);
        }

        /// <summary>
        /// Request a Purch, lCode, and even a PIN from the Able Strategies server. Main entry point.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="city">installation city</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="lCode">installation assigned license code, null if unknown *</param>
        /// <param name="purchase">validation code from the purchase, null if unknown *</param>
        /// <returns>Results of the attempt to activate</returns>
        /// <remarks>
        /// * addr. zip, city, phone, email are required fields and should be valid
        /// * If both lCode and purchase are passed in as null, it will trigger a PayPal purchase.
        ///   If either are passed in as null, the system will first attempt to find the transaction
        ///   and then, only if it cannot, will it trigger a PayPal purchase.
        /// </remarks>
        public ActivationResults OnlineActivation(string addr, string city, string zip, string phone, string email, 
            string feature, string lCode = null, string purchase = null)
        {
            // populate unknown lCode, feature, purch (reactivate, new activation, or even first activation)
            if(!CallServerForLicenseInfo(addr, city, zip, phone, email, ref feature, ref lCode, ref purchase))
            {
                return new ActivationResults(ActivationStatus.UnknownAsOfYet, lCode, null, null, _serverErrorMessage);
            }

            // no purchase found, so bring up the PayPal screen and let the user buy it
            if (string.IsNullOrEmpty(purchase) && !string.IsNullOrEmpty(lCode)) // IS THIS CORRECT ???
            {
                purchase = PurchaseOnline(addr, city, zip, phone, email, feature, lCode);
            }

            // user didn't pay, something was incorrect, or other unexpected issue
            if (purchase == null || purchase.Trim().Length < 1)
            {   
                return new ActivationResults(ActivationStatus.UnknownAsOfYet, lCode, null, null, _serverErrorMessage);
            }

            // fetch the PIN in order to activate
            string pin = CallServerForActivationPin(addr, city, zip, phone, email, feature, lCode, purchase);

            // comm error? other processing problem? somehow we got no pin.
            if (pin == null)
            {   
                return new ActivationResults(ActivationStatus.NotActivated, lCode, null, purchase, _serverErrorMessage);
            }

            // success!!!
            return new ActivationResults(ActivationStatus.Activated, lCode, pin, purchase, _serverErrorMessage);
        }

        ////////////////////////////// Purchase //////////////////////////////

        /// <summary>
        /// Bring up the "Buy" page and proceed to PayPal or whatever
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="city">installation city</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased</param>
        /// <param name="lCode">installation assigned license code, null or blank if unknown</param>
        /// <returns>purch val code, or null on error</returns>
        private string PurchaseOnline(
            string addr, string city, string zip, string phone, string email, string feature, string lCode)
        {
            _serverErrorMessage = null;  // "[I9] ..."
            string url = "https://ablestrategies.com/ablecheckbook/PushedCheckout.html?mode=basic" +
                "&sid=" + Uri.EscapeDataString(_sid) +
                "&user=" + Uri.EscapeDataString(_user) +
                "&ip=" + Uri.EscapeDataString(_ip) +
                "&addr=" + Uri.EscapeDataString(addr) +
                "&city=" + Uri.EscapeDataString(city) +
                "&zip=" + Uri.EscapeDataString(zip) +
                "&phone=" + Uri.EscapeDataString(phone) +
                "&email=" + Uri.EscapeDataString(email) +
                "&feature=" + Uri.EscapeDataString(feature) +
                "&license=" + Uri.EscapeDataString(lCode);
            Activation.Instance.LoggerHook("[--] PurchaseOnline() " + feature + " " + lCode + " " + _sid);
            Browser2Form form = new Browser2Form("Buy", url,
               "https://www.google.com/search?q=site%3Aablestrategies.com+checkbook+help+", null);
            form.Show();
            // Now call the server to see if it was succesful and to register the purchase val code.
            return CallServerToRegisterPurchase(addr, city, zip, phone, email, feature, lCode);
        }

        /////////////////////// Web Service API Calls ////////////////////////
        
        /// <summary>
        /// Verify that we can connect to the server.
        /// </summary>
        /// <returns>Success (if false, populates error message)</returns>
        public bool CallServerToCheckConnection()
        {
            _serverErrorMessage = null; // "[A0] ..."
            bool okay = false;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync(_wsUrl, HttpCompletionOption.ResponseContentRead);
                    response.Result.EnsureSuccessStatusCode();
                    if (!(response.Result.Content is object) || !
                        response.Result.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    {
                        throw new HttpRequestException("Invalid server response");
                    }
                    System.IO.Stream contentStream = response.Result.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                    string json = new StreamReader(contentStream).ReadToEnd();
                    try
                    {
                        string[] strings = JsonSerializer.Deserialize<string[]>(json);
                        _serverErrorMessage = "[A0] " + json;
                        okay = true;
                    }
                    catch (Exception ex)
                    {
                        throw new HttpRequestException("Invalid JSON in server response: " + ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                _serverErrorMessage = "[A0] Problem Connecting to Server " + ex.Message;
            }
            Activation.Instance.LoggerHook("[A0] CallServerToCheckConnection() " + okay + " " + _serverErrorMessage);
            return okay;
        }



        /// <summary>
        /// Call the server to get remaining fields.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased (may be updated)</param>
        /// <param name="lCode">installation assigned license code ("" if unknown, will be filled-in)</param>
        /// <param name="purchase">validation code from the purchase ("" if unknown, may be filled-in)</param>
        /// <returns>The PIN, or null on error</returns>
        private bool CallServerForLicenseInfo(string addr, string city, string zip, 
            string phone, string email, ref string feature, ref string lCode, ref string purchase)
        {
            _serverErrorMessage = null; // "[A1] ..."
            bool okay = false;
            string pin = "";


            Activation.Instance.LoggerHook("[A1] CallServerForLicenseInfo() " + feature + " " + lCode + " " + purchase + " " + pin);
            _serverErrorMessage =
                "[A1] ???";
            return okay;
        }

        /// <summary>
        /// Confirm that a purchase has been paid for.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc that was purchased</param>
        /// <param name="lCode">installation assigned license code</param>
        /// <returns>purch val code, or null on error</returns>
        private string CallServerToRegisterPurchase(string addr, string city, string zip,
            string phone, string email, string feature, string lCode)
        {
            _serverErrorMessage = null; // "[B2] ..."
            string purchase = "";
            bool okay = false;


            Activation.Instance.LoggerHook("[B2] CallServerToRegisterPurchase() " + feature + " " + lCode + " " + purchase);
            if (okay && purchase != null && purchase.Trim().Length > 0)
            {
                return purchase;
            }
            _serverErrorMessage =
                "[B2] Your license code is " + lCode + " - write it down. The purchase went thru but further " +
                "server communication failed, despite multiple attempts. Try again later, using the offline " +
                "method described on our website. We are very sorry, as this should not happen, but we too " +
                "are subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

        /// <summary>
        /// Call the server to get an activation PIN.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="lCode">installation assigned license code</param>
        /// <param name="purchase">validation code from the purchase</param>
        /// <returns>The PIN, or null on error</returns>
        private string CallServerForActivationPin(string addr, string city, string zip,
            string phone, string email, string feature, string lCode, string purchase)
        {
            _serverErrorMessage = null; // "[C3] ..."
            string purch = "";
            bool okay = false;


            Activation.Instance.LoggerHook("[C3] CallServerForActivationPin() " + feature + " " + lCode + " " + purchase);
            _serverErrorMessage =
                "[C3] Your license code is " + lCode + " - write it down. Purchase went thru but activation " +
                "failed. Try again later, using the offline method as described on our website. We tried " +
                "a few times and we are very sorry, as this really should not happen, but we too are " +
                "subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

        /// <summary>
        /// Check to see if this site is still activated and populate pending userAlert as well.
        /// </summary>
        /// <param name="lCode">Current license code</param>
        /// <param name="userAlert">populated with user alert, if one is pending.</param>
        /// <returns>ActivationStatus</returns>
        private ActivationStatus CallServerToVerifyActivation(string lCode, out string userAlert)
        {
            _serverErrorMessage = null; // "[D4] ..."
            ActivationStatus status = ActivationStatus.NetworkProblems;

            userAlert = "???";
            Activation.Instance.LoggerHook("[D4] CallServerToVerifyActivation() " + lCode + " " + status);
            _serverErrorMessage =
                "[D4] ...";
            return status;
        }

    }

}
