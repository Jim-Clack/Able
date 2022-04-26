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
    public partial class ReconcileCandidatesForm : Form
    {

        /// <summary>
        /// Ctor.
        /// </summary>
        public ReconcileCandidatesForm()
        {
            InitializeComponent();
        }

        public void ToFront()
        {
            Show();
            textBoxCandidates.DeselectAll();
            BringToFront();
        }

        /// <summary>
        /// Are there any tips to show?
        /// </summary>
        /// <returns></returns>
        public bool TipsAvailable()
        {
            return textBoxCandidates.Text.Trim().Length > 0;
        }

        /// <summary>
        /// Clear the listing.
        /// </summary>
        public void Clear()
        {
            textBoxCandidates.Lines = new string[0];
        }

        /// <summary>
        /// Append a line to the content.
        /// </summary>
        /// <param name="line">to be appended as a new line</param>
        private void AppendLine(string line)
        {
            List<string> list = textBoxCandidates.Lines.ToList();
            list.Add(line);
            textBoxCandidates.Lines = list.ToArray();
        }

        /// <summary>
        /// Append a reconcile resolution candidate. (Tips)
        /// </summary>
        /// <param name="candidate"></param>
        public void AppendCandidate(CandidateEntry candidate)
        {
            switch(candidate.Issue)
            {
                case CandidateIssue.NoIssue:
                    return;
                case CandidateIssue.SumsToDifference:
                    AppendLine(Strings.Get("Entries Sum Up to Disparity:"));
                    break;
                case CandidateIssue.TransposedCents:
                    AppendLine(Strings.Get("Transposed Fractional Digits:"));
                    break;
                case CandidateIssue.TransposedDollars:
                    AppendLine(Strings.Get("Transposed Monetary Digits:"));
                    break;
                case CandidateIssue.WrongSignOnAmount:
                    AppendLine(Strings.Get("Sign (Payment/Deposit) Wrong:"));
                    break;
            }
            foreach(OpenEntry openEntry in candidate.OpenEntries)
            {
                AppendLine("  " + openEntry.CheckbookEntry.ToShortString() + (openEntry.CheckbookEntry.IsChecked ? " [X]" : ""));
            }
        }

        /// <summary>
        /// Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReconcileCandidatesForm_Load(object sender, EventArgs e)
        {
            if(this.Parent != null)
            {
                this.Left = this.Parent.PointToScreen(this.Parent.Location).X;
                this.Width = this.Parent.Width - 10;
            }
            this.Text = labelTitle.Text = Strings.Get("Reconcile Tips");
            buttonClose.Text = Strings.Get("Close");
            textBoxCandidates.DeselectAll();
            BringToFront();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ReconcileCandidatesForm_VisibleChanged(object sender, EventArgs e)
        {
            textBoxCandidates.DeselectAll();
            BringToFront();
        }
    }
}
