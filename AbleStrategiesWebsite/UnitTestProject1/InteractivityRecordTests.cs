using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class InteractivityRecordTests
    {
        [TestMethod]
        public void TestUpdate()
        {
            InteractivityRecord record = new InteractivityRecord();
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            InteractivityRecord record2 = new InteractivityRecord();
            record2.Update(record);
            Assert.AreEqual(record2.Desc, "Test Record");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
