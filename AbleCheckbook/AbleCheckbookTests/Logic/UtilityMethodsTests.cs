using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Logic;
using System.IO;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class UtilityMethodsTests
    {
        [TestMethod()]
        public void StringToDateTimeTest()
        {
            Assert.AreEqual(new DateTime(1994, 6, 2, 0, 0, 0), UtilityMethods.StringToDateTime("6/ 2/94")); // embedded blanks, 2-digit year
            Assert.AreEqual(new DateTime(2020, 1, 6, 0, 0, 0), UtilityMethods.StringToDateTime("January 6, 2020")); // standard in USA
            Assert.AreEqual(new DateTime(1999, 1, 7, 0, 0, 0), UtilityMethods.StringToDateTime("Jan 7, 1999")); // abbbreviated month
            Assert.AreEqual(new DateTime(2019, 5, 11, 0, 0, 0), UtilityMethods.StringToDateTime("2019/5/11")); // short form
            Assert.AreEqual(new DateTime(1999, 3, 11, 0, 0, 0), UtilityMethods.StringToDateTime("1999 Mar 11")); // standard in Canada
            Assert.AreEqual(new DateTime(2020, 5, 5, 21, 22, 33), UtilityMethods.StringToDateTime("5/5/2020 9:22:33pm")); // time formats
            Assert.AreEqual(new DateTime(2020, 5, 5, 21, 22, 33), UtilityMethods.StringToDateTime("5/5/2020 9:22:33 PM"));
            Assert.AreEqual(new DateTime(2020, 5, 5, 21, 22, 33), UtilityMethods.StringToDateTime("5/5/2020 21:22:33"));
            DateTime testDate = new DateTime(2000, 9, 22, 0, 0, 0); // .NET compatible DateTime formats...
            Assert.AreEqual(testDate, UtilityMethods.StringToDateTime(testDate.ToString()));
            Assert.AreEqual(testDate, UtilityMethods.StringToDateTime(testDate.ToLongDateString() + " " + testDate.ToLongTimeString()));
            Assert.AreEqual(testDate, UtilityMethods.StringToDateTime(testDate.ToShortDateString() + " " + testDate.ToShortTimeString()));
        }

        [TestMethod()]
        public void GuessAtCategoryTest()
        {
            Assert.AreEqual("Dining", UtilityMethods.GuessAtCategory("Taco Mac"));
            Assert.AreEqual("Groceries", UtilityMethods.GuessAtCategory("Stop and Shop"));
            Assert.AreEqual("Transportation", UtilityMethods.GuessAtCategory("Shell Oil"));
            Assert.AreEqual("Housing", UtilityMethods.GuessAtCategory("Rocket Mortgage"));
        }

        [TestMethod()]
        public void DateTimeToDateStringTest()
        {
            DateTime dateTime = new DateTime(2012, 12, 1, 0, 0, 0);
            Assert.AreEqual("12/1/2012", UtilityMethods.DateTimeToString(dateTime));
            Assert.AreEqual("12/1/2012 12:00 AM", UtilityMethods.DateTimeToString(dateTime, true));
            dateTime = DateTime.Now.AddSeconds(30);
            Assert.AreEqual("Just Now", UtilityMethods.DateTimeToString(dateTime, false, true));
            dateTime = DateTime.Now.AddMinutes(30);
            Assert.AreEqual("Today", UtilityMethods.DateTimeToString(dateTime, false, true));
            dateTime = DateTime.Now.AddDays(1);
            Assert.AreEqual("Tomorrow", UtilityMethods.DateTimeToString(dateTime, false, true));
            dateTime = DateTime.Now.AddDays(-1);
            Assert.AreEqual("Yesterday", UtilityMethods.DateTimeToString(dateTime, false, true));
        }

        [TestMethod()]
        public void SearchDbTest()
        {
            string dbName = "UtEsTsearch-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            AddCheckbookEntry(db, "Bellington", false, 4, "", "Misc", 52575, "", 0);
            Guid cashId = AddCheckbookEntry(db, "Kroger", true, 3, "", "Groceries", 2250, "Cash", 1000).Splits[1].CategoryId;
            Guid autoId = AddCheckbookEntry(db, "Shell", true, 2, "1200", "Transportation", 2500, "", 0).Splits[0].CategoryId;
            Guid grocId = AddCheckbookEntry(db, "Kroger", true, 1, "1201", "Groceries", 10195, "", 0).Splits[0].CategoryId;
            AddCheckbookEntry(db, "ConEd", true, 0, "1202", "Utilities", 9999, "", 0);
            List<Guid> list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.Payee, "Kroger", Guid.Empty, 0, 0, false, DateTime.Now);
            Assert.AreEqual(2, list.Count);
            CheckbookEntry entry = db.GetCheckbookEntryById(list[0]);
            Assert.AreEqual("Kroger", entry.Payee);
            entry = db.GetCheckbookEntryById(list[1]);
            Assert.AreEqual("Kroger", entry.Payee);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.Category, "", grocId, 0, 0, false, DateTime.Now);
            Assert.AreEqual(2, list.Count);
            entry = db.GetCheckbookEntryById(list[0]);
            Assert.AreEqual("Kroger", entry.Payee);
            entry = db.GetCheckbookEntryById(list[1]);
            Assert.AreEqual("Kroger", entry.Payee);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.Category, "", grocId, 0, 0, false, DateTime.Now.AddDays(-2));
            Assert.AreEqual(1, list.Count);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.Category, "", cashId, 0, 0, false, DateTime.Now);
            Assert.AreEqual(1, list.Count);
            entry = db.GetCheckbookEntryById(list[0]);
            Assert.AreEqual("Kroger", entry.Payee);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.PayeeSubstring, "LL", Guid.Empty, 0, 0, false, DateTime.Now);
            Assert.AreEqual(2, list.Count);
            entry = db.GetCheckbookEntryById(list[0]);
            Assert.IsTrue(entry.Payee.ToUpper().Contains("LL"));
            entry = db.GetCheckbookEntryById(list[1]);
            Assert.IsTrue(entry.Payee.ToUpper().Contains("LL"));
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.CheckNumberRange, "", Guid.Empty, 1201, 1202, true, DateTime.Now);
            Assert.AreEqual(2, list.Count);
            entry = db.GetCheckbookEntryById(list[0]);
            Assert.IsTrue((new string[] { "1201", "1202" }).Contains(entry.CheckNumber));
            entry = db.GetCheckbookEntryById(list[1]);
            CheckbookEntry newEntry = entry.Clone();
            Assert.IsTrue((new string[] { "1201", "1202" }).Contains(entry.CheckNumber));
            newEntry.IsCleared = true;
            newEntry.Memo = "This is a Test Memo";
            db.UpdateEntry(newEntry, entry, true);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.CheckNumberRange, "", Guid.Empty, 1201, 1202, true, DateTime.Now);
            Assert.AreEqual(1, list.Count);
            list = UtilityMethods.SearchDb(db, UtilityMethods.EntryField.MemoSubstring, "tEST", Guid.Empty, 0, 0, false, DateTime.Now);
            Assert.AreEqual(1, list.Count);
            db.SyncAndClose();
        }

        private static CheckbookEntry AddCheckbookEntry(IDbAccess db, string payee, bool isDebit, int daysAgo,
            string checkNbr, string catName1, int catAmt1, string catName2, int catAmt2)
        {
            TransactionKind kind = isDebit ? TransactionKind.Payment : TransactionKind.Deposit;
            CheckbookEntry ckbkEntry = null;
            ckbkEntry = new CheckbookEntry();
            Guid catId1 = UtilityMethods.GetOrCreateCategory(db, catName1, !isDebit).Id;
            ckbkEntry.AddSplit(catId1, kind, catAmt1);
            if (catAmt2 > 0)
            {
                Guid catId2 = UtilityMethods.GetOrCreateCategory(db, catName2, !isDebit).Id;
                ckbkEntry.AddSplit(catId2, kind, catAmt2);
            }
            ckbkEntry.Payee = payee;
            ckbkEntry.IsCleared = false;
            ckbkEntry.CheckNumber = checkNbr;
            ckbkEntry.DateOfTransaction = DateTime.Now.AddDays(-daysAgo);
            db.InsertEntry(ckbkEntry);
            return ckbkEntry;
        }

        [TestMethod()]
        public void ParseCurrencyTest()
        {
            long amount = UtilityMethods.ParseCurrency("3.00");
            Assert.AreEqual(300L, amount);
            amount = UtilityMethods.ParseCurrency("3");
            Assert.AreEqual(300L, amount);
            amount = UtilityMethods.ParseCurrency("$3");
            Assert.AreEqual(300L, amount);
            amount = UtilityMethods.ParseCurrency("3000");
            Assert.AreEqual(300000L, amount);
            amount = UtilityMethods.ParseCurrency("3,000");
            Assert.AreEqual(300000L, amount);
            amount = UtilityMethods.ParseCurrency("3,000.00");
            Assert.AreEqual(300000L, amount);
            amount = UtilityMethods.ParseCurrency("3.50");
            Assert.AreEqual(350L, amount);
            amount = UtilityMethods.ParseCurrency("3.5");
            Assert.AreEqual(350L, amount);
            amount = UtilityMethods.ParseCurrency("$3.50");
            Assert.AreEqual(350L, amount);
            amount = UtilityMethods.ParseCurrency("3.500");
            Assert.AreEqual(350L, amount);
            amount = UtilityMethods.ParseCurrency("X");
            Assert.AreEqual(0L, amount);
        }
    }
}