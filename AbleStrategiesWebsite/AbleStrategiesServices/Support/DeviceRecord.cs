using AbleLicensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public class DeviceRecord : BaseDbRecord
    {

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        private Guid fkLicenseId = Guid.Empty;

        /// <summary>
        /// Site ID (device) abbreviation.
        /// </summary>
        private string deviceSite = "";

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        private UserLevelPunct userLevelPunct = UserLevelPunct.Unlicensed;

        /// <summary>
        /// Binary authentication/authorization info.
        /// </summary>
        private string codesAndPin = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public DeviceRecord() : base()
        {
            
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
        /// Foreign key - license.
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
        /// Site abbreviation.
        /// </summary>
        public string DeviceSite
        {
            get
            {
                return deviceSite;
            }
            set
            {
                deviceSite = value;
                Mod();
            }
        }

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        public UserLevelPunct UserLevelPunct
        {
            get
            {
                return userLevelPunct;
            }
            set
            {
                userLevelPunct = value;
                Mod();
            }
        }

        /// <summary>
        /// Binary authentication/authorization info.
        /// </summary>
        public string CodesAndPin
        {
            get
            {
                return codesAndPin;
            }
            set
            {
                codesAndPin = value;
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
            this.FkLicenseId = ((DeviceRecord)source).FkLicenseId;
            this.DeviceSite = ((DeviceRecord)source).DeviceSite;
            this.UserLevelPunct = ((DeviceRecord)source).UserLevelPunct;
            this.CodesAndPin = ((DeviceRecord)source).CodesAndPin;
        }

    }
}
