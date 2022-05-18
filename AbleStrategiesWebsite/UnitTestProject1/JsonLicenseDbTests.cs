using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class JsonLicenseDbTests
    {
        [TestMethod]
        public void TestDbBasicFunctionality()
        {
            JsonLicenseDb.PurgeExisting();
            JsonLicenseDb db = JsonLicenseDb.Instance;
            LicenseRecord record = new LicenseRecord();
            record.Desc = "Test Record";
            record.ContactName = "Ben Dover";
            record.ContactAddress = "123 Main, NYC, NY 01010";
            record.ContactPhone = "123-456-7890";
            record.ContactEMail = "abc@xyz.com";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);
            List<LicenseRecord> records = db.LicensesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            LicenseRecord record2 = records[0];
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.ContactName = "Erasmus B Dragon";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);
            LicenseRecord record3 = new LicenseRecord();
            record3.Update(record2);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Erasmus B Dragon", record3.ContactName);
            db.UpdateDb(record3);
            bool ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);
            records = db.LicensesByDescription(".*");
            Assert.AreEqual(1, records.Count);
        }
    }
}
