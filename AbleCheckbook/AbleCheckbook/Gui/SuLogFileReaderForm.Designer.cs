namespace AbleCheckbook.Gui
{
    partial class SuLogFileReaderForm
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
            this.buttonOpenLogFile = new System.Windows.Forms.Button();
            this.textBoxSearchPattern = new System.Windows.Forms.TextBox();
            this.buttonSearchForward = new System.Windows.Forms.Button();
            this.buttonSearchBackward = new System.Windows.Forms.Button();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.richTextBoxViewer = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonOpenLogFile
            // 
            this.buttonOpenLogFile.Location = new System.Drawing.Point(13, 13);
            this.buttonOpenLogFile.Name = "buttonOpenLogFile";
            this.buttonOpenLogFile.Size = new System.Drawing.Size(132, 32);
            this.buttonOpenLogFile.TabIndex = 0;
            this.buttonOpenLogFile.Text = "Open Log File";
            this.buttonOpenLogFile.UseVisualStyleBackColor = true;
            // 
            // textBoxSearchPattern
            // 
            this.textBoxSearchPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchPattern.Location = new System.Drawing.Point(170, 18);
            this.textBoxSearchPattern.Name = "textBoxSearchPattern";
            this.textBoxSearchPattern.Size = new System.Drawing.Size(124, 22);
            this.textBoxSearchPattern.TabIndex = 1;
            // 
            // buttonSearchForward
            // 
            this.buttonSearchForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchForward.Location = new System.Drawing.Point(311, 13);
            this.buttonSearchForward.Name = "buttonSearchForward";
            this.buttonSearchForward.Size = new System.Drawing.Size(132, 32);
            this.buttonSearchForward.TabIndex = 2;
            this.buttonSearchForward.Text = "Search Forward";
            this.buttonSearchForward.UseVisualStyleBackColor = true;
            // 
            // buttonSearchBackward
            // 
            this.buttonSearchBackward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchBackward.Location = new System.Drawing.Point(458, 13);
            this.buttonSearchBackward.Name = "buttonSearchBackward";
            this.buttonSearchBackward.Size = new System.Drawing.Size(132, 32);
            this.buttonSearchBackward.TabIndex = 3;
            this.buttonSearchBackward.Text = "Search Backward";
            this.buttonSearchBackward.UseVisualStyleBackColor = true;
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInfo.Location = new System.Drawing.Point(15, 379);
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.ReadOnly = true;
            this.textBoxInfo.Size = new System.Drawing.Size(428, 22);
            this.textBoxInfo.TabIndex = 5;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(458, 373);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(132, 32);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // richTextBoxViewer
            // 
            this.richTextBoxViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxViewer.Location = new System.Drawing.Point(15, 59);
            this.richTextBoxViewer.Name = "richTextBoxViewer";
            this.richTextBoxViewer.Size = new System.Drawing.Size(575, 300);
            this.richTextBoxViewer.TabIndex = 4;
            this.richTextBoxViewer.Text = "Not Yet Implemented\n{\\pict\\pngblip\\picw200\\pich100\\picwgoal200\\pichgoal100 hex FF" +
    "E5778800...}";
            // 
            // SuLogFileReaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 419);
            this.Controls.Add(this.richTextBoxViewer);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxInfo);
            this.Controls.Add(this.buttonSearchBackward);
            this.Controls.Add(this.buttonSearchForward);
            this.Controls.Add(this.textBoxSearchPattern);
            this.Controls.Add(this.buttonOpenLogFile);
            this.MinimumSize = new System.Drawing.Size(622, 260);
            this.Name = "SuLogFileReaderForm";
            this.Text = "Log File Reader";
            this.Load += new System.EventHandler(this.LogFileReaderForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonOpenLogFile;
        private System.Windows.Forms.TextBox textBoxSearchPattern;
        private System.Windows.Forms.Button buttonSearchForward;
        private System.Windows.Forms.Button buttonSearchBackward;
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.RichTextBox richTextBoxViewer;
    }
}