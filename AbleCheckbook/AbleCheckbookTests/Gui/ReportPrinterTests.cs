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
using AbleCheckbook.Db;

namespace AbleCheckbook.Gui.Tests
{
    [TestClass()]
    public class PrinterTests
    {
        [TestMethod()]
        public void PrintRegisterReportTest()
        {
            if (false) // disabled for purposes of continuous testing
            {
                JsonDbAccess db = CreateTestDb("UtEsTprtreg-");
                ReportPrinter printer = new ReportPrinter();
                printer.PrintRegisterReport(db, new DateTime(2020, 5, 17), new DateTime(2030, 6, 17));
            }
        }

        [TestMethod()]
        public void PrintRegisterReportWithDlgTest()
        {
            if (false) // disabled for purposes of continuous testing - must be run in Main()
            {
                JsonDbAccess db = CreateTestDb("UtEsTprtreg2-");
                ReportPrinter printer = new ReportPrinter();
                printer.PrintRegisterReport(db);
            }
        }

        [TestMethod()]
        public void PrintCategoryReportTest()
        {
            if (false) // disabled for purposes of continuous testing
            {
                JsonDbAccess db = CreateTestDb("UtEsTprtcat-");
                ReportPrinter printer = new ReportPrinter();
                printer.PrintCategoryReport(db, new DateTime(2020, 5, 17), new DateTime(2030, 6, 17), true);
            }
        }

        [TestMethod()]
        public void PrintCategoryReportWithDlgTest()
        {
            if (false) // disabled for purposes of continuous testing - must be run in Main()
            {
                JsonDbAccess db = CreateTestDb("UtEsTprtcat2-");
                ReportPrinter printer = new ReportPrinter();
                printer.PrintCategoryReport(db);
            }
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
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 16),
                "First Baptist", true, "Charity", -2000, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 16),
                "Toyota", true, "Transportation", -47750, null, 0, false).CheckNumber = "1261";
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 17),
                "Kroger", true, "Groceries", -8345, "Cash", -2000, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 16),
                "Megans", true, "Personal", -1750, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 17),
                "KSU", true, "Education", -9966, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 17),
                "Ruby Tuesday", true, "Dining", -4466, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 18),
                "Publix", true, "Groceries", -4529, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 17),
                "Fridays", true, "Dining", -3975, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 28),
                "Aldi", true, "Groceries", -5022, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 26),
                "Hairs To Ya", true, "Personal", -2250, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 27),
                "KSU", true, "Education", -9966, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 27),
                "Fridays", true, "Dining", -3975, null, 0, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 28),
                "Whole Foods", true, "Groceries", -5022, "Cash", -2000, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 21),
                "Mobile", true, "Transportation", -2550, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 1),
                "Shell", true, "Transportation", -2000, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 17),
                "Mobile", true, "Transportation", -2100, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 29),
                "Mobile", true, "Transportation", -1850, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 7, 5),
                "Publix", true, "Groceries", -9975, "Cash", 2000, true);
            db.Sync();
            return db;
        }

    }

}