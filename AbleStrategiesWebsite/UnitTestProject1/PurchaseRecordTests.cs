using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class PurchaseRecordTests
    {
        [TestMethod]
        public void TestUpdate()
        {
            PurchaseRecord record = new PurchaseRecord();
            record.Desc = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            PurchaseRecord record2 = new PurchaseRecord();
            record2.Update(record);
            Assert.AreEqual(record2.Desc, "Test Record");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
