using System;
using System.Collections.Generic;
using AbleLicensing;
using AbleStrategiesServices.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbleStrategies.Testing
{
    [TestClass]
    public class UserInfoDboTests
    {

        private Guid id1 = Guid.Empty;
        private Guid id2 = Guid.Empty;
        private DateTime startTime = DateTime.Now;
        private DateTime betweenTime = DateTime.Now;

        [TestInitialize]
        public void TestInitialize()
        {
            JsonUsersDb.PurgeExisting();
            // UserInfo 
            UserInfo userInfo = null;
            userInfo = new UserInfo();
            userInfo.LicenseRecord = new LicenseRecord();
            userInfo.DeviceRecords = new List<DeviceRecord>();
            userInfo.PurchaseRecords = new List<PurchaseRecord>();
            userInfo.InteractivityRecords = new List<InteractivityRecord>();
            userInfo.LicenseRecord.LicenseCode = "Test-Record";
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
            DeviceRecord device1 = new DeviceRecord();
            device1.DeviceSite = "12345-00000";
            device1.UserLevelPunct = UserLevelPunct.Standard;
            device1.CodesAndPin = "9999-56-7890";
            userInfo.DeviceRecords.Add(device0);
            userInfo.DeviceRecords.Add(device1);
            InteractivityRecord interactivity0 = new InteractivityRecord();
            interactivity0.ClientInfo = "123.45.67.002 joshf";
            interactivity0.Conversation = "Initial Activation";
            interactivity0.InteractivityClient = InteractivityClient.ActivationWs;
            userInfo.InteractivityRecords.Add(interactivity0);
            // Pause, then add another Interactivity
            System.Threading.Thread.Sleep(50);
            betweenTime = DateTime.Now;
            System.Threading.Thread.Sleep(50);
            InteractivityRecord interactivity1 = new InteractivityRecord();
            interactivity1.ClientInfo = "123.45.67.007 joshf";
            interactivity1.Conversation = "Spoke w Josh re assistance";
            interactivity1.ClientInfo = "123.45.67.007 Josh F";
            interactivity1.Conversation = "Call back to confirm ok";
            interactivity1.InteractivityClient = InteractivityClient.PhoneCall;
            userInfo.InteractivityRecords.Add(interactivity1);
            UserInfoDbo.Instance.Update(userInfo);
            id1 = userInfo.LicenseRecord.Id;
            // Another User Info
            userInfo = new UserInfo();
            userInfo.LicenseRecord = new LicenseRecord();
            userInfo.DeviceRecords = new List<DeviceRecord>();
            userInfo.PurchaseRecords = new List<PurchaseRecord>();
            userInfo.InteractivityRecords = new List<InteractivityRecord>();
            userInfo.LicenseRecord.LicenseCode = "Second-Test";
            userInfo.LicenseRecord.ContactName = "Hugh Jass";
            userInfo.LicenseRecord.ContactAddress = "100 Main";
            userInfo.LicenseRecord.ContactCity = "Anywhere";
            userInfo.LicenseRecord.ContactEMail = "xxx@xyz.com";
            userInfo.LicenseRecord.ContactPhone = "333-444-5555";
            userInfo.LicenseRecord.LicenseFeatures = "";
            DeviceRecord device2 = new DeviceRecord();
            device2.DeviceSite = "00000-00000";
            device2.UserLevelPunct = UserLevelPunct.Unlicensed;
            device2.CodesAndPin = "5555-5555-555";
            userInfo.DeviceRecords.Add(device2);
            InteractivityRecord interactivity2 = new InteractivityRecord();
            interactivity2.ClientInfo = "123.45.67.111";
            interactivity2.Conversation = "Another Test";
            interactivity2.InteractivityClient = InteractivityClient.Email;
            userInfo.InteractivityRecords.Add(interactivity2);
            UserInfoDbo.Instance.Update(userInfo);
            id2 = userInfo.LicenseRecord.Id;
        }

        [TestMethod]
        public void TestGetById()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetById(id1);
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
            int index0 = 0;
            int index1 = 1;
            if(licenses[0].DeviceRecords[0].DeviceSite.CompareTo("12345-00000") == 0)
            {
                index0 = 1;
                index1 = 0;
            }
            Assert.AreEqual("1234-56-7890", licenses[0].DeviceRecords[index0].CodesAndPin);
            Assert.AreEqual("9999-56-7890", licenses[0].DeviceRecords[index1].CodesAndPin);
            Assert.AreEqual(2, licenses[0].InteractivityRecords.Count);
            index0 = 0;
            index1 = 1;
            if (licenses[0].InteractivityRecords[0].ClientInfo.CompareTo("123.45.67.007 Josh F") == 0)
            {
                index0 = 1;
                index1 = 0;
            }
            Assert.AreEqual("123.45.67.002 joshf", licenses[0].InteractivityRecords[index0].ClientInfo);
            Assert.AreEqual(InteractivityClient.PhoneCall, licenses[0].InteractivityRecords[index1].InteractivityClient);

            licenses = UserInfoDbo.Instance.GetById(id2);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Hugh Jass");
            Assert.AreEqual(1, licenses[0].DeviceRecords.Count);
            Assert.AreEqual("00000-00000", licenses[0].DeviceRecords[0].DeviceSite);

            licenses = UserInfoDbo.Instance.GetById(Guid.Empty);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(0, licenses.Count);
        }

        [TestMethod]
        public void TestGetByDescription()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByDescription("Test-Record");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            licenses = UserInfoDbo.Instance.GetByDescription(".*Test.*");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
        }

        [TestMethod]
        public void TestGetByContactName()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByContactName("Ben Dover");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactAddress, "123 Main");
            licenses = UserInfoDbo.Instance.GetByContactName("[BH].*");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
            licenses = UserInfoDbo.Instance.GetByContactName("XKG");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(0, licenses.Count);
        }

        [TestMethod]
        public void TestGetByContactAddress()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByContactAddress("123 Main");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            licenses = UserInfoDbo.Instance.GetByContactAddress(".*Main.*");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
        }

        [TestMethod]
        public void TestGetByContactCity()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByContactCity("Anywhere");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Hugh Jass");
            licenses = UserInfoDbo.Instance.GetByContactCity(".*here");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
        }

        [TestMethod]
        public void TestGetByContactEmail()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByContactEmail("abc@xyz.com");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
        }

        [TestMethod]
        public void TestGetByContactPhone()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByContactPhone("123-456");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
        }

        [TestMethod]
        public void TestGetBySiteCode()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetBySiteCode("12345-67890");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            licenses = UserInfoDbo.Instance.GetBySiteCode("12345-00000");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            licenses = UserInfoDbo.Instance.GetBySiteCode("00000-00000");
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Hugh Jass");
        }

        [TestMethod]
        public void TestGetByRecentInteractivity()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByRecentInteractivity(startTime);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
            licenses = UserInfoDbo.Instance.GetByRecentInteractivity(betweenTime);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
            licenses = UserInfoDbo.Instance.GetByRecentInteractivity(DateTime.Now);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(0, licenses.Count);
        }

        [TestMethod]
        public void TestGetByOriginalDate()
        {
            List<UserInfo> licenses = UserInfoDbo.Instance.GetByOriginalDate(startTime, betweenTime);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Ben Dover");
            licenses = UserInfoDbo.Instance.GetByOriginalDate(betweenTime, DateTime.Now);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(1, licenses.Count);
            Assert.AreEqual(licenses[0].LicenseRecord.ContactName, "Hugh Jass");
            licenses = UserInfoDbo.Instance.GetByOriginalDate(startTime, DateTime.Now);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(2, licenses.Count);
            licenses = UserInfoDbo.Instance.GetByOriginalDate(DateTime.Now, DateTime.Now);
            Assert.IsNotNull(licenses);
            Assert.AreEqual(0, licenses.Count);
        }

    }

}