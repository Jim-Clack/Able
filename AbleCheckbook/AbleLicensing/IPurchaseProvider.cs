using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing
{
    public interface IPurchaseProvider
    {

        /// <summary>
        /// Purchase authority prefix
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Create a purchase designator string.
        /// </summary>
        /// <param name="transactionNumber">From payment processor</param>
        /// <param name="validationCode">From payment processor</param>
        /// <param name="otherCode">optional additional value</param>
        /// <returns>Purchase Descriptor string</returns>
        string ToPurchaseDesignator(string transactionNumber, string validationCode, string otherCode);

        /// <summary>
        /// Populate UserInfo with new purchase info - add Purchase & Interactivity records too. (Does NOT call provider)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data</param>
        /// <param name="purchaseDesignator">Source of purchase data</param>
        /// <param name="amount">Amount in smallest units of currency</param>
        /// <returns>purchase verified?</returns>
        /// <remarks>Does not update purchase date or details</remarks>
        bool FromPurchaseDesignator(ref UserInfo userInfo, string purchaseDesignator, long amount);

        /// <summary>
        /// Set the purchaser information.
        /// </summary>
        /// <param name="lastName">Per payment provider</param>
        /// <param name="firstName">Per payment provider</param>
        /// <param name="phone">Per payment provider</param>
        /// <param name="email">Per payment provider</param>
        /// <param name="streetAddress">Per payment provider</param>
        /// <param name="apt">Per payment provider</param>
        /// <param name="city">Per payment provider</param>
        /// <param name="state">Per payment provider</param>
        /// <param name="zip">Per payment provider</param>
        /// <param name="country">Per payment provider</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        bool SetPurchaser(string lastName, string firstName, string phone, string email, string streetAddress, string apt, string city, string state, string zip, string country);

        /// <summary>
        /// Add an item to the purchase.
        /// </summary>
        /// <param name="label">Name of item</param>
        /// <param name="description">Description of item</param>
        /// <param name="amount">Unit price of item</param>
        /// <param name="quantity">Quantity of items</param>
        /// <param name="extendedPrice">Price for given quantity (typically amount times quantity)</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        bool SetItem(string label, string description, long amount, int quantity, long extendedPrice);

        /// <summary>
        /// Set the payment method.
        /// </summary>
        /// <param name="ccNumber">credit card number</param>
        /// <param name="ccExpMonth">expiration date month</param>
        /// <param name="ccExpYear">expiration date year</param>
        /// <param name="cvv2">CVV2 code</param>
        /// <param name="ccType">Visa, Mastercard, etc.</param>
        /// <returns>Success - see ErrorMessage if false</returns>
        bool SetPayment(string ccNumber, string ccExpMonth, string ccExpYear, string cvv2, string ccType);

        /// <summary>
        /// Verify that a purchase has been made and update the user info accordingly. (calls PayPal)
        /// </summary>
        /// <returns>Success - see ErrorMessage if false</returns>
        /// <remarks>On success, get purchaseDesignator</remarks>
        bool CompletePurchase();

        /// <summary>
        /// After the purchase is successful, this will be populated.
        /// </summary>
        string PurchaseDesignator { get; }

        /// <summary>
        /// Return last error.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Verify that a purchase has been made per existing user info. (calls provider)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data, possibly replacing the existing purchase record</param>
        /// <param name="purchaseDesignator">Source of purchase data</param>
        /// <returns>purchase verified?</returns>
        /// <remarks>Does not verify purchaseAmount</remarks>
        string VerifyPurchase(UserInfo userInfo, string purchaseDesignator);

    }

}
