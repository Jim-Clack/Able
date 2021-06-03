using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AbleCheckbook.Gui.Tests
{
    [TestClass()]
    public class HelperMethodsTests
    {
        [TestMethod()]
        public void DrawPieChartTest()
        {
            // Not much we can test here other than making sure it doesn't crash.
            // This is here mainly to provide an example of how to use DrawPieChart().
            Form form = new Form();
            using (Graphics graphics = form.CreateGraphics())
            {
                UiHelperMethods.DrawPieChart(graphics, 10, 10, 250,
                    new long[] { 12345, 9255, 797, 2147, 8722, 7777, 5425, 4429 },
                    new string[] { "Housing", "Groceries", "Personal", "Transportation", "Utilities", "Medical", "Clothing" });
            }
            form.Dispose();
            Assert.IsNotNull(form);
        }

    }
}