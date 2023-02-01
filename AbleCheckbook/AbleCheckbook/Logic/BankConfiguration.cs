using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class BankConfiguration : IComparable, IEqualityComparer<BankConfiguration>
    {

        static Dictionary<String, SortedList<BankConfiguration, BankConfiguration>> lookupMap = null;

        static HashSet<String> regions = null;

        static HashSet<String> longNames = null;

        private string bank = Guid.NewGuid().ToString();
        private string region = "USA";
        private string account = "Checking";
        private string revision = "Std";
        private DateTime dateModified = DateTime.Now;
        private string modifiedBy = "(unspecified)";
        private string method = "HTML";
        private string baseURL = "https://unknown.com";
        private string initialRequestQuery = "";
        private string initialResponse = "";
        private string capture6FromEntryNamed = "";
        private string capture7FromEntryNamed = "";
        private string capture8FromEntryNamed = "";
        private string capture9FromEntryNamed = "";
        private string accountRequestQuery = "";
        private string accountRequestAddToHeader = "";
        private string accountRequestBody = "";
        private string statementRequestQuery = "";
        private string statementRequestAddToHeader = "";
        private string statementRequestBody = "";
        private string statementResponseHead = "";
        private string statementResponseTail = "";
        private string terminateRequestQuery = "";
        private string dateFormatsRequest = "yyyy/MM/dd";
        private string dateFormatsResponse = "yyyy/MM/dd";
        private string amountFormatsResponse = "";
        private string requestType = "F";
        private string responseType = "M";
        private string fieldSeparator = ",";
        private string filenameRegex = "[A-Za-z0-9_]+\\.csv";
        private int headerNumberOfLines = 0;
        private int dateColumnNumber = 0;
        private int payeeColumnNumber = 0;
        private int amountColumnNumber = 0;
        private int addlInfoColumnNumber = -1;
        private int creditFlagColumnNumber = 0;
        private string creditFlagIsCreditRegex = "";
        private int checkNumberColumnNumber = 0;
        private int transactionColumnNumber = -1;
        private int balanceColumnNumber = -1;

        public string Bank { get => bank; set => bank = value; }
        public string Region { get => region; set => region = value; }
        public string Account { get => account; set => account = value; }
        public string Revision { get => revision; set => revision = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public string ModifiedBy { get => modifiedBy; set => modifiedBy = value; }
        public string Method { get => method; set => method = value; }
        public string BaseURL { get => baseURL; set => baseURL = value; }
        public string InitialRequestQuery { get => initialRequestQuery; set => initialRequestQuery = value; }
        public string InitialResponse { get => initialResponse; set => initialResponse = value; }
        public string Capture6FromEntryNamed { get => capture6FromEntryNamed; set => capture6FromEntryNamed = value; }
        public string Capture7FromEntryNamed { get => capture7FromEntryNamed; set => capture7FromEntryNamed = value; }
        public string Capture8FromEntryNamed { get => capture8FromEntryNamed; set => capture8FromEntryNamed = value; }
        public string Capture9FromEntryNamed { get => capture9FromEntryNamed; set => capture9FromEntryNamed = value; }
        public string AccountRequestQuery { get => accountRequestQuery; set => accountRequestQuery = value; }
        public string AccountRequestAddToHeader { get => accountRequestAddToHeader; set => accountRequestAddToHeader = value; }
        public string AccountRequestBody { get => accountRequestBody; set => accountRequestBody = value; }
        public string StatementRequestQuery { get => statementRequestQuery; set => statementRequestQuery = value; }
        public string StatementRequestAddToHeader { get => statementRequestAddToHeader; set => statementRequestAddToHeader = value; }
        public string StatementRequestBody { get => statementRequestBody; set => statementRequestBody = value; }
        public string StatementResponseHead { get => statementResponseHead; set => statementResponseHead = value; }
        public string StatementResponseTail { get => statementResponseTail; set => statementResponseTail = value; }
        public string TerminateRequestQuery { get => terminateRequestQuery; set => terminateRequestQuery = value; }
        public string DateFormatsRequest { get => dateFormatsRequest; set => dateFormatsRequest = value; }
        public string DateFormatsResponse { get => dateFormatsResponse; set => dateFormatsResponse = value; }
        public string AmountFormatsResponse { get => amountFormatsResponse; set => amountFormatsResponse = value; }
        public string RequestType { get => requestType; set => requestType = value; }
        public string ResponseType { get => responseType; set => responseType = value; }
        public string FieldSeparator { get => fieldSeparator; set => fieldSeparator = value; }
        public string FilenameRegex { get => filenameRegex; set => filenameRegex = value; }
        public int HeaderNumberOfLines { get => headerNumberOfLines; set => headerNumberOfLines = value; }
        public int DateColumnNumber { get => dateColumnNumber; set => dateColumnNumber = value; }
        public int PayeeColumnNumber { get => payeeColumnNumber; set => payeeColumnNumber = value; }
        public int AmountColumnNumber { get => amountColumnNumber; set => amountColumnNumber = value; }
        public int AddlInfoColumnNumber { get => addlInfoColumnNumber; set => addlInfoColumnNumber = value; }
        public int CreditFlagColumnNumber { get => creditFlagColumnNumber; set => creditFlagColumnNumber = value; }
        public string CreditFlagIsCreditRegex { get => creditFlagIsCreditRegex; set => creditFlagIsCreditRegex = value; }
        public int CheckNumberColumnNumber { get => checkNumberColumnNumber; set => checkNumberColumnNumber = value; }
        public int TransactionColumnNumber { get => transactionColumnNumber; set => transactionColumnNumber = value; }
        public int BalanceColumnNumber { get => balanceColumnNumber; set => balanceColumnNumber = value; }

        public void OnDeserialized(System.Runtime.Serialization.StreamingContext c)
        {
            Bank = Bank.Replace("-", "$");
            Region = Region.Replace("-", "$");
            Account = Account.Replace("-", "$");
            Revision = Revision.Replace("-", "$");
            ModifiedBy = ModifiedBy.Replace("-", "$");
            if (lookupMap == null)
            {
                lookupMap = new Dictionary<String, SortedList<BankConfiguration, BankConfiguration>>();
                regions = new HashSet<string>();
                longNames = new HashSet<String>();
            }
            regions.Add(Region);
            longNames.Add(LongName);
            if (lookupMap.ContainsKey(Name))
            {
                if (!lookupMap[Name].Values.Contains(this))
                {
                    lookupMap[Name].Values.Add(this);
                }
            }
            else
            {
                SortedList<BankConfiguration, BankConfiguration> kindredBanks = 
                    new SortedList<BankConfiguration, BankConfiguration>();
                kindredBanks.Add(this, this);
                lookupMap.Add(Name, kindredBanks);
            }
        }

        public string Name
        {
            get => Bank + "-" + Region + "-" + Account;
        }

        public string LongName
        {
            get => 
                Bank + "-" + Region + "-" + Account + "-" + Revision + "-" + DateModified.ToString("yyyyMMdd") + " - " + ModifiedBy;
        }

        /// <summary>
        /// Is this the most resent of all the bank configurations that have the same (short) Name?
        /// </summary>
        /// <returns>True if this is the most recent of those</returns>
        public bool IsMostRecent()
        {
            IList<BankConfiguration> kindred = lookupMap[Name].Values;
            foreach(BankConfiguration config in kindred)
            {
                if(config.DateModified > this.DateModified)
                {
                    return false;
                }
            }
            return true;
        }

        public static int CompareNames(String lhs, String rhs)
        {
            string[] leftSide = lhs.Split('-');
            string[] rightSide = rhs.Split('-');
            if(leftSide.Length < 1 || rightSide.Length < 1)
            {
                return 0;
            }
            int result = String.Compare(leftSide[0], rightSide[0], true);
            if (result != 0)
            {
                return result;
            }
            if (leftSide.Length < 2 || rightSide.Length < 2)
            {
                return 0;
            }
            result = String.Compare(leftSide[1], rightSide[1], true);
            if (result != 0)
            {
                return result;
            }
            if (leftSide.Length < 3 || rightSide.Length < 3)
            {
                return 0;
            }
            result = String.Compare(leftSide[2], rightSide[2], true);
            if (result != 0)
            {
                return result;
            }
            if (leftSide.Length < 4 || rightSide.Length < 4)
            {
                return 0;
            }
            bool isLeftStd = leftSide[3].ToUpper().StartsWith("STD");
            bool isRightStd = rightSide[3].ToUpper().StartsWith("STD");
            if (isLeftStd && !isRightStd)   // STD revs are always listed before others
            {
                return -1;
            }
            if (!isLeftStd && isRightStd)
            {
                return 1;
            }
            result = String.Compare(leftSide[3], rightSide[3], true);
            if (result != 0)
            {
                return result;
            }
            if (leftSide.Length < 5 || rightSide.Length < 5)
            {
                return 0;
            }
            result = String.Compare(rightSide[4], leftSide[4], true); // (reversed comparison)
            if (result != 0)
            {
                return result;
            }
            if (leftSide.Length < 6 || rightSide.Length < 6)
            {
                return 0;
            }
            result = String.Compare(leftSide[5], rightSide[5], true);
            if(result != 0)
            {
                return result;
            }
            return String.Compare(lhs, rhs);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            BankConfiguration rhs = obj as BankConfiguration;
            return CompareNames(this.LongName, rhs.LongName);
        }

        public int Compare(BankConfiguration lhs, BankConfiguration rhs)
        {
            return CompareNames(this.LongName, rhs.LongName);
        }

        bool IEqualityComparer<BankConfiguration>.Equals(BankConfiguration lhs, BankConfiguration rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        int IEqualityComparer<BankConfiguration>.GetHashCode(BankConfiguration obj)
        {
            return obj.GetHashCode();
        }

        public static String PruneLongToShortName(String longName)
        {
            string[] buffer = longName.Split('-');
            if (buffer.Length < 3)
            {
                return "???-???-???";
            }
            return buffer[0] + "-" + buffer[1] + "-" + buffer[2];
        }

        /// <summary>
        /// Look up a bank configuration by name.
        /// </summary>
        /// <param name="name">Long or short bank name</param>
        /// <returns>The bank configuration, null if not found</returns>
        public static BankConfiguration Get(String name)
        {
            IList<BankConfiguration> banks = lookupMap[PruneLongToShortName(name)].Values;
            foreach(BankConfiguration bank in banks)
            {
                if(bank.Name.Equals(name))
                {
                    return bank;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of the long names, qualified by recency, bank, account, etc.
        /// </summary>
        /// <param name="mostRecent">true to only get the most recent one</param>
        /// <param name="Bank">Expected bank name, leave empty to skip this comparison</param>
        /// <param name="Region">Expected region, leave empty to skip this comparison</param>
        /// <param name="Account">Expected account type, leave empty to skip this comparison</param>
        /// <param name="Revision">Expected revision type, can be shortened or zero-length for partial match</param>
        /// <returns>List of qualified long names</returns>
        public static List<String> GetLongNames(bool mostRecent,
            string Bank, string Region, string Account, string Revision)
        {
            List<String> longNames = LongNames;
            List<string> retained = new List<string>();
            foreach (String longName in longNames)
            {
                BankConfiguration bank = Get(longName);
                if (bank == null)
                {
                    continue;
                }
                if (bank.IsAmong(Bank, Region, Account, Revision))
                {
                    if (!mostRecent || bank.IsMostRecent())
                    {
                        retained.Add(longName);
                    }
                }
            }
            return retained;
        }

        public static List<String> LongNames
        {
            get
            {
                List<String> bankNames = longNames.ToList<String>();
                bankNames.Sort(CompareNames);
                return bankNames;
            }
        }

        public static List<String> Regions
        {
            get
            {
                List<string> sorted = regions.ToList<String>();
                sorted.Sort(String.Compare);
                return sorted;
            }
        }

        /// <summary>
        /// Is this bank configuration among the desired ones?
        /// </summary>
        /// <param name="Bank">Expected bank name, leave empty to skip this comparison</param>
        /// <param name="Region">Expected region, leave empty to skip this comparison</param>
        /// <param name="Account">Expected account type, leave empty to skip this comparison</param>
        /// <param name="Revision">Expected revision type, can be shortened or zero-length for partial match</param>
        /// <returns>true only if it is among the selected candidates</returns>
        public bool IsAmong(string Bank, string Region, string Account, string Revision)
        {
            if (Bank.Length > 0 && String.Compare(this.Bank, Bank, true) != 0)
            {
                return false;
            }
            if (Region.Length > 0 && String.Compare(this.Region, Region, true) != 0)
            {
                return false;
            }
            if (Account.Length > 0 && String.Compare(this.Account, Account, true) != 0)
            {
                return false;
            }
            if (Revision.Length > 0 && String.Compare(this.Revision, 0, Revision, 0, Revision.Length, true) != 0)
            {
                return false;
            }
            return true;
        }

    }

}
