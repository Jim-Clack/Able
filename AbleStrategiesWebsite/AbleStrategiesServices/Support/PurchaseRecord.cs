﻿using AbleLicensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    public class PurchaseRecord : BaseDbRecord
    {

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        private Guid fkLicenseId = Guid.Empty;

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        private string details = "";

        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        private PurchaseAuthority purchaseAuthority = PurchaseAuthority.Unknown;

        /// <summary>
        /// Provider-specific string containing transaction, verification, etc.
        /// </summary>
        private string purchaseDesignator = "";

        /// <summary>
        /// i.e. AbleCheckbookStd, AbleCheckbookANY(combo mask), etc.
        /// </summary>
        private ProductBitMask productBitMask = ProductBitMask.None;

        /// <summary>
        /// Date of purchase.
        /// </summary>
        private DateTime purchaseDate = DateTime.Now;

        /// <summary>
        /// Price paid.
        /// </summary>
        private long purchaseAmount = 0;

        /// <summary>
        /// Ctor.
        /// </summary>
        public PurchaseRecord() : base()
        {
        }

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        public string Details
        {
            get
            {
                return details;
            }
            set
            {
                details = value;
                Mod();
            }
        }

        /// <summary>
        /// Forieng key - license.
        /// </summary>
        public Guid FkLicenseId
        {
            get
            {
                return fkLicenseId;
            }
            set
            {
                fkLicenseId = value;
                Mod();
            }
        }

        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        public PurchaseAuthority PurchaseAuthority
        {
            get
            {
                return purchaseAuthority;
            }
            set
            {
                purchaseAuthority = value;
                Mod();
            }
        }

        /// <summary>
        /// Authority's transaction number.
        /// </summary>
        public string PurchaseDesignator
        {
            get
            {
                return purchaseDesignator;
            }
            set
            {
                purchaseDesignator = value;
                Mod();
            }
        }

        /// <summary>
        /// Date of purchase.
        /// </summary>
        public DateTime PurchaseDate
        {
            get
            {
                return purchaseDate;
            }
            set
            {
                purchaseDate = value;
                Mod();
            }
        }

        /// <summary>
        /// Price paid.
        /// </summary>
        public long PurchaseAmount
        {
            get
            {
                return purchaseAmount;
            }
            set
            {
                purchaseAmount = value;
                Mod();
            }
        }

        /// <summary>
        /// What product was purchased
        /// </summary>
        public ProductBitMask ProductBitMask
        {
            get
            {
                return productBitMask;
            }
            set
            {
                productBitMask = value;
            }
        }

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PurRec{" + SupportMethods.Shorten(Id.ToString()) +
                "," + SupportMethods.Shorten(fkLicenseId.ToString()) +
                "," + productBitMask +
                "," + purchaseDesignator +
                "," + SupportMethods.Shorten(purchaseAmount.ToString()) +
                "," + purchaseDate.ToShortDateString() + "}";
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void PopulateFrom(BaseDbRecord source)
        {
            if (!PopulateBaseFrom(source))
            {
                return;
            }
            this.Details = ((PurchaseRecord)source).Details;
            this.FkLicenseId = ((PurchaseRecord)source).FkLicenseId;
            this.PurchaseAuthority = ((PurchaseRecord)source).PurchaseAuthority;
            this.purchaseDesignator = ((PurchaseRecord)source).purchaseDesignator;
            this.productBitMask = ((PurchaseRecord)source).ProductBitMask;
            this.PurchaseDate = ((PurchaseRecord)source).PurchaseDate;
        }

    }
}
