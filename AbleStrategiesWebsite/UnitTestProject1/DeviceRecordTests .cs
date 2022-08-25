using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class DeviceRecordTests
    {
        [TestMethod]
        public void TestUpdate()
        {
            DeviceRecord record = new DeviceRecord();
            record.DeviceSiteId = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            DeviceRecord record2 = new DeviceRecord();
            record2.PopulateFrom(record);
            Assert.AreEqual(record2.DeviceSiteId, "Test Record");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
