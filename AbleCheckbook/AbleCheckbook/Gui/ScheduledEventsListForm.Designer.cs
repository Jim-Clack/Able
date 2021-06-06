namespace AbleCheckbook.Gui
{
    partial class ScheduledEventsListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduledEventsListForm));
            this.dataGridViewEvents = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Payee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Due = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Memo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastPosting = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAddNew = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewEvents
            // 
            this.dataGridViewEvents.AllowUserToAddRows = false;
            this.dataGridViewEvents.AllowUserToDeleteRows = false;
            this.dataGridViewEvents.AllowUserToResizeRows = false;
            this.dataGridViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEvents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Payee,
            this.Amount,
            this.Status,
            this.Due,
            this.Memo,
            this.LastPosting});
            this.dataGridViewEvents.Location = new System.Drawing.Point(13, 17);
            this.dataGridViewEvents.MultiSelect = false;
            this.dataGridViewEvents.Name = "dataGridViewEvents";
            this.dataGridViewEvents.ReadOnly = true;
            this.dataGridViewEvents.RowHeadersVisible = false;
            this.dataGridViewEvents.RowTemplate.Height = 24;
            this.dataGridViewEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEvents.Size = new System.Drawing.Size(506, 249);
            this.dataGridViewEvents.TabIndex = 0;
            this.dataGridViewEvents.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEvents_CellClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Payee
            // 
            this.Payee.DataPropertyName = "Payee";
            this.Payee.HeaderText = "Payee";
            this.Payee.Name = "Payee";
            this.Payee.ReadOnly = true;
            this.Payee.Width = 120;
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "Amount";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            this.Amount.Width = 88;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.Width = 88;
            // 
            // Due
            // 
            this.Due.DataPropertyName = "Due";
            this.Due.HeaderText = "Due";
            this.Due.Name = "Due";
            this.Due.ReadOnly = true;
            this.Due.Width = 200;
            // 
            // Memo
            // 
            this.Memo.DataPropertyName = "Memo";
            this.Memo.HeaderText = "Memo";
            this.Memo.Name = "Memo";
            this.Memo.ReadOnly = true;
            this.Memo.Width = 200;
            // 
            // LastPosting
            // 
            this.LastPosting.DataPropertyName = "LastPosting";
            this.LastPosting.HeaderText = "Last Posting";
            this.LastPosting.Name = "LastPosting";
            this.LastPosting.ReadOnly = true;
            // 
            // buttonAddNew
            // 
            this.buttonAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddNew.Location = new System.Drawing.Point(13, 280);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new System.Drawing.Size(105, 32);
            this.buttonAddNew.TabIndex = 1;
            this.buttonAddNew.Text = "Add New";
            this.buttonAddNew.UseVisualStyleBackColor = true;
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(414, 280);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(105, 32);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ScheduledEventsListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(534, 328);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonAddNew);
            this.Controls.Add(this.dataGridViewEvents);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScheduledEventsListForm";
            this.Text = "Scheduled Events";
            this.Load += new System.EventHandler(this.ScheduledEventsListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewEvents;
        private System.Windows.Forms.Button buttonAddNew;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Payee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Due;
        private System.Windows.Forms.DataGridViewTextBoxColumn Memo;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastPosting;
    }
}