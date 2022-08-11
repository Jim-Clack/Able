using System;
using System.Collections.Generic;
using System.Text;

namespace AbleStrategies.CheckbookWsApi
{

    public interface PurchaseRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// When was this record created?
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        DateTime DateModified { get; set; }

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        Guid FkLicenseId { get; set; }

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        string Details { get; set; }
        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        int PurchaseAuthority { get; set; }

        /// <summary>
        /// Authority's transaction number.
        /// </summary>
        string PurchaseTransaction { get; set; }

        /// <summary>
        /// Authority's verification code.
        /// </summary>
        string PurchaseVerification { get; set; }

        /// <summary>
        /// Date of purchase.
        /// </summary>
        DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Price paid.
        /// </summary>
        long PurchaseAmount { get; set; }

}
}
