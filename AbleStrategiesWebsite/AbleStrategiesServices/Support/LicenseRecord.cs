using System;

namespace AbleStrategiesServices.Support
{
    public class LicenseRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record type discriminator.
        /// </summary>
        public string _desc = "";

        /// <summary>
        /// Contact name.
        /// </summary>
        public string _contactName = "";

        /// <summary>
        /// Street address.
        /// </summary>
        public string _contactAddress = "";

        /// <summary>
        /// Contact phone.
        /// </summary>
        public string _contactPhone = "";

        /// <summary>
        /// Contact email.
        /// </summary>
        public string _contactEMail = "";

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
                return _desc;
            }
            set
            {
                _desc = value;
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
                return _contactName;
            }
            set
            {
                _contactName = value;
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
                return _contactAddress;
            }
            set
            {
                _contactAddress = value;
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
                return _contactPhone;
            }
            set
            {
                _contactPhone = value;
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
                return _contactEMail;
            }
            set
            {
                _contactEMail = value;
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
