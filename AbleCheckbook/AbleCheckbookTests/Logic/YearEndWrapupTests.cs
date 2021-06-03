using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AbleCheckbook.Logic;
using AbleCheckbookTests.Db;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class YearEndWrapupTests
    {
        [TestMethod()]
        public void YearEndWrapupTest()
        {
            DateTime newDate = DateTime.Now;
            DateTime oldDate = newDate.AddYears(-1);
            string dbName = "UtEsTwrap-" + newDate.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, "UtEsTwrap-" + oldDate.Year + ".acb"));
            JsonDbAccess db = new JsonDbAccess(dbName, null);
            CheckbookEntry entry;
            // entered and cleared last year
            entry = StaticTestSupport.AddEntry(db, oldDate, "OLD-ABC", false, "Paycheck", 223490, null, 0, false);
            entry.IsCleared = true;
            entry.DateCleared = oldDate;
            // entered last year, cleared this year
            entry = StaticTestSupport.AddEntry(db, oldDate, "BOTH-DEF", true, "Groceries", 999, null, 0, false);
            entry.IsCleared = true;
            entry.DateCleared = newDate;
            // entered last year, not yet cleared
            entry = StaticTestSupport.AddEntry(db, oldDate, "BOTH-GHI", true, "Transportation", 888, null, 0, false);
            // entered this year, not yet cleared
            entry = StaticTestSupport.AddEntry(db, newDate, "NEW-PQR", true, "Groceries", 777, "Cash", 77, false);
            // entered and cleared this year
            entry = StaticTestSupport.AddEntry(db, newDate, "NEW-STU", true, "Utilities", 666, null, 0, false);
            entry.IsCleared = true;
            entry.DateCleared = newDate;
            db.Sync();

            // Try forcing a year-end
            db = new JsonDbAccess(dbName, null);
            YearEndWrapup yew = new YearEndWrapup(db);
            bool ok = yew.SplitDbsAtDec31(true);
            Assert.IsTrue(ok);
            string message = yew.Message;
            db.Sync();

            // check the old archive db
            db = new JsonDbAccess(dbName.Replace(("" + newDate.Year), ("" + oldDate.Year)), null);
            CheckbookEntryIterator iterator;
            iterator = db.CheckbookEntryIterator;
            HashSet<string> payees = new HashSet<string>();
            while(iterator.HasNextEntry())
            {
                entry = iterator.GetNextEntry();
                payees.Add(entry.Payee);
            }
            Assert.IsTrue(payees.Contains("OLD-ABC"));
            Assert.IsTrue(payees.Contains("BOTH-DEF"));
            Assert.IsTrue(payees.Contains("BOTH-GHI"));

            // check the new updated db
            db = new JsonDbAccess(dbName, null);
            iterator = db.CheckbookEntryIterator;
            payees = new HashSet<string>();
            while (iterator.HasNextEntry())
            {
                entry = iterator.GetNextEntry();
                payees.Add(entry.Payee);
            }
            Assert.IsTrue(payees.Contains("BOTH-DEF"));
            Assert.IsTrue(payees.Contains("BOTH-GHI"));
            Assert.IsTrue(payees.Contains("NEW-PQR"));
            Assert.IsTrue(payees.Contains("NEW-STU"));
        }
    }
}