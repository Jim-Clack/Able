namespace AbleCheckbook.Gui
{
    partial class PreferencesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
            this.textBoxBaseDir = new System.Windows.Forms.TextBox();
            this.labelBaseDir = new System.Windows.Forms.Label();
            this.buttonBrowseDb = new System.Windows.Forms.Button();
            this.labelSchedEventDays = new System.Windows.Forms.Label();
            this.labelLogLevel = new System.Windows.Forms.Label();
            this.checkBoxReconcileNote = new System.Windows.Forms.CheckBox();
            this.checkBoxYearEndNote = new System.Windows.Forms.CheckBox();
            this.numericUpDownDays = new System.Windows.Forms.NumericUpDown();
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelBadDirectory = new System.Windows.Forms.Label();
            this.checkBoxTwoColumns = new System.Windows.Forms.CheckBox();
            this.checkBoxHighVisibility = new System.Windows.Forms.CheckBox();
            this.buttonBrowseBackups = new System.Windows.Forms.Button();
            this.labelBackupsDir = new System.Windows.Forms.Label();
            this.textBoxBackupsDir = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDays)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxBaseDir
            // 
            this.textBoxBaseDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBaseDir.Location = new System.Drawing.Point(135, 28);
            this.textBoxBaseDir.Name = "textBoxBaseDir";
            this.textBoxBaseDir.Size = new System.Drawing.Size(337, 22);
            this.textBoxBaseDir.TabIndex = 0;
            this.textBoxBaseDir.Leave += new System.EventHandler(this.textBoxBaseDir_Leave);
            this.textBoxBaseDir.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxBaseDir_Validating);
            // 
            // labelBaseDir
            // 
            this.labelBaseDir.AutoSize = true;
            this.labelBaseDir.Location = new System.Drawing.Point(13, 32);
            this.labelBaseDir.Name = "labelBaseDir";
            this.labelBaseDir.Size = new System.Drawing.Size(105, 17);
            this.labelBaseDir.TabIndex = 1;
            this.labelBaseDir.Text = "Base Directory:";
            // 
            // buttonBrowseDb
            // 
            this.buttonBrowseDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseDb.Location = new System.Drawing.Point(485, 25);
            this.buttonBrowseDb.Name = "buttonBrowseDb";
            this.buttonBrowseDb.Size = new System.Drawing.Size(104, 32);
            this.buttonBrowseDb.TabIndex = 1;
            this.buttonBrowseDb.Text = "Change";
            this.buttonBrowseDb.UseVisualStyleBackColor = true;
            this.buttonBrowseDb.Click += new System.EventHandler(this.buttonBrowseDb_Click);
            // 
            // labelSchedEventDays
            // 
            this.labelSchedEventDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSchedEventDays.AutoSize = true;
            this.labelSchedEventDays.Location = new System.Drawing.Point(13, 114);
            this.labelSchedEventDays.Name = "labelSchedEventDays";
            this.labelSchedEventDays.Size = new System.Drawing.Size(435, 17);
            this.labelSchedEventDays.TabIndex = 3;
            this.labelSchedEventDays.Text = "Number of days in advance to post scheduled events to checkbook:";
            // 
            // labelLogLevel
            // 
            this.labelLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLogLevel.AutoSize = true;
            this.labelLogLevel.Location = new System.Drawing.Point(13, 147);
            this.labelLogLevel.Name = "labelLogLevel";
            this.labelLogLevel.Size = new System.Drawing.Size(427, 17);
            this.labelLogLevel.TabIndex = 4;
            this.labelLogLevel.Text = "Log Level (Trace = Detailed, Diag = Normal, Warn = Smaller Logs)";
            // 
            // checkBoxReconcileNote
            // 
            this.checkBoxReconcileNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxReconcileNote.AutoSize = true;
            this.checkBoxReconcileNote.Location = new System.Drawing.Point(19, 179);
            this.checkBoxReconcileNote.Name = "checkBoxReconcileNote";
            this.checkBoxReconcileNote.Size = new System.Drawing.Size(411, 21);
            this.checkBoxReconcileNote.TabIndex = 4;
            this.checkBoxReconcileNote.Text = "Display the Reconcile Overdue notification when appropriate";
            this.checkBoxReconcileNote.UseVisualStyleBackColor = true;
            this.checkBoxReconcileNote.CheckedChanged += new System.EventHandler(this.checkBoxReconcileNote_CheckedChanged);
            // 
            // checkBoxYearEndNote
            // 
            this.checkBoxYearEndNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxYearEndNote.AutoSize = true;
            this.checkBoxYearEndNote.Location = new System.Drawing.Point(19, 208);
            this.checkBoxYearEndNote.Name = "checkBoxYearEndNote";
            this.checkBoxYearEndNote.Size = new System.Drawing.Size(445, 21);
            this.checkBoxYearEndNote.TabIndex = 5;
            this.checkBoxYearEndNote.Text = "Display the Year-End Wrap-Up Due notification When appropriate";
            this.checkBoxYearEndNote.UseVisualStyleBackColor = true;
            // 
            // numericUpDownDays
            // 
            this.numericUpDownDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDays.Location = new System.Drawing.Point(464, 112);
            this.numericUpDownDays.Name = "numericUpDownDays";
            this.numericUpDownDays.Size = new System.Drawing.Size(124, 22);
            this.numericUpDownDays.TabIndex = 2;
            this.numericUpDownDays.ValueChanged += new System.EventHandler(this.numericUpDownDays_ValueChanged);
            // 
            // comboBoxLogLevel
            // 
            this.comboBoxLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLogLevel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxLogLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Location = new System.Drawing.Point(464, 145);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(124, 24);
            this.comboBoxLogLevel.TabIndex = 3;
            this.comboBoxLogLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxLogLevel_SelectedIndexChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(485, 272);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(103, 32);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelBadDirectory
            // 
            this.labelBadDirectory.AutoSize = true;
            this.labelBadDirectory.ForeColor = System.Drawing.Color.Red;
            this.labelBadDirectory.Location = new System.Drawing.Point(128, 8);
            this.labelBadDirectory.Name = "labelBadDirectory";
            this.labelBadDirectory.Size = new System.Drawing.Size(278, 17);
            this.labelBadDirectory.TabIndex = 7;
            this.labelBadDirectory.Text = "Illegal Directory Path Specified, Reverted...";
            this.labelBadDirectory.Visible = false;
            // 
            // checkBoxTwoColumns
            // 
            this.checkBoxTwoColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxTwoColumns.AutoSize = true;
            this.checkBoxTwoColumns.Location = new System.Drawing.Point(19, 237);
            this.checkBoxTwoColumns.Name = "checkBoxTwoColumns";
            this.checkBoxTwoColumns.Size = new System.Drawing.Size(474, 21);
            this.checkBoxTwoColumns.TabIndex = 8;
            this.checkBoxTwoColumns.Text = "Display amounts in two columns (Debit/Credit) instead of one (Amount)";
            this.checkBoxTwoColumns.UseVisualStyleBackColor = true;
            this.checkBoxTwoColumns.CheckedChanged += new System.EventHandler(this.checkBoxTwoColumns_CheckedChanged);
            // 
            // checkBoxHighVisibility
            // 
            this.checkBoxHighVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxHighVisibility.AutoSize = true;
            this.checkBoxHighVisibility.Location = new System.Drawing.Point(19, 266);
            this.checkBoxHighVisibility.Name = "checkBoxHighVisibility";
            this.checkBoxHighVisibility.Size = new System.Drawing.Size(207, 21);
            this.checkBoxHighVisibility.TabIndex = 9;
            this.checkBoxHighVisibility.Text = "High Visibility - Larger Fonts";
            this.checkBoxHighVisibility.UseVisualStyleBackColor = true;
            this.checkBoxHighVisibility.CheckedChanged += new System.EventHandler(this.checkBoxHighVisibility_CheckedChanged);
            // 
            // buttonBrowseBackups
            // 
            this.buttonBrowseBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseBackups.Location = new System.Drawing.Point(485, 67);
            this.buttonBrowseBackups.Name = "buttonBrowseBackups";
            this.buttonBrowseBackups.Size = new System.Drawing.Size(104, 32);
            this.buttonBrowseBackups.TabIndex = 12;
            this.buttonBrowseBackups.Text = "Change";
            this.buttonBrowseBackups.UseVisualStyleBackColor = true;
            this.buttonBrowseBackups.Click += new System.EventHandler(this.buttonBrowseBackups_Click);
            // 
            // labelBackupsDir
            // 
            this.labelBackupsDir.AutoSize = true;
            this.labelBackupsDir.Location = new System.Drawing.Point(13, 74);
            this.labelBackupsDir.Name = "labelBackupsDir";
            this.labelBackupsDir.Size = new System.Drawing.Size(116, 17);
            this.labelBackupsDir.TabIndex = 11;
            this.labelBackupsDir.Text = "Weekly Backups:";
            // 
            // textBoxBackupsDir
            // 
            this.textBoxBackupsDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBackupsDir.Location = new System.Drawing.Point(135, 70);
            this.textBoxBackupsDir.Name = "textBoxBackupsDir";
            this.textBoxBackupsDir.Size = new System.Drawing.Size(337, 22);
            this.textBoxBackupsDir.TabIndex = 10;
            this.textBoxBackupsDir.Leave += new System.EventHandler(this.textBoxBackupsDir_Leave);
            this.textBoxBackupsDir.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxBackupsDir_Validating);
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 320);
            this.Controls.Add(this.buttonBrowseBackups);
            this.Controls.Add(this.textBoxBackupsDir);
            this.Controls.Add(this.checkBoxHighVisibility);
            this.Controls.Add(this.checkBoxTwoColumns);
            this.Controls.Add(this.labelBadDirectory);
            this.Controls.Add(this.comboBoxLogLevel);
            this.Controls.Add(this.numericUpDownDays);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.checkBoxYearEndNote);
            this.Controls.Add(this.checkBoxReconcileNote);
            this.Controls.Add(this.buttonBrowseDb);
            this.Controls.Add(this.labelBaseDir);
            this.Controls.Add(this.textBoxBaseDir);
            this.Controls.Add(this.labelLogLevel);
            this.Controls.Add(this.labelSchedEventDays);
            this.Controls.Add(this.labelBackupsDir);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(680, 380);
            this.MinimumSize = new System.Drawing.Size(622, 362);
            this.Name = "PreferencesForm";
            this.Text = "PreferencesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreferencesForm_FormClosing);
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBaseDir;
        private System.Windows.Forms.Label labelBaseDir;
        private System.Windows.Forms.Button buttonBrowseDb;
        private System.Windows.Forms.Label labelSchedEventDays;
        private System.Windows.Forms.Label labelLogLevel;
        private System.Windows.Forms.CheckBox checkBoxReconcileNote;
        private System.Windows.Forms.CheckBox checkBoxYearEndNote;
        private System.Windows.Forms.NumericUpDown numericUpDownDays;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelBadDirectory;
        private System.Windows.Forms.CheckBox checkBoxTwoColumns;
        private System.Windows.Forms.CheckBox checkBoxHighVisibility;
        private System.Windows.Forms.Button buttonBrowseBackups;
        private System.Windows.Forms.Label labelBackupsDir;
        private System.Windows.Forms.TextBox textBoxBackupsDir;
    }
}