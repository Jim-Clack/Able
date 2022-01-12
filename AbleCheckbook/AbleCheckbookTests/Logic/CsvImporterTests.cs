using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AbleCheckbook.Logic;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class CsvImporterTests
    {
        [TestMethod()]
        public void ImportTest()
        {
            CreateCsv("import.csv", 0);
            string dbName = "UtEsTimpcsv-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CsvImporter importer = new CsvImporter(db);
            int lineNumber = importer.Import("import.csv");
            string errorMessage = importer.ErrorMessage;
            CheckbookEntry entry = null;
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(215443, entry.Amount);
            Assert.AreEqual("", entry.CheckNumber);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            Assert.IsTrue(entry.IsCleared);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-2100, entry.Amount);
            Assert.AreEqual("", entry.CheckNumber);
            Assert.AreEqual(TransactionKind.Payment, entry.Splits[0].Kind);
            Assert.IsTrue(entry.IsCleared);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-7826, entry.Amount);
            Assert.AreEqual("990", entry.CheckNumber);
            Assert.AreEqual(TransactionKind.Payment, entry.Splits[0].Kind);
            Assert.IsTrue(entry.IsCleared);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("991", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("992", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("993", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual("994", entry.CheckNumber);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-183432, entry.Amount);
            db.SyncAndClose();
        }

        /// <summary>
        /// Hardcoded CSV content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void CreateCsv(string filename, int variation)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(Configuration.Instance.DirectoryImportExport, filename));
            writer.WriteLine("Date,Check Number,Description,Debit,Credit,Running Balance");
            writer.WriteLine("01/01/2021,0, PAYCHECK                   XXXX,0.00,2154.43, 2154.43");
            writer.WriteLine("1/2/2021,0, CAPESIDE MEDICAL .P         000-,21.00,0, 1533.41");
            writer.WriteLine("01/03/2021,990, KROGER 39 3330             ACWO,78.26,0,1455.15");
            writer.WriteLine("01/04/2021,991, HENRYS LOUISIANA GRILL     ACWO,7.00,0.00,1563.98");
            writer.WriteLine("01/05/2021 ,  992, KROGER 39 3330             ACWO,9.57,0,1554.41");
            writer.WriteLine("01/06/2021,0, HENRYS LOUISIANA GRILL     ACWO,32.39,0,1570.98");
            writer.WriteLine("01/07/2021,0, AMEX EPAYMENT    ACH PMT    W82,126.02, 0,1730.56");
            writer.WriteLine("01/ 8/2021,0, PAYPAL           INST XFER  WAY,127.19,0,1603.37");
            writer.WriteLine("01/09/2021,993, GARDEN GATE SUBS           8003,32.00,0,3988.12");
            writer.WriteLine("01/10/2021,994, PUBLIX SUPER MAR           KENN, 155.41,0,3690.90");
            writer.WriteLine("01/11/2021,0, AMERICAN HOMES 4 WEB PMTS   HS5,1834.32,0,1856.58");
            writer.Close();
        }
    }
}