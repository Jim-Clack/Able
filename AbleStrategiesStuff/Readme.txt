Development Notes
-----------------

Workstation Setup Notes...
 A. Install MS 2019 Visual Studio complete
 B. Install MS 2017 Installer for Application Deployment via MSIx
 C. Install git and/or GitHub desktop
 D. You will want to install Postman and WinSCP as well
 E. AbleCheckbook VS Pre-Build Event: copy $(ProjectDir)Support\* $(TargetDir)
 F. Install via NuGet: System.Text.Json (AbleCheckbook & AbleLicensing)
 G. Install via NuGet: PayPal .NET SDK (AbleCheckbook & AbleLicensing)
 H. In AssemblyInfo.cs, if necessary, change to [assembly: AssemblyVersion("1.0.*")]
 I. Import AbleCheckbook.postman_collection.json into PostMan for sandboxing 
 J. Options:Debug:General Uncheck "Step over propertes and operators"

 Caveats...
 . If you are new to MS VS, see the notes at the end of this document
 . VS C# Bug: A DataGridView column called "Name" will instead apply to Form
 . VS Gotcha: See comments at end of MainScreen.Reconciler/Support.cs
 . Run unit tests only in DEBUG mode, as there is debug code to handle caveats
 . To avoid live web services (i.e. PayPal, as) always run in DEBUG mode
 . Debug Checkbook and WebServices in separate instances of VS on the same host
 . You will need to alter the port number on DEBUG_WebServiceUrl when you test

Terminology...
 - New-Entry-Row refers to the "Insert Entry" row in the checkbook register
 - Debit v Credit (Payment v Deposit) - amounts are stored as signed values
 - Admin mode vs Standard/ProCPA vs SuperUser levels are Different Things
 - Cleared == Reconciled == Archived (transaction locked unless admin mode)
 - During reconcile, entry check-off "IsChecked" means "IsCleared Tentatively"
 - Technical details: ".adb", auto-save, rolling-backups, weekly-backups, etc.
  
Class Topology...
 a. Start from IDbAccess in order to understand the class heirarchy
 b. DB Class stack: IDbAccess -> UndoableDbAccess -> JsonDbAccess
 c. IDbAccess uses custom IDbIterators to cursor through records
 d. Primary data record is CheckbookEntry, which contains SplitEntries
 e. GUI Entry Hierarchy: DataGridViewRow.RowOfCheckbook.CheckbookEntry
 f. Reconcile Entry Hierarchy: CandidateEntry.OpenEntry.CheckbookEntry

Developer Tips...
 1. Some APIs are public merely to support serialization or unit testing
 2. Surround each GUI operation with BeforeOperation() and AfterOperation() 
 3. Allow foreign language Categories and Payees - don't translate
 4. Filenames are in English and NOT translated per i18n
 5. Yes, it's not performant, deliberately -> simplicity until it's stable
 6. When you add/mod quoted text, be sure to update Strings.cs as well
 7. db.UpdateEntry() caveats galore, esp w/Undo, because of live data
 8. db.InsertEntry() caveats as well, don't modify record AFTER the insert
 9. Web Services: Rebuild references (i.e. AbleLicensing) after change/build

Design of this project...
 * Avoid the use of sophisticated libraries, as it's intended to be ported
 * Presume nothing about the presentation layer other than in the GUI code
 * A flat-file DB is used but may be ported to a relational DB in the future
 * Don't use a Windows Resource res/rc file as this uses a portable solution
 * Undo anything that impacts the DB, using granularity based on UI opers
 * Automatic recovery from crashes, corrupt-data, and other such conditions
 * Event handlers must be small, up to five or six statements max
 * Maintain average MI of 75+ and CycComp to 5 or less in non-GUI methods
 * JSON is shared in AbleLicensing.WsApi - enums are deliberately omitted
 * 65% test coverage of non-GUI code, addressing all happy-paths therein 

User Levels (and corresponding License Code delimiters)...
 0. Evaluation (Time Limited, Limited Undo, No Support, No Admin)
 1. Deactivated (abuse deactivates most latent)     en-dash, not a hyphen: –
 2. Standard (Regular activated version)                           hyphen: -
 3. ProCPA (Named Accts, SLA, Live Support, Mult Instances)     ampersand: &
 4. SuperUser (UserDb Maint, Log Reader, Activation Codes, etc.) (+DEBUG): @

When writing docs/help, explain...
 - See terminology, above
 - Reconcile-in-progress will maintain state across save/open cycles
 - To undo N-steps of reconcile: undo the last step, then Abandon Reconcile
 - Edits made during reconcile remain even if reconcile is abandoned
 - Translation/i18n, strings (i.e sequence doesn't matter), etc.
 - Licensing, UserLevel, Activation, Expiration, and Admin Mode
 - Time-limited: Days of use vs days since installed
 - Show credit for Upcounsel EULA, icons/fonts, and other third parties

DEBUG manifest constant usage
 Note that the PayPal sandbox is selected and localhost is selected for
 Able Strategies web service calls when in DEBUG mode. This is facilitated
 by the DEBUG_ constants in Configuration.cs. Thus it is important to have
 a second instance of VS up and running the web services on the same host
 when in DEBUG mode. It is also important to not test purchases (PayPal)
 in RELEASE mode or you will be attempting to spend real money. Unit tests
 must be run in DEBUG mode; refer to GetIsActivated for details. Also note
 that SuperUser mode is non-functional in RELEASE mode. 

Kludge to enable your system (via Able Licensing) for Super-User mode...
 Activation.Instance.SetDefaultDays(180, 366); // note1: not needed but shown for completeness
 Activation.Instance.LicenseCode = "MYNAME@99999"; // Name6 + @ + zip5 (plus compile/run in DEBUG mode) enables SuperUser mode
 string pin = Activation.Instance.CalculatePin(Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification));
 Activation.Instance.SetActivationPin(pin);
 Activation.Instance.SetFeatureBitmask(0x000000000000000FL, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification));
 Activation.Instance.SetExpiration(2, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification)); // note1

EULA...
 https://www.upcounsel.com/end-user-license-agreement
Regex tester/guide for .NET
 http://regexstorm.net/
applicaiton/x-www-form-urlencoded v multipart/form-data
 https://stackoverflow.com/questions/4007969/application-x-www-form-urlencoded-or-multipart-form-data
MS DotNet WebBrowser...
 https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/implement-two-way-com-between-dhtml-and-client?view=netframeworkdesktop-4.8
Open Banking API...
 https://www.openbankingtracker.com/country/united-states
 https://open-bank-project.readthedocs.io/en/latest/
 https://www.programmableweb.com/api/
 https://www.direct.id/?utm_medium=ppc&utm_campaign=%5BNEW%5D+Open+Banking+Data+US&utm_source=adwords&utm_term=open%20banking&hsa_mt=p&hsa_acc=4959752413&hsa_tgt=kwd-301019364898&hsa_kw=open%20banking&hsa_ad=554116540304&hsa_src=g&hsa_ver=3&hsa_net=adwords&hsa_cam=14987299884&hsa_grp=132264197727&gclid=CjwKCAjwm8WZBhBUEiwA178UnClEKJ45Y2qJrqz63ncTuylPSYRqR6I4PEOGLnQSu1evDMjKjQUWOBoCFdAQAvD_BwE
PayPal
 https://developer.paypal.com/home
 https://github.com/paypal/PayPal-NET-SDK/wiki/Make-Your-First-Call
 https://developer.paypal.com/docs/business/checkout/set-up-standard-payments/
 https://stackoverflow.com/questions/29474270/c-sharp-paypal-rest-api-checkout-with-credit-card
Desktop Icon
 https://techcommunity.microsoft.com/t5/windows-dev-appconsult/msix-create-desktop-shortcuts-with-package-support-framework-and/ba-p/3300891
Info on deploying .NET apps to Linux
 https://docs.microsoft.com/en-us/dotnet/core/deploying/#framework-dependent-deployments-fdd
Google Drive API...
 https://developers.google.com/drive/api/v2/about-sdk
Attribution on Website/About:
 <div>Icons made by <a href="https://www.flaticon.com/authors/dinosoftlabs" title="DinosoftLabs">DinosoftLabs</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
Inserting an image into a RichTextBox
 https://stackoverflow.com/questions/542850/how-can-i-insert-an-image-into-a-richtextbox
Workaround for MS limitation with checkbox in a DataGridView
 https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridview.currentcelldirtystatechanged?redirectedfrom=MSDN&view=net-5.0
Running single vs multiple instances of an app
 https://ehikioya.com/multiple-instances-of-same-application/
MS VS Installers/Setup...
 https://aka.ms/vdproj-docs
 https://docs.microsoft.com/en-us/visualstudio/deployment/installer-projects-net-core?view=vs-2019
 https://docs.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net
 https://docs.microsoft.com/en-us/windows/win32/msi/windows-installer-guide

Note to critics:
 Yes, I know the live-data backend sucks. Yes, I know that I should have used
 a real DBMS. But I wanted to ensure that it was portable, did not want to
 expect the customer to have SQL Server on their host, and researched embedded
 DBMS's and found that all good solutions were either clunky, non-portable,
 or expected royalties. I also knew that this app did not need the scale of a
 full DB and that all data could easily fit into memory for any expected user
 checkbook register. So I chose the JSON format as a temporary cheat, with
 live data records in memory. But I put a neutral DB interface on it so it 
 could be changed later. Do I regret it? Yes. And no. It's fast and achieves
 its objectives. But it's non-standard and nont extensible. Yet for now, don't
 fix it if it ain't broke. (I learned from this exercise and the server-side
 JSON DB is much cleaner. But it really should be replaced with SQL.)

Notes to developers who are MS/VS virgins...

 For some items in Solution Explorer (and in other views as well) there are 
 two completely separate and distinct collections of settings, one called 
 "Properties" and the other, very interestingly, called "Properties". Yes 
 you read that correctly. One of them can be found by right-clicking the 
 project/file/folder and then selecting "Properties" and the other is found
 by clicking the project/file/folder then going to the Properties tab/view. 
 In many cases these two distinct pages are essentially the same, but in
 others they are totally different. Two views of two different lists of 
 settings, both called Properties. Moreover there is another View named
 Property Manager that has nothing to do with either of these. Confused?

 Weird namespace/packaging: Array.ToList() is found in System.Linq. Really!

 When you set a background image you also want to set the layout. Of course
 you might think that to set the layout for a BackgroundImage you would set
 the proprety BackgroundImageLayout, but no. Instead you should set SizeMode.
 Yes, really.

 To view/edit the code for certain Forms classes, you may want to avoid the 
 obvious dpouble-click in the solution explorer as well as avoiding the 
 click-the-form-name-then-double-click-the-title-bar. These clicks also run
 the VS resource-compiler and form-generator which may do crazy things. Of 
 course there are many cases where you want that to occur but, when you don't,
 then click the expand arrow to the left of the form then right-click-the-
 code-file-beneath-it-then-select-view-code. Otherwise you will sometimes get
 bitten in the ass. This is because there are two source files for each form,
 the one you edit, which is not the one you get when you click a form in the
 Solution Explorer, as that opens the "xxx.Designer.cs" file. For instance, if
 you double-click MainScreen.Support.cs, VS generates an OnLoad handler that 
 conflicts with the existing OnLoad handler. You WILL be bitten by this!

 Speaking of the resource-compiler and form-generator, you will sometimes 
 see a form get scrambled, a form that suddenly cannot be rendered, or a
 form that comes up in a foreign language. Go back to the code and undo 
 whatever you did that caused the issue. You may want to avoid using Undo
 to do this because, when such situations occur, it will often undo much
 more than you expect. You will likely find the offending code to have been
 appended to the end of the source code but, before deleting it, remove any
 references to it in the designer - or you'll regret it.

 A ComboBox, in its default configuration, allows the user to type in any
 string they want. But when they leave the ComboBox it changes the user's
 entry back to match an item in the DataSource list. There's no property to
 override this behavior. So you'll see that the "Leave" event handler for 
 ComboBoxes calls a method named AllowTypedInComboItem() that adds the 
 typed-in string to the data source.

 When you add a ComboBox (this.Controls.Add(ComboBoxXxx)) it may screw up
 certain properties of the ComboBox because their activation gets deferred
 until then, so it's important to set SelectIndex, Text, BringToFront/Back, 
 etc. until afterward. Also after changing the DataSource you may have 
 to ComboBoxXxx.BindingContext = new BindingContext(). 

 Very confusing snag: If you move a folder, add then remove a project, or
 update a project outside of the current solution, Visual Studio may still
 hold onto the old version. You can do a Clean/Rebuild, you can even do 
 somersaults in your underoos, and it will still use the obsolete. And the 
 error message may be very misleading, related to an indirect effect.

 In the app.config (or web.config) file, <configSections> must be the first
 element inside the <configuration> or it will be ignored.

 There is an insidious VS C# Bug, in that a DataGridView column called 
 "Name" will instead apply to the Form, per code-time and intellisense.
 So if you have a DataGridView with one column named "Name" in the VS edit-
 form, you will get nonsense error messages and it will refuse to compile.

PayPal Web Services
 jim.clack@gmail.com p8
 https://developer.paypal.com/
 Account Type:     Personal (Merchant)
 Account Name:     AbleStrategies
 Sandbox Accounts:  
   App ID:         APP-80W284485P519543T
   Name:           John Doe
   Bus Email:      sb-lyvur20481988@business.example.com
   Bus Passwd:     %*#r2?rX
   Bus Phone:      4086517743
   Bus Acct:       4DV7CG5A7HN7Y
   Pers Email:     sb-krbto20482028@personal.example.com
   Pers Passwd:    aGVs5%sm
   Pers Phone:     4087983656
   Pers Acct:      BBG6Q73XGCFFL
   Basic Auth:     ---------------------------------------------------------------------------------------------------
 Live Account:  
   App ID:         
   Name:           
   Bus Email:      
   Bus Passwd:     
   Bus Phone:      
   Bus Acct:       
   Client ID:      
   Basic Auth:     

JSON REST API (AbleStrategies checkbook web services)
  All return a JsonUserInfoResponse except for GET verify connection
  Verify connection
    GET as/checkbook (returns strings in a JSON wrapper)
  Poll periodically, returns ReturnOk, ReturnOkReconfigure, ReturnNotFound, or ReturnDeactivate
    GET as/checkbook/poll/lcode/siteid/vv-vv
  Lookup license: returns ReturnOk, ReturnNotFound, ot ReturnLCodeTaken
    POST as/checkbook/2?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&designator=
  Register site: if successful, update LicenseCode from response
    POST as/checkbook/5?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&designator=
  Additional Device: if successful, update LicenseCode from response
    POST as/checkbook/9?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&designator=
  Call PayPal to make purchase
    Collect date from response in "designator" string "PtransactionNumber|dotDelimitedValidationData"
  Purchase: if successful, get PinNumber from response
    POST as/checkbook/11?name=Fred&addr=123%20Main&city=NYC&zip=12345&phone=1234567890&email=a.b%40abc.com&feature=0&lCode=abcde.12345&siteId=aBcD123&designator=P12345|7C67.890
  Example Response
  {
    "ApiState": 20,
    "Message": "",
    "PinNumber": "",
	"ReconfigurationRecords": [
	],
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
