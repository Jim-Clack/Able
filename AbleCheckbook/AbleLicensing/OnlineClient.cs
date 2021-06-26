using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing
{

    /// <summary>
    /// Activation status to be returned
    /// </summary>
    public enum ActivationStatus
    {
        Activated = 0,        // All is well
        DeActivated = 1,      // Most latent site deactivation due to more recent activation using same purchase 
        NotActivated = 2,     // Unknown user, siteID, or other activation issue
        NetworkProblems = 3,  // Cannot communicate with server
    }

    /// <summary>
    /// Class used only to return results to the caller.
    /// </summary>
    public class ActivationResults
    {
        /// <summary>
        /// Assigned site description.
        /// </summary>
        private string _siteDescription = null;

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
        /// <param name="siteDescription">Assigned site description.</param>
        /// <param name="pin">Returned PIN.</param>
        /// <param name="purchase">Purchase validation code.</param>
        /// <param name="errorMessage">If PIN is null or "", this contains the reason.</param>
        public ActivationResults(string siteDescription, string pin, string purchase, string errorMessage)
        {
            _siteDescription = siteDescription;
            _pin = pin;
            _purchase = purchase;
            _errorMessage = errorMessage;
        }

        public string SiteDescription { get => _siteDescription; set => _siteDescription = value; }
        public string Pin { get => _pin; set => _pin = value; }
        public string Purchase { get => _purchase; set => _purchase = value; }
        public string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }
    }

    /// <summary>
    /// Main class for this module.
    /// </summary>
    public class OnlineClient
    {

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
        /// A server call may return a user alert.
        /// </summary>
        private string _pendingUserAlert = null;

        /// <summary>
        /// Ctor for any kind of usage.
        /// </summary>
        /// <param name="sid">installation site identification</param>
        /// <param name="user">installation user ID</param>
        /// <param name="ip">installation IP address or host name</param>
        public OnlineClient(string sid, string user, string ip)
        {
            _sid = sid; 
            _user = user;
            _ip = ip;
        }

        /// <summary>
        /// Ctor for use when activating on this host.
        /// </summary>
        /// <param name="user">user name, null to use the environment's username</param>
        public OnlineClient(string user = null)
        {
            _sid = Activation.Instance.SiteIdentification;
            _ip = System.Environment.UserDomainName;
            _user = System.Environment.UserName;
            if(user != null && user.Trim().Length > 0)
            {
                _user = user;
            }
        }

        /// <summary>
        /// Fetch and clear any pending/awaiting user alert that was received on a call to the server.
        /// </summary>
        public string PendingUserAlert
        {
            get
            {
                string alert = _pendingUserAlert;
                _pendingUserAlert = null;
                return alert;
            }
        }

        /// <summary>
        /// Update the server about activity on this licensed site.
        /// </summary>
        /// <param name="desc">installation assigned description, possibly unknown/null</param>
        /// <returns>ActivationStatus</returns>
        public ActivationStatus IsStillActivated(string desc)
        {
            return CallServerToVerifyActivation(desc);
        }

        /// <summary>
        /// Request a Purch, Desc, and even a PIN from the Able Strategies server. Main entry point.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="desc">installation assigned description, null if unknown *</param>
        /// <param name="purchase">validation code from the purchase, null if unknown *</param>
        /// <returns>Results of the attempt to activate</returns>
        /// <remarks>
        /// * If both desc and purchase are passed in as null, it will trigger a PayPal purchase.
        ///   If either are passed in as null, the system will first attempt to find the transaction
        ///   and then, only if it cannot, will it trigger a PayPal purchase.
        /// </remarks>
        public ActivationResults OnlineActivation(string addr, string city, string zip, string phone, string email, 
            string feature, string desc = null, string purchase = null)
        {
            if ((desc == null || desc.Trim().Length < 1) && purchase != null && purchase.Trim().Length > 0)
            {   // if we have little to work with, start from scratch - first assemble a desc
                desc = CallServerForNewSiteDescription(zip, feature, purchase);
            }
            if (desc == null || desc.Trim().Length < 1)
            {   // this should never happen, but just in case
                return new ActivationResults(null, null, purchase, _serverErrorMessage);
            }
            if((purchase == null || purchase.Trim().Length < 1) && desc != null && desc.Trim().Length > 0)
            {   // have a desc but no purch, maybe there was a comm fault during prior CallServerToRegisterPurchase()
                purchase = CallServerToSearchForPurchase(feature, desc);
            }
            if (purchase == null || purchase.Trim().Length < 1)
            {   // it seems that the user has not yet paid, so bring up the paypal screen
                purchase = PurchaseOnline(addr, city, zip, phone, email, feature, desc); 
            }
            if (purchase == null || purchase.Trim().Length < 1)
            {   // user didn't pay, something was incorrect, or other unexpected issuee
                return new ActivationResults(desc, null, null, _serverErrorMessage);
            }
            string pin = CallServerForActivationPin(addr, city, zip, phone, email, feature, desc, purchase);
            if(pin == null)
            {   // comm error? other processing problem? somehow we got no pin.
                return new ActivationResults(desc, null, purchase, _serverErrorMessage);
            }
            // success!!!
            return new ActivationResults(desc, pin, purchase, _serverErrorMessage);
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
        /// <param name="desc">installation assigned description, null or blank if unknown</param>
        /// <returns>purch val code, or null on error</returns>
        private string PurchaseOnline(
            string addr, string city, string zip, string phone, string email, string feature, string desc)
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
                "&desc=" + Uri.EscapeDataString(desc);
            Browser2Form form = new Browser2Form("Buy", url,
               "https://www.google.com/search?q=site%3Aablestrategies.com+checkbook+help+", null);
            form.Show();
            // Now call the server to see if it was succesful and to register the purchase val code.
            return CallServerToRegisterPurchase(addr, city, zip, phone, email, feature, desc);
        }

        /////////////////////// Web Service API Calls ////////////////////////

        /// <summary>
        /// Call the server to get a (existing or possibly new) siteDescription (purchase may be queried this way)
        /// </summary>
        /// <param name="zip">installation postal code</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="purchase">validation code from the prior purchase</param>
        /// <returns>desc</returns>
        /// <remarks>
        ///   If the
        /// </remarks>
        private string CallServerForNewSiteDescription(string zip, string feature, string purchase)
        {
            _serverErrorMessage = null; // "[E5] ..."

            return null;
        }

        /// <summary>
        /// Call the server to get an activation PIN.
        /// </summary>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="desc">installation assigned description</param>
        /// <returns>purch val code, or null on error or not found</returns>
        private string CallServerToSearchForPurchase(string feature, string desc)
        {
            _serverErrorMessage = null; // "[D4] ..."

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
        /// <param name="desc">installation assigned description</param>
        /// <param name="purchase">validation code from the purchase</param>
        /// <returns>The PIN, or null on error</returns>
        private string CallServerForActivationPin(string addr, string city, string zip, 
            string phone, string email, string feature, string desc, string purchase)
        {
            _serverErrorMessage = null; // "[C3] ..."
            string purch = "";
            bool okay = false;


            if (okay && purch != null && purch.Trim().Length > 0)
            {
                return purch;
            }
            _serverErrorMessage =
                "[C3] Your DESC is " + desc + " - please write it down. The purchase went thru but activation " +
                "failed. Try again later, using the offline method as described on our website. We tried " +
                "a few times and we are very sorry, as this really should not happen, but we too are " +
                "subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

        /// <summary>
        /// Update the server about activity on this licensed site.
        /// </summary>
        /// <param name="desc">installation assigned description, possibly unknown/null</param>
        /// <returns>ActivationStatus</returns>
        private ActivationStatus CallServerToVerifyActivation(string desc)
        {
            _serverErrorMessage = null; // "[B2] ..."

            _pendingUserAlert = null; // ???
            return ActivationStatus.Activated;
        }

        /// <summary>
        /// Confirm a purchase.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc that was purchased</param>
        /// <param name="desc">installation assigned description</param>
        /// <returns>purch val code, or null on error</returns>
        private string CallServerToRegisterPurchase(string addr, string city, string zip, 
            string phone, string email, string feature, string desc)
        {
            _serverErrorMessage = null; // "[A1] ..."
            string purch = "";
            bool okay = false;


            if(okay && purch != null && purch.Trim().Length > 0)
            {
                return purch;
            }
            _serverErrorMessage = 
                "[A1] Your DESC is " + desc + " - please write it down. The purchase went thru but further " +
                "server communication failed, despite multiple attempts. Try again later, using the offline " +
                "method described on our website. We are very sorry, as this should not happen, but we too " +
                "are subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

    }

}
