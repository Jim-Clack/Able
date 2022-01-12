using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class JsonDbAccessTests
    {
        [TestMethod()]
        public void JsonDbAccessCheckbookEntryTest()
        {
            string dbName = "UtEsTckbk-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CheckbookEntry entry = null;
            entry = new CheckbookEntry();
            entry.AddSplit(Guid.NewGuid(), TransactionKind.Payment, 1234);
            entry.Payee = "ABCD";
            entry.IsCleared = false;
            db.InsertEntry(entry);
            Guid idAbcd = entry.Id;
            Thread.Sleep(100); // long enough to ensure that the two entries have different timestamps
            entry = new CheckbookEntry();
            entry.AddSplit(Guid.NewGuid(), TransactionKind.Deposit, 567);
            entry.Payee = "EFGH";
            entry.IsCleared = false;
            db.InsertEntry(entry);
            CheckbookEntry entryEfgh = entry;
            db.SyncAndClose();
            db = new JsonDbAccess(dbName, null);
            // must be in ascending order by tran date
            CheckbookEntryIterator iter = db.CheckbookEntryIterator;
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            entry = iter.GetNextEntry();
            Assert.AreEqual("ABCD", entry.Payee);
            Assert.AreEqual(-1234, entry.Splits[0].Amount);
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            entry = iter.GetNextEntry();
            Assert.AreEqual("EFGH", entry.Payee);
            entry = db.GetCheckbookEntryById(idAbcd);
            Assert.AreEqual("ABCD", entry.Payee);
            bool ok = db.DeleteEntry(entryEfgh);
            Assert.IsTrue(ok);
            entry = db.GetCheckbookEntryById(entryEfgh.Id);
            Assert.IsNull(entry);
            db.SyncAndClose();
        }

        [TestMethod()]
        public void JsonDbAccessScheduledEventTest()
        {
            string dbName = "UtEsTsched-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            ScheduledEvent schEvent = null;
            schEvent = new ScheduledEvent();
            schEvent.SetDueOneTime(false, DateTime.Now.AddDays(4));
            db.InsertEntry(schEvent);
            Guid id1 = schEvent.Id;
            Thread.Sleep(100); // long enough to ensure that the two entries have different timestamps
            schEvent = new ScheduledEvent();
            schEvent.SetDueOneTime(false, DateTime.Now.AddDays(2));
            db.InsertEntry(schEvent);
            ScheduledEvent entry2 = schEvent;
            db.SyncAndClose();
            db = new JsonDbAccess(dbName, null);
            // must be in ascending order by due date
            ScheduledEventIterator iter = db.ScheduledEventIterator;
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            schEvent = iter.GetNextEntry();
            Assert.AreEqual(schEvent.Period, SchedulePeriod.Annually);
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            schEvent = iter.GetNextEntry();
            Assert.IsTrue(schEvent.EndingDate.CompareTo(DateTime.Now.AddDays(10)) < 0);
            schEvent = db.GetScheduledEventById(id1);
            Assert.IsTrue(schEvent.EndingDate.CompareTo(DateTime.Now.AddDays(10)) < 0);
            bool ok = db.DeleteEntry(entry2);
            Assert.IsTrue(ok);
            schEvent = db.GetScheduledEventById(entry2.Id);
            Assert.IsNull(schEvent);
            db.SyncAndClose();
        }

        [TestMethod()]
        public void JsonDbAccessFinancialCategoryTest()
        {
            string dbName = "UtEsTcateg-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            FinancialCategory finCateg = null;
            finCateg = new FinancialCategory();
            finCateg.Name = "XYZ";
            db.InsertEntry(finCateg);
            Guid id1 = finCateg.Id;
            finCateg = new FinancialCategory();
            finCateg.Name = "ABC";
            db.InsertEntry(finCateg);
            FinancialCategory entry2 = finCateg;
            db.SyncAndClose();
            db = new JsonDbAccess(dbName, null);
            // must be in ascending order by name
            FinancialCategoryIterator iter = db.FinancialCategoryIterator;
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            finCateg = iter.GetNextEntry();
            Assert.AreEqual("ABC", finCateg.Name);
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            finCateg = iter.GetNextEntry();
            Assert.AreEqual("XYZ", finCateg.Name);
            finCateg = db.GetFinancialCategoryById(id1);
            Assert.AreEqual("XYZ", finCateg.Name);
            bool ok = db.DeleteEntry(entry2);
            Assert.IsTrue(ok);
            finCateg = db.GetFinancialCategoryById(entry2.Id);
            Assert.IsNull(finCateg);
            db.SyncAndClose();
        }

        [TestMethod()]
        public void JsonDbAccessUpsertTest()
        {
            string dbName = "UtEsTupsert-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CheckbookEntry entry = null;
            entry = new CheckbookEntry();
            entry.AddSplit(Guid.NewGuid(), TransactionKind.Deposit, 1234);
            entry.Payee = "ABCD";
            entry.IsCleared = false;
            db.InsertEntry(entry);
            Guid id1 = entry.Id;
            Thread.Sleep(100); // long enough to ensure that the two entries have different timestamps
            entry = new CheckbookEntry();
            entry.AddSplit(Guid.NewGuid(), TransactionKind.Deposit, 567);
            entry.Payee = "EFGH";
            entry.IsCleared = false;
            db.InsertEntry(entry);
            Guid id2 = entry.Id;
            CheckbookEntry entry2 = db.GetCheckbookEntryById(id2).Clone();
            entry2.Payee = "IJKL";
            db.UpdateEntry(entry2, entry, false);
            Assert.AreNotEqual(entry.Payee, entry2.Payee);
            Assert.AreEqual(entry.Amount, entry2.Amount);
            CheckbookEntryIterator iter = db.CheckbookEntryIterator;
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            entry = iter.GetNextEntry();
            Assert.AreEqual("ABCD", entry.Payee);
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            entry = iter.GetNextEntry();
            Assert.AreEqual("IJKL", entry.Payee);
            if (iter.HasNextEntry())
            {
                Assert.Fail();
            }
            db.SyncAndClose();
        }

        [TestMethod()]
        public void JsonDbAccessRelationshipTest()
        {
            string dbName = "UtEsTrelat-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            FinancialCategory finCateg = null;
            finCateg = new FinancialCategory();
            finCateg.Name = "XYZ";
            db.InsertEntry(finCateg);
            Guid catId1 = finCateg.Id;
            finCateg = new FinancialCategory();
            finCateg.Name = "PQR";
            db.InsertEntry(finCateg);
            Guid catId2 = finCateg.Id;
            CheckbookEntry ckbkEntry = null;
            ckbkEntry = new CheckbookEntry();
            ckbkEntry.AddSplit(catId1, TransactionKind.Payment, 1234);
            ckbkEntry.AddSplit(catId2, TransactionKind.Payment, 2345);
            ckbkEntry.Payee = "ABCD";
            ckbkEntry.IsCleared = false;
            db.InsertEntry(ckbkEntry);
            db.SyncAndClose();
            db = new JsonDbAccess(dbName, null);
            CheckbookEntryIterator iter = db.CheckbookEntryIterator;
            if (!iter.HasNextEntry())
            {
                Assert.Fail();
            }
            ckbkEntry = iter.GetNextEntry();
            Assert.AreEqual("ABCD", ckbkEntry.Payee);
            finCateg = db.GetFinancialCategoryById(ckbkEntry.Splits[0].CategoryId);
            Assert.AreEqual("XYZ", finCateg.Name);
            finCateg = db.GetFinancialCategoryById(ckbkEntry.Splits[1].CategoryId);
            Assert.AreEqual("PQR", finCateg.Name);
            Assert.AreEqual(-3579, ckbkEntry.Amount);
            db.SyncAndClose();
        }

    }

}