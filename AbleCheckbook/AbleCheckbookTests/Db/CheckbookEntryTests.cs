using AbleCheckbook.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class CheckbookEntryTests
    {
        [TestMethod()]
        public void CheckbookEntryTest()
        {
            CheckbookEntry ckbk1 = AddCheckbookEntry("ABCD", true, new Guid(), 1234, Guid.Empty, 0);
            ckbk1.CheckNumber = "1234";
            Assert.AreEqual("ABCD", ckbk1.Payee);
            Assert.AreEqual("1234", ckbk1.CheckNumber);
            Assert.AreEqual(1, ckbk1.Splits.Length);
            Assert.AreEqual(-1234, ckbk1.Splits[0].Amount);
        }

        [TestMethod()]
        public void ResetMemoTest()
        {
            CheckbookEntry ckbk1 = AddCheckbookEntry("ABCD", true, new Guid(), 1234, Guid.Empty, 0);
            ckbk1.Memo = "memo";
            Assert.AreEqual("memo", ckbk1.Memo);
            ckbk1.ResetMemo();
            Assert.AreEqual("", ckbk1.Memo);
        }

        [TestMethod()]
        public void DeleteSplitTest()
        {
            CheckbookEntry ckbk1 = AddCheckbookEntry("ABCD", true, new Guid(), 1234, new Guid(), 222);
            Assert.AreEqual(2, ckbk1.Splits.Length);
            ckbk1.DeleteSplit(ckbk1.Splits[0]);
            Assert.AreEqual(1, ckbk1.Splits.Length);
            Assert.AreEqual(-222, ckbk1.Splits[0].Amount);
        }

        ///////////////////////////////// support ////////////////////////////

        private static CheckbookEntry AddCheckbookEntry(string payee, bool isDebit, Guid catId1, int amt1, Guid catId2, int amt2)
        {
            TransactionKind kind = isDebit ? TransactionKind.Payment : TransactionKind.Deposit;
            CheckbookEntry ckbkEntry = null;
            ckbkEntry = new CheckbookEntry();
            ckbkEntry.AddSplit(catId1, kind, amt1);
            if (amt2 > 0)
            {
                ckbkEntry.AddSplit(catId2, kind, amt2);
            }
            ckbkEntry.Payee = payee;
            ckbkEntry.IsCleared = false;
            return ckbkEntry;
        }

    }
}