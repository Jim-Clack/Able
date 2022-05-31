using System;
using System.Collections.Generic;

namespace AbleStrategiesServices.Support
{
    public class LicenseRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record discriminator.
        /// </summary>
        public string licenseDesc = "";

        /// <summary>
        /// Contact name.
        /// </summary>
        public string contactName = "";

        /// <summary>
        /// Street address.
        /// </summary>
        public string contactAddress = "";

        /// <summary>
        /// City and state
        /// </summary>
        public string contactCity = "";

        /// <summary>
        /// Contact phone.
        /// </summary>
        public string contactPhone = "";

        /// <summary>
        /// Contact email.
        /// </summary>
        public string contactEMail = "";

        /// <summary>
        /// License features bitmap.
        /// </summary>
        public string licenseFeatures = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public LicenseRecord() : base()
        {
        }

        /// <summary>
        /// [static] Unique record type discriminator.
        /// </summary>
        /// <returns>unique string discriminator</returns>
        public static string GetRecordKind()
        {
            return typeof(LicenseRecord).Name;
        }

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        public override string RecordKind
        {
            get
            {
                return LicenseRecord.GetRecordKind();
            }
        }

        /// <summary>
        /// Description of record.
        /// </summary>
        public string LicenseDesc
        {
            get
            {
                return licenseDesc;
            }
            set
            {
                licenseDesc = value;
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
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void PopulateFrom(BaseDbRecord source)
        {
            if(!PopulateBaseFrom(source))
            {
                return;
            }
            this.LicenseDesc = ((LicenseRecord)source).LicenseDesc;
            this.ContactName = ((LicenseRecord)source).ContactName;
            this.ContactAddress = ((LicenseRecord)source).ContactAddress;
            this.ContactCity = ((LicenseRecord)source).ContactCity;
            this.ContactPhone = ((LicenseRecord)source).ContactPhone;
            this.ContactEMail = ((LicenseRecord)source).ContactEMail;
            this.LicenseFeatures = ((LicenseRecord)source).LicenseFeatures;
        }

    }
}
