using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void LogErrorTest()
        {
            Logger.Close();
            string fullPath = Path.Combine(Configuration.Instance.DirectoryLogs, "chkbk-diag.log");
            File.Delete(fullPath);
            Logger.Instance.Level = Logger.LogLevel.Warn;
            LogErrorTest1(); // run the test!
            Logger.Close();
            StreamReader reader = new StreamReader(fullPath);
            string log = reader.ReadToEnd();
            Assert.IsTrue(log.Contains("[Error] LoggerTests.LogErrorTest1: Failure in LogErrorTest1"));
            Assert.IsTrue(log.Contains("Exception: AppException in LogErrorTest2"));
            Assert.IsTrue(log.Contains("at AbleCheckbook.Logic.Tests.LoggerTests.LogErrorTest2() in"));
            Assert.IsTrue(log.Contains("at AbleCheckbook.Logic.Tests.LoggerTests.LogErrorTest1() in"));
            reader.Close();
        }

        private void LogErrorTest1()
        {
            try
            {
                LogErrorTest2();
            }
            catch (Exception ex)
            {
                Logger.Error("Failure in LogErrorTest1", ex);
            }
        }

        private void LogErrorTest2()
        {
            try
            {
                StreamReader reader = new StreamReader("QVG" + DateTime.Now.Ticks);
            }
            catch (Exception ex)
            {
                throw new AppException("AppException in LogErrorTest2", ex);
            }
        }

    }

}