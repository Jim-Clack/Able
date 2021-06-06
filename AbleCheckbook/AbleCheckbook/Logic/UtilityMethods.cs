using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public static class UtilityMethods
    {

        /// <summary>
        /// Clamp a value to a range between a min and a max.
        /// </summary>
        /// <param name="min">Lowest acceptable value</param>
        /// <param name="val">Input value</param>
        /// <param name="max">Highest acceptable value</param>
        /// <returns>Input value but clamped between min and max</returns>
        public static int Clamp(int min, int val, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        /// <summary>
        /// Convert a string containing a date and,optionally, time, to a DateTime.
        /// </summary>
        /// <param name="text">to be converted</param>
        /// <returns>The date</returns>
        /// <remarks>If the conversion fails, returns the current date and time</remarks>
        public static DateTime StringToDateTime(string text)
        {
            // try locale-specific date first
            try
            {
                DateTimeFormatInfo format = CultureInfo.CurrentCulture.DateTimeFormat;
                return DateTime.Parse(text, format);
            }
            catch (Exception)
            {
                // fall thru to below...
            }
            string[] splits = text.Split('/');
            // reassemble text by parsing and resequencing fields in it...
            if(splits.Length > 2)
            {
                splits[2] = splits[2].Trim().Split(' ')[0]; // discard extra stuff past date
                // two digit year format?
                if (splits[0].Length <= 2 && splits[2].Length <= 2) 
                {
                    int int0 = int.Parse(splits[0]);
                    int int2 = int.Parse(splits[2]);
                    if(int0 > int2 && int0 >= 70) // first field is year before 2000
                    {
                        text = text.Replace("" + splits[0] + "/", "19" + splits[0] + "/");
                    }
                    else if (int2 > int0 && int2 >= 70) // third field is year before 2000
                    {
                        text = text.Replace("/" + splits[2], "/19" + splits[2]);
                    }
                }
            }
            // swap month and year?
            splits = text.Split('/');
            if (splits.Length > 2)
            {
                splits[2] = splits[2].Trim().Split(' ')[0];
                int int0 = int.Parse(splits[0]);
                int int2 = int.Parse(splits[2]);
                if (int0 > 31)
                {
                    text = splits[1] + "/" + splits[2] + "/" + splits[0];
                }
            }
            // Finally, parse the date from it...
            try
            {
                DateTimeFormatInfo format = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                return DateTime.Parse(text, format);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Get the SHA1 of a string.
        /// </summary>
        /// <param name="input">string to process</param>
        /// <returns>SHA1 of that string</returns>
        public static string ToSha1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var sha1Hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var buffer = new StringBuilder(sha1Hash.Length * 2);
                foreach (byte b in sha1Hash)
                {
                    buffer.Append(b.ToString("X2"));
                }
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Get the full path, name, and extension of a DB file.
        /// </summary>
        /// <param name="baseName">Base name such as Checking, Business, Personal, or Alternate.</param>
        /// <param name="adjustYear">true to adjust the year per the next param: fromLastYear</param>
        /// <param name="fromLastYear">true to return the name of last year's DB.</param>
        /// <returns>full path to DB file like this: C:/Users/Ben/Docs/Able/Checking-2023.db</returns>
        public static string GetDbFilename(string baseName, bool adjustYear, bool fromLastYear)
        {
            int newYear = DateTime.Now.Year;
            int oldYear = newYear - 1;
            int returnYear = (adjustYear && fromLastYear) ? oldYear : newYear;
            baseName = baseName.Replace(".acb", "");
            if (adjustYear)
            {
                baseName = Path.GetFileNameWithoutExtension(baseName).Replace("-" + oldYear, "").Replace("-" + newYear, "");
                return Path.Combine(Configuration.Instance.DirectoryDatabase, baseName) + "-" + returnYear + ".acb";
            }
            if(!baseName.Contains("-" + oldYear) && !baseName.Contains("-" + newYear))
            {
                baseName = baseName + "-" + newYear;
            }
            return Path.Combine(Configuration.Instance.DirectoryDatabase, baseName) + ".acb";
        }

        /// <summary>
        /// CheckbookEntry field selector for searching. 
        /// </summary>
        public enum EntryField
        {
            MemoSubstring =    0,  // Search memo for a substring
            Payee =            1,  // Search for a specific payee
            PayeeSubstring =   2,  // Search for a specific payee
            CheckNumberRange = 3,  // Search numerically for a check number
            Category =         4,  // Search for a specific category by name
        }

        /// <summary>
        /// Search a db for a CheckbookEntry. (case-insensitive)
        /// </summary>
        /// <param name="db">To be searched</param>
        /// <param name="field">which field to search in</param>
        /// <param name="pattern">What text string to search for, "" if not used</param>
        /// <param name="categoryId">category ID if searching for Category, else Guid.Empty</param>
        /// <param name="loCheckNbr">check number min if searching for CheckNumber, else 0</param>
        /// <param name="hiCheckNbr">check number max if searching for CheckNumber, else 0</param>
        /// <param name="activeOnly">true = Not Reconciled or Archived</param>
        /// <param name="beforeDate">ScheduledEvent:Eternity for all</param>
        /// <returns>List of Guids of matching entries, possibly empty.</returns>
        public static List<Guid> SearchDb(IDbAccess db, EntryField field, 
            string pattern, Guid categoryId, int loCheckNbr, int hiCheckNbr, bool activeOnly, DateTime beforeDate)
        {
            pattern = pattern.ToUpper().Trim();
            List<Guid> list = new List<Guid>();
            List<CheckbookEntry> entries = new CheckbookSorter().GetSortedEntries(db, SortEntriesBy.TranDate);
            foreach(CheckbookEntry entry in entries)
            {
                if(activeOnly && entry.IsCleared)
                {
                    continue;
                }
                if(entry.DateOfTransaction.Date.CompareTo(beforeDate.Date) > 0)
                {
                    continue;
                }
                // assume true so this may be used with multiple seearch fields in the future...
                bool found = true; 
                switch(field)
                {
                    case EntryField.MemoSubstring:
                        found = found && entry.Memo.ToUpper().Trim().Contains(pattern);
                        break;
                    case EntryField.PayeeSubstring:
                        found = found && entry.Payee.ToUpper().Trim().Contains(pattern);
                        break;
                    case EntryField.Payee:
                        found = found && entry.Payee.ToUpper().Trim().Equals(pattern);
                        break;
                    case EntryField.CheckNumberRange:
                        long checkNumber = 0;
                        long.TryParse(entry.CheckNumber, out checkNumber);
                        found = found && (checkNumber >= loCheckNbr && checkNumber <= hiCheckNbr);
                        break;
                    case EntryField.Category:
                        bool foundCat = false;
                        foreach(SplitEntry split in entry.Splits)
                        {
                            if(split.CategoryId == categoryId)
                            {
                                foundCat = true;
                            }
                        }
                        found = found && foundCat;
                        break;
                    default:
                        found = false;
                        break;
                }
                if(found)
                {
                    list.Add(entry.Id);
                }
            }
            return list;
        }

        /// <summary>
        /// Convert a DateTime to a date and time string.
        /// </summary>
        /// <param name="dateTime">to be converted</param>
        /// <param name="timeToo">true to display time as well</param>
        /// <param name="descriptive">true to enable things like "Today" or "Just Now"</param>
        /// <returns>string formatted with the date and time</returns>
        public static string DateTimeToString(DateTime dateTime, bool timeToo = false, bool descriptive = false)
        {
            if (descriptive)
            {
                if (dateTime.Date.Equals(DateTime.Now.Date))
                {
                    if (Math.Abs(DateTime.Now.TimeOfDay.Subtract(dateTime.TimeOfDay).TotalMinutes) < 3)
                    {
                        return Strings.Get("Just Now");
                    }
                    return Strings.Get("Today");
                }
                TimeSpan timeSpan = dateTime.Date.Subtract(DateTime.Now.Date);
                if(timeSpan.Days == 1)
                {
                    return Strings.Get("Tomorrow");
                }
                if (timeSpan.Days == -1)
                {
                    return Strings.Get("Yesterday");
                }
            }
            DateTimeFormatInfo format = CultureInfo.CurrentCulture.DateTimeFormat;
            return dateTime.ToString(timeToo ? "g" : "d", format);
        }

        /// <summary>
        /// Format money.
        /// </summary>
        /// <param name="amount">Amount in smallest units, i.e. "cents".</param>
        /// <param name="width">Desired field width</param>
        /// <returns>formatted string</returns>
        public static string FormatCurrency(long amount, int width = 3)
        {
            double money = amount;
            int places = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            if (places > 0)
            {
                float divisor = (float)Math.Pow(10, places);
                money = money / divisor;
            }
            string formatted = string.Format("{0:C}", money);
            int pad = width - formatted.Length;
            if(pad > 0)
            {
                formatted = "                      ".Substring(0, pad) + formatted;
            }
            return formatted;
        }

        /// <summary>
        /// Parse a currency value from a string.
        /// </summary>
        /// <param name="text">Text containing a numeric value</param>
        /// <returns>MOney in smallest units (i.e. cents), or 0 if not valid.</returns>
        public static long ParseCurrency(string text)
        {
            long sign = text.Contains("(") || text.Contains("-") ? -1L : 1L;
            string moneySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            int places = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            string commaSymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
            string dotSymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            float coef = (float)Math.Pow(10, places);
            text = text.Trim().Replace(commaSymbol, "").Replace("(", "").Replace(")", "").Replace("-", "");
            decimal decValue = 0;
            // Allow culture-specific parsing to handle it...
            if(text.Contains(moneySymbol) && Decimal.TryParse(text, out decValue))
            { 
                if (places > 0 && text.Contains(dotSymbol))
                {
                    return sign * (long)((float)decValue * coef);
                }
                return sign * (long)decValue;
            }
            else
            {
                text = text.Replace(moneySymbol, "");
                if (text.Contains(dotSymbol))
                {
                    // It contains a decimal point...
                    string[] fields = text.Split(dotSymbol.ToCharArray());
                    if (fields.Length > 1)
                    {
                        while(fields[1].EndsWith("0"))
                        {
                            fields[1] = fields[1].Substring(0, fields[1].Length - 1);
                        }
                        if (fields[1].Length <= places)
                        {
                            // Supports trailing zero suppression after decimal point 
                            long value;
                            if (long.TryParse(fields[0] + fields[1], out value))
                            {
                                coef = (float)Math.Pow(10, places - fields[1].Length);
                                return sign * (long)((float)value * coef);
                            }
                        }
                    }
                }
                else
                {
                    // It has no decimal point...
                    long value = 0;
                    if(long.TryParse(text, out value))
                    {
                        return sign * (long)(value * coef);
                    }
                }
            }
            return 0L; // Failed!
        }

        /// <summary>
        /// Get a category from the DB or, if not ofund, create it and add it to the DB.
        /// </summary>
        /// <param name="db">To find or update with the category</param>
        /// <param name="name">Name of category</param>
        /// <param name="isCredit">True if this is a credit (positive amount)</param>
        /// <returns>The category, possibly newly created and entered</returns>
        public static FinancialCategory GetOrCreateCategory(IDbAccess db, string name, bool isCredit)
        {
            string catName = Strings.GetIff(name.Trim()).ToLower();
            if(catName.Length < 1)
            {
                catName = Strings.Get("Unknown");
            }
            FinancialCategory category = null;
            FinancialCategoryIterator iterator = db.FinancialCategoryIterator;
            while (iterator.HasNextEntry())
            {
                category = iterator.GetNextEntry();
                if (category.Name.ToLower().Equals(catName))
                {
                    break;
                }
                category = null;
            }
            if (category == null)
            {
                category = new FinancialCategory();
                category.IsCredit = isCredit;
                category.Name = name;
                db.InsertEntry(category);
            }
            return category;
        }

        /// <summary>
        /// Get a category from the DB or, if not found, return the "Unknnown" category.
        /// </summary>
        /// <param name="db">To find or update with the category</param>
        /// <param name="name">Name of category, null to default to "Unknown"</param>
        /// <returns>The category, possibly newly "Unknown"</returns>
        public static FinancialCategory GetCategoryOrUnknown(IDbAccess db, string name)
        {
            string unknownName = Strings.Get("Unknown").Trim().ToLower();
            string catName = unknownName;
            if (name != null && name.Trim().Length > 0)
            {
                catName = Strings.GetIff(name.Trim()).ToLower();
            }
            if (catName.Length < 1)
            {
                catName = unknownName;
            }
            FinancialCategory unknownCategory = null;
            FinancialCategory category = null;
            FinancialCategoryIterator iterator = db.FinancialCategoryIterator;
            while (iterator.HasNextEntry())
            {
                category = iterator.GetNextEntry();
                if (category.Name.ToLower().Equals(catName))
                {
                    break;
                }
                if (category.Name.ToLower().Equals(unknownName))
                {
                    unknownCategory = category;
                }
                category = null;
            }
            if (category == null)
            {
                if(unknownCategory != null)
                {
                    return unknownCategory;
                }
                // Should never happen, but...
                category = new FinancialCategory();
                category.IsCredit = false;
                category.Name = unknownName;
                db.InsertEntry(category);
            }
            return category;
        }

        /// <summary>
        /// Return a file's content as a hexadecimal string.
        /// </summary>
        /// <param name="filename">To be read</param>
        /// <returns>string representation</returns>
        public static string FileContentInHex(string filename)
        {
            if (filename == null || !File.Exists(filename))
            {
                return "";
            }
            FileStream file = null;
            byte[] buffer;
            try
            {
                file = new FileStream(filename, FileMode.Open);
                long length = file.Length;
                if(length < 1)
                {
                    return null;
                }
                buffer = new byte[length];
                if (file.Read(buffer, 0, (int)length) < length)
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Problem reading file content", ex);
                return null;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
            string hexDigits = "0123456789ABCDEF";
            StringBuilder builder = new StringBuilder();
            foreach (byte byt in buffer)
            {
                builder.Append(hexDigits[(byt / 16) & 0x0F]);
                builder.Append(hexDigits[byt & 0x0F]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Convert a hexadecmal string to binary and save it to a file. 
        /// </summary>
        /// <param name="hex">To be converted and saved.</param>
        /// <param name="filePath">Where to save it. Null to select a random fiklename in the log folder.</param>
        /// <returns>Path to file. Null on error.</returns>
        public static string HexToFileContent(string hex, string filePath = null)
        {
            long counter = 0;
            if(filePath == null)
            {
                filePath = Path.Combine(Configuration.Instance.DirectoryLogs, "HF" + counter + ".jpg");
            }
            StringBuilder buffer = new StringBuilder();
            FileStream file = null;
            try
            {
                File.Delete(filePath);
                file = new FileStream(filePath, FileMode.CreateNew);
                for (int index = 0; index < hex.Length; index = index + 2)
                {
                    char ch0 = hex[index];
                    char ch1 = hex[index + 1];
                    byte byt0 = (byte)(ch0 > '9' ? ((ch0 - 7) & 0x0F) : (ch0 & 0x0F));
                    byte byt1 = (byte)(ch1 > '9' ? ((ch1 - 7) & 0x0F) : (ch1 & 0x0F));
                    file.WriteByte((byte)(byt0 * 16 + byt1));
                }
                file.Close();
            }
            catch(Exception ex)
            {
                Logger.Warn("Problem saving file content", ex);
                return null;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
            return filePath;
        }

        /// <summary>
        /// Return the ordinal string of a cardinal number.
        /// </summary>
        /// <param name="cardinal">the number</param>
        /// <returns>its ordinal - "1st", "2nd", etc.</returns>
        public static string Ordinal(int cardinal)
        {
            string ending = "th";
            if ((cardinal % 100) < 11 || (cardinal % 100) > 13)
            {
                switch (cardinal % 10)
                {
                    case 1:
                        ending = "st";
                        break;
                    case 2:
                        ending = "nd";
                        break;
                    case 3:
                        ending = "rd";
                        break;
                    default:
                        break;
                }
            }
            return "" + cardinal + ending;
        }

        /// <summary>
        /// Capitalize the first letter of each word.
        /// </summary>
        /// <param name="text">to be uber capped</param>
        /// <returns>uber capped version</returns>
        public static string UberCaps(string text)
        {
            string[] words = text.Trim().Split(' ');
            StringBuilder buffer = new StringBuilder();
            foreach (string word in words)
            {
                if (word.Length > 1)
                {
                    buffer.Append(word.Substring(0, 1).ToUpper() + word.Substring(1) + " ");
                }
            }
            return buffer.ToString().Trim();
        }

        /// <summary>
        /// Get a list of potential backups.
        /// </summary>
        /// <param name="filename">Template, with or without path or extension</param>
        /// <returns>list of FileInfo for .db, .bu1, .bu2, ... .bw1, .bw2, ...</returns>
        public static List<FileInfo> PotentialBackups(string filename)
        {
            List<FileInfo> list = new List<FileInfo>();
            IEnumerable enumeration = null;
            string directory = Path.GetDirectoryName(filename);
            if(directory.Length < 3)
            {
                directory = Configuration.Instance.DirectoryDatabase;
            }
            string rootFilename = Path.GetFileNameWithoutExtension(filename);
            enumeration = Directory.EnumerateFiles(directory, rootFilename + ".acb", SearchOption.TopDirectoryOnly);
            AppendToFileList(list, enumeration);
            if (Configuration.Instance.DirectoryBackup1.Equals(Configuration.Instance.DirectoryBackup2))
            {
                enumeration = Directory.EnumerateFiles(Configuration.Instance.DirectoryBackup2, rootFilename + ".bu?", SearchOption.TopDirectoryOnly);
                AppendToFileList(list, enumeration);
            }
            else
            {
                enumeration = Directory.EnumerateFiles(Configuration.Instance.DirectoryBackup1, rootFilename + ".bu?", SearchOption.TopDirectoryOnly);
                AppendToFileList(list, enumeration);
            }
            enumeration = Directory.EnumerateFiles(Configuration.Instance.DirectoryBackup2, rootFilename + ".bw?", SearchOption.TopDirectoryOnly);
            AppendToFileList(list, enumeration);
            return list;
        }

        /// <summary>
        /// Support method to append FIleInfo objects to a list from an enumeration of file paths.
        /// </summary>
        /// <param name="list">To be populated with FileInfo's</param>
        /// <param name="enumeration">List of filenames with paths</param>
        private static void AppendToFileList(List<FileInfo> list, IEnumerable enumeration)
        {
            foreach (string filename in enumeration)
            {
                FileInfo fileInfo = new FileInfo(filename);
                list.Add(fileInfo);
            }
        }

        /// <summary>
        /// Populate the minimal records in a new DB.
        /// </summary>
        /// <param name="db">To be populated.</param>
        public static void PopulateNewDb(IDbAccess db)
        {
            GetOrCreateCategory(db, UberCaps(Strings.Get("Dining")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Groceries")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Medical")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Insurance")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Utilities")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Credit")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Cash")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Housing")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Clothing")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Household")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Charity")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Education")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Personal")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Transportation")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Miscellaneous")), false);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Balance Forward")), true);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Income")), true);
            GetOrCreateCategory(db, UberCaps(Strings.Get("Unknown")), false);
        }

        /// <summary>
        /// Guess at a catetgory based on the payee.
        /// </summary>
        /// <param name="payee">Payee name.</param>
        /// <returns>Best guess at a category name - in lowercase.</returns>
        /// <remarks>(hardcoded - should be data-driven and locale-specific)</remarks>
        public static string GuessAtCategory(string payee)
        {
            payee = payee.ToLower().Trim();
            bool found = false;
            string[] guesses = Strings.Get("CategoryGuesses").Split(new char[] { ',' });
            foreach(string guess in guesses)
            {
                if(guess.Length < 2)
                {
                    continue;
                }
                string match = guess.Substring(1);
                switch(guess[0])
                {
                    case '~':
                        if(payee.Contains(match))
                        {
                            found = true;
                        }
                        break;
                    case '=':
                        if (payee.Equals(match))
                        {
                            found = true;
                        }
                        break;
                    case '<':
                        if (payee.StartsWith(match))
                        {
                            found = true;
                        }
                        break;
                    case '>':
                        if (payee.EndsWith(match))
                        {
                            found = true;
                        }
                        break;
                    case '*':
                        return match;
                    case '!':
                        if (found)
                        {
                            return UberCaps(match);
                        }
                        break;
                }
            }
            return "";
        }
    }
}
