using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Logic;
using System.IO;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class BackupsTests
    {
        [TestMethod()]
        public void BackupNowTest()
        {
            string fileToBackup = Path.Combine(Configuration.Instance.DirectoryLogs, "testbak.jnk");
            string fileBackup1 = Path.Combine(Configuration.Instance.DirectoryLogs, "testbak.bu1");
            string fileBackup2 = Path.Combine(Configuration.Instance.DirectoryLogs, "testbak.bu2");
            string fileBackup3 = Path.Combine(Configuration.Instance.DirectoryLogs, "testbak.bu3");
            File.Delete(fileToBackup);
            File.Delete(fileBackup1);
            File.Delete(fileBackup2);
            File.Delete(fileBackup3);
            // File doesn't exist, so don't backup
            string errStr = Backups.BackupNow(fileToBackup, 2, ".bu", true, false);
            Assert.IsNull(errStr);
            Assert.IsFalse(File.Exists(fileToBackup));
            Assert.IsFalse(File.Exists(fileBackup1));
            StreamWriter writer = File.CreateText(fileToBackup);
            writer.WriteLine("Test");
            writer.Close();
            // Backup for the first time
            errStr = Backups.BackupNow(fileToBackup, 2, ".bu", true, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsFalse(File.Exists(fileBackup2));
            // Backup for the second time
            errStr = Backups.BackupNow(fileToBackup, 2, ".bu", true, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsTrue(File.Exists(fileBackup2));
            // Backup for the third time
            errStr = Backups.BackupNow(fileToBackup, 2, ".bu", true, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsTrue(File.Exists(fileBackup2));
            Assert.IsFalse(File.Exists(fileBackup3));
            File.Delete(fileToBackup);
            File.Delete(fileBackup1);
            File.Delete(fileBackup2);
            File.Delete(fileBackup3);
        }

        [TestMethod()]
        public void PeriodicBackupTest()
        {
            string fileToBackup = Path.Combine(Configuration.Instance.DirectoryLogs, "testper.pbk");
            string fileBackup1 = Path.Combine(Configuration.Instance.DirectoryBackup1, "testper.pd1");
            string fileBackup2 = Path.Combine(Configuration.Instance.DirectoryBackup1, "testper.pd2");
            File.Delete(fileToBackup);
            File.Delete(fileBackup1);
            File.Delete(fileBackup2);
            StreamWriter writer = File.CreateText(fileToBackup);
            writer.WriteLine("Test");
            writer.Close();
            // Backup is missing, so backup
            string errStr = Backups.PeriodicBackup(fileToBackup, 7, 2, ".pd", false, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsFalse(File.Exists(fileBackup2));
            // Backup is fresh, so NO further backup
            errStr = Backups.PeriodicBackup(fileToBackup, 7, 2, ".pd", false, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsFalse(File.Exists(fileBackup2));
            // Backup is only five days old, so NO further backup
            File.SetLastWriteTime(fileBackup1, DateTime.Now.AddDays(-5));
            errStr = Backups.PeriodicBackup(fileToBackup, 7, 2, ".pd", false, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsFalse(File.Exists(fileBackup2));
            // Backup is eight days old, so backup
            File.SetLastWriteTime(fileBackup1, DateTime.Now.AddDays(-8));
            errStr = Backups.PeriodicBackup(fileToBackup, 7, 2, ".pd", false, false);
            Assert.IsNull(errStr);
            Assert.IsTrue(File.Exists(fileToBackup));
            Assert.IsTrue(File.Exists(fileBackup1));
            Assert.IsTrue(File.Exists(fileBackup2));
            File.Delete(fileToBackup);
            File.Delete(fileBackup1);
            File.Delete(fileBackup2);
        }
    }
}