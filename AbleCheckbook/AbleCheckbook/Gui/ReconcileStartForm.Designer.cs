namespace AbleCheckbook.Gui
{
    partial class ReconcileStartForm
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
            this.groupBoxSource = new System.Windows.Forms.GroupBox();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.textBoxWebConnection = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxCsvFile = new System.Windows.Forms.TextBox();
            this.radioButtonCsv = new System.Windows.Forms.RadioButton();
            this.radioButtonWeb = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelPrompt = new System.Windows.Forms.Label();
            this.dateTimePickerPrevRecon = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerThisRecon = new System.Windows.Forms.DateTimePicker();
            this.labelPrevRecon = new System.Windows.Forms.Label();
            this.labelThisRecon = new System.Windows.Forms.Label();
            this.textBoxThisBalance = new System.Windows.Forms.TextBox();
            this.textBoxPrevBalance = new System.Windows.Forms.TextBox();
            this.groupBoxSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSource
            // 
            this.groupBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSource.Controls.Add(this.radioButtonManual);
            this.groupBoxSource.Controls.Add(this.textBoxWebConnection);
            this.groupBoxSource.Controls.Add(this.buttonBrowse);
            this.groupBoxSource.Controls.Add(this.textBoxCsvFile);
            this.groupBoxSource.Controls.Add(this.radioButtonCsv);
            this.groupBoxSource.Controls.Add(this.radioButtonWeb);
            this.groupBoxSource.Location = new System.Drawing.Point(13, 13);
            this.groupBoxSource.Name = "groupBoxSource";
            this.groupBoxSource.Size = new System.Drawing.Size(575, 121);
            this.groupBoxSource.TabIndex = 0;
            this.groupBoxSource.TabStop = false;
            this.groupBoxSource.Text = "Source";
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Checked = true;
            this.radioButtonManual.Location = new System.Drawing.Point(11, 24);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(381, 21);
            this.radioButtonManual.TabIndex = 1;
            this.radioButtonManual.TabStop = true;
            this.radioButtonManual.Text = "Manual - Clear one entry at a time per a bank statement";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // textBoxWebConnection
            // 
            this.textBoxWebConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWebConnection.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxWebConnection.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWebConnection.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBoxWebConnection.Location = new System.Drawing.Point(205, 54);
            this.textBoxWebConnection.Name = "textBoxWebConnection";
            this.textBoxWebConnection.Size = new System.Drawing.Size(351, 15);
            this.textBoxWebConnection.TabIndex = 4;
            this.textBoxWebConnection.Text = "(none)";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(477, 79);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(82, 26);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxCsvFile
            // 
            this.textBoxCsvFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCsvFile.Location = new System.Drawing.Point(104, 79);
            this.textBoxCsvFile.Name = "textBoxCsvFile";
            this.textBoxCsvFile.Size = new System.Drawing.Size(361, 22);
            this.textBoxCsvFile.TabIndex = 2;
            // 
            // radioButtonCsv
            // 
            this.radioButtonCsv.AutoSize = true;
            this.radioButtonCsv.Location = new System.Drawing.Point(11, 80);
            this.radioButtonCsv.Name = "radioButtonCsv";
            this.radioButtonCsv.Size = new System.Drawing.Size(86, 21);
            this.radioButtonCsv.TabIndex = 333;
            this.radioButtonCsv.Text = "CSV File:";
            this.radioButtonCsv.UseVisualStyleBackColor = true;
            // 
            // radioButtonWeb
            // 
            this.radioButtonWeb.AutoSize = true;
            this.radioButtonWeb.Location = new System.Drawing.Point(11, 52);
            this.radioButtonWeb.Name = "radioButtonWeb";
            this.radioButtonWeb.Size = new System.Drawing.Size(186, 21);
            this.radioButtonWeb.TabIndex = 2;
            this.radioButtonWeb.Text = "Web: Financial Institution";
            this.radioButtonWeb.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.buttonCancel.Location = new System.Drawing.Point(390, 204);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(89, 26);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(501, 204);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(86, 26);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // labelPrompt
            // 
            this.labelPrompt.AutoSize = true;
            this.labelPrompt.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.Red;
            this.labelPrompt.Location = new System.Drawing.Point(13, 142);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(469, 18);
            this.labelPrompt.TabIndex = 3;
            this.labelPrompt.Text = "Start/Prev Date is Inclusive, End/This date is Exclusive (after)";
            // 
            // dateTimePickerPrevRecon
            // 
            this.dateTimePickerPrevRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimePickerPrevRecon.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerPrevRecon.Location = new System.Drawing.Point(14, 170);
            this.dateTimePickerPrevRecon.Name = "dateTimePickerPrevRecon";
            this.dateTimePickerPrevRecon.ShowUpDown = true;
            this.dateTimePickerPrevRecon.Size = new System.Drawing.Size(125, 22);
            this.dateTimePickerPrevRecon.TabIndex = 344;
            // 
            // dateTimePickerThisRecon
            // 
            this.dateTimePickerThisRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimePickerThisRecon.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerThisRecon.Location = new System.Drawing.Point(14, 204);
            this.dateTimePickerThisRecon.Name = "dateTimePickerThisRecon";
            this.dateTimePickerThisRecon.ShowUpDown = true;
            this.dateTimePickerThisRecon.Size = new System.Drawing.Size(125, 22);
            this.dateTimePickerThisRecon.TabIndex = 355;
            // 
            // labelPrevRecon
            // 
            this.labelPrevRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelPrevRecon.AutoSize = true;
            this.labelPrevRecon.Location = new System.Drawing.Point(143, 173);
            this.labelPrevRecon.Name = "labelPrevRecon";
            this.labelPrevRecon.Size = new System.Drawing.Size(103, 17);
            this.labelPrevRecon.TabIndex = 9;
            this.labelPrevRecon.Text = "Prev Reconcile";
            // 
            // labelThisRecon
            // 
            this.labelThisRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelThisRecon.AutoSize = true;
            this.labelThisRecon.Location = new System.Drawing.Point(143, 207);
            this.labelThisRecon.Name = "labelThisRecon";
            this.labelThisRecon.Size = new System.Drawing.Size(101, 17);
            this.labelThisRecon.TabIndex = 10;
            this.labelThisRecon.Text = "This Reconcile";
            // 
            // textBoxThisBalance
            // 
            this.textBoxThisBalance.AcceptsReturn = true;
            this.textBoxThisBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxThisBalance.Location = new System.Drawing.Point(250, 204);
            this.textBoxThisBalance.Name = "textBoxThisBalance";
            this.textBoxThisBalance.Size = new System.Drawing.Size(90, 22);
            this.textBoxThisBalance.TabIndex = 4;
            this.textBoxThisBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPrevBalance
            // 
            this.textBoxPrevBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxPrevBalance.Location = new System.Drawing.Point(250, 170);
            this.textBoxPrevBalance.Name = "textBoxPrevBalance";
            this.textBoxPrevBalance.Size = new System.Drawing.Size(90, 22);
            this.textBoxPrevBalance.TabIndex = 366;
            this.textBoxPrevBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ReconcileStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 245);
            this.Controls.Add(this.textBoxThisBalance);
            this.Controls.Add(this.textBoxPrevBalance);
            this.Controls.Add(this.labelThisRecon);
            this.Controls.Add(this.labelPrevRecon);
            this.Controls.Add(this.dateTimePickerThisRecon);
            this.Controls.Add(this.dateTimePickerPrevRecon);
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBoxSource);
            this.MaximumSize = new System.Drawing.Size(622, 292);
            this.MinimumSize = new System.Drawing.Size(622, 292);
            this.Name = "ReconcileStartForm";
            this.Text = "Start Reconciliation";
            this.Load += new System.EventHandler(this.ReconcileSourceForm_Load);
            this.groupBoxSource.ResumeLayout(false);
            this.groupBoxSource.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSource;
        private System.Windows.Forms.TextBox textBoxWebConnection;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxCsvFile;
        private System.Windows.Forms.RadioButton radioButtonCsv;
        private System.Windows.Forms.RadioButton radioButtonWeb;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.DateTimePicker dateTimePickerPrevRecon;
        private System.Windows.Forms.DateTimePicker dateTimePickerThisRecon;
        private System.Windows.Forms.Label labelPrevRecon;
        private System.Windows.Forms.Label labelThisRecon;
        private System.Windows.Forms.TextBox textBoxThisBalance;
        private System.Windows.Forms.TextBox textBoxPrevBalance;
    }
}