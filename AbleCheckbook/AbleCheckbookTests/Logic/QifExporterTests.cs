using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Logic;
using System.IO;
using AbleCheckbookTests.Db;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class QifExporterTests
    {
        [TestMethod()]
        public void QifExporterTest()
        {
            string dbName = "UtEsTexpqif-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            DateTime dateTran = new DateTime(2020, 6, 15);
            StaticTestSupport.AddEntry(db, dateTran, 
                "ABCD", false, "Paycheck", 223450, null, 0, false);
            StaticTestSupport.AddEntry(db, dateTran, 
                "DEFG", true, "Rent", -93450, null, 0, false);
            StaticTestSupport.AddEntry(db, dateTran, 
                "HIJK", true, "Groceries", -12345, "Cash", -4000, true); ;
            QifExporter exporter = new QifExporter(db);
            exporter.Export("export.qif");
            StreamReader reader = new StreamReader(Path.Combine(Configuration.Instance.DirectoryImportExport, "export.qif"));
            string buffer = reader.ReadToEnd();
            Assert.IsTrue(buffer.Contains("!Type:Bank"));
            Assert.IsTrue(buffer.Contains("!Type:Cat"));
            Assert.IsTrue(buffer.Contains("!Clear:AutoSwitch\r\n!Account\r\nN"));
            Assert.IsTrue(buffer.Contains("\r\nDChecking\r\nTBank\r\n^"));
            Assert.IsTrue(buffer.Contains("NGroceries\r\nDGroceries (Expense)\r\nE\r\n^"));
            Assert.IsTrue(buffer.Contains("D6/15'20\r\nT-163.45\r\nPHIJK\r\nCX\r\nSGroceries\r\n$-123.45\r\nSCash\r\n$-40.00\r\n^"));
            Assert.IsTrue(buffer.Contains("D6/15'20\r\nT2234.50\r\nPABCD\r\nLPaycheck\r\n^"));
            Assert.IsTrue(buffer.Length > 350);
            reader.Close();
            db.SyncAndClose();
        }

    }
}