using AbleLicensing.WsApi;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace AbleLicensing
{
    public class PayPalPurchaseProvider : IPurchaseProvider
    {

        /// <summary>
        /// [static] Purchase authority prefix
        /// </summary>
        public static char ProviderPrefix = (char)PurchaseAuthority.PayPalStd;

        /// <summary>
        /// Field delimiter in purchase designator string.
        /// </summary>
        private char delimiter = '|';

        /// <summary>
        /// URL for paypal WS.
        /// </summary>
        private string payPalUrl = "";

        /// <summary>
        /// Configuration settings.
        /// </summary>
        private string payPalConfiguration = "";

        /// <summary>
        /// After the purchase, this will be populated.
        /// </summary>
        private string purchaseDesignator = "";

        /// <summary>
        /// Return last error.
        /// </summary>
        private string errorMessage = "";

        /// <summary>
        /// Call timeout in millis.
        /// </summary>
        private int timeout = 20000;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="url">URL for paypal WS</param>
        /// <param name="configuration">Configuration settings</param>
        /// <param name="timeout">call timeout in milllis</param>
        public PayPalPurchaseProvider(string url, string configuration, int timeout)
        {
            this.payPalUrl = url.Trim();
            this.payPalConfiguration = configuration.Trim();
            this.timeout = timeout;
            if(!payPalUrl.EndsWith("/"))
            {
                payPalUrl += "/";
            }


            // NO NO NO   TODO 

            string accessToken = GetAccessToken();

        }

        /// <summary>
        /// Purchase authority prefix
        /// </summary>
        public string Prefix
        {
            get
            {
                return "" + ProviderPrefix;
            }
        }

        /// <summary>
        /// Create a purchase designator string.
        /// </summary>
        /// <param name="transactionNumber">From payment processor</param>
        /// <param name="validationCode">From payment processor</param>
        /// <param name="amount">Amount in smallest units of currency</param>
        /// <returns>Purchase Descriptor string</returns>
        public string ToPurchaseDesignator(string transactionNumber, string validationCode, string otherCode)
        {
            return Prefix + transactionNumber.Trim() + delimiter + validationCode.Trim() + delimiter + otherCode.Trim();
        }

        /// <summary>
        /// Populate UserInfo with new purchase info - add Purchase & Interactivity records too. (Does NOT call PayPal)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data</param>
        /// <param name="purchaseDesignator">Source of purchase data</param>
        /// <param name="amount">Amount in smallest units of currency</param>
        /// <returns>purchase verified?</returns>
        /// <remarks>Does not update purchase date or details</remarks>
        public bool FromPurchaseDesignator(ref UserInfo userInfo, string purchaseDesignator, long amount)
        {
            if(string.IsNullOrEmpty(purchaseDesignator) || !purchaseDesignator.Contains("" + delimiter) || !purchaseDesignator.StartsWith(Prefix))
            {
                return false;
            }
            if(userInfo.PurchaseRecords == null)
            {
                userInfo.PurchaseRecords = new List<PurchaseRecord>();
            }
            PurchaseRecord purchaseRecord = userInfo.GetPurchaseRecord(purchaseDesignator, true);
            purchaseRecord.PurchaseAuthority = (int)PurchaseAuthority.PayPalStd;
            purchaseRecord.PurchaseDesignator = purchaseDesignator;
            purchaseRecord.PurchaseAmount = amount;
            return true;
        }

        /// <summary>
        /// Set the purchaser information.
        /// </summary>
        /// <param name="lastName">Per payment provider</param>
        /// <param name="firstName">Per payment provider</param>
        /// <param name="phone">Per payment provider</param>
        /// <param name="email">Per payment provider</param>
        /// <param name="streetAddress">Per payment provider</param>
        /// <param name="city">Per payment provider</param>
        /// <param name="state">Per payment provider</param>
        /// <param name="zip">Per payment provider</param>
        /// <param name="country">Per payment provider</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool SetPurchaser(string lastName, string firstName, string phone, string email, string streetAddress, string city, string state, string zip, string country)
        {
            errorMessage = "";


            return false;
        }

        /// <summary>
        /// Add an item to the purchase.
        /// </summary>
        /// <param name="label">Name of item</param>
        /// <param name="description">Description of item</param>
        /// <param name="amount">Unit price of item</param>
        /// <param name="quantity">Quantity of items</param>
        /// <param name="extendedPrice">Price for given quantity (typically amount times quantity)</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool AddItem(string label, string description, long amount, int quantity, long extendedPrice)
        {
            errorMessage = "";


            return false;
        }

        /// <summary>
        /// Set te payment method.
        /// </summary>
        /// <param name="ccNumber">credit card number</param>
        /// <param name="ccExpMonth">expiration date month</param>
        /// <param name="ccExpYear">expiration date year</param>
        /// <param name="cvv2">CVV2 code</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool SetPayment(string ccNumber, string ccExpMonth, string ccExpYear, string cvv2)
        {
            errorMessage = "";


            return false;
        }

        /// <summary>
        /// Verify that a purchase has been made and update the user info accordingly. (calls PayPal)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data, possibly replacing the existing purchase record</param>
        /// <param name="details">details - man-readable</param>
        /// <param name="addTax">not yet supported</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool CompletePurchase(ref UserInfo userInfo, string details, bool addTax)
        {
            errorMessage = "";
            string paymentResponse = null;
            string accessToken = GetAccessToken();
            try
            {
                string url = payPalUrl + "XXX";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                Byte[] body = Encoding.ASCII.GetBytes("" + "");
                request.GetRequestStream().Write(body, 0, body.Length);
                Activation.Instance.LoggerHook("???");
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        paymentResponse = (string)JsonSerializer.Deserialize(json, typeof(string));
                    }
                }
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook("Exception" + e.Message);
            }
            if (paymentResponse != null)
            {
                Activation.Instance.LoggerHook(paymentResponse.ToString());
                //return paymentResponse;
            }
            Activation.Instance.LoggerHook("Response is null");
            // TODO
            return true;
        }

        /// <summary>
        /// After the purchase is successful, this will be populated.
        /// </summary>
        public string PurchaseDesignator
        {
            get
            {
                return purchaseDesignator;
            }
        }

        /// <summary>
        /// Return last error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        /// <summary>
        /// Verify that a purchase has been made per existing user info. (Calls Paypal)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data, possibly replacing the existing purchase record</param>
        /// <param name="purchaseDesignator">Source of purchase data</param>
        /// <returns>purchase verified?</returns>
        /// <remarks>Does not verify purchaseAmount</remarks>
        public string VerifyPurchase(UserInfo userInfo, string purchaseDesignator)
        {
            // TODO
            return "";
        }

        /// <summary>
        /// [static] Get credit card type. i.e. Visa, MC, Amex
        /// </summary>
        /// <param name="ccNumber">credit card number</param>
        /// <returns>Credit card type, null if not a legal CC number</returns>
        public static string CreditCardType(string ccNumber)
        {

            return null;
        }

        ///////////////////////////// Support ////////////////////////////////

        private string PerformTransaction()
        {

            return "";
        }

        private string GetAccessToken()
        { 
            string accessToken = null;
            string[] strings = payPalConfiguration.Split(delimiter);
            if(strings.Length != 2)
            {
                errorMessage = "PayPal Configuration is bad " + payPalConfiguration;
                Activation.Instance.LoggerHook(errorMessage);
                return accessToken;
            }
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(strings[0] + ":" + strings[1]));
            Byte[] body = Encoding.ASCII.GetBytes(HttpUtility.UrlEncode("grant_type:client_credentials"));
            try
            {
                /* 
                 * Request:
                 *   https://api-m.sandbox.paypal.com/v1/oauth2/token
                 *   POST
                 *   Authorization: Basic QWZscHJ6...4wSmFCNlpGekRnMU9qQjg= (from Client ID and Secret)
                 *   Content-Type: application/x-www-form-urlencoded
                 *   Body: grant_type:client_credentials
                 * Response:
                 *   {
                 *       "scope": "https://uri.paypal.com/services/invoicing ... openid ... https://uri.paypal.com/services/applications/webhooks",
                 *       "access_token": "A21AALDNImyn3a0FvcFRxl_AuZh-V99wi5M_UnxAKQHocQrdUjRrPZsI91Q8rgua47FxY7-Fh92SpHGJ06YPPOA48D-QTbPig",
                 *       "token_type": "Bearer",
                 *       "app_id": "APP-80W284485P519543T",
                 *       "expires_in": 32310,
                 *       "nonce": "2022-08-27T23:01:39ZoyZ2NnVEKuu3f-8vN30bz8CInmQ2pnJVgAj4Zt1z11I"
                 *   }
                 */
                string url = payPalUrl + "v1/oauth2/token";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add("Authorization", "Bearer " + accessToken);
                request.Headers.Add("Authorization", "Basic " + credentials);
                request.GetRequestStream().Write(body, 0, body.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        Regex regex = new Regex("access_token\\\"\\:[ ]*\\\"([^\\\"]*)", RegexOptions.Multiline);
                        accessToken = regex.Match(json).Groups[0].Value.Trim();
                        if(accessToken == null || accessToken.Length < 50 || accessToken.Length > 150)
                        {
                            accessToken = null;

      Activation.Instance.LoggerHook("Access Token: " + accessToken);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Activation.Instance.LoggerHook("Exception getting access token: " + e.Message);
            }
            return accessToken;
        }

    }

}
