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
    public class AutofillPayeeTests
    {
        [TestMethod()]
        public void AutofillPayeeTest()
        {
            string dbName = "UtEsTautofill-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null);
            Guid catId1 = new Guid();
            Guid catId2 = new Guid();
            Guid catId3 = new Guid();
            CheckbookEntry entry = null;
            entry = new CheckbookEntry();
            entry.AddSplit(catId1, TransactionKind.Payment, 1234);
            entry.Payee = "ABCD";
            entry.IsCleared = false;
            db.InsertEntry(entry);
            MemorizedPayee payee = new MemorizedPayee("DEFG", catId2, TransactionKind.Payment, -3333);
            db.InsertEntry(payee);
            AutofillPayee autofill = new AutofillPayee(db);
            payee = new MemorizedPayee("DEFZ", catId3, TransactionKind.Payment, -55);
            autofill.UpdateFromMemorizedPayee(payee);
            List<MemorizedPayee> payees = null;
            payees = autofill.LookUp("AB");
            Assert.AreEqual(1, payees.Count);
            Assert.AreEqual("ABCD", payees.First<MemorizedPayee>().Payee);
            payees = autofill.LookUp("DEF");
            Assert.AreEqual(2, payees.Count);
            db.Sync();
        }
    }
}