using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing
{
    public interface IPurchaseProvider
    {

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
        ///  Verify that a purchase has been made and update the user info accordingly. (calls provider)
        /// </summary>
        /// <param name="userInfo">To be updated with purchase data, possibly replacing the existing purchase record</param>
        /// <param name="amount">Amount in smallest units of currency, i.e. cents</param>
        /// <param name="details">details - man-readable</param>
        /// <returns>purchase verified?</returns>
        bool CompletePurchase(ref UserInfo userInfo, long amouont, string details);

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
