using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbookTests.Db
{
    public static class StaticTestSupport
    {

        public static CheckbookEntry AddEntry(
            IDbAccess db, DateTime dateTran, string payee, bool isDebit,
            string name1, int amt1, string name2, int amt2, bool cleared)
        {
            CheckbookEntry ckbkEntry = null;
            ckbkEntry = new CheckbookEntry();
            TransactionKind kind = isDebit ? TransactionKind.Payment : TransactionKind.Deposit;
            amt1 = Math.Abs(amt1);
            amt2 = Math.Abs(amt2);
            if (isDebit)
            {
                amt1 = -amt1;
                amt2 = -amt2;
            }
            FinancialCategory cat1 = db.GetFinancialCategoryByName(name1);
            if (cat1 == null)
            {
                cat1 = new FinancialCategory();
            }
            else
            {
                db.DeleteEntry(cat1);
            }
            cat1.Name = name1;
            cat1.IsCredit = amt1 > 0;
            db.InsertEntry(cat1);
            ckbkEntry.AddSplit(cat1.Id, kind, amt1);
            if (amt2 != 0)
            {
                FinancialCategory cat2 = db.GetFinancialCategoryByName(name2);
                if (cat2 == null)
                {
                    cat2 = new FinancialCategory();
                }
                else
                {
                    db.DeleteEntry(cat2);
                }
                cat2.Name = name2;
                cat2.IsCredit = amt2 > 0;
                db.InsertEntry(cat2);
                ckbkEntry.AddSplit(cat2.Id, kind, amt2);
            }
            ckbkEntry.Payee = payee;
            ckbkEntry.IsCleared = cleared;
            ckbkEntry.DateOfTransaction = dateTran;
            db.InsertEntry(ckbkEntry);
            return ckbkEntry;
        }

        /// <summary>
        /// Use as first line in ad hoc tests (needed by XNA specifically)
        /// </summary>
        public static void SetEntryAssembly()
        {
            SetEntryAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Allows setting the Entry Assembly when needed. 
        /// Use AssemblyUtilities.SetEntryAssembly() as first line in XNA ad hoc tests
        /// </summary>
        /// <param name="assembly">Assembly to set as entry assembly</param>
        public static void SetEntryAssembly(Assembly assembly)
        {
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }

    }

}
