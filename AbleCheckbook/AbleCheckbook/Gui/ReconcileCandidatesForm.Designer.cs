namespace AbleCheckbook.Gui
{
    partial class ReconcileCandidatesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxCandidates = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxCandidates
            // 
            this.textBoxCandidates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCandidates.Location = new System.Drawing.Point(12, 33);
            this.textBoxCandidates.Multiline = true;
            this.textBoxCandidates.Name = "textBoxCandidates";
            this.textBoxCandidates.ReadOnly = true;
            this.textBoxCandidates.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCandidates.Size = new System.Drawing.Size(574, 140);
            this.textBoxCandidates.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(487, 182);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(101, 32);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(11, 8);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(120, 18);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Reconcile Tips";
            // 
            // ReconcileCandidatesForm
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 223);
            this.ControlBox = false;
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxCandidates);
            this.Name = "ReconcileCandidatesForm";
            this.Text = "ReconcileCandidatesForm";
            this.Load += new System.EventHandler(this.ReconcileCandidatesForm_Load);
            this.VisibleChanged += new System.EventHandler(this.ReconcileCandidatesForm_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCandidates;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelTitle;
    }
}