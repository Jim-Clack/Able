using System;

namespace AbleStrategiesServices.Support
{
    public class LicenseRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record type discriminator.
        /// </summary>
        public string desc = "";

        /// <summary>
        /// Contact name.
        /// </summary>
        public string contactName = "";

        /// <summary>
        /// Street address.
        /// </summary>
        public string contactAddress = "";

        /// <summary>
        /// Contact phone.
        /// </summary>
        public string contactPhone = "";

        /// <summary>
        /// Contact email.
        /// </summary>
        public string contactEMail = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public LicenseRecord()
        {
        }

        /// <summary>
        /// Description of record.
        /// </summary>
        public string Desc
        {
            get
            {
                return desc;
            }
            set
            {
                desc = value;
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
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void Update(BaseDbRecord source)
        {
            if(!UpdateBase(source))
            {
                return;
            }
            this.Desc = ((LicenseRecord)source).Desc;
            this.ContactName = ((LicenseRecord)source).ContactName;
            this.ContactAddress = ((LicenseRecord)source).ContactAddress;
            this.ContactPhone = ((LicenseRecord)source).ContactPhone;
            this.ContactEMail = ((LicenseRecord)source).ContactEMail;
        }

    }
}
