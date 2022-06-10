using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using AbleCheckbook.Logic;
using AbleLicensing;
using static AbleCheckbook.Logic.Logger;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// User levels.
    /// </summary>
    public enum UserLevel
    {
        Unlicensed = 0,
        Deactivated = 1,
        Standard = 2,
        ProCPA = 4,
        SuperUser = 8,
    }

    /// <summary>
    /// Singleton, exposes settings.
    /// </summary>
    public class Configuration : AbleLicensing.SiteSettings
    {

        private static Configuration _instance = null;

        private string[] _legalFilenames = new string[] { "Checking", "Business", "Personal", "Alternate" };
        private string _wsUrlOverride = "";
        private string _directoryLogs = "";
        private string _directoryDatabase = "";
        private string _directorySupportFiles = "";
        private string _directoryConfiguration = ""; // cannot be reconfigured
        private string _directoryImportExport = "";
        private string _directoryBackup1 = "";
        private string _directoryBackup2 = "";
        private string _lastDbName = "";
        private int _postEventAdvanceDays = 30;
        private LogLevel _logLevel = LogLevel.Diag;
        private string _loadedFrom = "";
        private bool _disableSanityChecks = false;
        private bool _suppressReconcileAlert = false;
        private bool _suppressYearEndAlert = false;
        private bool _twoAmountColumns = false;
        private bool _highVisibility = false;
        private bool _adminMode = false;
        private bool _showCalendars = true;
        private int _windowLeft = 0;
        private int _windowTop = 0;
        private int _windowWidth = 630;
        private int _windowHeight = 440;

        // Used by iSiteSettings overrides.
        private long _featuresBitmask = 0;
        private string _licenseCode = "";
        private string _activityTracking = "";
        private string _activationPin = "";

        // Getters/Setters
        public string DirectoryLogs { get => _directoryLogs; set => _directoryLogs = value; }
        public string DirectoryDatabase { get => _directoryDatabase; set => _directoryDatabase = value; }
        public string DirectorySupportFiles { get => _directorySupportFiles; set => _directorySupportFiles = value; }
        public string DirectoryConfiguration { get => _directoryConfiguration; set => _directoryConfiguration = value; }
        public string DirectoryImportExport { get => _directoryImportExport; set => _directoryImportExport = value; }
        public string DirectoryBackup1 { get => _directoryBackup1; set => _directoryBackup1 = value; }
        public string DirectoryBackup2 { get => _directoryBackup2; set => _directoryBackup2 = value; }
        public int PostEventAdvanceDays { get => _postEventAdvanceDays; set => _postEventAdvanceDays = value; }
        public string LoadedFrom { get => _loadedFrom; set => _loadedFrom = value; }
        public string LastDbName { get => _lastDbName; set => _lastDbName = value; }
        public bool DisableSanityChecks { get => _disableSanityChecks; set => _disableSanityChecks = value; }
        public bool HighVisibility { get => _highVisibility; set => _highVisibility = value; }
        public bool ShowCalendars { get => _showCalendars; set => _showCalendars = value; }
        public int WindowLeft { get => _windowLeft; set => _windowLeft = value; }
        public int WindowTop { get => _windowTop; set => _windowTop = value; }
        public int WindowWidth { get => _windowWidth; set => _windowWidth = value; }
        public int WindowHeight { get => _windowHeight; set => _windowHeight = value; }
        public LogLevel LogLevel { get => _logLevel; set => _logLevel = value; }

        /// <summary>
        /// Ctor - should be private, as this is a singleton, but you know, json serialization. 
        /// </summary>
        public Configuration()
        {
            if (_directoryConfiguration.Length < 1)
            {
                _directoryConfiguration =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACheckbook");
            }
        }

        /// <summary>
        /// The one-and-only instance.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!LoadFromFile())
                    {
                        _instance = new Configuration();
                        _instance.SetDefaults();
                    }
                    SaveFile();
                    _instance.Verify();
                    _instance._adminMode = false; // resets to false on successful load
                    if ((int)_instance._logLevel > (int)LogLevel.Diag) // defaults to, at least, Diag
                    {
                        _instance._logLevel = LogLevel.Diag;
                    }
                }
                return _instance;
            }
        }

        // Where is the config file?
        public static string PathToConfigurationFile()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "AbleStrategies\\ACheckbook\\preferences.cnf");
        }

        /// <summary>
        /// Saves the current configuration to a file after having called setters.
        /// </summary>
        public override void Save()
        {
            string prevLoadedFrom = _loadedFrom;
            _loadedFrom = "LoadedFromFile";
            SaveFile();
            _loadedFrom = prevLoadedFrom;
        }

        /// <summary>
        /// Legal root filenames to be appended with year and ".acb" and prepended wtih path info.
        /// </summary>
        public string[] GetLegalFilenames()
        {
            return _legalFilenames;
        }

        /// <summary>
        /// Is this is a licensed version?
        /// </summary>
        public bool GetIsLicensedVersion()
        {
#if DEBUG
            // dont proceed if in UNIT TEST mode
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies.Any(a => a.FullName.ToLowerInvariant().Contains("testplatform")) ||
                assemblies.Any(a => a.FullName.ToLowerInvariant().Contains("nunit.fraemwork"))) 
            {
                    return true;
            }
#endif
            return Activation.Instance.IsLicensed;
        }

        /// <summary>
        /// Get the locale code (i.e. en-US) for the current system.
        /// </summary>
        public string GetLocaleCode()
        {
            return GetLanguageCode() + "-" + GetRegionCode();
        }

        /// <summary>
        /// Get the language code (i.e. en) for the current system.
        /// </summary>
        public string GetLanguageCode()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        }

        /// <summary>
        /// Get the region code (i.e. US) for the current system.
        /// </summary>
        public string GetRegionCode()
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpper();
        }

        /// <summary>
        /// Allow user to perform dangerous changes?
        /// </summary>
        public bool GetAdminMode()
        {
            return _adminMode;
        }

        /// <summary>
        /// Allow user to perform dangerous changes?
        /// </summary>
        public void SetAdminMode(bool val)
        {
            _adminMode = val;
        }

        /// <summary>
        /// Set the log level. (Use this, not the setter, in order to have immediatre impact)
        /// </summary>
        public void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
            Logger.Instance.Level = _logLevel;
        }

        /// <summary>
        /// Display debit/credit instead of signed amount?
        /// </summary>
        public bool TwoAmountColumns
        {
            get
            {
                return _twoAmountColumns;
            }
            set
            {
                _twoAmountColumns = value;
            }
        }

        /// <summary>
        /// Suppress the alert that tells the user to reconcile?
        /// </summary>
        public bool SuppressReconcileAlert
        {
            get
            {
                return _suppressReconcileAlert;
            }
            set
            {
                _suppressReconcileAlert = value;
            }
        }

        /// <summary>
        /// Suppress the alert that tells the user to perform year-end wrap-up?
        /// </summary>
        public bool SuppressYearEndAlert
        {
            get
            {
                return _suppressYearEndAlert;
            }
            set
            {
                _suppressYearEndAlert = value;
            }
        }

        /// <summary>
        /// Save window bounds.
        /// </summary>
        /// <param name="left">x of left</param>
        /// <param name="top">y of top</param>
        /// <param name="width">width of window</param>
        /// <param name="height">height of window</param>
        public void SetWindowBounds(int left, int top, int width, int height)
        {
            _windowLeft = left;
            _windowTop = top;
            _windowWidth = width;
            _windowHeight = height;
        }

        /// <summary>
        /// Restore window bounds.
        /// </summary>
        /// <param name="left">x of left</param>
        /// <param name="top">y of top</param>
        /// <param name="width">width of window</param>
        /// <param name="height">height of window</param>
        public void GetWindowBounds(out int left, out int top, out int width, out int height)
        {
            left = _windowLeft;
            top = _windowTop;
            width = _windowWidth;
            height = _windowHeight;
        }

        /// <summary>
        /// Get the path and name of a support file based on locale. i.e. "custom", "en-US", "en-*".
        /// </summary>
        /// <param name="baseName">Name of existing file with no path, with a ### rep-symbol in it.</param>
        /// <returns>Full path to support file. May be non-existant file if it cannot be found.</returns>
        public string FindSupportFilePath(string baseName)
        {
            string path = Path.Combine(DirectorySupportFiles, baseName.Replace("###", "custom"));
            if (File.Exists(path))
            {
                return path; // i.e. C:/supportFiles/thisfile-custom.txt
            }
            path = Path.Combine(DirectorySupportFiles, baseName.Replace("###", GetLocaleCode()));
            if (File.Exists(path))
            {
                return path; // i.e. C:/supportFiles/thisfile-en-US.txt
            }
            foreach (string currentFilename in Directory.EnumerateFiles(DirectorySupportFiles,
                baseName.Replace("###", GetLanguageCode() + "*"), SearchOption.TopDirectoryOnly))
            {
                return currentFilename; // i.e. C:/supportFiles/thisfile-en-UK.txt
            }
            path = Path.Combine(DirectorySupportFiles, baseName.Replace("###", GetRegionCode()));
            return Path.Combine(DirectorySupportFiles, baseName); // else leave ### in place
        }

        /// <summary>
        /// Get the license level. i.e. Unlicensed, Basic, SuperUser.
        /// </summary>
        /// <returns></returns>
        public UserLevel GetUserLevel()
        {
            bool isLicensed = GetIsLicensedVersion();
            if (!isLicensed || _licenseCode == null || _licenseCode.Length < 3 || _licenseCode.ToUpper().Contains("UNLICENSED"))
            {
                return UserLevel.Unlicensed;
            }
            // We should augment this with Activation.IsFeatureEnabled().
            switch ((int)_licenseCode[6])
            {
                case (int)UserLevelPunct.Standard:
                    return UserLevel.Standard;
                case (int)UserLevelPunct.Deactivated:
                    return UserLevel.Deactivated;
                case (int)UserLevelPunct.ProCPA:
                    return UserLevel.ProCPA;
                case (int)UserLevelPunct.SuperUser:
                    return UserLevel.SuperUser;
            }
            return UserLevel.Unlicensed;
        }

        /// <summary>
        /// Set up configuration with defaults.
        /// </summary>
        private void SetDefaults()
        {
            String backup2DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "G:/My Drive");
            _directoryLogs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACheckbook");
            _directoryImportExport = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _directorySupportFiles = AppContext.BaseDirectory;
            _directoryDatabase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACheckbook");
            _directoryBackup1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACheckbook");
            _directoryBackup2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACheckbook/Backups");
            if(File.Exists(backup2DefaultPath))
            {
                _directoryBackup2 = Path.Combine(backup2DefaultPath, "/ACheckbook");
            }
            _postEventAdvanceDays = 30;
            _logLevel = LogLevel.Diag;
            _licenseCode = Strings.Get("(Unlicensed)");
            _showCalendars = true;
            _suppressReconcileAlert = false;
            _suppressYearEndAlert = false;
            _loadedFrom = "LoadedDefaults";
            GetUserLevel();
        }

        /// <summary>
        /// Set the base directory to something other than "~/ACheckbook"
        /// </summary>
        /// <param name="baseDirectory">Full path to the desired directory</param>
        /// <returns>true on success</returns>
        public bool SetBaseDirectory(string baseDirectory)
        {
            if (baseDirectory == null || baseDirectory.Trim().Length < 1)
            {
                return false;
            }
            bool isBackupsInBaseDir = _directoryBackup2.Contains(_directoryDatabase);
            DirectoryInfo info = Directory.CreateDirectory(Path.Combine(baseDirectory, ""));
            if (info.Exists && info.Attributes.HasFlag(FileAttributes.Directory))
            {
                _directoryLogs = Path.Combine(baseDirectory, "");
                _directoryDatabase = Path.Combine(baseDirectory, "");
                _directoryBackup1 = Path.Combine(baseDirectory, "");
                if (isBackupsInBaseDir)
                {
                    _directoryBackup2 = Path.Combine(baseDirectory, "Backups");
                }
                Verify();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verify that this is a licensed version.
        /// </summary>
        public void Verify()
        {
            GetUserLevel();
            Directory.CreateDirectory(DirectoryConfiguration);
            Directory.CreateDirectory(DirectoryBackup1);
            Directory.CreateDirectory(DirectoryBackup2);
            Directory.CreateDirectory(DirectoryDatabase);
            Directory.CreateDirectory(DirectoryImportExport);
            Directory.CreateDirectory(DirectoryLogs);
        }

        /// <summary>
        /// Load self from a file.
        /// </summary>
        /// <returns>success</returns>
        private static bool LoadFromFile()
        {
            try
            {
                string filename = PathToConfigurationFile();
                if (File.Exists(filename))
                {
                    using (FileStream stream = File.OpenRead(filename))
                    {
                        _instance = JsonSerializer.DeserializeAsync<Configuration>(stream).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            if (_instance == null)
            {
                return false;
            }
            return _instance._loadedFrom.Contains("Loaded");
        }

        /// <summary>
        /// Save self to a diagnostic file.
        /// </summary>
        private static void SaveFile()
        {
            try
            {
                string filename = PathToConfigurationFile();
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                using (FileStream stream = File.Create(filename))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    JsonSerializer.SerializeAsync<Configuration>(stream, _instance, options).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                // ignore any problems
            }
        }

        ///////////////////////////// SiteSettings ///////////////////////////

        /// <summary>
        /// Hook for Activation to call the logger with a DIAG-level mesage.
        /// </summary>
        /// <param name="message">To be logged</param>
        public override void LoggerHook(string message)
        {
            Logger.Diag(message);
        }

        /// <summary>
        /// URL override for calling MASTER web services
        /// </summary>
        public override string WsUrlOverride
        {
            get
            {
                return _wsUrlOverride;
            }
        }

        /// <summary>
        /// Fetch the product key, a unique string for each application. (typically the app name)
        /// </summary>
        public override string MfrAndAppName
        {
            get
            {
                return "AbleStrategies/ACheckbook";
            }
        }

        /// <summary>
        /// 12-character license code. (set by app or by Activation class)
        /// </summary>
        public override string LicenseCode
        {
            get
            {
                return _licenseCode;
            }
            set
            {
                _licenseCode = value;
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
                return _featuresBitmask;
            }
            set
            {
                _featuresBitmask = value;
            }
        }

        /// <summary>
        /// Encoded days-remaining. (set by Activation class)
        /// </summary>
        public override string ActivityTracking
        {
            get
            {
                return _activityTracking;
            }
            set
            {
                _activityTracking = value;
            }
        }

        /// <summary>
        /// Persisted activation PIN. May be set by the app (prompt user for pin) or by Activation. (web service)
        /// </summary>
        public override string ActivationPin
        {
            get
            {
                return _activationPin;
            }
            set
            {
                _activationPin = value;
            }
        }

    }

}
