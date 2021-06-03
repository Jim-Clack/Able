namespace AbleCheckbook.Gui
{
    partial class StartingBalanceForm
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
            this.labelNotice = new System.Windows.Forms.Label();
            this.labelBalance = new System.Windows.Forms.Label();
            this.textBoxAmount = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.dateTimePickerOpeningDate = new System.Windows.Forms.DateTimePicker();
            this.labelAsOf = new System.Windows.Forms.Label();
            this.textBoxInstructions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelNotice
            // 
            this.labelNotice.AutoSize = true;
            this.labelNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotice.ForeColor = System.Drawing.Color.Red;
            this.labelNotice.Location = new System.Drawing.Point(10, 15);
            this.labelNotice.Name = "labelNotice";
            this.labelNotice.Size = new System.Drawing.Size(266, 17);
            this.labelNotice.TabIndex = 0;
            this.labelNotice.Text = "Starting Balance for New Account...";
            // 
            // labelBalance
            // 
            this.labelBalance.AutoSize = true;
            this.labelBalance.Location = new System.Drawing.Point(14, 95);
            this.labelBalance.Name = "labelBalance";
            this.labelBalance.Size = new System.Drawing.Size(154, 17);
            this.labelBalance.TabIndex = 1;
            this.labelBalance.Text = "Initial Account Balance:";
            // 
            // textBoxAmount
            // 
            this.textBoxAmount.Location = new System.Drawing.Point(184, 94);
            this.textBoxAmount.Name = "textBoxAmount";
            this.textBoxAmount.Size = new System.Drawing.Size(110, 22);
            this.textBoxAmount.TabIndex = 2;
            this.textBoxAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAmount_KeyPress);
            this.textBoxAmount.Leave += new System.EventHandler(this.textBoxAmount_Leave);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(184, 167);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(110, 32);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // dateTimePickerOpeningDate
            // 
            this.dateTimePickerOpeningDate.Location = new System.Drawing.Point(63, 130);
            this.dateTimePickerOpeningDate.Name = "dateTimePickerOpeningDate";
            this.dateTimePickerOpeningDate.Size = new System.Drawing.Size(231, 22);
            this.dateTimePickerOpeningDate.TabIndex = 4;
            // 
            // labelAsOf
            // 
            this.labelAsOf.AutoSize = true;
            this.labelAsOf.Location = new System.Drawing.Point(13, 131);
            this.labelAsOf.Name = "labelAsOf";
            this.labelAsOf.Size = new System.Drawing.Size(44, 17);
            this.labelAsOf.TabIndex = 5;
            this.labelAsOf.Text = "As of:";
            // 
            // textBoxInstructions
            // 
            this.textBoxInstructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInstructions.Location = new System.Drawing.Point(13, 46);
            this.textBoxInstructions.Multiline = true;
            this.textBoxInstructions.Name = "textBoxInstructions";
            this.textBoxInstructions.ReadOnly = true;
            this.textBoxInstructions.Size = new System.Drawing.Size(282, 41);
            this.textBoxInstructions.TabIndex = 6;
            this.textBoxInstructions.TabStop = false;
            this.textBoxInstructions.Text = "Normally this is the closing balance and closing date from your last bank stateme" +
    "nt.";
            this.textBoxInstructions.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StartingBalanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 215);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxInstructions);
            this.Controls.Add(this.labelAsOf);
            this.Controls.Add(this.dateTimePickerOpeningDate);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxAmount);
            this.Controls.Add(this.labelBalance);
            this.Controls.Add(this.labelNotice);
            this.MaximumSize = new System.Drawing.Size(332, 262);
            this.MinimumSize = new System.Drawing.Size(332, 262);
            this.Name = "StartingBalanceForm";
            this.Text = "New Account, Starting Balance";
            this.Load += new System.EventHandler(this.StartingBalanceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNotice;
        private System.Windows.Forms.Label labelBalance;
        private System.Windows.Forms.TextBox textBoxAmount;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.DateTimePicker dateTimePickerOpeningDate;
        private System.Windows.Forms.Label labelAsOf;
        private System.Windows.Forms.TextBox textBoxInstructions;
    }
}