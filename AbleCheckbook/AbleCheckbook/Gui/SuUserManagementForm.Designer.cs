namespace AbleCheckbook.Gui
{
    partial class SuUserManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuUserManagementForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBoxSearchPattern = new System.Windows.Forms.TextBox();
            this.buttonSearchNotes = new System.Windows.Forms.Button();
            this.buttonSearchInfo = new System.Windows.Forms.Button();
            this.textBoxSiteIdCode = new System.Windows.Forms.TextBox();
            this.buttonActivationPin = new System.Windows.Forms.Button();
            this.textBoxActivationPin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSiteLicenseCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelCannotDelete = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.LicenseCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SiteId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Company = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Important = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PhoneNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmailAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZipCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OtherInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActivBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IpAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateEnter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateActiv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateLastAcc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateLastWebService = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HiddenInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LicenseCode,
            this.SiteId,
            this.Contact,
            this.Company,
            this.Important,
            this.PhoneNum,
            this.EmailAddr,
            this.ZipCode,
            this.OtherInfo,
            this.ActivBy,
            this.Notes,
            this.IpAddress,
            this.DateEnter,
            this.DateActiv,
            this.DateLastAcc,
            this.DateLastWebService,
            this.Id,
            this.HiddenInfo});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowLeave);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this.dataGridView1.Leave += new System.EventHandler(this.dataGridView1_Leave);
            // 
            // textBoxSearchPattern
            // 
            resources.ApplyResources(this.textBoxSearchPattern, "textBoxSearchPattern");
            this.textBoxSearchPattern.Name = "textBoxSearchPattern";
            this.textBoxSearchPattern.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchPattern_KeyUp);
            // 
            // buttonSearchNotes
            // 
            resources.ApplyResources(this.buttonSearchNotes, "buttonSearchNotes");
            this.buttonSearchNotes.Name = "buttonSearchNotes";
            this.buttonSearchNotes.UseVisualStyleBackColor = true;
            this.buttonSearchNotes.Click += new System.EventHandler(this.buttonSearchNotes_Click);
            // 
            // buttonSearchInfo
            // 
            resources.ApplyResources(this.buttonSearchInfo, "buttonSearchInfo");
            this.buttonSearchInfo.Name = "buttonSearchInfo";
            this.buttonSearchInfo.UseVisualStyleBackColor = true;
            this.buttonSearchInfo.Click += new System.EventHandler(this.buttonSearchInfo_Click);
            // 
            // textBoxSiteIdCode
            // 
            resources.ApplyResources(this.textBoxSiteIdCode, "textBoxSiteIdCode");
            this.textBoxSiteIdCode.Name = "textBoxSiteIdCode";
            // 
            // buttonActivationPin
            // 
            resources.ApplyResources(this.buttonActivationPin, "buttonActivationPin");
            this.buttonActivationPin.Name = "buttonActivationPin";
            this.buttonActivationPin.UseVisualStyleBackColor = true;
            this.buttonActivationPin.Click += new System.EventHandler(this.buttonActivationPin_Click);
            // 
            // textBoxActivationPin
            // 
            resources.ApplyResources(this.textBoxActivationPin, "textBoxActivationPin");
            this.textBoxActivationPin.Name = "textBoxActivationPin";
            this.textBoxActivationPin.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxSiteLicenseCode
            // 
            resources.ApplyResources(this.textBoxSiteLicenseCode, "textBoxSiteLicenseCode");
            this.textBoxSiteLicenseCode.Name = "textBoxSiteLicenseCode";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelCannotDelete
            // 
            resources.ApplyResources(this.labelCannotDelete, "labelCannotDelete");
            this.labelCannotDelete.Name = "labelCannotDelete";
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // LicenseCode
            // 
            this.LicenseCode.DataPropertyName = "LicenseCode";
            resources.ApplyResources(this.LicenseCode, "LicenseCode");
            this.LicenseCode.Name = "LicenseCode";
            this.LicenseCode.ReadOnly = true;
            // 
            // SiteId
            // 
            this.SiteId.DataPropertyName = "SiteId";
            resources.ApplyResources(this.SiteId, "SiteId");
            this.SiteId.Name = "SiteId";
            this.SiteId.ReadOnly = true;
            // 
            // Contact
            // 
            this.Contact.DataPropertyName = "Contact";
            resources.ApplyResources(this.Contact, "Contact");
            this.Contact.Name = "Contact";
            // 
            // Company
            // 
            this.Company.DataPropertyName = "Company";
            resources.ApplyResources(this.Company, "Company");
            this.Company.Name = "Company";
            // 
            // Important
            // 
            this.Important.DataPropertyName = "Important";
            resources.ApplyResources(this.Important, "Important");
            this.Important.Name = "Important";
            // 
            // PhoneNum
            // 
            this.PhoneNum.DataPropertyName = "PhoneNum";
            resources.ApplyResources(this.PhoneNum, "PhoneNum");
            this.PhoneNum.Name = "PhoneNum";
            // 
            // EmailAddr
            // 
            this.EmailAddr.DataPropertyName = "EmailAddr";
            resources.ApplyResources(this.EmailAddr, "EmailAddr");
            this.EmailAddr.Name = "EmailAddr";
            // 
            // ZipCode
            // 
            this.ZipCode.DataPropertyName = "ZipCode";
            resources.ApplyResources(this.ZipCode, "ZipCode");
            this.ZipCode.Name = "ZipCode";
            // 
            // OtherInfo
            // 
            this.OtherInfo.DataPropertyName = "OtherInfo";
            resources.ApplyResources(this.OtherInfo, "OtherInfo");
            this.OtherInfo.Name = "OtherInfo";
            // 
            // ActivBy
            // 
            this.ActivBy.DataPropertyName = "ActivBy";
            resources.ApplyResources(this.ActivBy, "ActivBy");
            this.ActivBy.Name = "ActivBy";
            // 
            // Notes
            // 
            this.Notes.DataPropertyName = "Notes";
            resources.ApplyResources(this.Notes, "Notes");
            this.Notes.Name = "Notes";
            // 
            // IpAddress
            // 
            this.IpAddress.DataPropertyName = "IpAddress";
            resources.ApplyResources(this.IpAddress, "IpAddress");
            this.IpAddress.Name = "IpAddress";
            // 
            // DateEnter
            // 
            this.DateEnter.DataPropertyName = "DateEnter";
            resources.ApplyResources(this.DateEnter, "DateEnter");
            this.DateEnter.Name = "DateEnter";
            // 
            // DateActiv
            // 
            this.DateActiv.DataPropertyName = "DateActiv";
            resources.ApplyResources(this.DateActiv, "DateActiv");
            this.DateActiv.Name = "DateActiv";
            // 
            // DateLastAcc
            // 
            this.DateLastAcc.DataPropertyName = "DateLastAcc";
            resources.ApplyResources(this.DateLastAcc, "DateLastAcc");
            this.DateLastAcc.Name = "DateLastAcc";
            // 
            // DateLastWebService
            // 
            this.DateLastWebService.DataPropertyName = "DateLastWebService";
            resources.ApplyResources(this.DateLastWebService, "DateLastWebService");
            this.DateLastWebService.Name = "DateLastWebService";
            this.DateLastWebService.ReadOnly = true;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            resources.ApplyResources(this.Id, "Id");
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // HiddenInfo
            // 
            this.HiddenInfo.DataPropertyName = "HiddenInfo";
            resources.ApplyResources(this.HiddenInfo, "HiddenInfo");
            this.HiddenInfo.Name = "HiddenInfo";
            this.HiddenInfo.ReadOnly = true;
            // 
            // SuUserManagementForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.labelCannotDelete);
            this.Controls.Add(this.textBoxActivationPin);
            this.Controls.Add(this.textBoxSiteIdCode);
            this.Controls.Add(this.textBoxSiteLicenseCode);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonActivationPin);
            this.Controls.Add(this.buttonSearchInfo);
            this.Controls.Add(this.buttonSearchNotes);
            this.Controls.Add(this.textBoxSearchPattern);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "SuUserManagementForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SuUserManagementForm_FormClosed);
            this.Load += new System.EventHandler(this.SuUserManagement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxSearchPattern;
        private System.Windows.Forms.Button buttonSearchNotes;
        private System.Windows.Forms.Button buttonSearchInfo;
        private System.Windows.Forms.TextBox textBoxSiteIdCode;
        private System.Windows.Forms.Button buttonActivationPin;
        private System.Windows.Forms.TextBox textBoxActivationPin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSiteLicenseCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelCannotDelete;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn LicenseCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn SiteId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contact;
        private System.Windows.Forms.DataGridViewTextBoxColumn Company;
        private System.Windows.Forms.DataGridViewTextBoxColumn Important;
        private System.Windows.Forms.DataGridViewTextBoxColumn PhoneNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmailAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZipCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn OtherInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
        private System.Windows.Forms.DataGridViewTextBoxColumn IpAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateEnter;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateActiv;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateLastAcc;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateLastWebService;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn HiddenInfo;
    }
}