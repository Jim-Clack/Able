namespace AbleCheckbook.Gui
{
    partial class CheckbookEntryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckbookEntryForm));
            this.datePickerTransaction = new System.Windows.Forms.DateTimePicker();
            this.labelTransDate = new System.Windows.Forms.Label();
            this.textBoxCheckNbr = new System.Windows.Forms.TextBox();
            this.labelCheckNumber = new System.Windows.Forms.Label();
            this.labelPayee = new System.Windows.Forms.Label();
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.labelMemoOverlay = new System.Windows.Forms.Label();
            this.checkBoxCleared = new System.Windows.Forms.CheckBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxTotalAmt = new System.Windows.Forms.TextBox();
            this.labelTotal = new System.Windows.Forms.Label();
            this.buttonBar = new System.Windows.Forms.Button();
            this.textBoxAssistance = new System.Windows.Forms.TextBox();
            this.labelPointer = new System.Windows.Forms.Label();
            this.comboBoxPayee = new System.Windows.Forms.ComboBox();
            this.textBoxScaling = new System.Windows.Forms.TextBox();
            this.textBoxBankInfo = new System.Windows.Forms.TextBox();
            this.buttonUnMerge = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // datePickerTransaction
            // 
            this.datePickerTransaction.AllowDrop = true;
            this.datePickerTransaction.Location = new System.Drawing.Point(153, 15);
            this.datePickerTransaction.MaxDate = new System.DateTime(2199, 12, 31, 0, 0, 0, 0);
            this.datePickerTransaction.MinDate = new System.DateTime(1950, 1, 1, 0, 0, 0, 0);
            this.datePickerTransaction.Name = "datePickerTransaction";
            this.datePickerTransaction.ShowUpDown = true;
            this.datePickerTransaction.Size = new System.Drawing.Size(231, 22);
            this.datePickerTransaction.TabIndex = 0;
            this.datePickerTransaction.ValueChanged += new System.EventHandler(this.datePickerTransaction_ValueChanged);
            // 
            // labelTransDate
            // 
            this.labelTransDate.AutoSize = true;
            this.labelTransDate.ForeColor = System.Drawing.Color.Black;
            this.labelTransDate.Location = new System.Drawing.Point(24, 17);
            this.labelTransDate.Name = "labelTransDate";
            this.labelTransDate.Size = new System.Drawing.Size(117, 17);
            this.labelTransDate.TabIndex = 1;
            this.labelTransDate.Text = "Transaction Date";
            // 
            // textBoxCheckNbr
            // 
            this.textBoxCheckNbr.AllowDrop = true;
            this.textBoxCheckNbr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCheckNbr.Location = new System.Drawing.Point(476, 15);
            this.textBoxCheckNbr.Name = "textBoxCheckNbr";
            this.textBoxCheckNbr.Size = new System.Drawing.Size(85, 22);
            this.textBoxCheckNbr.TabIndex = 1;
            this.textBoxCheckNbr.Enter += new System.EventHandler(this.textBoxCheckNbr_Enter);
            // 
            // labelCheckNumber
            // 
            this.labelCheckNumber.AutoSize = true;
            this.labelCheckNumber.ForeColor = System.Drawing.Color.Black;
            this.labelCheckNumber.Location = new System.Drawing.Point(410, 17);
            this.labelCheckNumber.Name = "labelCheckNumber";
            this.labelCheckNumber.Size = new System.Drawing.Size(55, 17);
            this.labelCheckNumber.TabIndex = 3;
            this.labelCheckNumber.Text = "Check#";
            // 
            // labelPayee
            // 
            this.labelPayee.AutoSize = true;
            this.labelPayee.ForeColor = System.Drawing.Color.Black;
            this.labelPayee.Location = new System.Drawing.Point(93, 49);
            this.labelPayee.Name = "labelPayee";
            this.labelPayee.Size = new System.Drawing.Size(48, 17);
            this.labelPayee.TabIndex = 6;
            this.labelPayee.Text = "Payee";
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.AllowDrop = true;
            this.textBoxMemo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMemo.Location = new System.Drawing.Point(12, 155);
            this.textBoxMemo.MaxLength = 600;
            this.textBoxMemo.Multiline = true;
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxMemo.Size = new System.Drawing.Size(374, 62);
            this.textBoxMemo.TabIndex = 11;
            this.textBoxMemo.TextChanged += new System.EventHandler(this.textBoxMemo_TextChanged);
            // 
            // labelMemoOverlay
            // 
            this.labelMemoOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMemoOverlay.AutoSize = true;
            this.labelMemoOverlay.BackColor = System.Drawing.SystemColors.Window;
            this.labelMemoOverlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelMemoOverlay.ForeColor = System.Drawing.Color.Silver;
            this.labelMemoOverlay.Location = new System.Drawing.Point(174, 174);
            this.labelMemoOverlay.Name = "labelMemoOverlay";
            this.labelMemoOverlay.Size = new System.Drawing.Size(46, 17);
            this.labelMemoOverlay.TabIndex = 14;
            this.labelMemoOverlay.Text = "Memo";
            // 
            // checkBoxCleared
            // 
            this.checkBoxCleared.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxCleared.AutoSize = true;
            this.checkBoxCleared.ForeColor = System.Drawing.Color.Black;
            this.checkBoxCleared.Location = new System.Drawing.Point(409, 201);
            this.checkBoxCleared.Name = "checkBoxCleared";
            this.checkBoxCleared.Size = new System.Drawing.Size(161, 21);
            this.checkBoxCleared.TabIndex = 0;
            this.checkBoxCleared.TabStop = false;
            this.checkBoxCleared.Text = "Cleared / Reconciled";
            this.checkBoxCleared.UseVisualStyleBackColor = true;
            this.checkBoxCleared.CheckedChanged += new System.EventHandler(this.checkBoxCleared_CheckedChanged);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.Location = new System.Drawing.Point(12, 281);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(125, 32);
            this.buttonDelete.TabIndex = 13;
            this.buttonDelete.TabStop = false;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(293, 281);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(124, 32);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(438, 281);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(124, 32);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxTotalAmt
            // 
            this.textBoxTotalAmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTotalAmt.Location = new System.Drawing.Point(458, 170);
            this.textBoxTotalAmt.Name = "textBoxTotalAmt";
            this.textBoxTotalAmt.ReadOnly = true;
            this.textBoxTotalAmt.Size = new System.Drawing.Size(103, 22);
            this.textBoxTotalAmt.TabIndex = 20;
            this.textBoxTotalAmt.TabStop = false;
            this.textBoxTotalAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTotal
            // 
            this.labelTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotal.AutoSize = true;
            this.labelTotal.ForeColor = System.Drawing.Color.Black;
            this.labelTotal.Location = new System.Drawing.Point(408, 171);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(40, 17);
            this.labelTotal.TabIndex = 21;
            this.labelTotal.Text = "Total";
            // 
            // buttonBar
            // 
            this.buttonBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBar.Enabled = false;
            this.buttonBar.Location = new System.Drawing.Point(458, 154);
            this.buttonBar.Name = "buttonBar";
            this.buttonBar.Size = new System.Drawing.Size(103, 4);
            this.buttonBar.TabIndex = 22;
            this.buttonBar.TabStop = false;
            this.buttonBar.Text = "button1";
            this.buttonBar.UseVisualStyleBackColor = true;
            // 
            // textBoxAssistance
            // 
            this.textBoxAssistance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAssistance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxAssistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAssistance.ForeColor = System.Drawing.Color.Red;
            this.textBoxAssistance.Location = new System.Drawing.Point(11, 259);
            this.textBoxAssistance.Multiline = true;
            this.textBoxAssistance.Name = "textBoxAssistance";
            this.textBoxAssistance.ReadOnly = true;
            this.textBoxAssistance.Size = new System.Drawing.Size(559, 21);
            this.textBoxAssistance.TabIndex = 23;
            this.textBoxAssistance.TabStop = false;
            this.textBoxAssistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelPointer
            // 
            this.labelPointer.AutoSize = true;
            this.labelPointer.Location = new System.Drawing.Point(-1, 8);
            this.labelPointer.Name = "labelPointer";
            this.labelPointer.Size = new System.Drawing.Size(18, 17);
            this.labelPointer.TabIndex = 24;
            this.labelPointer.Text = "◀";
            // 
            // comboBoxPayee
            // 
            this.comboBoxPayee.AllowDrop = true;
            this.comboBoxPayee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPayee.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxPayee.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPayee.FormattingEnabled = true;
            this.comboBoxPayee.Location = new System.Drawing.Point(153, 47);
            this.comboBoxPayee.Name = "comboBoxPayee";
            this.comboBoxPayee.Size = new System.Drawing.Size(408, 24);
            this.comboBoxPayee.TabIndex = 2;
            this.comboBoxPayee.Leave += new System.EventHandler(this.comboBoxPayee_Leave);
            // 
            // textBoxScaling
            // 
            this.textBoxScaling.Enabled = false;
            this.textBoxScaling.Location = new System.Drawing.Point(12, 83);
            this.textBoxScaling.Multiline = true;
            this.textBoxScaling.Name = "textBoxScaling";
            this.textBoxScaling.Size = new System.Drawing.Size(500, 50);
            this.textBoxScaling.TabIndex = 25;
            this.textBoxScaling.Text = "   \r\n500x50 @ (12, 83) - DON\'T MODIFY THIS - IT\'S USED FOR CALCULATING SCALE";
            this.textBoxScaling.Visible = false;
            // 
            // textBoxBankInfo
            // 
            this.textBoxBankInfo.AllowDrop = true;
            this.textBoxBankInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBankInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxBankInfo.Location = new System.Drawing.Point(12, 234);
            this.textBoxBankInfo.Name = "textBoxBankInfo";
            this.textBoxBankInfo.ReadOnly = true;
            this.textBoxBankInfo.Size = new System.Drawing.Size(411, 15);
            this.textBoxBankInfo.TabIndex = 26;
            this.textBoxBankInfo.Visible = false;
            // 
            // buttonUnMerge
            // 
            this.buttonUnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUnMerge.Enabled = false;
            this.buttonUnMerge.Location = new System.Drawing.Point(438, 227);
            this.buttonUnMerge.Name = "buttonUnMerge";
            this.buttonUnMerge.Size = new System.Drawing.Size(124, 32);
            this.buttonUnMerge.TabIndex = 27;
            this.buttonUnMerge.Text = "◀ No: Un-Merge";
            this.buttonUnMerge.UseVisualStyleBackColor = true;
            this.buttonUnMerge.Visible = false;
            this.buttonUnMerge.Click += new System.EventHandler(this.buttonUnMerge_Click);
            // 
            // CheckbookEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(583, 326);
            this.Controls.Add(this.checkBoxCleared);
            this.Controls.Add(this.buttonUnMerge);
            this.Controls.Add(this.textBoxBankInfo);
            this.Controls.Add(this.textBoxScaling);
            this.Controls.Add(this.labelMemoOverlay);
            this.Controls.Add(this.comboBoxPayee);
            this.Controls.Add(this.textBoxTotalAmt);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.textBoxCheckNbr);
            this.Controls.Add(this.datePickerTransaction);
            this.Controls.Add(this.labelPointer);
            this.Controls.Add(this.buttonBar);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelPayee);
            this.Controls.Add(this.labelCheckNumber);
            this.Controls.Add(this.labelTransDate);
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.textBoxAssistance);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(640, 510);
            this.MinimumSize = new System.Drawing.Size(599, 353);
            this.Name = "CheckbookEntryForm";
            this.Text = "Checkbook Entry";
            this.Load += new System.EventHandler(this.CheckbookEntryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker datePickerTransaction;
        private System.Windows.Forms.Label labelTransDate;
        private System.Windows.Forms.TextBox textBoxCheckNbr;
        private System.Windows.Forms.Label labelCheckNumber;
        private System.Windows.Forms.Label labelPayee;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.Label labelMemoOverlay;
        private System.Windows.Forms.CheckBox checkBoxCleared;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxTotalAmt;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Button buttonBar;
        private System.Windows.Forms.TextBox textBoxAssistance;
        private System.Windows.Forms.Label labelPointer;
        private System.Windows.Forms.ComboBox comboBoxPayee;
        private System.Windows.Forms.TextBox textBoxScaling;
        private System.Windows.Forms.TextBox textBoxBankInfo;
        private System.Windows.Forms.Button buttonUnMerge;
    }
}