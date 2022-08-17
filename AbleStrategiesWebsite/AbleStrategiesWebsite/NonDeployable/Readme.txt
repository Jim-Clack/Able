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
  AbleStrategies.CheckbookWsApi is redundant but necessary because of C# limitations

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
