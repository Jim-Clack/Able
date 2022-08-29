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
        public Guid Id = Guid.Empty;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated = DateTime.Now;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified = DateTime.Now;

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId = Guid.Empty;

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        public string Details = "";

        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        public int PurchaseAuthority = 0;

        /// <summary>
        /// Authority's coded values for this transaction.
        /// </summary>
        public string PurchaseDesignator = "";

        /// <summary>
        /// i.e. AbleCheckbookStd = 2, AbleCheckbookANY(combo mask) = 31 
        /// </summary>
        public int ProductBitMask = 0;

        /// <summary>
        /// Date of purchase.
        /// </summary>
        public DateTime PurchaseDate = DateTime.Now;

        /// <summary>
        /// Price paid.
        /// </summary>
        public long PurchaseAmount = 0L;

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
