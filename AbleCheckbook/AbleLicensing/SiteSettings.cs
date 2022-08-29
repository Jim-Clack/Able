using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing
{

    /// <summary>
    /// Base class for interface to product from Activation.
    /// </summary>
    /// <remarks>
    /// This class expects to be able to persist certain data through an implementation of 
    /// SiteSettings, which is usually a preferences/configuration class that gets saved to
    /// the registry or a file in AppData. Those settings are protected by an encryption key
    /// and a PIN that is persisted amongst them, so they cannot be mocified by the user.
    /// </remarks>
    public abstract class SiteSettings
    {

        /// <summary>
        /// Implementation must be a singleton.
        /// </summary>
        // public static SiteSettings Instance { get; } // commented-out static but must be implemented

        /// <summary>
        /// URL override for calling MASTER web services
        /// </summary>
        public abstract string WebServiceUrl { get; set; }

        /// <summary>
        /// Hook for Activation to call the logger with a DIAG-level mesage.
        /// </summary>
        /// <param name="message">To be logged</param>
        public abstract void LoggerHook(string message);

        /// <summary>
        /// Fetch the product key, a unique string for each application. (typically the app name)
        /// </summary>
        public abstract string MfrAndAppName { get; }

        /// <summary>
        /// 12-character license code. (set by app or by Activation class)
        /// </summary>
        public abstract string LicenseCode { get; set; }

        /// <summary>
        /// Bitmask of enabled features. (set by app or by Activation class)
        /// </summary>
        public abstract long FeaturesBitMask { get; set; }

        /// <summary>
        /// Encoded tracking data. (set and used by Activation class)
        /// </summary>
        public abstract string ActivityTracking { get; set; }

        /// <summary>
        /// Persisted activation PIN. May be set by the app (prompt user for pin) or by Activation. (web service)
        /// </summary>
        public abstract string ActivationPin { get; set;  }

        /// <summary>
        /// Save changes.
        /// </summary>
        public abstract void Save();

    }
}
