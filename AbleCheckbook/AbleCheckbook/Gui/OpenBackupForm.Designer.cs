namespace AbleCheckbook.Gui
{
    partial class OpenBackupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenBackupForm));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModPayee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumEntries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Last30Days = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Last90Days = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ThisYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SchedEvnts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.Ok,
            this.SaveDate,
            this.ModifDate,
            this.LastModPayee,
            this.LastModAmount,
            this.NumEntries,
            this.Last30Days,
            this.Last90Days,
            this.ThisYear,
            this.SchedEvnts,
            this.Bytes,
            this.Score,
            this.Path});
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 24;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // FileName
            // 
            this.FileName.DataPropertyName = "FileName";
            resources.ApplyResources(this.FileName, "FileName");
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            // 
            // Ok
            // 
            this.Ok.DataPropertyName = "LooksOkay";
            resources.ApplyResources(this.Ok, "Ok");
            this.Ok.Name = "Ok";
            this.Ok.ReadOnly = true;
            // 
            // SaveDate
            // 
            this.SaveDate.DataPropertyName = "SaveDate";
            resources.ApplyResources(this.SaveDate, "SaveDate");
            this.SaveDate.Name = "SaveDate";
            this.SaveDate.ReadOnly = true;
            // 
            // ModifDate
            // 
            this.ModifDate.DataPropertyName = "ModifDate";
            resources.ApplyResources(this.ModifDate, "ModifDate");
            this.ModifDate.Name = "ModifDate";
            this.ModifDate.ReadOnly = true;
            // 
            // LastModPayee
            // 
            this.LastModPayee.DataPropertyName = "LastModPayee";
            resources.ApplyResources(this.LastModPayee, "LastModPayee");
            this.LastModPayee.Name = "LastModPayee";
            this.LastModPayee.ReadOnly = true;
            // 
            // LastModAmount
            // 
            this.LastModAmount.DataPropertyName = "LastModAmount";
            resources.ApplyResources(this.LastModAmount, "LastModAmount");
            this.LastModAmount.Name = "LastModAmount";
            this.LastModAmount.ReadOnly = true;
            // 
            // NumEntries
            // 
            this.NumEntries.DataPropertyName = "EntriesTotal";
            resources.ApplyResources(this.NumEntries, "NumEntries");
            this.NumEntries.Name = "NumEntries";
            this.NumEntries.ReadOnly = true;
            // 
            // Last30Days
            // 
            this.Last30Days.DataPropertyName = "EntriesLast30Days";
            resources.ApplyResources(this.Last30Days, "Last30Days");
            this.Last30Days.Name = "Last30Days";
            this.Last30Days.ReadOnly = true;
            // 
            // Last90Days
            // 
            this.Last90Days.DataPropertyName = "EntriesLast90Days";
            resources.ApplyResources(this.Last90Days, "Last90Days");
            this.Last90Days.Name = "Last90Days";
            this.Last90Days.ReadOnly = true;
            // 
            // ThisYear
            // 
            this.ThisYear.DataPropertyName = "EntriesThisYear";
            resources.ApplyResources(this.ThisYear, "ThisYear");
            this.ThisYear.Name = "ThisYear";
            this.ThisYear.ReadOnly = true;
            // 
            // SchedEvnts
            // 
            this.SchedEvnts.DataPropertyName = "ScheduledEvents";
            resources.ApplyResources(this.SchedEvnts, "SchedEvnts");
            this.SchedEvnts.Name = "SchedEvnts";
            this.SchedEvnts.ReadOnly = true;
            // 
            // Bytes
            // 
            this.Bytes.DataPropertyName = "FileSizeBytes";
            resources.ApplyResources(this.Bytes, "Bytes");
            this.Bytes.Name = "Bytes";
            this.Bytes.ReadOnly = true;
            // 
            // Score
            // 
            this.Score.DataPropertyName = "Score";
            resources.ApplyResources(this.Score, "Score");
            this.Score.Name = "Score";
            this.Score.ReadOnly = true;
            // 
            // Path
            // 
            this.Path.DataPropertyName = "Path";
            resources.ApplyResources(this.Path, "Path");
            this.Path.Name = "Path";
            this.Path.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // OpenBackupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView);
            this.Name = "OpenBackupForm";
            this.Load += new System.EventHandler(this.OpenBackupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModPayee;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumEntries;
        private System.Windows.Forms.DataGridViewTextBoxColumn Last30Days;
        private System.Windows.Forms.DataGridViewTextBoxColumn Last90Days;
        private System.Windows.Forms.DataGridViewTextBoxColumn ThisYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn SchedEvnts;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bytes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Score;
        private System.Windows.Forms.DataGridViewTextBoxColumn Path;
    }
}