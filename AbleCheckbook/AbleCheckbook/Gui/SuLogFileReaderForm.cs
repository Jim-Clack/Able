using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class SuLogFileReaderForm : Form
    {

        /// <summary>
        /// Support methods.
        /// </summary>
        private SuLogFileReader _LogFileReader = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        public SuLogFileReaderForm()
        {
            if (Configuration.Instance.GetUserLevel() != UserLevel.SuperUser)
            {
                throw new Exception("Failed SU Attempt");
            }
            InitializeComponent();
        }

        private void LogFileReaderForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Log Reader");
            buttonOpenLogFile.Text = Strings.Get("Open Log File");
            buttonSearchForward.Text = Strings.Get("Search Forward");
            buttonSearchBackward.Text = Strings.Get("Search Backward");
            buttonClose.Text = Strings.Get("Close");
            _LogFileReader = new SuLogFileReader();
        }

    }

}
