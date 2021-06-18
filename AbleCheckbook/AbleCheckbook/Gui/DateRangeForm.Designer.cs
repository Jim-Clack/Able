namespace AbleCheckbook.Gui
{
    partial class DateRangeForm
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
            this.dateFirst = new System.Windows.Forms.DateTimePicker();
            this.dateLast = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkDetailed = new System.Windows.Forms.CheckBox();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSelectPrinter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateFirst
            // 
            this.dateFirst.Location = new System.Drawing.Point(126, 18);
            this.dateFirst.Name = "dateFirst";
            this.dateFirst.Size = new System.Drawing.Size(259, 22);
            this.dateFirst.TabIndex = 0;
            this.dateFirst.Leave += new System.EventHandler(this.dateFirst_Leave);
            // 
            // dateLast
            // 
            this.dateLast.Location = new System.Drawing.Point(126, 52);
            this.dateLast.Name = "dateLast";
            this.dateLast.Size = new System.Drawing.Size(259, 22);
            this.dateLast.TabIndex = 1;
            this.dateLast.Leave += new System.EventHandler(this.dateLast_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start/First Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "End/Last Date";
            // 
            // checkDetailed
            // 
            this.checkDetailed.AutoSize = true;
            this.checkDetailed.Location = new System.Drawing.Point(24, 87);
            this.checkDetailed.Name = "checkDetailed";
            this.checkDetailed.Size = new System.Drawing.Size(307, 21);
            this.checkDetailed.TabIndex = 4;
            this.checkDetailed.Text = "Detailed Report: With Itemized Transactions";
            this.checkDetailed.UseVisualStyleBackColor = true;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(265, 171);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(120, 32);
            this.buttonPrint.TabIndex = 5;
            this.buttonPrint.Text = "Go / Print";
            this.buttonPrint.UseCompatibleTextRendering = true;
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(88, 171);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 32);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSelectPrinter
            // 
            this.buttonSelectPrinter.Location = new System.Drawing.Point(172, 124);
            this.buttonSelectPrinter.Name = "buttonSelectPrinter";
            this.buttonSelectPrinter.Size = new System.Drawing.Size(213, 32);
            this.buttonSelectPrinter.TabIndex = 7;
            this.buttonSelectPrinter.Text = "Select Printer and Settings";
            this.buttonSelectPrinter.UseVisualStyleBackColor = true;
            this.buttonSelectPrinter.Click += new System.EventHandler(this.buttonSelectPrinter_Click);
            // 
            // DateRangeForm
            // 
            this.AcceptButton = this.buttonPrint;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(406, 223);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.checkDetailed);
            this.Controls.Add(this.dateLast);
            this.Controls.Add(this.dateFirst);
            this.Controls.Add(this.buttonSelectPrinter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(424, 270);
            this.MinimumSize = new System.Drawing.Size(424, 270);
            this.Name = "DateRangeForm";
            this.Text = "Date Range";
            this.Load += new System.EventHandler(this.DateRangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateFirst;
        private System.Windows.Forms.DateTimePicker dateLast;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkDetailed;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSelectPrinter;
    }
}