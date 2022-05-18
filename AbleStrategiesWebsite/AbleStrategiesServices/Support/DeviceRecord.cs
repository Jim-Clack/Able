using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public class DeviceRecord : BaseDbRecord
    {

        /// <summary>
        /// Unique record type discriminator.
        /// </summary>
        public string desc = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public DeviceRecord()
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
        /// [static] Unique record type discriminator.
        /// </summary>
        /// <returns>unique string discriminator</returns>
        public static string GetRecordKind()
        {
            return typeof(DeviceRecord).Name;
        }

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        public override string RecordKind
        {
            get
            {
                return DeviceRecord.GetRecordKind();
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
            this.Desc = ((DeviceRecord)source).Desc;
            // TODO
        }

    }
}
