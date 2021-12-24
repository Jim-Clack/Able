using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using AbleCheckbook.Logic;
using AbleCheckbookTests.Db;
using System.IO;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class ReconciliationHelperTests
    {
        [TestMethod()]
        public void FindSumCandidatesTest()
        {
            string dbName = "UtEsTrecsum-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null);
            CheckbookEntry entry = null;
            entry = StaticTestSupport.AddEntry(db, DateTime.Now, 
                "Acme", false, "Paycheck", 150000, "", 0, false);       // 1500.00 paycheck
            entry = StaticTestSupport.AddEntry(db, DateTime.Now,
                "Kroger", true, "Grocery", 12345, "Misc", 4000, false); // -123.45 and -40.00
            entry = StaticTestSupport.AddEntry(db, DateTime.Now,
                "Shell", true, "Transportation", 2378, "", 0, false);   // -23.78
            entry = StaticTestSupport.AddEntry(db, DateTime.Now,
                "Taco Bell", true, "Dining", 2378, "", 0, true);        // already reconciled, ignore
            entry = StaticTestSupport.AddEntry(db, DateTime.Now,
                "Chase", true, "Housing", 99521, "", 0, false);         // -995.21
            entry = StaticTestSupport.AddEntry(db, DateTime.Now,
                "McDonalds", true, "Dining", 1267, "", 0, false);       // ClearIt(), see below...
            ReconciliationHelper helper = new ReconciliationHelper(db);
            helper.CheckIt(entry.Id); 
            List<CandidateEntry> list = helper.FindSumCandidates(-2378, 2, new DateTime().AddYears(10));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].OpenEntries.Count);
            Assert.AreEqual("Shell", list[0].OpenEntries[0].CheckbookEntry.Payee);
            list = helper.FindSumCandidates(-1267, 2, new DateTime().AddYears(10));
            Assert.AreEqual(0, list.Count);
            list = helper.FindSumCandidates(1267, 2, new DateTime().AddYears(10));
            Assert.AreEqual(1, list.Count);
            list = helper.FindSumCandidates(-(99521+2378), 3, new DateTime().AddYears(10));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, list[0].OpenEntries.Count);
            list = helper.FindSumCandidates(150000-(99521+2378), 3, new DateTime().AddYears(10));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(3, list[0].OpenEntries.Count);
            db.Sync();
        }

        [TestMethod()]
        public void FindTransposeCandidatesTest()
        {
            string dbName = "UtEsTrectran-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null);
            StaticTestSupport.AddEntry(db, DateTime.Now,
                "Acme", false, "Paycheck", 150000, "", 0, false);       // 1500.00 paycheck
            StaticTestSupport.AddEntry(db, DateTime.Now,
                "Kroger", true, "Grocery", 12345, "Misc", 4000, false); // -123.45 and -40.00
            StaticTestSupport.AddEntry(db, DateTime.Now,
                "Shell", true, "Transportation", 2378, "", 0, false);   // -23.78
            StaticTestSupport.AddEntry(db, DateTime.Now,
                "Chase", true, "Housing", 99521, "", 0, false);         // -995.21
            StaticTestSupport.AddEntry(db, DateTime.Now,
                "McDonalds", true, "Dining", 1267, "", 0, true);        // cleared, ignore
            ReconciliationHelper helper = new ReconciliationHelper(db);
            List<CandidateEntry> list = helper.FindTransposeCandidates(9, new DateTime().AddYears(10));
            Assert.AreEqual(2, list.Count);
            list = helper.FindTransposeCandidates(-3600, new DateTime().AddYears(10));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Chase", list[0].OpenEntries[0].CheckbookEntry.Payee);
            db.Sync();
        }

    }
}