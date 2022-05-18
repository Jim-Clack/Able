using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public class PurchaseRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record type discriminator.
        /// </summary>
        public string _desc = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="desc"></param>
        public PurchaseRecord(string desc)
        {
            _desc = desc;
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
                return LicenseRecord.GetRecordKind();
            }
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void Update(BaseDbRecord source)
        {
            if (!UpdateBase(source))
            {
                return;
            }
            this.Desc = ((LicenseRecord)source).Desc;
            // TODO
        }

    }
}
