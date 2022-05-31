using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            Guid id = record.Id;
            record.LicenseDesc = "Test Record";
            record.ContactName = "Ben Dover";
            record.ContactAddress = "123 Main, NYC, NY 01010";
            record.ContactPhone = "123-456-7890";
            record.ContactEMail = "abc@xyz.com";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(id, record.Id);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);

            List<LicenseRecord> records = db.LicensesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            LicenseRecord record2 = records[0];
            Assert.AreEqual(id, record2.Id);
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.ContactName = "Erasmus B Dragon";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);

            LicenseRecord record3 = new LicenseRecord();
            record3.PopulateFrom(record2);
            Assert.AreEqual(id, record3.Id);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Erasmus B Dragon", record3.ContactName);
            bool ok = db.UpdateDb(record3);
            Assert.IsTrue(ok);
            ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);

            records = db.LicensesByDescription(".*");
            Assert.AreEqual(1, records.Count);
            LicenseRecord record4 = records[0];
            Assert.AreEqual(id, record4.Id);
            Assert.AreEqual("Erasmus B Dragon", record4.ContactName);

            db.SyncAndClose();
        }

        [TestMethod]
        public void TestDbDeviceRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;

            DeviceRecord record = new DeviceRecord();
            Guid id = record.Id;
            record.DeviceSite = "Test Record";
            Guid fkId = Guid.NewGuid();
            record.FkLicenseId = fkId;
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(id, record.Id);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);
            List<DeviceRecord> records = db.DevicesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            DeviceRecord record2 = records[0];
            Assert.AreEqual(id, record2.Id);
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.DeviceSite = "Altered Desc";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);

            DeviceRecord record3 = new DeviceRecord();
            record3.PopulateFrom(record2);
            Assert.AreEqual(id, record3.Id);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Altered Desc", record3.DeviceSite);
            bool ok = db.UpdateDb(record3);
            Assert.IsTrue(ok);
            ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);

            records = db.DevicesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            DeviceRecord record4 = records[0];
            Assert.AreEqual(id, record4.Id);
            Assert.AreEqual("Altered Desc", record4.DeviceSite);

            db.SyncAndClose();
        }

        [TestMethod]
        public void TestDbPurchaseRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;

            PurchaseRecord record = new PurchaseRecord();
            Guid id = record.Id;
            record.Details = "Test Record";
            Guid fkId = Guid.NewGuid();
            record.FkLicenseId = fkId;
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            db.UpdateDb(record);
            Assert.AreEqual(id, record.Id);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);

            List<PurchaseRecord> records = db.PurchasesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            PurchaseRecord record2 = records[0];
            Assert.AreEqual(id, record2.Id);
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.Details = "Altered Desc";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);

            PurchaseRecord record3 = new PurchaseRecord();
            record3.PopulateFrom(record2);
            Assert.AreEqual(id, record3.Id);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Altered Desc", record3.Details);
            bool ok = db.UpdateDb(record3);
            Assert.IsTrue(ok);
            ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);

            records = db.PurchasesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            PurchaseRecord record4 = records[0];
            Assert.AreEqual(id, record4.Id);
            Assert.AreEqual("Altered Desc", record4.Details);

            db.SyncAndClose();
        }

        [TestMethod]
        public void TestDbInteractivityRecords()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;

            InteractivityRecord record = new InteractivityRecord();
            Guid id = record.Id;
            record.ClientInfo = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            Guid fkId = Guid.NewGuid();
            record.FkLicenseId = fkId;
            db.UpdateDb(record);
            Assert.AreEqual(id, record.Id);
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);

            List<InteractivityRecord> records = db.InteractivitiesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            InteractivityRecord record2 = records[0];
            Assert.AreEqual(id, record2.Id);
            Assert.AreEqual(EditFlag.Unchanged, record2.EditFlag);
            record2.ClientInfo = "Altered Desc";
            Assert.AreEqual(EditFlag.Modified, record2.EditFlag);

            InteractivityRecord record3 = new InteractivityRecord();
            record3.PopulateFrom(record2);
            Assert.AreEqual(id, record3.Id);
            Assert.AreEqual(EditFlag.Zombie, record2.EditFlag);
            Assert.AreEqual(EditFlag.Modified, record3.EditFlag);
            Assert.AreEqual("Altered Desc", record3.ClientInfo);
            bool ok = db.UpdateDb(record3);
            Assert.IsTrue(ok);
            ok = db.UpdateDb(record2);
            Assert.IsFalse(ok);

            records = db.InteractivitiesByFkLicense(fkId);
            Assert.AreEqual(1, records.Count);
            InteractivityRecord record4 = records[0];
            Assert.AreEqual(id, record4.Id);
            Assert.AreEqual("Altered Desc", record4.ClientInfo);

            db.SyncAndClose();
        }

        [TestMethod]
        public void TestDbPersistence()
        {
            JsonUsersDb.PurgeExisting();
            JsonUsersDb db = JsonUsersDb.Instance;

            LicenseRecord record = new LicenseRecord();
            Guid id = record.Id;
            record.LicenseDesc = "Test Rec New";
            record.ContactName = "Ben Dover";
            record.ContactAddress = "123 Main, NYC, NY 01010";
            record.ContactPhone = "123-456-7890";
            record.ContactEMail = "abc@xyz.com";
            db.UpdateDb(record);

            record = new LicenseRecord();
            record.LicenseDesc = "Different Rec";
            record.ContactName = "Hal O Ween";
            db.UpdateDb(record);

            db.SyncAndClose();
            db.Dispose();

            db = JsonUsersDb.Instance;

            List<LicenseRecord> records = db.LicensesByDescription("Test.*");
            Assert.AreEqual(1, records.Count);
            record = records[0];
            Assert.AreEqual(id, record.Id);
            Assert.AreEqual("Test Rec New", record.LicenseDesc);
            Assert.AreEqual(record.ContactName, "Ben Dover");
            Assert.AreEqual(record.ContactAddress, "123 Main, NYC, NY 01010");
            Assert.AreEqual(record.ContactPhone, "123-456-7890");
            Assert.AreEqual(record.ContactEMail, "abc@xyz.com");
            Assert.AreEqual(EditFlag.Unchanged, record.EditFlag);

            records = db.LicensesByDescription("Diff.*");
            Assert.AreEqual(1, records.Count);
            record = records[0];
            Assert.AreNotEqual(id, record.Id);
            Assert.AreEqual("Different Rec", record.LicenseDesc);

            records = db.LicensesByDescription(".*");
            Assert.AreEqual(2, records.Count);

            db.SyncAndClose();
        }

    }

}
