namespace AbleCheckbook.Gui
{
    partial class NewDbForm
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
            this.comboBoxAcctNames = new System.Windows.Forms.ComboBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelErrorExists = new System.Windows.Forms.Label();
            this.labelErrorIllegal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxAcctNames
            // 
            this.comboBoxAcctNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAcctNames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxAcctNames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxAcctNames.FormattingEnabled = true;
            this.comboBoxAcctNames.Location = new System.Drawing.Point(97, 22);
            this.comboBoxAcctNames.Name = "comboBoxAcctNames";
            this.comboBoxAcctNames.Size = new System.Drawing.Size(143, 24);
            this.comboBoxAcctNames.TabIndex = 0;
            this.comboBoxAcctNames.SelectedIndexChanged += new System.EventHandler(this.comboBoxAcctNames_SelectedIndexChanged);
            this.comboBoxAcctNames.Enter += new System.EventHandler(this.comboBoxAcctNames_Enter);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(20, 80);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(97, 32);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(143, 80);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(97, 32);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Acct";
            // 
            // labelErrorExists
            // 
            this.labelErrorExists.AutoSize = true;
            this.labelErrorExists.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelErrorExists.ForeColor = System.Drawing.Color.Red;
            this.labelErrorExists.Location = new System.Drawing.Point(13, 53);
            this.labelErrorExists.Name = "labelErrorExists";
            this.labelErrorExists.Size = new System.Drawing.Size(196, 17);
            this.labelErrorExists.TabIndex = 4;
            this.labelErrorExists.Text = "Sorry - File Already Exists";
            this.labelErrorExists.Visible = false;
            // 
            // labelErrorIllegal
            // 
            this.labelErrorIllegal.AutoSize = true;
            this.labelErrorIllegal.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelErrorIllegal.ForeColor = System.Drawing.Color.Red;
            this.labelErrorIllegal.Location = new System.Drawing.Point(15, 53);
            this.labelErrorIllegal.Name = "labelErrorIllegal";
            this.labelErrorIllegal.Size = new System.Drawing.Size(215, 17);
            this.labelErrorIllegal.TabIndex = 5;
            this.labelErrorIllegal.Text = "Sorry - Illegal Account Name";
            this.labelErrorIllegal.Visible = false;
            // 
            // NewDbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(261, 131);
            this.Controls.Add(this.labelErrorIllegal);
            this.Controls.Add(this.labelErrorExists);
            this.Controls.Add(this.comboBoxAcctNames);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.Name = "NewDbForm";
            this.Text = "NewDbForm";
            this.Load += new System.EventHandler(this.NewDbForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxAcctNames;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelErrorExists;
        private System.Windows.Forms.Label labelErrorIllegal;
    }
}