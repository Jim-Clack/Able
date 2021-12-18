using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// Resource for future i18n. 
    /// </summary>
    public static class Strings
    {

        /// <summary>
        /// Man-readable strings with string indexes.
        /// </summary>
        private static Dictionary<string, string> _strings = null;

        /// <summary>
        /// Fetch a string by index name.
        /// </summary>
        /// <param name="index">Name of string.</param>
        /// <returns>Selected text. If not found, logs the error and returns the Index name.</returns>
        public static string Get(string index)
        {
            if (_strings == null)
            {
                LoadStrings();
            }
            string result = index;
            try
            {
                result = _strings[index];
            }
            catch (KeyNotFoundException)
            {
                Logger.Info("Missing string: [" + index + "]");
            }
            return result;
        }

        /// <summary>
        /// Fetch a string by index name if it is overridden, else just return the index name.
        /// </summary>
        /// <param name="index">Name of string.</param>
        /// <returns>Selected text. If not found, return Index name.</returns>
        public static string GetIff(string index)
        {
            if (_strings == null)
            {
                LoadStrings();
            }
            string result = index;
            try
            {
                result = _strings[index];
            }
            catch (Exception)
            {
                // Ignore exceptions
            }
            return result;
        }

        /// <summary>
        /// Populate the strings based on the locale. (hardcoded US-en currently)
        /// </summary>
        private static void LoadStrings()
        {
            _strings = new Dictionary<string, string>();
            LoadHardCodedDefaults();
            if (!LoadFile())
            {
                LoadHardCodedDefaults();
            }
            SaveFile();
        }

        /// <summary>
        /// Add a string to the disctionary.
        /// </summary>
        /// <param name="index">lookup index</param>
        /// <param name="content">extual content</param>
        private static void AddString(string index, string content)
        {
            int count = 0;
            bool ok = false;
            while (!ok)
            {
                try
                {
                    if(_strings.Keys.Contains<string>(index))
                    {
                        _strings.Remove(index);
                    }
                    _strings.Add(index, content);
                    ok = true;
                }
                catch (Exception)
                {
                    // ignore
                }
                index = index + (++count).ToString();
            }
        }

        /// <summary>
        /// Shortcut to add a string to the dictionary.
        /// </summary>
        /// <param name="both">the string that is both the index and the content</param>
        private static void AddString(string both)
        {
            AddString(both, both);
        }

        /// <summary>
        /// Load the default strings for EN-us.
        /// </summary>
        private static void LoadHardCodedDefaults()
        {
            AddString("AboutThisFile", "Loads from launch path, i.e. chkbk-EN-us.json  (depending on locale)");
            AddString("PuncHash", "#");
            AddString("PuncQuestion", "?");
            AddString("Error");
            AddString("Warning");
            AddString("Notification");
            AddString("Reconcile");
            AddString("Adjustment");
            AddString("Clear");
            AddString("Transaction");
            AddString("Date of Transaction");
            AddString("Modified Date");
            AddString("Reconciled Date");
            AddString("Modified");
            AddString("By");
            AddString("Check Number");
            AddString("Check#");
            AddString("Chk#", "Ck#");
            AddString("Payee");
            AddString("Total");
            AddString("Category");
            AddString("Date");
            AddString("Memo");
            AddString("Amount");
            AddString("Balance");
            AddString("Kind");
            AddString("Status");
            AddString("Payment");
            AddString("Deposit");
            AddString("Other");
            AddString("Weekly Backups:");
            AddString("Base DB Directory");
            AddString("Weekly Backups Directory");
            AddString("Normally this is the closing date and closing balance from your last bank statement");
            AddString("(Split)");
            AddString("Help");
            AddString("Able Strategies AbleCheckbook");
            AddString("Category Report");
            AddString("Print Register");
            AddString("User:");
            AddString("Account:");
            AddString("Date:");
            AddString("From:");
            AddString("Thru:");
            AddString("Just Now");
            AddString("Today");
            AddString("Tomorrow");
            AddString("Yesterday");
            AddString("Dining");
            AddString("Groceries");
            AddString("Medical");
            AddString("Insurance");
            AddString("Utilities");
            AddString("Cash");
            AddString("Credit");
            AddString("Debit");
            AddString("Housing");
            AddString("Clothing");
            AddString("Household");
            AddString("Charity");
            AddString("Education");
            AddString("Personal");
            AddString("Transportation");
            AddString("Income");
            AddString("Miscellaneous");
            AddString("Balance Forward");
            AddString("Unknown");
            AddString("Deposits:");
            AddString("Start First Date", "Start/First Date");
            AddString("End Last Date", "End/Last Date");
            AddString("Detailed Report", "Detailed Report: With Itemized Transactions");
            AddString("Go Print", "Go / Print");
            AddString("Id");
            AddString("Cancel");
            AddString("OK");
            AddString("Ok");
            AddString("Notice");
            AddString("Transaction Date");
            AddString("Date Range");
            AddString("Checkbook Entry");
            AddString("Click to Insert");
            AddString("Delete");
            AddString("Cleared");
            AddString("Cleared / Reconciled");
            AddString("Payment");
            AddString("XferOut");
            AddString("XferIn");
            AddString("Deposit");
            AddString("Refund");
            AddString("Adjustment");
            AddString("Print");
            AddString("Close");
            AddString("No Activity");
            AddString("Select Printer and Settings");
            AddString("This can corrupt your checkbook, please cancel");
            AddString("&New Acct");
            AddString("&Open Acct");
            AddString("&Save Acct");
            AddString("Open &Backup File");
            AddString("&Year-End Wrap-Up");
            AddString("&Print Register");
            AddString("Search Entries");
            AddString("E&xit");
            AddString("&Undo");
            AddString("&Redo");
            AddString("&Copy Entry");
            AddString("&New Entry");
            AddString("&Delete Entry");
            AddString("Rename &Payee");
            AddString("&Search Entries");
            AddString("Sort by &Date");
            AddString("Sort by &Payee");
            AddString("Sort by &Category");
            AddString("Sort by Match");
            AddString("Sort by Reconcile");
            AddString("Sort by Check &Number");
            AddString("&Itemize Splits");
            AddString("Search for Payee");
            AddString("&Uncleared Only");
            AddString("&Scheduled Events");
            AddString("&Categories");
            AddString("&Memorized Payees");
            AddString("&Reconcile (Monthly)");
            AddString("C&ategory Report");
            AddString("Diagnostics");
            AddString("&Diagnostics");
            AddString("&About");
            AddString("&Help Contents");
            AddString("&Search Help");
            AddString("&Preferences");
            AddString("Activate &License");
            AddString("Search Help For...");
            AddString("Log Reader");
            AddString("Open Log File");
            AddString("Search Forward");
            AddString("Search Backward");
            AddString("Base Directory:");
            AddString("Base Directory");
            AddString("Preferences");
            AddString("Change");
            AddString("Checkbox");
            AddString("Commit");
            AddString("Highlight Entry as a Reminder");
            AddString("Entries Sum Up to Disparity:");
            AddString("Transposed Fractional Digits:");
            AddString("Transposed Monetary Digits:");
            AddString("Sign (Payment/Deposit) Wrong:");
            AddString("Illegal Directory Path Specified, Reverted...");
            AddString("Number of days in advance to post scheduled events to checkbook:");
            AddString("Log Level (Trace = Detailed, Diag = Normal, Warn = Smaller Logs)");
            AddString("Edit dates via calendar instead of day/month/year spinners");
            AddString("Display the Reconcile Overdue notification when appropriate");
            AddString("Display the Year-End Wrap-Up Due notification When appropriate");
            AddString("Display amounts in two columns (Debit/Credit) instead of one (Amount)");
            AddString("Check off entries that are cleared in bank statement");
            AddString("Fill in details at left from your bank statement");
            AddString(" days away from today! Are you sure?");
            AddString("Confirm");
            AddString("Disable sanity-checks for wild dates and amounts during data-entry");
            AddString("Account Settings - ");
            AddString("Live sync to bank acct online (instead of only for monthly reconcile)");
            AddString("Aggressively merge transactions (you can still un-merge if desired)");
            AddString("Bank/branch req'd");
            AddString("Acct type required");
            AddString("Bank login required");
            AddString("Bank password req'd");
            AddString("Click Test...");
            AddString("Select Bank");
            AddString("Account");
            AddString("Your Login");
            AddString("Password");
            AddString("Test");
            AddString(" in total currency! Are you sure?");
            AddString("Disparity (should be zero):");
            AddString("Done With Reconciliation");
            AddString("Create Balance Adjustment");
            AddString("Tips");
            AddString("High Visibility - Larger Fonts");
            AddString("Edit/Del Entry");
            AddString("Search");
            AddString("Year End");
            AddString("Rename Payee");
            AddString("Import CSV");
            AddString("Import QIF");
            AddString("Export CSV");
            AddString("Export QIF");
            AddString("Successful");
            AddString("Note: No Importable Entries Read");
            AddString("Open Backup of DB File: ");
            AddString("Save the current DB as ");
            AddString("If already paid-for, enter the Purchase Val Code here");
            AddString("; Open the selected backup as ");
            AddString("Search only those Entries before or on:");
            AddString("Search for substring in Payee");
            AddString("Search for Payee match");
            AddString("Search for Category match");
            AddString("Search for substring in Memo");
            AddString("Search for Check Numbers between");
            AddString("=> Thru =>");
            AddString("Matches Found");
            AddString("Matches");
            AddString("Go...");
            AddString("database files (*.acb)|*.acb");
            AddString("Open DB");
            AddString("Save");
            AddString("Rename Payee");
            AddString("Old Payee Name");
            AddString("New Payee Name");
            AddString("Number of Entries Changed");
            AddString("Close");
            AddString("Too soon or too many uncleared entries remain.");
            AddString("Abandon Reconcile");
            AddString("Abandon");
            AddString("Uncheck All Checked Entries Too?");
            AddString("Not Yet");
            AddString("Sorry");
            AddString("Admin Mode - Force Year-End Wrap-Up?");
            AddString("Try Again?");
            AddString("Category Report");
            AddString("Print Register");
            AddString("Undo");
            AddString("Redo");
            AddString("Copy");
            AddString("Copy Entry");
            AddString("New Entry");
            AddString("Delete Entry");
            AddString("Search For...");
            AddString("Search Memos");
            AddString("Scheduled Event");
            AddString("Scheduled Events");
            AddString("Category Report");
            AddString("Preferences");
            AddString("Search for Entries by Payee");
            AddString("Help");
            AddString("←  Back");
            AddString("⌂  Home");
            AddString("Ꙭ  Search");
            AddString("◀ No: Un-Merge");
            AddString("New DB Acct");
            AddString("Select Acct");
            AddString("New Account, Starting Balance");
            AddString("Initial Account Balance:");
            AddString("Starting Balance for New Account...");
            AddString("As of:");
            AddString("Go To Most Recent");
            AddString("Sort Matches First");
            AddString("Prev Closing:");
            AddString("This Closing:");
            AddString("Sorry - File Already Exists");
            AddString("Sorry - Illegal Account Name");
            AddString("Year-End Wrap-Up Overdue");
            AddString("Monthly Reconciliation Due");
            AddString("Cannot mix Payments and Deposits in the same entry");
            AddString("Category and amount required");
            AddString("Payee must be specified");
            AddString("Reconciled/cleared entry - cannot be changed");
            AddString("Reconciled/cleared entry - DO NOT CHANGE ANYTHING");
            AddString("Danger Danger Danger");
            AddString("Direct modification can corrupt your account data!");
            AddString("Problem with Year-End Wrap-Up (Suggest Undo) ");
            AddString("Active DB name indicates that it is already current.");
            AddString("Cannot perform year-end with large proportion unreconciled.");
            AddString("Too soon for year-end: many old entries not yet been cleared.");
            AddString("#Invalid Site Description - Expected 6-char Name, a Hyphen, then 5-char Postal Code.");
            AddString("#Requires Super-User Permission");
            AddString("#Invalid Site Identification Code.");
            AddString("Able Strategies AbleCheckbook - See Terms of License");
            AddString("Activation");
            AddString("Activate");
            AddString("Activated");
            AddString("(Unlicensed)");
            AddString("Site Identification Code");
            AddString("Contact/User Name");
            AddString("Address and Street");
            AddString("City and State/Province");
            AddString("Postal/ZIP Code or CC");
            AddString("10-12 Digit Phone Nbr");
            AddString("Contact Email Address");
            AddString("Computer Identification");
            AddString("I have read and accept the terms of the EULA");
            AddString("EULA - End User License Agreement");
            AddString("Assigned site description");
            AddString("Offline/Manual Activation PIN");
            AddString("Use manual activation - Call support for a PIN");
            AddString("Invalid Description and/or PIN");
            AddString("User name too short");
            AddString("Invalid postal/CC code");
            AddString("Phone and email required");
            AddString("You must accept the EULA");
            AddString("Call support for Description");
            AddString("Call for an Activation PIN");
            AddString("Turning Off Admin Mode");
            AddString("Time's Up");
            AddString(" days");
            AddString("Expired Trial Evaluation Period");
            AddString("Already activated - Do you wish to continue?");
            AddString("Are you sure you want to change this value?");
            AddString("Add New");
            AddString("Due");
            AddString("Success");
            AddString("Expired");
            AddString("Active");
            AddString("Every");
            AddString("Monthly");
            AddString("Monthly on");
            AddString("Even");
            AddString("Odd");
            AddString("Edit Scheduled Event");
            AddString("i.e. Pay bill on the 17th of each month");
            AddString("You may select multiple days per month");
            AddString("Yearly");
            AddString("i.e. Pay bill on April 25 each year");
            AddString("Weekly");
            AddString("i.e. Paycheck deposit every Thursday");
            AddString("Hold down Ctrl to select more than one");
            AddString("Nth Wkday");
            AddString("i.e. SSA check deposit 2nd Thursday of each month");
            AddString("Biweekly");
            AddString("i.e. Mortgage payment every other Friday");
            AddString("Next occurrence date...");
            AddString("Estimate");
            AddString("Final Payment if Different");
            AddString("Duration");
            AddString("Final:");
            AddString("Occurrences:");
            AddString("Occurrence");
            AddString("Continues Forever");
            AddString("Trans Kind");
            AddString("1st occurrence in month");
            AddString("2nd occurrence in month");
            AddString("3rd occurence in month");
            AddString("4th occurrence in month");
            AddString("1st (first day of month)");
            AddString("2nd");
            AddString("3rd");
            AddString("4th");
            AddString("5th");
            AddString("6th");
            AddString("7th");
            AddString("8th");
            AddString("9th");
            AddString("10th");
            AddString("11th");
            AddString("12th");
            AddString("13th");
            AddString("14th");
            AddString("15th");
            AddString("16th");
            AddString("17th");
            AddString("18th");
            AddString("19th");
            AddString("20th");
            AddString("21st");
            AddString("22nd");
            AddString("23rd");
            AddString("24th");
            AddString("25th");
            AddString("26th");
            AddString("27th");
            AddString("28th");
            AddString("29th (or 28th in short Feb)");
            AddString("30th (or last day in Feb)");
            AddString("31st (or last day of month)");
            AddString("Select Day(s) of Month");
            AddString("Select Day of Month");
            AddString("Select Month");
            AddString("Select Day of Week");
            AddString("Select Week of Month");
            AddString("Invalid Next Date");
            AddString("FYI: Multiple days per month selected.");
            AddString("Next Date Not ");
            AddString("Must Specify Payee");
            AddString("Category Required");
            AddString("Set Debit/Credit");
            AddString("Amount is Needed");
            AddString("Autosaving...");
            AddString("Set Occurrences Count");
            AddString("Invalid Final Date");
            AddString("Must be Within 2 Wks");
            AddString("Time-Limited Evaluation Copy");
            AddString("Licensed to: ");
            AddString("Level: ");
            AddString("Version: ");
            AddString("UNLICENSED_VERSION");
            // lowercase comma-separated, prefix: =equal, ~contains, <startwith, >endwith, *default, !category
            AddString("CategoryGuesses",
                "~cafe,~bistro,~steak,~carrabba,~ruby tuesday,~fridays,~mcdonalds,~tim horton,~taco bell" +
                ",~burger,=kfc,~tavern,~china,~taco,~pizza,~italian,~texmex,>pho,~sushi,~seafood,>spaghetti" +
                ",>grill,~chili,~diner" + 
            ",!Dining" +
                ",~kroger,~& shop,~and shop,~publix,>foods,~pathmark,~albertsons,<sams,=costco,~aldi" +
                ",~trader joe,=heb,~piggly,=shoprite,~grocer,~market,~ingles,~save " +
            ",!Groceries" +
                ",~doctor,~hospital,~clinic,~psychiatr,~ob/gyn,~lab test,~patholog,~counselor,~pediatric" +
                ",~health,<dr.,<dr ,~aetna,~blue cross,~cvs,~walgreens,~cigna,~optum,~humana" +
            ",!Medical" +
                ",~state farm,~progressive,~nationwide,~geico,~mutual,~prudential,~farmers,~new york life,~insurance" +
            ",!Insurance" +
                ",~comcast,~xfinity,~verizon,~dish,~at&t,~electric,~frontier,<cox,<altice,~direct,~water" +
                ",~consolidated,~power,~t-mobile,~tmobile" +
            ",!Utilities" +
                ",~discover,~visa,~master card,~mastercard,~american express,~amex" +
            ",!Credit" +
                ",<rocket,~chase,~truist,~leasing,~mortgage,~bank,~lending,~quicken,~wells fargo,~citicorp" +
                ",~bancorp,~hbsc,~capital,~state street,~goldman,~hoa,~financ,~apartment" +
            ",!Housing" +
                ",~amazon,~kohls,~target,~walmart,~etsy,~zappo" +
            ",!Clothing" +
                ",~home depot,~lowes,~ace h,~lumber,~flooring,~furniture,~bathr,~garden,~roof" +
                ",~paint,~best buy,~appliance" +
            ",!Household" +
                ",~church,~march of dimes,>temple,>.org,~united way,~red cross" +
            ",!Charity" +
                ",>school,~college,~university,~tuition,~msu,<u of ,~notre dame,~duke,~suny,~cuny,~ucsd" +
                ",<ucla,~berkeley,~harvard,~stanford,~usf,~yale,~princeton,~dartmouth,<mit,>a&m,>a & m" +
                ",>.edu,~student,~sallie mae,~sally may" +
            ",!Education" +
                ",~hair,~nails,>salon,>cuts,>clips,~styling" +
            ",!Personal" +
                ",~mobil,~shell,~amoco,~exxon,=bp,~marathon,~sunoco,~texaco,~citgo,~racetrack,=gulf,~valero" +
                ",~garage,~auto repair,~tire,~ toyota,~ ford,~ chev,~ honda,~ acura,~ lexus,~ buick,~ infiniti" +
                ",~ chrysler,~ cadillac,~ lincoln,~ jeep,~ hyundai,~ kia,~ nissan,~ mercedes,~bmw,~ audi" +
                ",~parking,~oil,~gas,~petro" +
            ",!Transportation" +
            ",*Miscellaneous");
            // This flags the data as "loaded in full"
            AddString("TERMINATE", "Default");
        }

        /// <summary>
        /// Load strings from a file.
        /// </summary>
        /// <returns>success</returns>
        private static bool LoadFile()
        {
            try
            {
                // Load chkbk-custom.json, if it...
                string filename = Configuration.Instance.FindSupportFilePath("chkbk-###.json");
                if (File.Exists(filename))
                {
                    using (FileStream stream = File.OpenRead(filename))
                    {
                        _strings = JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return _strings.Keys.Contains<string>("TERMINATE") && !Get("TERMINATE").Equals("Default"); ;
        }

        /// <summary>
        /// Save current strings to a diagnostic file.
        /// </summary>
        private static void SaveFile()
        {
            try
            {
                string filename = Path.Combine(Configuration.Instance.DirectoryLogs, "chkbk-xx-XX.json");
                using (FileStream stream = File.Create(filename))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    JsonSerializer.SerializeAsync<Dictionary<string, string>>(stream, _strings, options).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                // ignore any problems
            }
        }

    }

}
