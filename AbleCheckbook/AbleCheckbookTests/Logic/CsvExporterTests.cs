using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AbleCheckbookTests.Db;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class CsvExporterTests
    {
        [TestMethod()]
        public void ExportTest()
        {
            string dbName = "UtEsTexpcsv-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 15), 
                "ABCD", false, "Paycheck", 223490, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 16), 
                "DEFG", true, "Rent", -93450, null, 0, false);
            StaticTestSupport.AddEntry(db, new DateTime(2020, 6, 17), 
                "HIJK", true, "Groceries", -12345, "Cash", -4000, true);
            CsvExporter exporter = new CsvExporter(db);
            exporter.Export("export.csv");
            StreamReader reader = new StreamReader(Path.Combine(Configuration.Instance.DirectoryImportExport, "export.csv"));
            string buffer = reader.ReadToEnd();
            string csv =
                "\"Date\",\"Check#\",\"Payee\",\"Category\",\"Memo\",\"Debit\",\"Credit\",\"XCleared\"\r\n" +
                "\"6/15/2020\",\"\",\"ABCD\",\"Paycheck\",\"\",\"0\",\"2234.90\",\"\"\r\n" +
                "\"6/16/2020\",\"\",\"DEFG\",\"Rent\",\"\",\"-934.50\",\"0\",\"\"\r\n" +
                "\"6/17/2020\",\"\",\"HIJK\",\"Groceries\",\"\",\"-163.45\",\"0\",\"X\"\r\n";
            Assert.AreEqual(csv, buffer.ToString());
            reader.Close();
            db.SyncAndClose();
        }

    }
}