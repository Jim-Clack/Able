using System;
using System.Collections.Generic;
using AbleLicensing;
using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class UserObjectDbTests
    {

        private Guid id1 = Guid.Empty;
        private Guid id2 = Guid.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            JsonUsersDb.PurgeExisting();
            UserInfo userInfo = null;
            userInfo = new UserInfo();
            userInfo.LicenseRecord = new LicenseRecord();
            userInfo.DeviceRecords = new List<DeviceRecord>();
            userInfo.PurchaseRecords = new List<PurchaseRecord>();
            userInfo.InteractivityRecords = new List<InteractivityRecord>();
            userInfo.LicenseRecord.ContactName = "Ben Dover";
            userInfo.LicenseRecord.ContactAddress = "123 Main";
            userInfo.LicenseRecord.ContactCity = "Nowhere";
            userInfo.LicenseRecord.ContactEMail = "abc@xyz.com";
            userInfo.LicenseRecord.ContactPhone = "123-456-7890";
            userInfo.LicenseRecord.LicenseFeatures = "";
            PurchaseRecord purchase = new PurchaseRecord();
            purchase.Details = "nothing";
            purchase.PurchaseAuthority = PurchaseAuthority.PayPalStd;
            purchase.PurchaseTransaction = "abc123def456";
            purchase.PurchaseVerification = "5555555";
            userInfo.PurchaseRecords.Add(purchase);
            DeviceRecord device0 = new DeviceRecord();
            device0.DeviceSite = "12345-67890";
            device0.UserLevelPunct = UserLevelPunct.Standard;
            device0.CodesAndPin = "1234-56-7890";
            userInfo.DeviceRecords.Add(device0);
            DeviceRecord device1 = new DeviceRecord();
            device1.DeviceSite = "12345-00000";
            device1.UserLevelPunct = UserLevelPunct.Standard;
            device1.CodesAndPin = "9999-56-7890";
            userInfo.DeviceRecords.Add(device1);
            InteractivityRecord interactivity0 = new InteractivityRecord();
            interactivity0.ClientInfo = "123.45.67.002 joshf";
            interactivity0.Conversation = "Initial Activation";
            interactivity0.InteractivityClient = InteractivityClient.ActivationWs;
            userInfo.InteractivityRecords.Add(interactivity0);
            InteractivityRecord interactivity1 = new InteractivityRecord();
            interactivity1.ClientInfo = "123.45.67.007 joshf";
            interactivity1.Conversation = "Spoke w Josh re assistance";
            interactivity1.InteractivityClient = InteractivityClient.PhoneCall;
            interactivity1.ClientInfo = "123.45.67.007 joshf";
            interactivity1.Conversation = "Call back to confirm ok";
            interactivity1.InteractivityClient = InteractivityClient.PhoneCall;
            userInfo.InteractivityRecords.Add(interactivity1);
            ObjectDb.Instance.Update(userInfo);
            id1 = userInfo.LicenseRecord.Id;
            userInfo.LicenseRecord = new LicenseRecord();
            userInfo.DeviceRecords = new List<DeviceRecord>();
            userInfo.PurchaseRecords = new List<PurchaseRecord>();
            userInfo.InteractivityRecords = new List<InteractivityRecord>();
            userInfo.LicenseRecord.ContactName = "Hugh Jass";
            userInfo.LicenseRecord.ContactAddress = "100 Main";
            userInfo.LicenseRecord.ContactCity = "Anywhere";
            userInfo.LicenseRecord.ContactEMail = "xxx@xyz.com";
            userInfo.LicenseRecord.ContactPhone = "333-444-5555";
            userInfo.LicenseRecord.LicenseFeatures = "";
            DeviceRecord device2 = new DeviceRecord();
            device2.DeviceSite = "00000-00000";
            device2.UserLevelPunct = UserLevelPunct.Unlicensed;
            device1.CodesAndPin = "";
            userInfo.DeviceRecords.Add(device2);
            ObjectDb.Instance.Update(userInfo);
            id2 = userInfo.LicenseRecord.Id;
        }

        [TestMethod]
        public void TestGetById()
        {
            List<UserInfo> licenses = ObjectDb.Instance.GetById(id1);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            Assert.AreEqual(licenses[0].LicenseRecord.ContactAddress, "123 Main");
            Assert.AreEqual(licenses[0].LicenseRecord.ContactCity, "Nowhere");
            Assert.AreEqual(licenses[0].LicenseRecord.ContactEMail, "abc@xyz.com");
            Assert.AreEqual(licenses[0].LicenseRecord.ContactPhone, "123-456-7890");
            Assert.AreEqual(1, licenses[0].PurchaseRecords.Count);
            Assert.AreEqual(PurchaseAuthority.PayPalStd, licenses[0].PurchaseRecords[0].PurchaseAuthority);
            Assert.AreEqual("abc123def456", licenses[0].PurchaseRecords[0].PurchaseTransaction);
            Assert.AreEqual("5555555", licenses[0].PurchaseRecords[0].PurchaseVerification);
            Assert.AreEqual("nothing", licenses[0].PurchaseRecords[0].Details);
            Assert.AreEqual(2, licenses[0].DeviceRecords.Count);
            // More to do...
            Assert.AreEqual(2, licenses[0].InteractivityRecords.Count);

            licenses = ObjectDb.Instance.GetById(id2);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);

            licenses = ObjectDb.Instance.GetById(Guid.Empty);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(0, licenses.Count);
        }

        [TestMethod]
        public void TestGetByContactName()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByContactAddress()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByContactCity()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByContactEmail()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByContactPhone()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetBySiteCode()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByRecentInteractivity()
        {
            // Many more tests to do
        }

        [TestMethod]
        public void TestGetByOriginalDate()
        {
            // Many more tests to do
        }

    }

}