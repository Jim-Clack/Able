using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AbleLicensing
{
    public class PayPalPurchaseProvider : IPurchaseProvider
    {

        /// <summary>
        /// [static] Purchase authority prefix
        /// </summary>
        public static char ProviderPrefix = (char)PurchaseAuthority.PayPalStd;

        /// <summary>
        /// Purchase authority prefix
        /// </summary>
        private string Prefix { get { return "" + ProviderPrefix; } }

        /// <summary>
        /// Field delimiter in purchase desgnator string.
        /// </summary>
        private string Delimiter = "|";

        /// <summary>
        /// URL for paypal WS.
        /// </summary>
        private string payPalUrl = "";

        /// <summary>
        /// Configuration settings.
        /// </summary>
        private string payPalConfiguration = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="url">URL for paypal WS</param>
        /// <param name="configuration">Configuration settings</param>
        public PayPalPurchaseProvider(string url, string configuration)
        {
            payPalUrl = url;
            payPalConfiguration = configuration;
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
            return Prefix + transactionNumber.Trim() + Delimiter + validationCode.Trim() + Delimiter + otherCode.Trim();
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
            if(string.IsNullOrEmpty(purchaseDesignator) || !purchaseDesignator.Contains(Delimiter) || !purchaseDesignator.StartsWith(Prefix))
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
        ///  Verify that a purchase has been made and update the user info accordingly. (calls PayPal)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data, possibly replacing the existing purchase record</param>
        /// <param name="amount">Amount in smallest units of currency, i.e. cents</param>
        /// <param name="details">details - man-readable</param>
        /// <returns>purchase verified?</returns>
        public bool CompletePurchase(ref UserInfo userInfo, long amount, string details)
        {

            // TODO
            return true;
        }

        private string PerformTransaction(int timeout)
        {
            string accessToken = "???"; // TODO
            string paymentResponse = null;
            try
            {
                /* TOKEN REQUEST
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
                string url = payPalUrl + "???";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "GET";
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + accessToken);
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
                return paymentResponse;
            }
            Activation.Instance.LoggerHook("Response is null");
            return null;
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

    }

}
