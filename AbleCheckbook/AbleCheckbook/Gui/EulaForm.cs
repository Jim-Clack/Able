using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class EulaForm : Form
    {
        public EulaForm()
        {
            InitializeComponent();
            this.Text = Strings.Get("EULA");
            buttonAccept.Text = Strings.Get("I accept these terms. (If you do not accept then close and uninstall this program)");
            string eulaPath = Path.Combine(Configuration.Instance.DirectorySupportFiles, "eula.txt");
            textBoxEula.Lines = File.ReadAllLines(eulaPath);
            textBoxEula.Select(0, 0);
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
