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
            record.Details = "Test Record";
            Assert.AreEqual(EditFlag.New, record.EditFlag);
            PurchaseRecord record2 = new PurchaseRecord();
            record2.PopulateFrom(record);
            Assert.AreEqual(record2.Details, "Test Record");
            Assert.AreEqual(EditFlag.New, record2.EditFlag);
            Assert.AreEqual(EditFlag.Zombie, record.EditFlag);
        }
    }
}
