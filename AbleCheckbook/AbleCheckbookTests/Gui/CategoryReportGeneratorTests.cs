using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using AbleCheckbookTests.Db;
using AbleCheckbook.Logic;
using System.Drawing;
using System.Windows.Forms;
using AbleCheckbook.Db;

namespace AbleCheckbook.Gui.Tests
{
    [TestClass()]
    public class CategoryReportTests
    {

        [TestMethod()]
        public void BuildPieChartArraysTest()
        {
            long[] amountsArray;
            string[] captionsArray;
            JsonDbAccess db = CreateTestDb("UtEsTcatbld-");
            CategoryReportGenerator categoryReport = new CategoryReportGenerator(
                db, new DateTime(2020, 5, 17), new DateTime(2030, 6, 17), true);
            Dictionary<Guid, ReportCategory> reportCategories = categoryReport.GetCategories();
            long income = categoryReport.BuildPieChartArrays(
                reportCategories, out amountsArray, out captionsArray);
            Assert.AreEqual(223490L, income);
            Assert.IsTrue(amountsArray.Length > 4);
            Assert.AreEqual(amountsArray.Length, captionsArray.Length);
            for(int i = 0; i < amountsArray.Length; i++)
            {
                Assert.IsFalse(captionsArray[i].Length < 1);
                Assert.IsFalse(captionsArray[i].Equals("Miscellaneous"));
                if (captionsArray[i].Equals("Other"))
                {
                    Assert.IsTrue(amountsArray[i] > 0);
                }
                else if (captionsArray[i].Equals("Housing"))
                {
                    Assert.AreEqual(63450L, amountsArray[i]);
                }
                else if (captionsArray[i].Equals("Groceries"))
                {
                    Assert.AreEqual(17900L, amountsArray[i]);
                }
                else if (captionsArray[i].Equals("Transportation"))
                {
                    Assert.AreEqual(47750L, amountsArray[i]);
                }
                else if (captionsArray[i].Equals("Dining"))
                {
                    Assert.AreEqual(6666L, amountsArray[i]);
                }
                else if (captionsArray[i].Equals("Cash"))
                {
                    Assert.AreEqual(4000L, amountsArray[i]);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public void CategoryReportTest()
        {
            JsonDbAccess db = CreateTestDb("UtEsTcatrpt-");
            string filePath = Path.Combine(Configuration.Instance.DirectoryLogs, "diagcrpt.jpg");
            File.Delete(filePath);
            Image image = new Bitmap(1500, 2000);
            Rectangle margins = new Rectangle(0, 0, 1500, 2000);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                CategoryReportGenerator categoryReport = new CategoryReportGenerator(
                    db, new DateTime(2020, 5, 17), new DateTime(2030, 6, 17), true);
                categoryReport.PrintPage(graphics, margins);
            }
            image.Save(filePath, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(filePath));
        }

        private static JsonDbAccess CreateTestDb(string name)
        {
            string dbName = name + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null);
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