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

API
  GET as/checkbook
    (...to verify connection)
  POST as/checkbook/2?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&purchase=
    (returns ReturnNotFound, ReturnLCodeTaken, or ReturnOk)
  POST as/checkbook/5?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&purchase=
    (if successful, update LicenseCode from response)
  Call PayPal to make purchase
    (collect date from response in "purchase" string "PtransactionNumber|dotDelimitedValidationData")
  POST as/checkbook/11?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&purchase=P12345%7C67.890
    (if successful, get PinNumber from response)
  GET as/checkbook/user/lcode/siteid/vv-vv
    (...to poll, periodically)
  Response
  {
    "ApiState": 20,
    "Message": "",
    "PinNumber": "",
    "UserInfos": [
        {
            "LicenseRecord": {
                "LicenseCode": "FREDYY-23456",
                "ContactName": "Fred",
                "ContactAddress": "123 Main",
                "ContactCity": "NYC",
                "ContactZip": "12345",
                "ContactPhone": "1234567890",
                "ContactEMail": "a.b@abc.com",
                "LicenseFeatures": "0",
                "Id": "2908398b-d153-4b5e-87d8-c8fc736c0c27",
                "DateCreated": "2022-08-09T16:29:45.9440931-04:00",
                "DateModified": "2022-08-09T16:29:45.9472974-04:00"
            },
            "PurchaseRecords": [
			    {
                    "FkLicenseId": "2908398b-d153-4b5e-87d8-c8fc736c0c27",
					"PurchaseAuthority": 2,
					"PurchaseTransaction": "123456",
					"PurchaseVerification": "32312.45.789ABC477",
					"PurchaseAmount": "2999",
					"Details": "v1"
                    "Id": "3333398b-d153-4b5e-87d8-c8fc999c0c97",
                    "DateCreated": "2022-08-09T16:29:45.9441046-04:00",
                    "DateModified": "2022-08-09T16:29:45.9479129-04:00"
				}
			],
            "DeviceRecords": [
                {
                    "FkLicenseId": "2908398b-d153-4b5e-87d8-c8fc736c0c27",
                    "DeviceSite": "aBcD123",
                    "UserLevelPunct": 45,
                    "CodesAndPin": "",
                    "Id": "ae9766be-60fd-426b-ae23-24762f70c977",
                    "DateCreated": "2022-08-09T16:29:45.9441046-04:00",
                    "DateModified": "2022-08-09T16:29:45.9479129-04:00"
                }
            ],
            "InteractivityRecords": [
                {
                    "FkLicenseId": "2908398b-d153-4b5e-87d8-c8fc736c0c27",
                    "InteractivityClient": 8,
                    "ClientInfo": "::1",
                    "Conversation": "RegisterLicense - License Code rqst: FredY;12345 - generated as: FREDYY-23456",
                    "History": "",
                    "Id": "5f6d5d75-bc97-4b64-95d0-f979d6647970",
                    "DateCreated": "2022-08-09T16:29:45.9472846-04:00",
                    "DateModified": "2022-08-09T16:29:45.9484617-04:00"
                }
            ]
        }
    ]
}
