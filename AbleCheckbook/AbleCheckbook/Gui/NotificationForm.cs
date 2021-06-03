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
    public partial class NotificationForm : Form
    {

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="IsError">True only for a serious error.</param>
        /// <param name="message">Brief description - will be looked up in Strings.Get()</param>
        /// <param name="details">Details - will be looked up in Strings.Get()</param>
        /// <param name="Cancelable">True to show the cancel button</param>
        public NotificationForm(bool IsError, string message, string details, bool Cancelable)
        {
            InitializeComponent();
            string title = Strings.Get(IsError ? "Error" : "Notice");
            labelMessage.Text = Strings.GetIff(message);
            textBoxDetails.Text = Strings.GetIff(details);
            buttonCancel.Text = Strings.Get("Cancel");
            buttonOk.Text = Strings.Get("OK");
            if (!Cancelable)
            {
                buttonCancel.Visible = false;
            }
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
