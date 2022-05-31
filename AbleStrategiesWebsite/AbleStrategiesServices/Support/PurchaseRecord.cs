using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    public enum PurchaseAuthority
    {
        Unknown = 0,
        NoCharge = 1,
        PayPalStd = 2,
    }

    public class PurchaseRecord : BaseDbRecord
    {

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        public Guid fkLicenseId = Guid.Empty;

        /// <summary>
        /// Comments, additional info.
        /// </summary>
        public string details = "";

        /// <summary>
        /// Typically PayPalStd.
        /// </summary>
        public PurchaseAuthority purchaseAuthority = PurchaseAuthority.Unknown;

        /// <summary>
        /// Authority's transaction number.
        /// </summary>
        public string purchaseTransaction = "";

        /// <summary>
        /// Authority's verification code.
        /// </summary>
        public string purchaseVerification = "";

        /// <summary>
        /// Date of purchase.
        /// </summary>
        public DateTime purchaseDate = DateTime.Now; 

        /// <summary>
        /// Ctor.
        /// </summary>
        public PurchaseRecord() : base()
        {
        }

        /// <summary>
        /// [static] Unique record type discriminator.
        /// </summary>
        /// <returns>unique string discriminator</returns>
        public static string GetRecordKind()
        {
            return typeof(PurchaseRecord).Name;
        }

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        public override string RecordKind
        {
            get
            {
                return PurchaseRecord.GetRecordKind();
            }
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
        public string PurchaseTransaction
        {
            get
            {
                return purchaseTransaction;
            }
            set
            {
                purchaseTransaction = value;
                Mod();
            }
        }

        /// <summary>
        /// Authority's verification code.
        /// </summary>
        public string PurchaseVerification
        {
            get
            {
                return purchaseVerification;
            }
            set
            {
                purchaseVerification = value;
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
            this.PurchaseTransaction = ((PurchaseRecord)source).PurchaseTransaction;
            this.PurchaseVerification = ((PurchaseRecord)source).PurchaseVerification;
            this.PurchaseDate = ((PurchaseRecord)source).PurchaseDate;
        }

    }
}
