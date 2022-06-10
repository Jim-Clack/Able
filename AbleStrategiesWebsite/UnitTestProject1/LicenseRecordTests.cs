using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class LicenseRecordTests
    {
        [TestMethod]
        public void TestUpdate()
        {
            LicenseRecord record = new LicenseRecord();
            record.LicenseCode = "Test Record";
            record.ContactName = "Ben Dover";
            record.ContactAddress = "123 Main, NYC, NY 01010";
            record.ContactPhone = "123-456-7890";
            record.ContactEMail = "abc@xyz.com";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            LicenseRecord record2 = new LicenseRecord();
            record2.PopulateFrom(record);
            Assert.AreEqual(record2.LicenseCode, "Test Record");
            Assert.AreEqual(record2.ContactName, "Ben Dover");
            Assert.AreEqual(record2.ContactAddress, "123 Main, NYC, NY 01010");
            Assert.AreEqual(record2.ContactPhone, "123-456-7890");
            Assert.AreEqual(record2.ContactEMail, "abc@xyz.com");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
