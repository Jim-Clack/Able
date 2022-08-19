using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbleLicensing;

namespace AbleStrategiesServices.Support
{

    /// <summary>
    /// Stub for activation code.
    /// </summary>
    public class ServerSettings : AbleLicensing.SiteSettings
    {

        private static ServerSettings _instance = null;

        /// <summary>
        /// Ctor - should be private, as this is a singleton, but you know, json serialization. 
        /// </summary>
        private ServerSettings()
        {
        }

        /// <summary>
        /// The one-and-only instance.
        /// </summary>
        public static ServerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerSettings();
                    Logger.Diag(null, "Instantiating ServerSettings");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Saves the current configuration to a file after having called setters.
        /// </summary>
        public override void Save()
        {
            Logger.Diag(null, "Shouldn't Happen 0");
        }

        /// <summary>
        /// Hook for Activation to call the logger with a DIAG-level mesage.
        /// </summary>
        /// <param name="message">To be logged</param>
        public override void LoggerHook(string message)
        {
            Logger.Diag(null, message);
        }

        /// <summary>
        /// URL override for calling MASTER web services
        /// </summary>
        public override string WsUrlOverride
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 1");
                return "XXX1XXX";
            }
        }

        /// <summary>
        /// Fetch the product key, a unique string for each application. (typically the app name)
        /// </summary>
        public override string MfrAndAppName
        {
            get
            {
                return "AbleStrategies/Services";
            }
        }

        /// <summary>
        /// 12-character license code. (set by app or by Activation class)
        /// </summary>
        public override string LicenseCode
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 2");
                return "XXX2XXX";
            }
            set
            {
                Logger.Diag(null, "Shouldn't Happen 3");
            }
        }

        /// <summary>
        /// Bitmask of enabled features. (set by app or by Activation class)
        /// </summary>
        /// <remarks>
        /// Currently no features are set by this capability. Instead the 6th character of
        /// the license code is used to set the User Level.
        /// </remarks>
        public override long FeaturesBitMask
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 4");
                return 4000000L;
            }
            set
            {
                Logger.Diag(null, "Shouldn't Happen 5");
            }
        }

        /// <summary>
        /// Encoded days-remaining. (set by Activation class)
        /// </summary>
        public override string ActivityTracking
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 6");
                return "XXX6XXX";
            }
            set
            {
                Logger.Diag(null, "Shouldn't Happen 7");
            }
        }

        /// <summary>
        /// For storing contact and site data. 
        /// </summary>
        public override string[] ContactValues
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 8");
                return new string[] { "XXX8XXX" };
            }
            set
            {
                Logger.Diag(null, "Shouldn't Happen 8");
            }
        }

        /// <summary>
        /// Persisted activation PIN. May be set by the app (prompt user for pin) or by Activation. (web service)
        /// </summary>
        public override string ActivationPin
        {
            get
            {
                Logger.Diag(null, "Shouldn't Happen 9");
                return "XXX9XXX";
            }
            set
            {
                Logger.Diag(null, "Shouldn't Happen 9");
            }
        }

    }

}
