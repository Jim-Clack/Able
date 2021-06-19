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
        /// Update the server about activity on this licensed site.
        /// </summary>
        /// <param name="desc">installation assigned description, possibly unknown/null</param>
        /// <returns>ActivationStatus</returns>
        /// <remarks>
        /// It is recommended that you do NOT deactive if this returns false. Instead record and
        /// persist the failure along with its cause, then only deactivate after 10 sequential 
        /// network failures or 2 DeActivated or NotActivated failures.
        /// </remarks>
        public ActivationStatus IsStillActivation(string desc)
        {
            return CallServerToVerifyActivation(desc);
        }

        /// <summary>
        /// Request a PIN from the Able Strategies server.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="desc">installation assigned description, null or blank if unknown</param>
        /// <param name="purchase">null to trigger a Paypal purchase, else the validation code from the prior purchase</param>
        /// <returns>Results of the attempt to activate</returns>
        public ActivationResults RequestPin(string addr, string city, string zip, string phone, string email, 
            string feature, string desc = null, string purchase = null)
        {
            if ((desc == null || desc.Trim().Length < 1) && purchase != null && purchase.Trim().Length > 0)
            {
                desc = CallServerForNewSiteDescription(zip, feature, purchase);
            }
            if (desc == null || desc.Trim().Length < 1)
            {
                return new ActivationResults(null, null, purchase, _serverErrorMessage);
            }
            if (purchase == null || purchase.Trim().Length < 1)
            {
                purchase = PurchaseOnline(addr, city, zip, phone, email, feature, desc);
            }
            if (purchase == null || purchase.Trim().Length < 1)
            {
                return new ActivationResults(desc, null, null, _serverErrorMessage);
            }
            string pin = CallServerForActivationPin(addr, city, zip, phone, email, feature, desc, purchase);
            return new ActivationResults(desc, pin, purchase, _serverErrorMessage);
        }

        ////////////////////////////// Purchase //////////////////////////////

        /// <summary>
        /// Bring up the "Buy" page and proceed to PayPal or whatever
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased</param>
        /// <param name="desc">installation assigned description, null or blank if unknown</param>
        /// <returns></returns>
        private string PurchaseOnline(
            string addr, string zip, string city, string phone, string email, string feature, string desc)
        {
            _serverErrorMessage = null;
            string url = "https://ablestrategies.com/ablecheckbook/PushedCheckout.html?mode=basic" +
                "&sid=" + Uri.EscapeDataString(_sid) +
                "&user=" + Uri.EscapeDataString(_user) +
                "&ip=" + Uri.EscapeDataString(_ip) +
                "&feature=" + Uri.EscapeDataString(feature) +
                "&desc=" + Uri.EscapeDataString(desc);
            Browser2Form form = new Browser2Form("Buy", url,
               "https://www.google.com/search?q=site%3Aablestrategies.com+checkbook+help+", null);
            form.Show();
            // Now call the server to see if it was succesful and to fetch the purchase val code.
            return CallServerToConfirmPurchase(feature, desc);
        }

        /////////////////////// Web Service API Calls ////////////////////////

        /// <summary>
        /// Call the server to get a (existing or possibly new) siteDescription (purchase may be queried this way)
        /// </summary>
        /// <param name="zip">installation postal code</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="purchase">validation code from the prior purchase</param>
        /// <returns></returns>
        private string CallServerForNewSiteDescription(string zip, string feature, string purchase)
        {
            _serverErrorMessage = null;

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
            _serverErrorMessage = null;

            return null;
        }

        /// <summary>
        /// Update the server about activity on this licensed site.
        /// </summary>
        /// <param name="desc">installation assigned description, possibly unknown/null</param>
        /// <returns>ActivationStatus</returns>
        private ActivationStatus CallServerToVerifyActivation(string desc)
        {
            _serverErrorMessage = null;

            return ActivationStatus.Activated;
        }

        /// <summary>
        /// Confirm a purchase.
        /// </summary>
        /// <param name="feature">installation edition/features/etc that was purchased</param>
        /// <param name="desc">installation assigned description, possibly unknown/null</param>
        /// <returns></returns>
        private string CallServerToConfirmPurchase(string feature, string desc)
        {
            _serverErrorMessage = null;

            return null;
        }

    }

}
