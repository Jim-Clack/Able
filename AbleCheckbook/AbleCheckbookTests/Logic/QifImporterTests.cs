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
    public class QifImporterTests
    {
        [TestMethod()]
        public void QifImporterTest()
        {
            createQif("import.qif", 1);
            string dbName = "UtEsTimpqif-" + DateTime.Now.Year + ".acb";
            File.Delete(Path.Combine(Configuration.Instance.DirectoryDatabase, dbName));
            JsonDbAccess db = new JsonDbAccess(dbName, null, true);
            QifImporter importer = new QifImporter(db);
            int lineNumber = importer.Import("import.qif");
            string errorMessage = importer.ErrorMessage;
            string warningMessages = importer.WarningMessages;
            db.SyncAndClose();
        }

        /// <summary>
        /// Hardcoded QIF content for testing.
        /// </summary>
        /// <param name="filename">Where to write it.</param>
        /// <param name="variation">Provided to create test variations in the file.</param>
        private void createQif(string filename, int variation)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(Configuration.Instance.DirectoryImportExport, filename));
            writer.WriteLine("!Type:Tag");
            writer.WriteLine("Nmanicure");
            writer.WriteLine("^ ");
            writer.WriteLine("Nnails");
            writer.WriteLine("^ ");
            writer.WriteLine("NReplaced");
            writer.WriteLine("^ ");
            writer.WriteLine("!Type:Cat");
            writer.WriteLine("NAdjustment");
            writer.WriteLine("I");
            writer.WriteLine("^ ");
            writer.WriteLine("NBonus");
            writer.WriteLine("DBonus Income");
            writer.WriteLine("T");
            writer.WriteLine("R460");
            writer.WriteLine("I");
            writer.WriteLine("^ ");
            writer.WriteLine("!Option:AutoSwitch");
            writer.WriteLine("!Account");
            writer.WriteLine("NSun Workplace");
            writer.WriteLine("TBank");
            writer.WriteLine("DChecking");
            writer.WriteLine("^ ");
            writer.WriteLine("!Clear:AutoSwitch");
            writer.WriteLine("!Account");
            writer.WriteLine("NSun Workplace");
            writer.WriteLine("DChecking");
            writer.WriteLine("TBank");
            writer.WriteLine("^ ");
            writer.WriteLine("!Type:Bank");
            writer.WriteLine("D12 / 14'19");
            writer.WriteLine("U647.45");
            writer.WriteLine("T647.45");
            writer.WriteLine("CX");
            writer.WriteLine("POpening Balance");
            writer.WriteLine("L[Sun Workplace]");
            writer.WriteLine("^ ");
            writer.WriteLine("D12 / 15'19");
            writer.WriteLine("U - 50.00");
            writer.WriteLine("T - 50.00");
            writer.WriteLine("N10016");
            writer.WriteLine("PMadi Arella");
            writer.WriteLine("LGifts Given");
            writer.WriteLine("^ ");
            writer.WriteLine("D1 / 2'20");
            writer.WriteLine("U - 56.02");
            writer.WriteLine("T - 56.02");
            writer.WriteLine("CX");
            writer.WriteLine("NEFT");
            writer.WriteLine("PWal Mart");
            writer.WriteLine("LGroceries");
            writer.WriteLine("SGroceries");
            writer.WriteLine("$-16.02");
            writer.WriteLine("SCash");
            writer.WriteLine("$-40.00");
            writer.WriteLine("^ ");
            writer.WriteLine("D1 / 1'20");
            writer.WriteLine("U3,000.00");
            writer.WriteLine("T3,000.00");
            writer.WriteLine("CX");
            writer.WriteLine("PEdward Jones");
            writer.WriteLine("LRetirement");
            writer.WriteLine("^ ");
            writer.WriteLine("D6 / 1 / 94");
            writer.WriteLine("T - 1,000.00");
            writer.WriteLine("N1005");
            writer.WriteLine("PBank Of Mortgage");
            writer.WriteLine("L[linda]");
            writer.WriteLine("S[linda]");
            writer.WriteLine("$-253.64");
            writer.WriteLine("SMort Int");
            writer.WriteLine("$-746.36");
            writer.WriteLine("^ ");
            writer.WriteLine("!Type:Memorized");
            writer.WriteLine("KP");
            writer.WriteLine("U - 9.62");
            writer.WriteLine("T - 9.62");
            writer.WriteLine("PAce");
            writer.WriteLine("LHome Improvements");
            writer.WriteLine("^ ");
            writer.WriteLine("KP");
            writer.WriteLine("U - 10.41");
            writer.WriteLine("T - 10.41");
            writer.WriteLine("PAce");
            writer.WriteLine("LHousehold");
            writer.WriteLine("^ ");
            writer.WriteLine("KD");
            writer.WriteLine("U8.58");
            writer.WriteLine("T8.58");
            writer.WriteLine("PWithlacoochee Electric");
            writer.WriteLine("LMisc");
            writer.WriteLine("^");
            writer.Close();
        }
    }
}