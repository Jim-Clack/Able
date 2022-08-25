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
        private string deviceSiteId = "";

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        private int userLevelPunct = (int)(AbleLicensing.UserLevelPunct.Unlicensed);

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
        public string DeviceSiteId
        {
            get
            {
                return deviceSiteId;
            }
            set
            {
                deviceSiteId = value;
                Mod();
            }
        }

        /// <summary>
        /// Current status of device activation.
        /// </summary>
        public int UserLevelPunct
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
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DevRec{" + SupportMethods.Shorten(Id.ToString()) +
                "," + SupportMethods.Shorten(fkLicenseId.ToString()) +
                "," + userLevelPunct.ToString() +
                "," + deviceSiteId + "}";
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
            this.DeviceSiteId = ((DeviceRecord)source).DeviceSiteId;
            this.UserLevelPunct = ((DeviceRecord)source).UserLevelPunct;
            this.CodesAndPin = ((DeviceRecord)source).CodesAndPin;
        }

    }
}
