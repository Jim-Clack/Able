using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{

    public class PurchaseRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified;

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId;

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        public string Details;
        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        public int PurchaseAuthority;

        /// <summary>
        /// Authority's coded values for this transaction.
        /// </summary>
        public string PurchaseDesignator;

        /// <summary>
        /// Date of purchase.
        /// </summary>
        public DateTime PurchaseDate;

        /// <summary>
        /// Price paid.
        /// </summary>
        public long PurchaseAmount;

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PurRec{" + PurchaseDesignator + ", " + PurchaseAmount.ToString() + ", " + PurchaseDate.ToShortDateString() + ", " + Details + "}";
        }

    }

}
