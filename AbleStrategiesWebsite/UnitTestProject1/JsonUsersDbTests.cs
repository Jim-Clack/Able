using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class JsonUsersDbTests
    {
        [TestMethod]
        public void TestDbLicenseRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;
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
            LicenseRecord record4 = records[0];
            Assert.AreEqual("Erasmus B Dragon", record4.ContactName);
        }

        [TestMethod]
        public void TestDbDeviceRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;
            DeviceRecord record = new DeviceRecord();
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);
            List<DeviceRecord> records = db.DevicesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            DeviceRecord record2 = records[0];
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.Desc = "Erasmus B Dragon";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);
            DeviceRecord record3 = new DeviceRecord();
            record3.Update(record2);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Erasmus B Dragon", record3.Desc);
            db.UpdateDb(record3);
            bool ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);
            records = db.DevicesByDescription(".*");
            Assert.AreEqual(1, records.Count);
            DeviceRecord record4 = records[0];
            Assert.AreEqual("Erasmus B Dragon", record4.Desc);
        }

        [TestMethod]
        public void TestDbPurchaseRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;
            PurchaseRecord record = new PurchaseRecord();
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);
            List<PurchaseRecord> records = db.PurchasesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            PurchaseRecord record2 = records[0];
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.Desc = "Erasmus B Dragon";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);
            PurchaseRecord record3 = new PurchaseRecord();
            record3.Update(record2);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Erasmus B Dragon", record3.Desc);
            db.UpdateDb(record3);
            bool ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);
            records = db.PurchasesByDescription(".*");
            Assert.AreEqual(1, records.Count);
            PurchaseRecord record4 = records[0];
            Assert.AreEqual("Erasmus B Dragon", record4.Desc);
        }

        [TestMethod]
        public void TestDbInteractivityRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;
            InteractivityRecord record = new InteractivityRecord();
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);
            List<InteractivityRecord> records = db.InteractivitiesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            InteractivityRecord record2 = records[0];
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.Desc = "Erasmus B Dragon";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);
            InteractivityRecord record3 = new InteractivityRecord();
            record3.Update(record2);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Erasmus B Dragon", record3.Desc);
            db.UpdateDb(record3);
            bool ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);
            records = db.InteractivitiesByDescription(".*");
            Assert.AreEqual(1, records.Count);
            InteractivityRecord record4 = records[0];
            Assert.AreEqual("Erasmus B Dragon", record4.Desc);
        }

    }
}
