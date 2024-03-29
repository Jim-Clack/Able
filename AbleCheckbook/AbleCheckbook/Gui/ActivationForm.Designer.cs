﻿namespace AbleCheckbook.Gui
{
    partial class ActivationForm
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
            this.textBoxSiteId = new System.Windows.Forms.TextBox();
            this.textBoxUserId = new System.Windows.Forms.TextBox();
            this.textBoxPostalCode = new System.Windows.Forms.TextBox();
            this.textBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.textBoxEmailAddress = new System.Windows.Forms.TextBox();
            this.textBoxPin = new System.Windows.Forms.TextBox();
            this.buttonActivate = new System.Windows.Forms.Button();
            this.labelPin = new System.Windows.Forms.Label();
            this.labelSiteId = new System.Windows.Forms.Label();
            this.labelUserId = new System.Windows.Forms.Label();
            this.labelPostalCode = new System.Windows.Forms.Label();
            this.labelPhoneNumber = new System.Windows.Forms.Label();
            this.labelEmailAddress = new System.Windows.Forms.Label();
            this.checkBoxAcceptTerms = new System.Windows.Forms.CheckBox();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.textBoxIpAddress = new System.Windows.Forms.TextBox();
            this.linkLabelEula = new System.Windows.Forms.LinkLabel();
            this.textBoxLicenseCode = new System.Windows.Forms.TextBox();
            this.labelLicenseCode = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxCityState = new System.Windows.Forms.TextBox();
            this.textBoxStreetAddress = new System.Windows.Forms.TextBox();
            this.labelCityState = new System.Windows.Forms.Label();
            this.labelStreetAddress = new System.Windows.Forms.Label();
            this.textBoxPurchase = new System.Windows.Forms.TextBox();
            this.labelPurchase = new System.Windows.Forms.Label();
            this.labelSaveTheseNotice = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.labelAlreadyPurchased = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxSiteId
            // 
            this.textBoxSiteId.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSiteId.Location = new System.Drawing.Point(169, 15);
            this.textBoxSiteId.Name = "textBoxSiteId";
            this.textBoxSiteId.ReadOnly = true;
            this.textBoxSiteId.Size = new System.Drawing.Size(124, 24);
            this.textBoxSiteId.TabIndex = 12;
            this.textBoxSiteId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxSiteId.TextChanged += new System.EventHandler(this.criticalEntry_TextChanged);
            // 
            // textBoxUserId
            // 
            this.textBoxUserId.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUserId.Location = new System.Drawing.Point(169, 49);
            this.textBoxUserId.Name = "textBoxUserId";
            this.textBoxUserId.Size = new System.Drawing.Size(124, 24);
            this.textBoxUserId.TabIndex = 0;
            this.textBoxUserId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPostalCode
            // 
            this.textBoxPostalCode.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPostalCode.Location = new System.Drawing.Point(169, 116);
            this.textBoxPostalCode.Name = "textBoxPostalCode";
            this.textBoxPostalCode.Size = new System.Drawing.Size(124, 24);
            this.textBoxPostalCode.TabIndex = 4;
            this.textBoxPostalCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(459, 116);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(150, 24);
            this.textBoxPhoneNumber.TabIndex = 5;
            this.textBoxPhoneNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxEmailAddress
            // 
            this.textBoxEmailAddress.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEmailAddress.Location = new System.Drawing.Point(459, 49);
            this.textBoxEmailAddress.Name = "textBoxEmailAddress";
            this.textBoxEmailAddress.Size = new System.Drawing.Size(150, 24);
            this.textBoxEmailAddress.TabIndex = 1;
            this.textBoxEmailAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPin
            // 
            this.textBoxPin.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxPin.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPin.Location = new System.Drawing.Point(459, 282);
            this.textBoxPin.Name = "textBoxPin";
            this.textBoxPin.Size = new System.Drawing.Size(150, 24);
            this.textBoxPin.TabIndex = 11;
            this.textBoxPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxPin.TextChanged += new System.EventHandler(this.criticalEntry_TextChanged);
            // 
            // buttonActivate
            // 
            this.buttonActivate.Location = new System.Drawing.Point(197, 177);
            this.buttonActivate.Name = "buttonActivate";
            this.buttonActivate.Size = new System.Drawing.Size(234, 32);
            this.buttonActivate.TabIndex = 7;
            this.buttonActivate.Text = "Activate";
            this.buttonActivate.UseVisualStyleBackColor = true;
            this.buttonActivate.Click += new System.EventHandler(this.buttonActivate_Click);
            // 
            // labelPin
            // 
            this.labelPin.AutoSize = true;
            this.labelPin.Location = new System.Drawing.Point(308, 285);
            this.labelPin.Name = "labelPin";
            this.labelPin.Size = new System.Drawing.Size(140, 17);
            this.labelPin.TabIndex = 7;
            this.labelPin.Text = "Coded Activation PIN";
            // 
            // labelSiteId
            // 
            this.labelSiteId.AutoSize = true;
            this.labelSiteId.Location = new System.Drawing.Point(9, 18);
            this.labelSiteId.Name = "labelSiteId";
            this.labelSiteId.Size = new System.Drawing.Size(152, 17);
            this.labelSiteId.TabIndex = 8;
            this.labelSiteId.Text = "Site Identification Code";
            // 
            // labelUserId
            // 
            this.labelUserId.AutoSize = true;
            this.labelUserId.Location = new System.Drawing.Point(9, 51);
            this.labelUserId.Name = "labelUserId";
            this.labelUserId.Size = new System.Drawing.Size(131, 17);
            this.labelUserId.TabIndex = 9;
            this.labelUserId.Text = "Contact/User Name";
            // 
            // labelPostalCode
            // 
            this.labelPostalCode.AutoSize = true;
            this.labelPostalCode.Location = new System.Drawing.Point(9, 118);
            this.labelPostalCode.Name = "labelPostalCode";
            this.labelPostalCode.Size = new System.Drawing.Size(148, 17);
            this.labelPostalCode.TabIndex = 10;
            this.labelPostalCode.Text = "Postal/ZIP Code or CC";
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(302, 119);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(149, 17);
            this.labelPhoneNumber.TabIndex = 11;
            this.labelPhoneNumber.Text = "10-12 Digit Phone Nbr";
            // 
            // labelEmailAddress
            // 
            this.labelEmailAddress.AutoSize = true;
            this.labelEmailAddress.Location = new System.Drawing.Point(301, 51);
            this.labelEmailAddress.Name = "labelEmailAddress";
            this.labelEmailAddress.Size = new System.Drawing.Size(150, 17);
            this.labelEmailAddress.TabIndex = 12;
            this.labelEmailAddress.Text = "Contact Email Address";
            // 
            // checkBoxAcceptTerms
            // 
            this.checkBoxAcceptTerms.AutoSize = true;
            this.checkBoxAcceptTerms.Location = new System.Drawing.Point(13, 148);
            this.checkBoxAcceptTerms.Name = "checkBoxAcceptTerms";
            this.checkBoxAcceptTerms.Size = new System.Drawing.Size(318, 21);
            this.checkBoxAcceptTerms.TabIndex = 6;
            this.checkBoxAcceptTerms.Text = "I have read and accept the terms of the EULA";
            this.checkBoxAcceptTerms.UseVisualStyleBackColor = true;
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(301, 18);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(152, 17);
            this.labelIpAddress.TabIndex = 15;
            this.labelIpAddress.Text = "Computer Identification";
            // 
            // textBoxIpAddress
            // 
            this.textBoxIpAddress.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxIpAddress.Location = new System.Drawing.Point(459, 15);
            this.textBoxIpAddress.Name = "textBoxIpAddress";
            this.textBoxIpAddress.ReadOnly = true;
            this.textBoxIpAddress.Size = new System.Drawing.Size(150, 24);
            this.textBoxIpAddress.TabIndex = 13;
            this.textBoxIpAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxIpAddress.TextChanged += new System.EventHandler(this.criticalEntry_TextChanged);
            // 
            // linkLabelEula
            // 
            this.linkLabelEula.AutoSize = true;
            this.linkLabelEula.Location = new System.Drawing.Point(346, 148);
            this.linkLabelEula.Name = "linkLabelEula";
            this.linkLabelEula.Size = new System.Drawing.Size(242, 17);
            this.linkLabelEula.TabIndex = 16;
            this.linkLabelEula.TabStop = true;
            this.linkLabelEula.Text = "EULA - End User License Agreement";
            this.linkLabelEula.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEula_LinkClicked);
            // 
            // textBoxLicenseCode
            // 
            this.textBoxLicenseCode.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxLicenseCode.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLicenseCode.Location = new System.Drawing.Point(171, 282);
            this.textBoxLicenseCode.Name = "textBoxLicenseCode";
            this.textBoxLicenseCode.Size = new System.Drawing.Size(124, 24);
            this.textBoxLicenseCode.TabIndex = 28;
            this.textBoxLicenseCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxLicenseCode.TextChanged += new System.EventHandler(this.criticalEntry_TextChanged);
            // 
            // labelLicenseCode
            // 
            this.labelLicenseCode.AutoSize = true;
            this.labelLicenseCode.Location = new System.Drawing.Point(8, 285);
            this.labelLicenseCode.Name = "labelLicenseCode";
            this.labelLicenseCode.Size = new System.Drawing.Size(156, 17);
            this.labelLicenseCode.TabIndex = 18;
            this.labelLicenseCode.Text = "Assigned License Code";
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(478, 177);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(131, 32);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxCityState
            // 
            this.textBoxCityState.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCityState.Location = new System.Drawing.Point(459, 82);
            this.textBoxCityState.Name = "textBoxCityState";
            this.textBoxCityState.Size = new System.Drawing.Size(150, 24);
            this.textBoxCityState.TabIndex = 3;
            this.textBoxCityState.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxStreetAddress
            // 
            this.textBoxStreetAddress.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStreetAddress.Location = new System.Drawing.Point(169, 82);
            this.textBoxStreetAddress.Name = "textBoxStreetAddress";
            this.textBoxStreetAddress.Size = new System.Drawing.Size(124, 24);
            this.textBoxStreetAddress.TabIndex = 2;
            this.textBoxStreetAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelCityState
            // 
            this.labelCityState.AutoSize = true;
            this.labelCityState.Location = new System.Drawing.Point(301, 84);
            this.labelCityState.Name = "labelCityState";
            this.labelCityState.Size = new System.Drawing.Size(155, 17);
            this.labelCityState.TabIndex = 23;
            this.labelCityState.Text = "City or County";
            // 
            // labelStreetAddress
            // 
            this.labelStreetAddress.AutoSize = true;
            this.labelStreetAddress.Location = new System.Drawing.Point(9, 84);
            this.labelStreetAddress.Name = "labelStreetAddress";
            this.labelStreetAddress.Size = new System.Drawing.Size(130, 17);
            this.labelStreetAddress.TabIndex = 22;
            this.labelStreetAddress.Text = "Address and Street";
            // 
            // textBoxPurchase
            // 
            this.textBoxPurchase.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxPurchase.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPurchase.Location = new System.Drawing.Point(224, 248);
            this.textBoxPurchase.Name = "textBoxPurchase";
            this.textBoxPurchase.Size = new System.Drawing.Size(385, 24);
            this.textBoxPurchase.TabIndex = 9;
            this.textBoxPurchase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxPurchase.TextChanged += new System.EventHandler(this.criticalEntry_TextChanged);
            // 
            // labelPurchase
            // 
            this.labelPurchase.AutoSize = true;
            this.labelPurchase.Location = new System.Drawing.Point(8, 250);
            this.labelPurchase.Name = "labelPurchase";
            this.labelPurchase.Size = new System.Drawing.Size(204, 17);
            this.labelPurchase.TabIndex = 25;
            this.labelPurchase.Text = "Purchase designator (if known)";
            // 
            // labelNotice
            // 
            this.labelSaveTheseNotice.AutoSize = true;
            this.labelSaveTheseNotice.ForeColor = System.Drawing.Color.Red;
            this.labelSaveTheseNotice.Location = new System.Drawing.Point(8, 218);
            this.labelSaveTheseNotice.Name = "labelNotice";
            this.labelSaveTheseNotice.Size = new System.Drawing.Size(595, 17);
            this.labelSaveTheseNotice.TabIndex = 26;
            this.labelSaveTheseNotice.Text = "Record the following values in a safe place as your proof of purchase and activat" +
    "ion numbers";
            this.labelSaveTheseNotice.Visible = false;
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(10, 177);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(130, 32);
            this.buttonReset.TabIndex = 27;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // labelAlreadyPurchased
            // 
            this.labelAlreadyPurchased.AutoSize = true;
            this.labelAlreadyPurchased.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelAlreadyPurchased.Location = new System.Drawing.Point(8, 218);
            this.labelAlreadyPurchased.Name = "labelAlreadyPurchased";
            this.labelAlreadyPurchased.Size = new System.Drawing.Size(572, 17);
            this.labelAlreadyPurchased.TabIndex = 29;
            this.labelAlreadyPurchased.Text = "If you have already paid and activated, you may fill out your codes in the spa" +
    "ces below";
            this.labelAlreadyPurchased.Visible = false;
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(622, 317);
            this.Controls.Add(this.labelSaveTheseNotice);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.textBoxPurchase);
            this.Controls.Add(this.labelPurchase);
            this.Controls.Add(this.textBoxCityState);
            this.Controls.Add(this.textBoxStreetAddress);
            this.Controls.Add(this.labelStreetAddress);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxPin);
            this.Controls.Add(this.textBoxLicenseCode);
            this.Controls.Add(this.linkLabelEula);
            this.Controls.Add(this.textBoxIpAddress);
            this.Controls.Add(this.checkBoxAcceptTerms);
            this.Controls.Add(this.buttonActivate);
            this.Controls.Add(this.textBoxEmailAddress);
            this.Controls.Add(this.textBoxPhoneNumber);
            this.Controls.Add(this.textBoxPostalCode);
            this.Controls.Add(this.textBoxUserId);
            this.Controls.Add(this.textBoxSiteId);
            this.Controls.Add(this.labelUserId);
            this.Controls.Add(this.labelSiteId);
            this.Controls.Add(this.labelPostalCode);
            this.Controls.Add(this.labelLicenseCode);
            this.Controls.Add(this.labelCityState);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.labelEmailAddress);
            this.Controls.Add(this.labelPhoneNumber);
            this.Controls.Add(this.labelPin);
            this.Controls.Add(this.labelAlreadyPurchased);
            this.MaximumSize = new System.Drawing.Size(640, 364);
            this.MinimumSize = new System.Drawing.Size(640, 364);
            this.Name = "ActivationForm";
            this.Text = "Activation";
            this.Load += new System.EventHandler(this.ActivationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSiteId;
        private System.Windows.Forms.TextBox textBoxUserId;
        private System.Windows.Forms.TextBox textBoxPostalCode;
        private System.Windows.Forms.TextBox textBoxPhoneNumber;
        private System.Windows.Forms.TextBox textBoxEmailAddress;
        private System.Windows.Forms.TextBox textBoxPin;
        private System.Windows.Forms.Button buttonActivate;
        private System.Windows.Forms.Label labelPin;
        private System.Windows.Forms.Label labelSiteId;
        private System.Windows.Forms.Label labelUserId;
        private System.Windows.Forms.Label labelPostalCode;
        private System.Windows.Forms.Label labelPhoneNumber;
        private System.Windows.Forms.Label labelEmailAddress;
        private System.Windows.Forms.CheckBox checkBoxAcceptTerms;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.TextBox textBoxIpAddress;
        private System.Windows.Forms.LinkLabel linkLabelEula;
        private System.Windows.Forms.TextBox textBoxLicenseCode;
        private System.Windows.Forms.Label labelLicenseCode;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxCityState;
        private System.Windows.Forms.TextBox textBoxStreetAddress;
        private System.Windows.Forms.Label labelCityState;
        private System.Windows.Forms.Label labelStreetAddress;
        private System.Windows.Forms.TextBox textBoxPurchase;
        private System.Windows.Forms.Label labelPurchase;
        private System.Windows.Forms.Label labelSaveTheseNotice;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label labelAlreadyPurchased;
    }
}