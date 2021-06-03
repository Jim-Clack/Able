using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class UndoableDbAccessTests
    {
        [TestMethod()]
        public void MarkUndoRedoTest()
        {
            string dbName = "UtEsTundo-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            UndoableDbAccess db = new UndoableDbAccess(dbName);
            // undo A: fin categ catId1 name=XYZ
            db.MarkUndoBlock("A");
            Guid catId1 = AddFinancialCategory(db, "XYZ").Id;
            // undo B: chbk entry ckbkId1 payee = ABCD
            db.MarkUndoBlock("B");
            Guid ckbkId1 = AddCheckbookEntry(db, "ABCD", true, catId1, 1234, catId1, 0).Id;
            db.MarkUndoBlock("C");
            // undo C: chbk entry ckbkId2 payee = WXYZ
            Guid ckbkId2 = AddCheckbookEntry(db, "WXYZ", true, catId1, 2222, catId1, 0).Id;
            // undo D: sch entry schId1 repeat = 12
            db.MarkUndoBlock("D");
            Guid schId1 = AddScheduledEvent(db, 12, 4).Id;
            // make sure they all exist
            FinancialCategory finCateg = db.GetFinancialCategoryById(catId1);
            Assert.AreEqual(finCateg.Name, "XYZ");
            CheckbookEntry ckbkEntry = db.GetCheckbookEntryById(ckbkId1);
            Assert.AreEqual(ckbkEntry.Payee, "ABCD");
            ckbkEntry = db.GetCheckbookEntryById(ckbkId2);
            Assert.AreEqual(ckbkEntry.Payee, "WXYZ");
            ScheduledEvent schEvent = db.GetScheduledEventById(schId1);
            Assert.AreEqual(schEvent.GetRepeatCount(DateTime.Now), 12);
            // try one undo
            Assert.AreEqual(db.DescriptionOfNextUndo, "D");
            db.UndoToLastMark();
            ckbkEntry = db.GetCheckbookEntryById(ckbkId2);
            Assert.AreEqual(ckbkEntry.Payee, "WXYZ");
            // try another
            Assert.AreEqual(db.DescriptionOfNextUndo, "C");
            db.UndoToLastMark();
            ckbkEntry = db.GetCheckbookEntryById(ckbkId1);
            Assert.AreEqual(ckbkEntry.Payee, "ABCD");
            // try a redo
            db.RedoToNextMark();
            ckbkEntry = db.GetCheckbookEntryById(ckbkId2);
            Assert.AreEqual(ckbkEntry.Payee, "WXYZ");
            // try a redo
            db.RedoToNextMark();
            schEvent = db.GetScheduledEventById(schId1);
            Assert.AreEqual(schEvent.GetRepeatCount(DateTime.Now), 12);
            // try releasing 1 undo
            db.ReleaseWeakData(1);
            db.UndoToLastMark(); // delete schev
            db.UndoToLastMark(); // delete ckbk2
            db.UndoToLastMark(); // delete ckbk1
            Assert.AreEqual(db.DescriptionOfNextUndo, "");
            Assert.AreEqual(db.DescriptionOfNextRedo, "B");
            db.UndoToLastMark(); // <-- should do nothing, fincat should remain
            finCateg = db.GetFinancialCategoryById(catId1); 
            Assert.AreEqual(finCateg.Name, "XYZ");
            db.RedoToNextMark();
            db.RedoToNextMark();
            db.RedoToNextMark();
            finCateg = db.GetFinancialCategoryById(catId1);
            Assert.AreEqual(finCateg.Name, "XYZ");
            ckbkEntry = db.GetCheckbookEntryById(ckbkId1);
            Assert.AreEqual(ckbkEntry.Payee, "ABCD");
            ckbkEntry = db.GetCheckbookEntryById(ckbkId2);
            Assert.AreEqual(ckbkEntry.Payee, "WXYZ");
            schEvent = db.GetScheduledEventById(schId1);
            Assert.AreEqual(schEvent.GetRepeatCount(DateTime.Now), 12);
            // try releasing all undo/redo steps
            db.ReleaseWeakData(100);
            db.UndoToLastMark(); // this should do nothing
            finCateg = db.GetFinancialCategoryById(catId1);
            Assert.AreEqual(finCateg.Name, "XYZ");
            ckbkEntry = db.GetCheckbookEntryById(ckbkId1);
            Assert.AreEqual(ckbkEntry.Payee, "ABCD");
            ckbkEntry = db.GetCheckbookEntryById(ckbkId2);
            Assert.AreEqual(ckbkEntry.Payee, "WXYZ");
            schEvent = db.GetScheduledEventById(schId1);
            Assert.AreEqual(schEvent.GetRepeatCount(DateTime.Now), 12);
            db.Sync(); 
        }

        private static ScheduledEvent AddScheduledEvent(IDbAccess db, short repeatCount, int addDays)
        {
            ScheduledEvent schEvent = null;
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(false, DateTime.Now.AddDays(addDays).Day, DateTime.Now.AddDays(addDays).AddMonths(1));
            schEvent.SetRepeatCount(repeatCount);
            db.InsertEntry(schEvent);
            return schEvent;
        }

        private static FinancialCategory AddFinancialCategory(IDbAccess db, string name)
        {
            FinancialCategory finCateg = null;
            finCateg = new FinancialCategory();
            finCateg.Name = name;
            db.InsertEntry(finCateg);
            return finCateg;
        }

        private static CheckbookEntry AddCheckbookEntry(
            IDbAccess db, string payee, bool isDebit, Guid catId1, int amt1, Guid catId2, int amt2)
        {
            TransactionKind kind = isDebit ? TransactionKind.Payment : TransactionKind.Deposit;
            CheckbookEntry ckbkEntry = null;
            ckbkEntry = new CheckbookEntry();
            ckbkEntry.AddSplit(catId1, kind, amt1);
            if(amt2 > 0)
            {
                ckbkEntry.AddSplit(catId2, kind, amt2);
            }
            ckbkEntry.Payee = payee;
            ckbkEntry.IsCleared = false;
            db.InsertEntry(ckbkEntry);
            return ckbkEntry;
        }
    }
}