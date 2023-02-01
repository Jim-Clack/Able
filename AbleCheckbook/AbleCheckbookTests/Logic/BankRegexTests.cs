using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class BankRegexTests
    {
        [TestMethod()]
        public void ProcessRequestHashCodeTest()
        {
            BankConfiguration config = new BankConfiguration();
            BankRegex bankRegex = new BankRegex(config,
                "userid", "password", "12345", "77777777", DateTime.Now.AddDays(-30), DateTime.Now);
            string result = bankRegex.ProcessRequest("Test\nRout: \\#r   Acct: \\#a\nUser: \\#u   Pass: \\#p\nBackslash: \\");
            Assert.AreEqual("Test\nRout: 12345   Acct: 77777777\nUser: userid   Pass: password\nBackslash: \\", result);
        }

        [TestMethod()]
        public void ProcessRequestBackslashTest()
        {
            BankConfiguration config = new BankConfiguration();
            BankRegex bankRegex = new BankRegex(config,
                "userid", "password", "12345", "77777777", DateTime.Now.AddDays(-30), DateTime.Now);
            string result = bankRegex.ProcessRequest("\\\\r\\n\\t\\\\ \\#r\\#n\\#t\\#q\\");
            Assert.AreEqual("\\\x0d\x0a\x09\\ \x0d\x0a\x09\x22\\", result);
        }

        [TestMethod()]
        public void ProcessResponseBasicTest()
        {
            BankConfiguration config = new BankConfiguration();
            BankRegex bankRegex = new BankRegex(config, 
                "userid", "password", "12345", "77777777", DateTime.Now.AddDays(-30), DateTime.Now);
            string result = bankRegex.ProcessResponse("ABC\\#c5KLM", "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUV");
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUV", result);
            Assert.AreEqual("", bankRegex.ErrorMessage);
            Assert.AreEqual("DEFGHIJ", bankRegex.GetCapture(5));
        }

        [TestMethod()]
        public void ProcessResponseBase64Test()
        {
            BankConfiguration config = new BankConfiguration();
            BankRegex bankRegex = new BankRegex(config,
                "userid", "password", "12345", "77777777", DateTime.Now.AddDays(-30), DateTime.Now);
            string result = bankRegex.ProcessResponse("File=\\#q\\#c4\\#q Content=\\#q\\#6\\#q", "Start\nFile=\"zombo.csv\" Content=\"RGF0ZSxBbXQsUGF5ZWUsSW5mbyxUcmFuLEJhbAoxMi8wMS8yMDIxLC01NS41MCxLcm9nZXIsR3JvYywxMjM0NTY3OCwxNDQuNTAKMTIvMDEvMjAyMSwxMC4wMCxDYXNoLERlcCwsMTU0LjUwCjEyLzI1LjIwMjEsLTExLjExLE1vYmlsLFRyYW5zLDIyMjIyMjIsMTMzLjM5\"\nEnd\n");
            Assert.AreEqual("Start\nFile=\"zombo.csv\" Content=\"Date,Amt,Payee,Info,Tran,Bal\n12/01/2021,-55.50,Kroger,Groc,12345678,144.50\n12/01/2021,10.00,Cash,Dep,,154.50\n12/25.2021,-11.11,Mobil,Trans,2222222,133.39\"\nEnd\n", result);
            Assert.AreEqual("zombo.csv", bankRegex.GetCapture(4));
        }

        [TestMethod()]
        public void ProcessResponseAndRequestTest()
        {
            BankConfiguration config = new BankConfiguration();
            BankRegex bankRegex = new BankRegex(config,
                "userid", "password", "12345", "77777777", DateTime.Now.AddDays(-30), DateTime.Now);
            string result = bankRegex.ProcessResponse("ABC\\#c5KLMNOPQRSTUVWX\\#c6CDE", "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFG");
            result = bankRegex.ProcessRequest("Test\r\n\\tCapture5=<\\#i5>\r\n\\#tCapture6=<\\#i6>\r\n");
            Assert.AreEqual("Test\r\n\tCapture5=DEFGHIJ\r\n\tCapture6=YZAB\r\n", result);
        }

    }

}