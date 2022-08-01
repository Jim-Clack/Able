using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace AbleLicensing
{

    /// <summary>
    /// Delimiter in LicenseCode corresponds to user level in Configuration.
    /// </summary>
    public enum UserLevelPunct
    {
        Unlicensed = 0,
        Deactivated = (int)'–', // note: en-dash, not a hyphen
        Standard = (int)'-',
        ProCPA = (int)'&',
        SuperUser = (int)'@',
    }

    /// <summary>
    /// Web service API states (both request and response states)
    /// </summary>
    public enum ApiState
    {
        Unknown = 0,
        // Requests that Pass Minimal Info
        FuzzyLookup = 1,             // Find by fuzzy match on license code, addr, phone, etc.
        LookupLicense = 2,           // Find by license code
        // Requests that expect info, as is known, to be populated
        UpdateInfo = 3,              // Change addr, phone, email, etc
        ChangeFeature = 4,           // Change the feature mask
        ChangeLevel = 5,             // Change permission level (and site id)
        // Requests that license host devices
        MakePurchase = 11,           // Complete the purchase
        AddlDevice = 12,             // Activate add'l device on same license at no charge
        // Successful Non-Purchase Responses
        ReturnOk = 20,               // Completed non-purchase okay, return PinNumber
        ReturnOkAddlDev = 21,        // Purchase ok, no charge, existing lic, return PinNumber, SiteId
        ReturnNotActivated = 22,     // Not activated, no paid license found, return Message
        ReturnDeactivate = 23,       // Too many devices, deactive, return Message
        // Failed Non-Purchase Responses
        ReturnBadArg = 31,           // Invalid city, phone, email, etc, return Message
        ReturnNotFound = 32,         // License not found, return Message
        ReturnNotMatched = 33,       // Name or other incorrect, return Message
        // Purchase Responses
        PurchaseOk = 40,             // Purchase went thru, return PinNumber, SiteId, LicCode
        PurchaseOkUpgrade = 41,      // Purchase ok, upgrade fee, existing lic, return PinNumber, SiteId
        PurchaseFailed = 42,         // Purchase failed, return Message, LicCode
        PurchaseOkOtherFailed = 43,  // Purchase went thru but something else failed, return Message
    }

    /// <summary>
    /// Because this is deliberately obfuscated, you'll have to read the code to figure out some things.
    /// </summary>
    /// <remarks>
    /// This is not intended to be uncrackable at all, but just to make it nearly impossible for 
    /// everyday hackers and script-kiddies to break. Do not make it more difficult than it already
    /// is because that makes it difficult to maintain. In the future we may switch to a commercial
    /// activation/licensing system. The sequence in which API's are called is critical. See below.
    /// Usage:
    ///  Activation act = Activation.Instance;
    ///  act.SetDefaultDays(92, 183);
    ///  act.LicenseCode = "JonDoe-60606";
    ///  string pin = act.ResetAllEntries(act.ChecksumOfString(act.SiteIdentification));
    ///  act.SetActivationPin(pin);
    ///  // And optionally...
    ///  act.SetFeatureBitmask(0x000000000000000FL, act.ChecksumOfString(act.SiteIdentification));
    ///  act.SetExpiration(92, act.ChecksumOfString(act.SiteIdentification));
    /// Licensing Fields and Methods:
    ///  SiteId: This identifies the host by IP/Domain/Hdwr and may not be unique.
    ///  LCode: Uniquely identifies the user by nickname/location as well as his/her license level.
    ///  Purch: Uniquely identifies the transaction in which a license was purchased.
    ///  PIN: Calculated per a specific combination of SiteId and LCode above.
    ///  ActivityTracking: Scratch area used by Activation.
    ///  ChecksumOfString(siteId): creates repeatable scramble for encoding/decoding
    ///  Feature: Future use, ignored for now.
    /// Notes:
    /// - Be sure to call dummy method VerifyPin() in key places to act as a hacker distraction.
    /// - Thre must be Different SiteSettings implementations for the client and the server.
    /// - There must be exactly one class in the entry assembly that implements SiteSettings.
    /// - Client SiteSettings implementation must persist and restore all data from setters.
    /// - SiteSettings class should return the same MfrAndAppName as is used during installation.
    /// - The main steps in activation are setting the LicenseCode and ActivationPin.
    /// - To check "is licensed": if(Activation.Instance.IsLicensed) ...
    /// - LicenseCode is typically 12-chars: 6-char name, 1-char hyphen/punct, 5-char location
    /// - Calc activation PIN: string pin = ResetAllEntries(ChecksumOfString(SiteIdentification));
    /// - PIN must be set correctly before setting features or expiration
    /// - i.e. features: SetFeatureBitmask((int)(MyFeatures.B | MyFeatures.E)); where B=2 and E=16
    /// - Check feature: if(Activation.Instance.IsFeatureEnabled((int)MyFeatures.C)...
    /// - Check expiration: int days = UpdateSiteSettings(); note: returns -1 if non-expiring
    /// - Each site is uniquely ID'd by the combination of siteIdentification and licenseCode
    /// - Each site is tracked as a site (possibly many-to-one) from a purchase val code
    /// - When a new site is activated, the most latent one on that purchase gets deactivated
    /// - A Purch# (Purchase Validation Code) is a "P" followed by the PayPal trasnsaction number
    /// </remarks>
    public class Activation
    {

        /// <summary>
        /// How many seconds must pass between local activation attempts.
        /// </summary>
        private const int SecondsBetweenAttempts = 10;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static Activation _instance = null;

        /// <summary>
        /// Our connection to the configuration/preferences class.
        /// </summary>
        private SiteSettings _siteSettings = null;

        /// <summary>
        /// Seed for randoms.
        /// </summary>
        private int _counter = DateTime.Now.Day;

        /// <summary>
        /// Default number of days that the app can be used before it expires.
        /// </summary>
        private int _defaultDaysOfUse = 92;

        /// <summary>
        /// Default number of days that the app can be installed before it expires.
        /// </summary>
        private int _defaultDaysSinceInstalled = 183;

        /// <summary>
        /// Optimization so it doesn't need to be rechecked too often.
        /// </summary>
        private bool _isLicensed = false;

        /// <summary>
        /// When did we last attempt to activate locally?
        /// </summary>
        private DateTime _lastAttempt = DateTime.Now.AddDays(-1);

        /// <summary>
        /// Expose site settings.
        /// </summary>
        public SiteSettings SiteSettings { get => _siteSettings; }

        /// <summary>
        /// Ctor.
        /// </summary>
        private Activation()
        {
            _siteSettings = null;
            // This throws an exception if run in DEBUG mode, of course!
            Type[] clazzes = Assembly.GetEntryAssembly().GetExportedTypes();
            // Get the instance of the SiteSettings implementation
            foreach (Type clazz in clazzes)
            {
                if(typeof(SiteSettings).IsAssignableFrom(clazz))
                {
                    if(SiteSettings != null)
                    {
                        LoggerHook("[V5a] Activation() ERROR - " + SiteSettings.GetType().Name + " " + clazz.Name);
                        throw new ApplicationException("More than one class implements SiteSettings");
                    }
                    PropertyInfo property = clazz.GetProperty("Instance", 
                        BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public);
                    MethodInfo method = property.GetGetMethod();
                    _siteSettings = (SiteSettings)method.Invoke(null, null);
                }
            }
            if (SiteSettings == null)
            {
                LoggerHook("[V5b] Activation() ERROR - No class found that implements SiteSettings");
                throw new ApplicationException("No class found that implements SiteSettings");
            }
        }

        /// <summary>
        /// Singleton - get Instance.
        /// </summary>
        public static Activation Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Activation();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Hook for to call the logger with a DIAG-level mesage.
        /// </summary> 
        /// <param name="message">To be logged</param>
        public void LoggerHook(string message)
        {
            SiteSettings.LoggerHook(message);
        }

        /// <summary>
        /// Set these default values - to be used if not deliberately set otherwise. 
        /// </summary>
        /// <param name="daysOfUse"></param>
        /// <param name="daysSinceInstallation"></param>
        public void SetDefaultDays(int daysOfUse, int daysSinceInstallation)
        {
            _defaultDaysOfUse = daysOfUse;
            _defaultDaysSinceInstalled = daysSinceInstallation;
        }

        /// <summary>
        /// Too many copies made from this license, so de-activate it.
        /// </summary>
        public void DeActivate()
        {
            SiteSettings.LicenseCode = SiteSettings.LicenseCode.Substring(0, 5) +
                (char)UserLevelPunct.Deactivated + SiteSettings.LicenseCode.Substring(7);
            LoggerHook("[W6] DeActivate() " + SiteSettings.LicenseCode);
            SiteSettings.Save();
        }

        /// <summary>
        /// Is this a licensed copy? (Is PIN valid?)
        /// </summary>
        /// <returns>true if licensed</returns>
        public bool IsLicensed
        {
            get
            {
                if (SiteSettings == null)
                {
                    return false;
                }
                if(_isLicensed)
                {
                    return true;
                }
                long verificationCode = ChecksumOfString(SiteIdentification);
                LoggerHook("[X7] IsLicensed() " + verificationCode);
                string pin = CalculatePin(verificationCode);
                bool ok = CheckVerificationCode(verificationCode, _lastAttempt.Ticks); // no-op: hacker diversion
                _isLicensed = (pin == SiteSettings.ActivationPin);
                ok = !VerifyPin(verificationCode); // no-op: hacker diversion
                pin = XorString(SiteIdentification + ok.ToString());  // no-op: hacker diversion
                if(pin.Length < 1)  // no-op: hacker diversion (never happens)
                {
                    _siteSettings = null;
                }
                return _isLicensed;
            }
        }

        /// <summary>
        /// Get a brief string that identifies this computer and its hardware uniquely.
        /// </summary>
        public string SiteIdentification
        {
            get
            {
                string machineName = Environment.MachineName.Trim();
                if (machineName.Length > 6) // first and last 3 characters only
                {
                    machineName = machineName.Substring(0, 3) + machineName.Substring(machineName.Length - 3);
                }
                string siteIdentification = "" + Environment.ProcessorCount + 
                    machineName + RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpper();
                char[] array = siteIdentification.ToCharArray();
                Array.Reverse(array);
                siteIdentification = new String(array);
                return siteIdentification;
            }
        }

        /// <summary>
        /// Set activity tracking for expiration. (to be called by a web service client)
        /// </summary>
        /// <param name="daysRemaining">Days remaining before expiration</param>
        /// <param name="verificationCode">Calculated code: checksum of chars in siteIdentification</param>
        public void SetExpiration(int daysRemaining, long verificationCode)
        {
            if (!CheckVerificationCode(ChecksumOfString(SiteIdentification), verificationCode))
            {
                return;
            }
            ClampAtDaysSinceInstallation(daysRemaining, DateTime.Now, true); // reset days-since-installed
            SiteSettings.ActivityTracking = XorString(
                daysRemaining.ToString() + "||" + DateTime.Now.ToShortDateString());
            SiteSettings.Save();
        }

        /// <summary>
        /// Set ActivationPin. (to be called by a web service client)
        /// </summary>
        /// <param name="pin">Activation PIN</param>
        public void SetActivationPin(string pin)
        {
            SiteSettings.ActivationPin = pin;
            SiteSettings.Save();
        }

        /// <summary>
        /// License Code. (12-character user6+hyphen+zip5)
        /// </summary>
        public string LicenseCode
        {
            get
            {
                return SiteSettings.LicenseCode;
            }
            set
            {
                SiteSettings.LicenseCode = value;
                SiteSettings.Save();
            }
        }
        /// <summary>
        /// Set FeaturesBitmask. (to be called by a web service client)
        /// </summary>
        /// <param name="featureBitmask">Bitmask of enabled features, typically per an external enum</param>
        /// <param name="verificationCode">Calculated code: checksum of chars in siteIdentification</param>
        public void SetFeatureBitmask(long featureBitmask, long verificationCode)
        {
            if (!CheckVerificationCode(ChecksumOfString(SiteIdentification), verificationCode))
            {
                return;
            }
            SiteSettings.FeaturesBitMask = TranslateLong(featureBitmask);
            SiteSettings.Save();
        }

        /// <summary>
        /// Get FeaturesBitmask. (to be called by a web service client)
        /// </summary>
        /// <param name="verificationCode">Calculated code: checksum of chars in siteIdentification</param>
        /// <returns>unencrypted feature bitmask, 0 if none</returns>
        public long GetFeatureBitmask(long verificationCode)
        {
            if (!CheckVerificationCode(ChecksumOfString(SiteIdentification), verificationCode))
            {
                return 0L;
            }
            return TranslateLong(SiteSettings.FeaturesBitMask);
        }

        /// <summary>
        /// Use the ordinals of an enum to check for enabled features.
        /// </summary>
        /// <param name="featureBitmask">Or'ed feature numbers bitmask, i.e. 4 | 32</param>
        /// <returns>true if feature is enabled AND this is a licensed version</returns>
        public bool IsFeatureEnabled(long featureBitmask)
        {
            long mask = TranslateLong(SiteSettings.FeaturesBitMask);
            return (featureBitmask & mask) == featureBitmask;
        }

        /// <summary>
        /// Get the checksum of a string with a random offset 100, 200, 300, 9300 added to it.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>funny checksum</returns>
        public long ChecksumOfString(string target)
        {
            long checksum = 0;
            foreach (char ch in target)
            {
                checksum += (byte)ch;
            }
            _counter = (_counter + 37) % 93;
            return checksum + 100 + (_counter * 100);
        }

        /// <summary>
        /// Dummy method to distract would-be hackers.
        /// </summary>
        /// <param name="pin">garbage</param>
        /// <returns>who knows?</returns>
        public bool VerifyPin(long pin)
        {
            return pin.ToString().CompareTo(DateTime.Now.ToString()) != 0;
        }

        /// <summary>
        /// Dummy method to distract would-be hackers.
        /// </summary>
        /// <returns>who knows?</returns>
        public string CalculatePin()
        {
            string pinString = SiteSettings.ActivationPin;
            long pinNumber = 1234;
            if(long.TryParse(pinString, out pinNumber))
            {
                pinString = TranslateLong(pinNumber).ToString();
            }
            if (!long.TryParse(pinString, out pinNumber))
            {
                pinString = SiteIdentification;
                pinString = XorString(pinString);
            }
            pinString = TranslateLong(pinNumber).ToString();
            SiteSettings.ActivationPin = pinString;
            return pinString;
        }

        /// <summary>
        /// CalculatePin().
        /// </summary>
        /// <param name="verificationCode">Calculated code: checksum of chars in siteIdentification</param>
        /// <param name="sid">site identification, "" to use the current host's siteIdentification</param>
        /// <param name="lCode">license code, "" to use the current host's license code</param>
        /// <returns>the PIN - always 4 digits on success, "" on error</returns>
        public string CalculatePin(long verificationCode, string sid = "", string lCode = "")
        {
            if(sid.Trim().Length < 1)
            {
                if(DateTime.Now.Subtract(_lastAttempt).TotalSeconds < SecondsBetweenAttempts)
                {
                    _lastAttempt = DateTime.Now;
                    return "";
                }
                _lastAttempt = DateTime.Now;
                sid = SiteIdentification;
            }
            if (lCode.Trim().Length < 1)
            {
                lCode = SiteSettings.LicenseCode;
            }
            if (!CheckVerificationCode(ChecksumOfString(SiteIdentification), verificationCode))
            {
                return "";
            }
            // Simple PIN checksum based on machineName, region, #processors, and lCode
            string buffer = sid + lCode.Trim().ToUpper();
            int accumulator = 0;
            for (int index = 0; index < buffer.Length; ++index)
            {
                accumulator += ((int)buffer[index] % 32) * (1 + index % 32);
            }
            string pin = "" + (1000 + accumulator % 9000);
            LoggerHook("[Y8] ResetAllEntries() " + sid + " " + lCode + " " + pin);
            return pin;
        }

        /// <summary>
        /// GetExpirationDays().
        /// </summary>
        /// <returns>Number of days remaining before expiration, negative if expired. 10000 if licensed/non-expiring.</returns>
        /// <remarks>
        /// Expiration is based on the more pessimistic of two metrics:
        ///  1. The number of dates seen/tracked by UpdateSiteSettings()
        ///  2. Limited by the age of a dummy AppData file creation-date
        /// Note:
        ///  Until it is down to 30 days or less, do not tell the user how 
        ///  many days remain (just say that it is time-limited) because there
        ///  may be a sudden jump due to the fact that there are 2 metrics, as
        ///  is listed above, and one may run past the other.
        /// </remarks>
        public int GetExpirationDays()
        {
            if (IsLicensed)
            {
                return 10000;
            }
            string tracking = XorString(SiteSettings.ActivityTracking).Trim();
            LoggerHook("[Z9a] UpdateSiteSettings() " + tracking);
            string[] daysAndDate = tracking.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            int daysRemaining = _defaultDaysOfUse;
            // populate daysRemaining and lastChecked from activity tracking data
            if (daysAndDate.Length > 0)
            {
                if (!int.TryParse(daysAndDate[0], out daysRemaining))
                {
                    daysAndDate = new string[0];
                }
            }
            if (daysAndDate.Length < 1)
            {
                daysAndDate = new string[] { daysRemaining.ToString(), DateTime.Now.AddDays(-1).ToShortDateString() };
            }
            else if (daysAndDate.Length < 2)
            {
                daysAndDate = new string[] { daysAndDate[0].ToString(), DateTime.Now.AddDays(-1).ToShortDateString() };
            }
            if (!int.TryParse(daysAndDate[0], out daysRemaining))
            {
                return 0; // error - corrupt data
            }
            DateTime lastChecked = DateTime.Now.AddDays(-_defaultDaysSinceInstalled); // default to worst-case
            DateTime.TryParse(daysAndDate[1], out lastChecked);
            if (lastChecked.Date != DateTime.Now.Date)
            {
                --daysRemaining;
            }
            // if dummy file is about a year old, trim the days remaining down
            daysRemaining = ClampAtDaysSinceInstallation(daysRemaining, lastChecked);
            // Save results
            SiteSettings.ActivityTracking = XorString(
                daysRemaining.ToString() + "||" + DateTime.Now.ToShortDateString());
            SiteSettings.Save();
            LoggerHook("[Z9b] UpdateSiteSettings() " + daysRemaining.ToString() + "||" + DateTime.Now.ToShortDateString());
            return daysRemaining;
        }

        /// <summary>
        /// ClampAtDaysSinceInstallation, based on days since app was installed.
        /// </summary>
        /// <param name="daysRemaining">Current expectation of days remaininng </param>
        /// <param name="lastChecked">when was the expiration last checked?</param>
        /// <param name="restart">true to restart the app-installed countdown at zero</param>
        /// <returns>daysRemaining, possibly updated based on how long the app has been installed</returns>
        private int ClampAtDaysSinceInstallation(int daysRemaining, DateTime lastChecked, bool restart = false)
        {
            string checkFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    SiteSettings.MfrAndAppName + "\\settings.cnf");
            if(restart)
            {
                File.Delete(checkFilePath);
            }
            if (File.Exists(checkFilePath))
            {
                FileInfo fileInfo = new FileInfo(checkFilePath);
                if (lastChecked.Subtract(fileInfo.CreationTime.Date) > TimeSpan.FromDays(_defaultDaysSinceInstalled - 30))
                {
                    daysRemaining = Math.Min(daysRemaining, 30);
                }
            }
            else
            {
                File.WriteAllText(checkFilePath, "0Z" + SiteIdentification + "T");
            }
            return daysRemaining;
        }

        /// <summary>
        /// Check a verification code
        /// </summary>
        /// <param name="value">to be tested</param>
        /// <param name="verificationCode">code to use in verifying it</param>
        /// <returns>true if valid (verificationCode ends in the same last two digits as the value)</returns>
        private bool CheckVerificationCode(long value, long verificationCode)
        {
            return (100 + value % 100 - verificationCode % 100) % 100 == 0;
        }

        /// <summary>
        /// Reversibly XOR the characters in a string with 0x0A to hide their true value
        /// </summary>
        /// <param name="inString">input text</param>
        /// <returns>XOR'ed result</returns>
        private string XorString(string inString)
        {
            StringBuilder buffer = new StringBuilder();
            foreach(char ch in inString)
            {
                buffer.Append((char)((((int)ch) & 0xFF) ^ 0x0A));
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Deceptive name: Reversibly XOR a value with another value that is based on the Activation PIN.
        /// </summary>
        private long TranslateLong(long value)
        {
            long keyValue = 0x0E1D2C3B4A59687L;
            long.TryParse(SiteSettings.ActivationPin, out keyValue);
            keyValue += 0x123456789ABCDEF;
            return value ^ keyValue;
        }

    }

}
