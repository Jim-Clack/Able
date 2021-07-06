namespace AbleCheckbook.Gui
{
    partial class AccountSettingsForm
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
            this.checkBoxLiveSync = new System.Windows.Forms.CheckBox();
            this.checkBoxAggressive = new System.Windows.Forms.CheckBox();
            this.labelBank = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.labelPwd = new System.Windows.Forms.Label();
            this.labelAccount = new System.Windows.Forms.Label();
            this.buttonTest = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxBank = new System.Windows.Forms.ComboBox();
            this.comboBoxAcct = new System.Windows.Forms.ComboBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBoxLiveSync
            // 
            this.checkBoxLiveSync.AutoSize = true;
            this.checkBoxLiveSync.Location = new System.Drawing.Point(13, 12);
            this.checkBoxLiveSync.Name = "checkBoxLiveSync";
            this.checkBoxLiveSync.Size = new System.Drawing.Size(448, 21);
            this.checkBoxLiveSync.TabIndex = 0;
            this.checkBoxLiveSync.Text = "Live sync with bank online (instead of only for statement reconcile)";
            this.checkBoxLiveSync.UseVisualStyleBackColor = true;
            // 
            // checkBoxAggressive
            // 
            this.checkBoxAggressive.AutoSize = true;
            this.checkBoxAggressive.Location = new System.Drawing.Point(13, 41);
            this.checkBoxAggressive.Name = "checkBoxAggressive";
            this.checkBoxAggressive.Size = new System.Drawing.Size(468, 21);
            this.checkBoxAggressive.TabIndex = 1;
            this.checkBoxAggressive.Text = "Aggressively merge transactions (you can still un-merge if necessary)";
            this.checkBoxAggressive.UseVisualStyleBackColor = true;
            // 
            // labelBank
            // 
            this.labelBank.AutoSize = true;
            this.labelBank.Location = new System.Drawing.Point(13, 74);
            this.labelBank.Name = "labelBank";
            this.labelBank.Size = new System.Drawing.Size(83, 17);
            this.labelBank.TabIndex = 2;
            this.labelBank.Text = "Select Bank";
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(13, 137);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(77, 17);
            this.labelUser.TabIndex = 3;
            this.labelUser.Text = "Your Login";
            // 
            // labelPwd
            // 
            this.labelPwd.AutoSize = true;
            this.labelPwd.Location = new System.Drawing.Point(13, 169);
            this.labelPwd.Name = "labelPwd";
            this.labelPwd.Size = new System.Drawing.Size(69, 17);
            this.labelPwd.TabIndex = 4;
            this.labelPwd.Text = "Password";
            // 
            // labelAccount
            // 
            this.labelAccount.AutoSize = true;
            this.labelAccount.Location = new System.Drawing.Point(13, 105);
            this.labelAccount.Name = "labelAccount";
            this.labelAccount.Size = new System.Drawing.Size(59, 17);
            this.labelAccount.TabIndex = 5;
            this.labelAccount.Text = "Account";
            // 
            // buttonTest
            // 
            this.buttonTest.Enabled = false;
            this.buttonTest.Location = new System.Drawing.Point(20, 210);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(102, 32);
            this.buttonTest.TabIndex = 6;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(243, 210);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 32);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(363, 210);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(98, 32);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // comboBoxBank
            // 
            this.comboBoxBank.FormattingEnabled = true;
            this.comboBoxBank.Location = new System.Drawing.Point(117, 72);
            this.comboBoxBank.Name = "comboBoxBank";
            this.comboBoxBank.Size = new System.Drawing.Size(344, 24);
            this.comboBoxBank.TabIndex = 9;
            // 
            // comboBoxAcct
            // 
            this.comboBoxAcct.FormattingEnabled = true;
            this.comboBoxAcct.Location = new System.Drawing.Point(117, 105);
            this.comboBoxAcct.Name = "comboBoxAcct";
            this.comboBoxAcct.Size = new System.Drawing.Size(344, 24);
            this.comboBoxAcct.TabIndex = 10;
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(117, 138);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(344, 22);
            this.textBoxUser.TabIndex = 11;
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(117, 169);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(344, 22);
            this.textBoxPwd.TabIndex = 12;
            // 
            // AccountSettingsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(488, 255);
            this.Controls.Add(this.textBoxPwd);
            this.Controls.Add(this.textBoxUser);
            this.Controls.Add(this.comboBoxAcct);
            this.Controls.Add(this.comboBoxBank);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.labelAccount);
            this.Controls.Add(this.labelPwd);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.labelBank);
            this.Controls.Add(this.checkBoxAggressive);
            this.Controls.Add(this.checkBoxLiveSync);
            this.MaximumSize = new System.Drawing.Size(506, 302);
            this.MinimumSize = new System.Drawing.Size(506, 302);
            this.Name = "AccountSettingsForm";
            this.Text = "AccountSettingsForm";
            this.Load += new System.EventHandler(this.AccountSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLiveSync;
        private System.Windows.Forms.CheckBox checkBoxAggressive;
        private System.Windows.Forms.Label labelBank;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Label labelPwd;
        private System.Windows.Forms.Label labelAccount;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxBank;
        private System.Windows.Forms.ComboBox comboBoxAcct;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPwd;
    }
}