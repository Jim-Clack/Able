using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Logic;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class AdminOperationsTests
    {
        [TestMethod()]
        public void GetActivationPinTest()
        {
            string siteId = "" + Environment.ProcessorCount + Environment.MachineName + Configuration.Instance.GetRegionCode();
            char[] array = siteId.ToCharArray();
            Array.Reverse(array);
            siteId = new String(array);
            string licCode = "ChasBu-60135";
            siteId = "DD6";
            string code = SuUserManagement.GetActivationPin(siteId, licCode);
            Assert.IsTrue(code.StartsWith("#"));
            licCode = "FTYUDIFTUYAITFUIYFATUI";
            code = SuUserManagement.GetActivationPin(siteId, licCode);
            Assert.IsTrue(code.StartsWith("#"));
            licCode = "FTY";
            code = SuUserManagement.GetActivationPin(siteId, licCode);
            Assert.IsTrue(code.StartsWith("#"));
        }
    }
}