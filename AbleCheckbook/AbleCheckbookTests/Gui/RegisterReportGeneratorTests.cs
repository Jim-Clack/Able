using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Logic;
using System.IO;
using AbleCheckbookTests.Db;
using System.Drawing;
using System.Drawing.Imaging;
using AbleCheckbook.Db;

namespace AbleCheckbook.Gui.Tests
{
    [TestClass()]
    public class RegisterReportTests
    {
        [TestMethod()]
        public void RegisterReportTest()
        {
            JsonDbAccess db = CreateTestDb("UtEsTregrpt-");
            RegisterReportGenerator registerReport = new RegisterReportGenerator(
                db, new DateTime(2020, 5, 17), new DateTime(2030, 6, 17));
            string filePath = Path.Combine(Configuration.Instance.DirectoryLogs, "diagrrpt.jpg");
            File.Delete(filePath);
            Image image = new Bitmap(1500, 2000);
            Rectangle margins = new Rectangle(0, 0, 1500, 2000);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                registerReport.PrintPage(graphics, margins);
            }
            image.Save(filePath, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(filePath));
        }

        private static JsonDbAccess CreateTestDb(string name)
        {
            string dbName = name + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 15),
                "Acme", false, "Paycheck", 223490, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 5, 16),
                "OldBank", true, "Housing", -53450, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 16),
                "CitiBank", true, "Housing", -63450, null, 0, false).CheckNumber = "1258";
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 17),
                "Kroger", true, "Groceries", -12345, "Cash", -4000, true).CheckNumber = "1259";
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 16),
                "Toyota", true, "Transportation", -47750, null, 0, false).CheckNumber = "1260";
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 17),
                "7-11", true, "Miscellany", -66, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 17),
                "Outback", true, "Dining", -6666, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 17),
                "Publix", true, "Groceries", -5555, null, 0, true);
            return db;
        }

    }

}