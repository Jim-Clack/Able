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
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            DeviceRecord record2 = new DeviceRecord();
            record2.Update(record);
            Assert.AreEqual(record2.Desc, "Test Record");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
