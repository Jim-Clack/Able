namespace AbleCheckbook.Gui
{
    partial class PaymentInfoForm
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
            this.labelInstructions = new System.Windows.Forms.Label();
            this.textBoxFirstName = new System.Windows.Forms.TextBox();
            this.textBoxLastName = new System.Windows.Forms.TextBox();
            this.textBoxAptNumber = new System.Windows.Forms.TextBox();
            this.textBoxStreetAddress = new System.Windows.Forms.TextBox();
            this.textBoxState = new System.Windows.Forms.TextBox();
            this.textBoxCity = new System.Windows.Forms.TextBox();
            this.textBoxZip = new System.Windows.Forms.TextBox();
            this.textBoxCcCvv2 = new System.Windows.Forms.TextBox();
            this.textBoxCcNumber = new System.Windows.Forms.TextBox();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.textBoxPhone = new System.Windows.Forms.TextBox();
            this.buttonBar = new System.Windows.Forms.Button();
            this.textBoxShoppingCart = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxErrorMessage = new System.Windows.Forms.TextBox();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelStreetAddress = new System.Windows.Forms.Label();
            this.labelAptNumber = new System.Windows.Forms.Label();
            this.labelCity = new System.Windows.Forms.Label();
            this.labelState = new System.Windows.Forms.Label();
            this.labelZip = new System.Windows.Forms.Label();
            this.labelCountryCode = new System.Windows.Forms.Label();
            this.labelPhone = new System.Windows.Forms.Label();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelCcNumber = new System.Windows.Forms.Label();
            this.labelCcCvv2 = new System.Windows.Forms.Label();
            this.labelCcExpMonth = new System.Windows.Forms.Label();
            this.labelCcExpYear = new System.Windows.Forms.Label();
            this.comboBoxCountryCode = new System.Windows.Forms.ComboBox();
            this.comboBoxCcExpMonth = new System.Windows.Forms.ComboBox();
            this.comboBoxCcExpYear = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // labelInstructions
            // 
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstructions.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelInstructions.Location = new System.Drawing.Point(12, 13);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(574, 17);
            this.labelInstructions.TabIndex = 0;
            this.labelInstructions.Text = "Entries must agree with credit card company data, or your purchase may not go thr" +
    "ough...";
            // 
            // textBoxFirstName
            // 
            this.textBoxFirstName.Location = new System.Drawing.Point(141, 42);
            this.textBoxFirstName.Name = "textBoxFirstName";
            this.textBoxFirstName.Size = new System.Drawing.Size(150, 22);
            this.textBoxFirstName.TabIndex = 1;
            // 
            // textBoxLastName
            // 
            this.textBoxLastName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLastName.Location = new System.Drawing.Point(430, 42);
            this.textBoxLastName.Name = "textBoxLastName";
            this.textBoxLastName.Size = new System.Drawing.Size(149, 22);
            this.textBoxLastName.TabIndex = 2;
            // 
            // textBoxAptNumber
            // 
            this.textBoxAptNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAptNumber.Location = new System.Drawing.Point(430, 74);
            this.textBoxAptNumber.Name = "textBoxAptNumber";
            this.textBoxAptNumber.Size = new System.Drawing.Size(149, 22);
            this.textBoxAptNumber.TabIndex = 4;
            this.textBoxAptNumber.Leave += new System.EventHandler(this.textBoxAptNumber_Leave);
            // 
            // textBoxStreetAddress
            // 
            this.textBoxStreetAddress.Location = new System.Drawing.Point(141, 74);
            this.textBoxStreetAddress.Name = "textBoxStreetAddress";
            this.textBoxStreetAddress.Size = new System.Drawing.Size(150, 22);
            this.textBoxStreetAddress.TabIndex = 3;
            // 
            // textBoxState
            // 
            this.textBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxState.Location = new System.Drawing.Point(430, 106);
            this.textBoxState.Name = "textBoxState";
            this.textBoxState.Size = new System.Drawing.Size(149, 22);
            this.textBoxState.TabIndex = 6;
            // 
            // textBoxCity
            // 
            this.textBoxCity.Location = new System.Drawing.Point(141, 106);
            this.textBoxCity.Name = "textBoxCity";
            this.textBoxCity.Size = new System.Drawing.Size(150, 22);
            this.textBoxCity.TabIndex = 5;
            // 
            // textBoxZip
            // 
            this.textBoxZip.Location = new System.Drawing.Point(141, 138);
            this.textBoxZip.Name = "textBoxZip";
            this.textBoxZip.Size = new System.Drawing.Size(150, 22);
            this.textBoxZip.TabIndex = 7;
            // 
            // textBoxCcCvv2
            // 
            this.textBoxCcCvv2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCcCvv2.Location = new System.Drawing.Point(518, 217);
            this.textBoxCcCvv2.Name = "textBoxCcCvv2";
            this.textBoxCcCvv2.Size = new System.Drawing.Size(61, 22);
            this.textBoxCcCvv2.TabIndex = 12;
            // 
            // textBoxCcNumber
            // 
            this.textBoxCcNumber.Location = new System.Drawing.Point(141, 217);
            this.textBoxCcNumber.Name = "textBoxCcNumber";
            this.textBoxCcNumber.Size = new System.Drawing.Size(254, 22);
            this.textBoxCcNumber.TabIndex = 11;
            this.textBoxCcNumber.TextChanged += new System.EventHandler(this.textBoxCcNumber_TextChanged);
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEmail.Location = new System.Drawing.Point(430, 170);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(149, 22);
            this.textBoxEmail.TabIndex = 10;
            // 
            // textBoxPhone
            // 
            this.textBoxPhone.Location = new System.Drawing.Point(141, 170);
            this.textBoxPhone.Name = "textBoxPhone";
            this.textBoxPhone.Size = new System.Drawing.Size(150, 22);
            this.textBoxPhone.TabIndex = 9;
            this.textBoxPhone.TextChanged += new System.EventHandler(this.textBoxPhone_TextChanged);
            // 
            // buttonBar
            // 
            this.buttonBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBar.Location = new System.Drawing.Point(16, 202);
            this.buttonBar.Name = "buttonBar";
            this.buttonBar.Size = new System.Drawing.Size(563, 6);
            this.buttonBar.TabIndex = 15;
            this.buttonBar.UseVisualStyleBackColor = true;
            // 
            // textBoxShoppingCart
            // 
            this.textBoxShoppingCart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxShoppingCart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxShoppingCart.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxShoppingCart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.textBoxShoppingCart.Location = new System.Drawing.Point(16, 284);
            this.textBoxShoppingCart.Multiline = true;
            this.textBoxShoppingCart.Name = "textBoxShoppingCart";
            this.textBoxShoppingCart.ReadOnly = true;
            this.textBoxShoppingCart.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxShoppingCart.Size = new System.Drawing.Size(275, 101);
            this.textBoxShoppingCart.TabIndex = 16;
            this.textBoxShoppingCart.WordWrap = false;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(460, 341);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(119, 32);
            this.buttonOk.TabIndex = 15;
            this.buttonOk.Text = "Purchase";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(317, 341);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(119, 32);
            this.buttonCancel.TabIndex = 16;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxErrorMessage
            // 
            this.textBoxErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxErrorMessage.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.textBoxErrorMessage.Location = new System.Drawing.Point(298, 284);
            this.textBoxErrorMessage.Multiline = true;
            this.textBoxErrorMessage.Name = "textBoxErrorMessage";
            this.textBoxErrorMessage.ReadOnly = true;
            this.textBoxErrorMessage.Size = new System.Drawing.Size(281, 51);
            this.textBoxErrorMessage.TabIndex = 19;
            this.textBoxErrorMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelFirstName
            // 
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Location = new System.Drawing.Point(12, 44);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(118, 17);
            this.labelFirstName.TabIndex = 20;
            this.labelFirstName.Text = "First Name on CC";
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(303, 45);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(118, 17);
            this.labelLastName.TabIndex = 21;
            this.labelLastName.Text = "Last Name on CC";
            // 
            // labelStreetAddress
            // 
            this.labelStreetAddress.AutoSize = true;
            this.labelStreetAddress.Location = new System.Drawing.Point(12, 75);
            this.labelStreetAddress.Name = "labelStreetAddress";
            this.labelStreetAddress.Size = new System.Drawing.Size(102, 17);
            this.labelStreetAddress.TabIndex = 22;
            this.labelStreetAddress.Text = "Street Address";
            // 
            // labelAptNumber
            // 
            this.labelAptNumber.AutoSize = true;
            this.labelAptNumber.Location = new System.Drawing.Point(304, 77);
            this.labelAptNumber.Name = "labelAptNumber";
            this.labelAptNumber.Size = new System.Drawing.Size(83, 17);
            this.labelAptNumber.TabIndex = 23;
            this.labelAptNumber.Text = "Apt Number";
            // 
            // labelCity
            // 
            this.labelCity.AutoSize = true;
            this.labelCity.Location = new System.Drawing.Point(13, 107);
            this.labelCity.Name = "labelCity";
            this.labelCity.Size = new System.Drawing.Size(107, 17);
            this.labelCity.TabIndex = 24;
            this.labelCity.Text = "City (per USPS)";
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(304, 109);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(107, 17);
            this.labelState.TabIndex = 25;
            this.labelState.Text = "State (2-letters)";
            // 
            // labelZip
            // 
            this.labelZip.AutoSize = true;
            this.labelZip.Location = new System.Drawing.Point(14, 139);
            this.labelZip.Name = "labelZip";
            this.labelZip.Size = new System.Drawing.Size(108, 17);
            this.labelZip.TabIndex = 26;
            this.labelZip.Text = "Zip/Postal Code";
            // 
            // labelCountryCode
            // 
            this.labelCountryCode.AutoSize = true;
            this.labelCountryCode.Location = new System.Drawing.Point(304, 139);
            this.labelCountryCode.Name = "labelCountryCode";
            this.labelCountryCode.Size = new System.Drawing.Size(94, 17);
            this.labelCountryCode.TabIndex = 27;
            this.labelCountryCode.Text = "Country Code";
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.labelPhone.Location = new System.Drawing.Point(14, 171);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(124, 17);
            this.labelPhone.TabIndex = 28;
            this.labelPhone.Text = "Phone (10+ digits)";
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(304, 171);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(98, 17);
            this.labelEmail.TabIndex = 29;
            this.labelEmail.Text = "Email Address";
            // 
            // labelCcNumber
            // 
            this.labelCcNumber.AutoSize = true;
            this.labelCcNumber.Location = new System.Drawing.Point(14, 218);
            this.labelCcNumber.Name = "labelCcNumber";
            this.labelCcNumber.Size = new System.Drawing.Size(112, 17);
            this.labelCcNumber.TabIndex = 30;
            this.labelCcNumber.Text = "Credit Card Num";
            // 
            // labelCcCvv2
            // 
            this.labelCcCvv2.AutoSize = true;
            this.labelCcCvv2.Location = new System.Drawing.Point(414, 218);
            this.labelCcCvv2.Name = "labelCcCvv2";
            this.labelCcCvv2.Size = new System.Drawing.Size(86, 17);
            this.labelCcCvv2.TabIndex = 31;
            this.labelCcCvv2.Text = "CVV2 (Help)";
            this.labelCcCvv2.Click += new System.EventHandler(this.labelCcCvv2_Click);
            // 
            // labelCcExpMonth
            // 
            this.labelCcExpMonth.AutoSize = true;
            this.labelCcExpMonth.Location = new System.Drawing.Point(14, 252);
            this.labelCcExpMonth.Name = "labelCcExpMonth";
            this.labelCcExpMonth.Size = new System.Drawing.Size(113, 17);
            this.labelCcExpMonth.TabIndex = 32;
            this.labelCcExpMonth.Text = "Expiration Month";
            // 
            // labelCcExpYear
            // 
            this.labelCcExpYear.AutoSize = true;
            this.labelCcExpYear.Location = new System.Drawing.Point(304, 251);
            this.labelCcExpYear.Name = "labelCcExpYear";
            this.labelCcExpYear.Size = new System.Drawing.Size(104, 17);
            this.labelCcExpYear.TabIndex = 33;
            this.labelCcExpYear.Text = "Expiration Year";
            // 
            // comboBoxCountryCode
            // 
            this.comboBoxCountryCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCountryCode.FormattingEnabled = true;
            this.comboBoxCountryCode.Items.AddRange(new object[] {
            "US",
            "CA",
            "GB",
            "JP",
            "FR",
            "DE"});
            this.comboBoxCountryCode.Location = new System.Drawing.Point(430, 137);
            this.comboBoxCountryCode.Name = "comboBoxCountryCode";
            this.comboBoxCountryCode.Size = new System.Drawing.Size(149, 24);
            this.comboBoxCountryCode.TabIndex = 8;
            this.comboBoxCountryCode.Text = "US";
            // 
            // comboBoxCcExpMonth
            // 
            this.comboBoxCcExpMonth.FormattingEnabled = true;
            this.comboBoxCcExpMonth.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12"});
            this.comboBoxCcExpMonth.Location = new System.Drawing.Point(141, 248);
            this.comboBoxCcExpMonth.Name = "comboBoxCcExpMonth";
            this.comboBoxCcExpMonth.Size = new System.Drawing.Size(149, 24);
            this.comboBoxCcExpMonth.TabIndex = 13;
            // 
            // comboBoxCcExpYear
            // 
            this.comboBoxCcExpYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCcExpYear.FormattingEnabled = true;
            this.comboBoxCcExpYear.Location = new System.Drawing.Point(430, 248);
            this.comboBoxCcExpYear.Name = "comboBoxCcExpYear";
            this.comboBoxCcExpYear.Size = new System.Drawing.Size(149, 24);
            this.comboBoxCcExpYear.TabIndex = 14;
            // 
            // PaymentInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 387);
            this.Controls.Add(this.comboBoxCcExpYear);
            this.Controls.Add(this.comboBoxCcExpMonth);
            this.Controls.Add(this.comboBoxCountryCode);
            this.Controls.Add(this.textBoxErrorMessage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxShoppingCart);
            this.Controls.Add(this.buttonBar);
            this.Controls.Add(this.textBoxEmail);
            this.Controls.Add(this.textBoxPhone);
            this.Controls.Add(this.textBoxCcCvv2);
            this.Controls.Add(this.textBoxCcNumber);
            this.Controls.Add(this.textBoxZip);
            this.Controls.Add(this.textBoxState);
            this.Controls.Add(this.textBoxCity);
            this.Controls.Add(this.textBoxAptNumber);
            this.Controls.Add(this.textBoxStreetAddress);
            this.Controls.Add(this.textBoxLastName);
            this.Controls.Add(this.textBoxFirstName);
            this.Controls.Add(this.labelInstructions);
            this.Controls.Add(this.labelCcExpYear);
            this.Controls.Add(this.labelCcExpMonth);
            this.Controls.Add(this.labelCcCvv2);
            this.Controls.Add(this.labelCcNumber);
            this.Controls.Add(this.labelEmail);
            this.Controls.Add(this.labelPhone);
            this.Controls.Add(this.labelCountryCode);
            this.Controls.Add(this.labelZip);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.labelCity);
            this.Controls.Add(this.labelAptNumber);
            this.Controls.Add(this.labelStreetAddress);
            this.Controls.Add(this.labelLastName);
            this.Controls.Add(this.labelFirstName);
            this.MaximumSize = new System.Drawing.Size(740, 480);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "PaymentInfoForm";
            this.Text = "PaymentInfoForm";
            this.Load += new System.EventHandler(this.PaymentInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.TextBox textBoxFirstName;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.TextBox textBoxAptNumber;
        private System.Windows.Forms.TextBox textBoxStreetAddress;
        private System.Windows.Forms.TextBox textBoxState;
        private System.Windows.Forms.TextBox textBoxCity;
        private System.Windows.Forms.TextBox textBoxZip;
        private System.Windows.Forms.TextBox textBoxCcCvv2;
        private System.Windows.Forms.TextBox textBoxCcNumber;
        private System.Windows.Forms.TextBox textBoxEmail;
        private System.Windows.Forms.TextBox textBoxPhone;
        private System.Windows.Forms.Button buttonBar;
        private System.Windows.Forms.TextBox textBoxShoppingCart;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxErrorMessage;
        private System.Windows.Forms.Label labelFirstName;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelStreetAddress;
        private System.Windows.Forms.Label labelAptNumber;
        private System.Windows.Forms.Label labelCity;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label labelZip;
        private System.Windows.Forms.Label labelCountryCode;
        private System.Windows.Forms.Label labelPhone;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelCcNumber;
        private System.Windows.Forms.Label labelCcCvv2;
        private System.Windows.Forms.Label labelCcExpMonth;
        private System.Windows.Forms.Label labelCcExpYear;
        private System.Windows.Forms.ComboBox comboBoxCountryCode;
        private System.Windows.Forms.ComboBox comboBoxCcExpMonth;
        private System.Windows.Forms.ComboBox comboBoxCcExpYear;
    }
}