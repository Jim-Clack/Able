using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class AutoReconcilerTests
    {
        [TestMethod()]
        public void ScoreTest()
        {
            AutoReconciler reconciler = new AutoReconciler(null, null);
            CheckbookEntry entry1, entry2;
            double score;
            // typical comparison
            entry1 = AddCheckbookEntry("Anchor Bank", true, new Guid(), 2233, Guid.Empty, 0);
            entry2 = AddCheckbookEntry("Hshld Bank", true, new Guid(), 2233, Guid.Empty, 0);
            entry2.DateOfTransaction = entry2.DateOfTransaction.AddDays(3);
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score >= (int)AutoReconciler.ThresholdScore.Probable, "AutoReconciler test1 Score: " + score);
            // matched check number
            entry1 = AddCheckbookEntry("NBC", true, new Guid(), 2233, Guid.Empty, 0);
            entry2 = AddCheckbookEntry("Check", true, new Guid(), 2233, Guid.Empty, 0);
            entry2.DateOfTransaction = entry2.DateOfTransaction.AddDays(3);
            entry1.CheckNumber = "333";
            entry2.CheckNumber = "333";
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score >= (int)AutoReconciler.ThresholdScore.Probable, "AutoReconciler test2 Score: " + score);
            // mismatched amounts
            entry1 = AddCheckbookEntry("Anchor Bank", true, new Guid(), 2233, Guid.Empty, 0);
            entry2 = AddCheckbookEntry("Hshld Bank", true, new Guid(), 999, Guid.Empty, 0);
            entry2.DateOfTransaction = entry2.DateOfTransaction.AddDays(3);
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score <= (int)AutoReconciler.ThresholdScore.Unlikely, "AutoReconciler test3 Score: " + score);
            // payee consanants comparison
            entry1 = AddCheckbookEntry("Anchor Ntnl", true, new Guid(), 2233, Guid.Empty, 0);
            entry2 = AddCheckbookEntry("Hshld National", true, new Guid(), 2233, Guid.Empty, 0);
            entry2.DateOfTransaction = entry2.DateOfTransaction.AddDays(3);
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score >= (int)AutoReconciler.ThresholdScore.Probable, "AutoReconciler test4 Score: " + score);
            // match payee
            entry1 = AddCheckbookEntry("timelife magazine", true, new Guid(), 2233, Guid.Empty, 0);
            entry2 = AddCheckbookEntry("Time Magazine", true, new Guid(), 2233, Guid.Empty, 0);
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score >= (int)AutoReconciler.ThresholdScore.Matched, "AutoReconciler test5 Score: " + score);
            // use bank info - neg & pos tests
            entry1 = AddCheckbookEntry("Abcdefg", true, new Guid(), 2233, Guid.Empty, 0);
            entry1.DateOfTransaction = DateTime.Now;
            entry2 = AddCheckbookEntry("", true, new Guid(), 0, Guid.Empty, 0);
            entry2.BankAmount = -2233;
            entry2.BankPayee = "Abcdefg";
            entry2.BankTranDate = DateTime.Now.AddDays(3);
            score = reconciler.Score(entry1, entry2, false);
            Assert.IsTrue(score <= (int)AutoReconciler.ThresholdScore.Possible, "AutoReconciler test6a Score: " + score);
            score = reconciler.Score(entry1, entry2, true);
            Assert.IsTrue(score >= (int)AutoReconciler.ThresholdScore.Probable, "AutoReconciler test6b Score: " + score);
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