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
        /// PayPal security.
        /// </summary>
        private string accessToken = null;

        /// <summary>
        /// Connection info for PayPal.
        /// </summary>
        private APIContext apiContext = null;

        /// <summary>
        /// Everything about the transaction will be passed via this object.
        /// </summary>
        private Payment payment = null;

        /// <summary>
        /// After the purchase, this will be populated.
        /// </summary>
        private string purchaseDesignator = "";

        /// <summary>
        /// Return last error. Empty ("") if no error has occurred.
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
        /// <remarks>Checked ErrorMessage afterward to ensure connectivity to Paypal</remarks>
        public PayPalPurchaseProvider(string url, string configuration, int timeout)
        {
            errorMessage = "";
            this.payPalUrl = url.Trim();
            this.payPalConfiguration = configuration.Trim();
            this.timeout = timeout;
            if(!payPalUrl.EndsWith("/"))
            {
                payPalUrl += "/";
            }
            accessToken = GetAccessToken();
            if(accessToken == null)
            {
                errorMessage = "Cannot access PayPal at this time.";
                return;
            }
            apiContext = new APIContext(accessToken);
            // Assemble the entire transaction tree before populating it via the SetXxxx methods.
            payment = new Payment();
            payment.payer = new Payer();
            payment.intent = "Sale";
            payment.transactions = new List<Transaction>();
            payment.note_to_payer = "AbleCheckbook Activation";
            payment.payer.payer_info = new PayerInfo();
            payment.payer.payer_info.shipping_address = new ShippingAddress();
            payment.payer.payer_info.billing_address = new Address();
            payment.payer.funding_instruments = new List<FundingInstrument>();
            payment.payer.funding_instruments.Add(new FundingInstrument());
            payment.payer.funding_instruments[0].credit_card = new CreditCard();
            payment.payer.funding_instruments[0].credit_card.billing_address = payment.payer.payer_info.shipping_address;
            payment.transactions.Add(new Transaction());
            payment.transactions[0].amount = new Amount();
            payment.transactions[0].amount.details = new Details();
            payment.transactions[0].item_list = new ItemList();
            payment.transactions[0].item_list.items = new List<Item>();
            payment.transactions[0].item_list.items.Add(new Item());
            payment.transactions[0].item_list.shipping_address = payment.payer.payer_info.shipping_address;
        }

        /// <summary>
        /// Set the purchaser information.
        /// </summary>
        /// <param name="firstName">Per payment provider</param>
        /// <param name="lastName">Per payment provider</param>
        /// <param name="phone">Per payment provider</param>
        /// <param name="email">Per payment provider</param>
        /// <param name="streetAddress">Per payment provider</param>
        /// <param name="apt">Per payment provider</param>
        /// <param name="city">Per payment provider</param>
        /// <param name="state">Per payment provider</param>
        /// <param name="zip">Per payment provider</param>
        /// <param name="country">Per payment provider</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool SetPurchaser(string firstName, string lastName, string phone, string email, string streetAddress, string apt, string city, string state, string zip, string country)
        {
            errorMessage = "";
            payment.state = state;
            payment.payer.payment_method = "credit_card";
            payment.payer.payer_info.first_name = firstName;
            payment.payer.payer_info.last_name = lastName;
            payment.payer.payer_info.country_code = country;
            payment.payer.payer_info.email = email;
            payment.payer.payer_info.phone = phone;
            payment.payer.payer_info.external_remember_me_id = Environment.MachineName + "/" + 
                Environment.UserName + "/" + Activation.Instance.SiteIdentification;
            payment.payer.payer_info.billing_address.line1 = payment.payer.payer_info.shipping_address.line1 = streetAddress;
            payment.payer.payer_info.billing_address.line2 = payment.payer.payer_info.shipping_address.line2 = apt;
            payment.payer.payer_info.billing_address.city = payment.payer.payer_info.shipping_address.city = city;
            payment.payer.payer_info.billing_address.state = payment.payer.payer_info.shipping_address.state = state;
            payment.payer.payer_info.billing_address.postal_code = payment.payer.payer_info.shipping_address.postal_code = zip;
            payment.payer.payer_info.billing_address.country_code = payment.payer.payer_info.shipping_address.country_code = country;
            payment.payer.payer_info.billing_address.phone = payment.payer.payer_info.shipping_address.phone = phone;
            payment.payer.payer_info.shipping_address.type = "HOME";
            payment.transactions[0].item_list.shipping_phone_number = phone;
            payment.payer.funding_instruments[0].credit_card.first_name = firstName;
            payment.payer.funding_instruments[0].credit_card.last_name = lastName;
            return true;
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
        public bool SetItem(string label, string description, long amount, int quantity, long extendedPrice)
        {
            errorMessage = "";
            string amountString = Math.Abs(amount).ToString();
            while(amountString.Length < 3)
            {
                amountString = "0" + amountString;
            }
            amountString = amountString.Insert(amountString.Length - 2, ".");
            if(amountString.CompareTo("29.95") != 0)
            {
                errorMessage = "Amount incorrect. (internal issue) " + amountString;
                return false;
            }
            payment.transactions[0].description = description;
            payment.transactions[0].item_list.shipping_method = "???";
            payment.transactions[0].item_list.items[0].currency = "USD";
            payment.transactions[0].item_list.items[0].name = label;
            payment.transactions[0].item_list.items[0].price = amountString;
            payment.transactions[0].item_list.items[0].quantity = "1";
            payment.transactions[0].item_list.items[0].tax = "0.00";
            payment.transactions[0].amount.currency = "USD";
            payment.transactions[0].amount.total = amountString;
            payment.transactions[0].amount.details.subtotal = amountString;
            payment.transactions[0].amount.details.tax = "0.00";
            return true;
        }

        /// <summary>
        /// Set the payment method.
        /// </summary>
        /// <param name="ccNumber">credit card number</param>
        /// <param name="ccExpMonth">expiration date month</param>
        /// <param name="ccExpYear">expiration date year</param>
        /// <param name="cvv2">CVV2 code</param>
        /// <param name="ccType">Visa, Mastercard, etc.</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        public bool SetPayment(string ccNumber, string ccExpMonth, string ccExpYear, string cvv2, string ccType)
        {
            errorMessage = "";
            payment.payer.funding_instruments[0].credit_card.number = ccNumber;
            payment.payer.funding_instruments[0].credit_card.type = ccType.ToLower();
            payment.payer.funding_instruments[0].credit_card.cvv2 = cvv2;
            try
            {
                payment.payer.funding_instruments[0].credit_card.expire_month = Convert.ToInt32(ccExpMonth);
                payment.payer.funding_instruments[0].credit_card.expire_year = Convert.ToInt32(ccExpYear);
            }
            catch(FormatException ex)
            {
                errorMessage = "Illegal expiration date: " + ex.Message; 
                return false;
            }
            return true;
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

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.IncludeFields = true;
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(payment, typeof(Payment), options);
            Activation.Instance.LoggerHook("@@@@@@@" + accessToken);
            Activation.Instance.LoggerHook("@@@@@@@" + json);

            // TODO

            purchaseDesignator = "????";
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
            if (string.IsNullOrEmpty(purchaseDesignator) || !purchaseDesignator.Contains("" + delimiter) || !purchaseDesignator.StartsWith(Prefix))
            {
                return false;
            }
            if (userInfo.PurchaseRecords == null)
            {
                userInfo.PurchaseRecords = new List<PurchaseRecord>();
            }
            PurchaseRecord purchaseRecord = userInfo.GetPurchaseRecord(purchaseDesignator, true);
            purchaseRecord.PurchaseAuthority = (int)PurchaseAuthority.PayPalStd;
            purchaseRecord.PurchaseDesignator = purchaseDesignator;
            purchaseRecord.PurchaseAmount = amount;
            return true;
        }

        ///////////////////////////// Support ////////////////////////////////

        /// <summary>
        /// [static] Assemble x-www-form-urlencoded entry
        /// </summary>
        /// <param name="name">Name of entry</param>
        /// <param name="value">Its value</param>
        /// <param name="appendTo">(optional)Existing entries to be appended to</param>
        /// <returns>Byte[] containing data</returns>
        private static Byte[] AssembleFormUrlEncodedEntry(string name, string value, Byte[] appendTo = null)
        {
            Byte[] entry = Encoding.ASCII.GetBytes(HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode(value));
            if (appendTo == null)
            {
                return entry;
            }
            Byte[] entries = new Byte[appendTo.Length + entry.Length + 1];
            Array.Copy(appendTo, 0, entries, 0, appendTo.Length);
            Array.Copy(Encoding.ASCII.GetBytes(new char[] {'&'}, 0, 1), 0, entries, entry.Length, 1);
            Array.Copy(entry, 0, entries, entry.Length + 1, entry.Length);
            return entries;
        }

        private string PerformTransaction()
        {

            return "";
        }

        /// <summary>
        /// GEt the access token from PayPal.
        /// </summary>
        /// <returns>PayPal access token</returns>
        private string GetAccessToken()
        { 
            string accessToken = null;
            try
            {
                string url = payPalUrl + "v1/oauth2/token";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "POST";
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Authorization", "Basic " + payPalConfiguration);
                Byte[] body = AssembleFormUrlEncodedEntry("grant_type", "client_credentials");
                request.GetRequestStream().Write(body, 0, body.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        Regex regex = new Regex("access_token\\\"\\:[ ]*\\\"(?<token>[^\\\"]*)", RegexOptions.Multiline);
                        accessToken = regex.Match(json).Groups["token"].Value.Trim();
                        if (accessToken == null || accessToken.Length < 50 || accessToken.Length > 150)
                        {
                            accessToken = null;
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
