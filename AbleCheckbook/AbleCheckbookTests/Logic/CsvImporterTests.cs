using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class CsvImporterTests
    {
        [TestMethod()]
        public void ImportTest1()
        {
            CreateCsv1("import1.csv", 0);
            string dbName = "UtEsTimpcsv1-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CsvImporter importer = new CsvImporter(db);
            int lineNumber = importer.Import("import1.csv");
            string errorMessage = importer.ErrorMessage;
            CheckbookEntry entry = null;
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(215443, entry.Amount);
            Assert.AreEqual("", entry.CheckNumber);
            Assert.IsTrue(entry.Payee.ToUpper().StartsWith("PAYCHECK"));
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

        [TestMethod()]
        public void ImportTest2()
        {
            CreateCsv2("import2.csv", 0);
            string dbName = "UtEsTimpcsv2-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CsvImporter importer = new CsvImporter(db);
            int lineNumber = importer.Import("import2.csv");
            string errorMessage = importer.ErrorMessage;
            CheckbookEntry entry = null;
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(2, entry.Amount);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            Assert.AreEqual("INTEREST PAYMENT", entry.Payee);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-400, entry.Amount);
            Assert.AreEqual(TransactionKind.Payment, entry.Splits[0].Kind);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(299, entry.Amount);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(360000, entry.Amount);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            db.SyncAndClose();
        }

        [TestMethod()]
        public void ImportTest3()
        {
            CreateCsv3("import3.csv", 0);
            string dbName = "UtEsTimpcsv3-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CsvImporter importer = new CsvImporter(db);
            int lineNumber = importer.Import("import3.csv");
            string errorMessage = importer.ErrorMessage;
            CheckbookEntry entry = null;
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-1585, entry.Amount);
            Assert.AreEqual(TransactionKind.Payment, entry.Splits[0].Kind);
            Assert.AreEqual("McDonalds", entry.Payee);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(123450, entry.Amount);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            db.SyncAndClose();
        }

        [TestMethod()]
        public void ImportTest4()
        {
            CreateCsv4("import4.csv", 0);
            string dbName = "UtEsTimpcsv4-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            CsvImporter importer = new CsvImporter(db);
            int lineNumber = importer.Import("import4.csv");
            string errorMessage = importer.ErrorMessage;
            CheckbookEntry entry = null;
            CheckbookEntryIterator iterator = db.CheckbookEntryIterator;
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(-1585, entry.Amount);
            Assert.AreEqual(TransactionKind.Payment, entry.Splits[0].Kind);
            Assert.AreEqual("Mc\"Donalds", entry.Payee);
            Assert.IsTrue(iterator.HasNextEntry());
            entry = iterator.GetNextEntry();
            Assert.AreEqual(123450, entry.Amount);
            Assert.AreEqual(TransactionKind.Deposit, entry.Splits[0].Kind);
            db.SyncAndClose();
        }

        /// <summary>
        /// Hardcoded CSV content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void CreateCsv1(string filename, int variation)
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

        /// <summary>
        /// Hardcoded CSV content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void CreateCsv2(string filename, int variation)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(Configuration.Instance.DirectoryImportExport, filename));
            writer.WriteLine("Date,Transaction Type,Serial Number,Description,Amount,Daily Posted Balance");
            writer.WriteLine("03/24/2022,Interest,,INTEREST PAYMENT,$+0.02,$3170.81");
            writer.WriteLine("03/25/2022,POS,,NYTimes* NYTimes 9341 DEBIT CARD RECURRING PYMT,($4),");
            writer.WriteLine("03/25/2022,Debit,,ACCOUNT NUMBER 194822572 PREAUTHORIZED TRANSFER,($25),$3055.68");
            writer.WriteLine("03/28/2022,POS,,HLU* Hulu 145921230 03-27 HULU.COM/BILL CA 9341 DEBIT CARD RETURN,$+2.99,");
            writer.WriteLine("03/31/2022,Credit,,SEC PPD EDWARD JONES 2011 JAMESCLACK & ACH CREDIT,$+3600,");
            writer.WriteLine("01/11/2021,0, AMERICAN HOMES 4 WEB PMTS   HS5,1834.32,0,1856.58");
            writer.Close();
        }
        /// <summary>
        /// Hardcoded CSV content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void CreateCsv3(string filename, int variation)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(Configuration.Instance.DirectoryImportExport, filename));
            writer.WriteLine("01/01/2021,222,\"McDonalds\",($15.85)");
            writer.WriteLine("01/01/2021,,\"Pay Check\",$1234.50");
            writer.Close();
        }

        /// <summary>
        /// Hardcoded CSV content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void CreateCsv4(string filename, int variation)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(Configuration.Instance.DirectoryImportExport, filename));
            writer.WriteLine("Date,Category,Memo,Amount,WhoKnow, Crap");
            writer.WriteLine("01/01/2021,\"Mc\"Donalds\",\"Garbage\",($15.85),222,555");
            writer.WriteLine("01/01/2021,\"Pay Check\",\"Junk\",$1234.50",4312);
            writer.Close();
        }
    }
}