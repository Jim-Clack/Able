Development Notes
-----------------

Workstation Setup Notes...
 A. Install MS 2019 Visual Studio complete
 B. Install MS 2017 Installer for Application Deployment via MSIx
 C. Install via NuGet: System.Text.Json (AbleCheckbook & AbleLicensing)
 D. Install git and/or GitHub desktop
 E. You may want to install Postman and WinSCP as well
 F. AbleCheckbook VS Pre-Build Event: copy $(ProjectDir)Support\* $(TargetDir)
 G. Set Build Configuration Symbols: Debug: SUPERUSER, Release: RELEASE 
 H. In AssemblyInfo.cs, if necessary, change to [assembly: AssemblyVersion("1.0.*")]

 Caveats...
 . If you are new to MS VS, see the notes at the end of this document
 . C# Bug: A DataGridView column called "Name" will instead apply to Form
 . VS Gotcha: See comments at end of MainScreen.Reconciler/Support.cs
 . Run unit tests only in DEBUG mode, as there is some debug code to handle caveats

Terminology...
 - New-Entry-Row refers to the "Insert Entry" row in the checkbook register
 - Debit v Credit (Payment v Deposit) - amounts are stored as signed values
 - Admin mode vs Standard/ProCPA vs SuperUser levels are Different Things
 - Cleared == Reconciled == Archived (transaction locked unless admin mode)
 - During reconcile, entry check-off "IsChecked" means "IsCleared Tentatively"
 - Technical details: ".adb", auto-save, rolling-backups, weekly-backups, etc.

Class Topology...
 a. Start from IDbAccess in order to understand the class heirarchy
 b. DB Class Inheritance: JsonDbAccess -> UndoableDbAccess -> IDbAccess
 c. IDbAccess uses custom IDbIterators to cursor through records
 d. Primary data record is CheckbookEntry, which contains SplitEntries
 e. GUI Entry Hier: DataGridViewRow.RowOfCheckbook.CheckbookEntry.SplitEntry
 f. Reconcile Entry Hier: CandidateEntry.OpenEntry.CheckbookEntry.SplitEntry

Developer Tips...
 1. Some APIs are public merely to support serialization or unit testing
 2. Before performing each operation from the GUI, call MarkUndoBlock()
 3. Allow foreign language Categories and Payees - don't translate
 4. Filenames are in English and NOT translated per i18n
 5. Yes, it's not performant, deliberately -> simplicity until it's stable
 6. When you add/mod quoted text, be sure to update Strings.cs as well
 7. db.UpdateEntry() caveats galore, esp w/Undo, because of live data
 8. db.InsertEntry() caveats as well, don't modify record AFTER the insert

Design of this project...
 * Avoid the use of sophisticated libraries, as it's intended to be ported
 * Presume nothing about the presentation layer other than in the GUI code
 * A flat-file DB is used but may be ported to a relational DB in the future
 * Don't use a Windows Resource res/rc file as this uses a portable solution
 * Undo of anything that impacts the DB, using granularity based on UI ops
 * Automatic recovery from crashes, corrupt-data, and other such conditions
 * Event handlers must be small, up to five or six statements max
 * Maintain average MI of 75+ and CycComp to 5 or less in 95% of methods
 * 60% test coverage with 99% of happy-path therein (in non-GUI code)

User Levels (and corresponding Site-Description delimiters)...
 0. Eval (Unlicensed, Time Limited, Limited Undo, No Support, No Admin)
 1. Deactivated (abuse deactivates most latent)     en-dash, not a hyphen: –
 2. Standard (Regular licensed version)                            hyphen: -
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

Kludge to enable your system (via Able Licensing) for Super-User mode...
 Activation.Instance.SetDefaultDays(180, 366); // note1: not needed but shown for completeness
 Activation.Instance.SiteDescription = "MYNAME@99999"; // The @-sign (plus compile/run in DEBUG mode) enables SuperUser mode
 string pin = Activation.Instance.ResetAllEntries(Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification));
 Activation.Instance.SetActivationPin(pin);
 Activation.Instance.SetFeatureBitmask(0x000000000000000FL, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification)); // note1
 Activation.Instance.SetExpiration(2, Activation.Instance.ChecksumOfString(Activation.Instance.SiteIdentification)); // note1

MS DotNet WebBrowser...
 https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/implement-two-way-com-between-dhtml-and-client?view=netframeworkdesktop-4.8
Open Banking API...
 https://www.openbankingtracker.com/country/united-states
 https://open-bank-project.readthedocs.io/en/latest/
 https://www.programmableweb.com/api/
PayPal
 https://developer.paypal.com/home
 https://developer.paypal.com/docs/business/checkout/set-up-standard-payments/
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
 live data records in memory. But put a neutral DB interface on it so it 
 could be changed later. Do I regret it? Yes. And no. It's fast and achieves
 its objectives. But it's error-prone and non-standard. Yet for now, don't
 fix it if it ain't broke.

Notes to developers who are MS/VS virgins:
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

 When you set a background image you also want to set the layout. Of course
 you might think that to set the layout for a BackgroundImage you would set
 the proprety BackgroundImageLayout, but no. That property seems to do NADA.
 Instead you should set SizeMode. Yes, really.

 To view/edit the code for certain Forms classes, you may want to avoid the 
 obvious click-the-form-name-then-double-click-the-title-bar. That also runs
 the VS resource-compiler and form-generator which may do crazy things. Of 
 course there are many cases where you want that to occur but, when you don't,
 then click the expand arrow to the left of the form then right-click-the-
 code-file-beneath-it-then-select-view-code. Otherwise you will sometimes get
 bitten in the ass.

 Speaking of the resource-compiler and form-generator, yoou will sometimes 
 see a form get scrambled, a form that suddenly cannot be rendered, or a
 form that comes up in a foreign language. Go back to the code and undo 
 whatever you did that caused the issue. You may want to avoid using Undo
 to do this because, when such situations occur, it will often undo much
 more than you expect.

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
