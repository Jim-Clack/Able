Setup
  Install Visual Studio 2019
  Open the solution
  UIn AbleStrategiesServices, update AbleLicensing reference, i.e. browse to:
    C:\Users\<user>\source\repos\Able\AbleCheckbook\AbleLicensing\bin\Release\netstandard2.0\AbleLicensing.dll
  If not already done, tell the IONOS installer to install .NET core: 
    Add the following line to AbleStrategiesWebsite.csproj <PropertyGoup> element:
      <PublishWithAspNetCoreTargetManifest>false</PublishWithAspNetCoreTargetManifest>
  Install WinSCP on your computer
  Note that VS w IIS Express defaults work directory to...
    \Users\USER\source\repos\Able\AbleStrategiesWebsite\UnitTestProject1\bin\Debug\netcoreappX.Y

Caveats
  Note: If the business expands, ASAP: rewrite the UserInfo DB to use a SQL server
  Rebuild AbleCheckbook (release & debug) if you alter AbleLicensing, or this will not build.
  The name of the userinfo DB is "users.json" in release mode but "debug.json" in debug mode.
  Uses ASP.NET Core, not ASP.NET 4.x - see below.

About ASP .NET Core
  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/choose-aspnet-framework?view=aspnetcore-6.0

Required Reading...
  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/?view=aspnetcore-6.0&tabs=windows#korh

Info on creating an ASMX web service
  https://www.patrickschadler.com/creating-a-rest-webservice-with-net-core/

Info on deploying .NET apps to Linux
  https://docs.microsoft.com/en-us/dotnet/core/deploying/#framework-dependent-deployments-fdd

Programmatic Connection to IONOS Server

  SessionOptions sessionOptions = new SessionOptions
  {
    Protocol = Protocol.Sftp,
    HostName = "access872699540.webspace-data.io",
    UserName = "u104847980",
    Password = "XXXXXXX.XX",
    SshHostKeyFingerprint = "ssh-ed25519 255 1gx2w8Rtv3wCgi7Jh8myf/KVd72cRQbow03UP8P095Q=",
  };

  using (Session session = new Session())
  {
    session.Open(sessionOptions);
    // etc.
  }

Notes for what must be added to the Help docs...
 - Reconcile-in-progress will maintain state across save/open cycles
 - To undo N-steps of reconcile: undo the last step, then Abandon Reconcile
 - Edits made during reconcile remain even if reconcile is abandoned
 - Translation/i18n, strings (i.e sequence doesn't matter), etc.
 - Licensing, UserLevel, Activation, Expiration, and Admin Mode
 - Time-limited: Days of use vs days since installed
 - Admin mode vs Standard/ProCPA vs SuperUser levels are Different Things
 - During reconcile, entry check-off "IsChecked" means "IsCleared Tentatively"
 - Technical details: ".adb", auto-save, rolling-backups, weekly-backups, etc.

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
    ///  LicCode: Uniquely identifies the user by nickname/location as well as his/her license level.
    ///  Purch: Uniquely identifies the transaction in which a license was purchased.
    ///  PIN: Calculated per a specific combination of Site Id and Lic Code above.
    ///  ActivityTracking: Scratch area used by Activation.
    ///  ChecksumOfString(siteId): creates repeatable scramble for encoding/decoding
    ///  Feature: Future use, ignored for now.
    ///  resetAllEntries(): fake name to foil hackers, really calculatePin()
    ///  updateSiteSettings(): fake name to foil hackers, really GetExpirationDays()
    /// Notes:
    /// - Be sure to call dummy method VerifyPin() in key places to act as a hacker distraction.
    /// - There must be Different SiteSettings implementations for the client and the server.
    /// - There must be exactly one class in the entry assembly that implements iSettings.
    /// - Client iSettings implementation must persist and restore all data from setters.
    /// - Ths iSettings class should return the same MfrAndAppName as is used during installation.
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


          /////////////////////// Web Service API Calls ////////////////////////

        /// <summary>
        /// Call the server to get remaining fields.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased (may be updated)</param>
        /// <param name="lCode">installation assigned license code ("" if unknown, will be filled-in)</param>
        /// <param name="purchase">validation code from the purchase ("" if unknown, may be filled-in)</param>
        /// <returns>The PIN, or null on error</returns>
        private bool CallServerForLicenseInfo(string addr, string city, string zip, 
            string phone, string email, ref string feature, ref string lCode, ref string purchase)
        {
            _serverErrorMessage = null; // "[A1] ..."
            bool okay = false;


            Activation.Instance.LoggerHook("[A1] CallServerForLicenseInfo() " + feature + " " + lCode + " " + purchase);
            _serverErrorMessage =
                "[A1] ???";
            return okay;
        }

        /// <summary>
        /// Confirm that a purchase has been paid for.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc that was purchased</param>
        /// <param name="lCode">installation assigned license code</param>
        /// <returns>purch val code, or null on error</returns>
        private string CallServerToRegisterPurchase(string addr, string city, string zip,
            string phone, string email, string feature, string lCode)
        {
            _serverErrorMessage = null; // "[B2] ..."
            string purchase = "";
            bool okay = false;


            Activation.Instance.LoggerHook("[B2] CallServerToRegisterPurchase() " + feature + " " + lCode + " " + purchase);
            if (okay && purchase != null && purchase.Trim().Length > 0)
            {
                return purchase;
            }
            _serverErrorMessage =
                "[B2] Your LCODE is " + lCode + " - please write it down. The purchase went thru but further " +
                "server communication failed, despite multiple attempts. Try again later, using the offline " +
                "method described on our website. We are very sorry, as this should not happen, but we too " +
                "are subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

        /// <summary>
        /// Call the server to get an activation PIN.
        /// </summary>
        /// <param name="addr">installation street address</param>
        /// <param name="zip">installation postal code</param>
        /// <param name="city">installation city</param>
        /// <param name="phone">installation phone number</param>
        /// <param name="email">installation email address</param>
        /// <param name="feature">installation edition/features/etc to be purchased, if necessary</param>
        /// <param name="lCode">installation assigned license code</param>
        /// <param name="purchase">validation code from the purchase</param>
        /// <returns>The PIN, or null on error</returns>
        private string CallServerForActivationPin(string addr, string city, string zip,
            string phone, string email, string feature, string lCode, string purchase)
        {
            _serverErrorMessage = null; // "[C3] ..."
            string purch = "";
            bool okay = false;


            Activation.Instance.LoggerHook("[C3] CallServerForActivationPin() " + feature + " " + lCode + " " + purchase);
            _serverErrorMessage =
                "[C3] Your LicCode is " + lCode + " - please write it down. The purchase went thru but activation " +
                "failed. Try again later, using the offline method as described on our website. We tried " +
                "a few times and we are very sorry, as this really should not happen, but we too are " +
                "subject to the unpredicable whims and fancies of cloud servers and the Internet itself.";
            return null;
        }

        /// <summary>
        /// Check to see if this site is still activated and populate pending userAlert as well.
        /// </summary>
        /// <param name="lCode">installation assigned license code</param>
        /// <param name="userAlert">populated with user alert, if one is pending.</param>
        /// <returns>ActivationStatus</returns>
        private ActivationStatus CallServerToVerifyActivation(string lCode, out string userAlert)
        {
            _serverErrorMessage = null; // "[D4] ..."
            ActivationStatus status = ActivationStatus.NetworkProblems;

            userAlert = "???";
            Activation.Instance.LoggerHook("[D4] CallServerToVerifyActivation() " + lCode + " " + status);
            _serverErrorMessage =
                "[D4] ...";
            return status;
        }

