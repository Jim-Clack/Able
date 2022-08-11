using System;
using System.Collections.Generic;

namespace AbleStrategiesServices.Support
{
    public class LicenseRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record discriminator.
        /// </summary>
        private string licenseCode = "";

        /// <summary>
        /// Contact name.
        /// </summary>
        private string contactName = "";

        /// <summary>
        /// Street address.
        /// </summary>
        private string contactAddress = "";

        /// <summary>
        /// City and state
        /// </summary>
        private string contactCity = "";

        /// <summary>
        /// Postal code
        /// </summary>
        private string contactZip = "";

        /// <summary>
        /// Contact phone.
        /// </summary>
        private string contactPhone = "";

        /// <summary>
        /// Contact email.
        /// </summary>
        private string contactEMail = "";

        /// <summary>
        /// License features bitmap.
        /// </summary>
        private string licenseFeatures = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public LicenseRecord() : base()
        {
        }

        /// <summary>
        /// License number.
        /// </summary>
        public string LicenseCode
        {
            get
            {
                return licenseCode;
            }
            set
            {
                licenseCode = value;
                Mod();
            }
        }

        /// <summary>
        /// Contact Name.
        /// </summary>
        public string ContactName
        {
            get
            {
                return contactName;
            }
            set
            {
                contactName = value;
                Mod();
            }
        }

        /// <summary>
        /// Contact Address.
        /// </summary>
        public string ContactAddress
        {
            get
            {
                return contactAddress;
            }
            set
            {
                contactAddress = value;
                Mod();
            }
        }

        /// <summary>
        /// Contact City and State.
        /// </summary>
        public string ContactCity
        {
            get
            {
                return contactCity;
            }
            set
            {
                contactCity = value;
                Mod();
            }
        }

        /// <summary>
        /// Postal Code.
        /// </summary>
        public string ContactZip
        {
            get
            {
                return contactZip;
            }
            set
            {
                contactZip = value;
                Mod();
            }
        }

        /// <summary>
        /// Contact Phone.
        /// </summary>
        public string ContactPhone
        {
            get
            {
                return contactPhone;
            }
            set
            {
                contactPhone = value;
                Mod();
            }
        }

        /// <summary>
        /// Contact EMail.
        /// </summary>
        public string ContactEMail
        {
            get
            {
                return contactEMail;
            }
            set
            {
                contactEMail = value;
                Mod();
            }
        }

        /// <summary>
        /// LiceneFeatures bitmap.
        /// </summary>
        public string LicenseFeatures
        {
            get
            {
                return licenseFeatures;
            }
            set
            {
                licenseFeatures = value;
                Mod();
            }
        }

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "LicRec{" + SupportMethods.Shorten(Id.ToString()) +
                "," + licenseCode +
                "," + contactName + "}";
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void PopulateFrom(BaseDbRecord source)
        {
            if(!PopulateBaseFrom(source))
            {
                return;
            }
            this.LicenseCode = ((LicenseRecord)source).LicenseCode;
            this.ContactName = ((LicenseRecord)source).ContactName;
            this.ContactAddress = ((LicenseRecord)source).ContactAddress;
            this.ContactCity = ((LicenseRecord)source).ContactCity;
            this.ContactPhone = ((LicenseRecord)source).ContactPhone;
            this.ContactEMail = ((LicenseRecord)source).ContactEMail;
            this.LicenseFeatures = ((LicenseRecord)source).LicenseFeatures;
        }

    }
}
