using AbleCheckbook.Logic;
using System.Drawing;

namespace AbleCheckbook
{
    partial class MainScreen
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.BgWorkerThread = new System.ComponentModel.BackgroundWorker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IsChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DateOfTransaction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CheckNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Payee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Debit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Credit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsCleared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateCleared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Memo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifiedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewEntryRow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Entry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Color = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntryBeforeEdit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowSplits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newAcctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAcctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAcctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acctSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importQifToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportQifToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.renamePayeeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byPayeeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byCheckNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byReconcileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.itemizeSplitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scheduledTransactionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.reconcileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yearEndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.categoryReportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.printRegisterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.activateStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDueNotice = new System.Windows.Forms.ToolStripMenuItem();
            this.superToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userMgmtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrintRegister = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNewEntry = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteEntry = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBoxSearchForPayee = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonSearchMemo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonScheduled = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCategoryReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPreferences = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBoxHelp = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripLabel();
            this.textBoxPrevReconBalance = new System.Windows.Forms.TextBox();
            this.textBoxThisReconBalance = new System.Windows.Forms.TextBox();
            this.dateTimePickerPrevRecon = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerThisRecon = new System.Windows.Forms.DateTimePicker();
            this.buttonReconcileTips = new System.Windows.Forms.Button();
            this.buttonAllDone = new System.Windows.Forms.Button();
            this.textBoxReconDisparity = new System.Windows.Forms.TextBox();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.labelReconDisparity = new System.Windows.Forms.Label();
            this.buttonAbandonReconcile = new System.Windows.Forms.Button();
            this.labelLastClosing = new System.Windows.Forms.Label();
            this.labelThisClosing = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // BgWorkerThread
            // 
            this.BgWorkerThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgWorkerThread_DoWork);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsChecked,
            this.DateOfTransaction,
            this.CheckNumber,
            this.Payee,
            this.Category,
            this.Amount,
            this.Debit,
            this.Credit,
            this.Balance,
            this.IsCleared,
            this.DateCleared,
            this.Memo,
            this.Status,
            this.DateModified,
            this.ModifiedBy,
            this.NewEntryRow,
            this.Entry,
            this.Color,
            this.EntryBeforeEdit,
            this.Id,
            this.ShowSplits,
            this.BankInfo});
            this.dataGridView1.Location = new System.Drawing.Point(12, 63);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(596, 296);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            this.dataGridView1.RowHeightChanged += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_RowHeightChanged);
            this.dataGridView1.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridView1_Paint);
            this.dataGridView1.Enter += new System.EventHandler(this.dataGridView1_Enter);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // IsChecked
            // 
            this.IsChecked.DataPropertyName = "IsChecked";
            this.IsChecked.HeaderText = "X";
            this.IsChecked.Name = "IsChecked";
            this.IsChecked.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsChecked.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsChecked.Visible = false;
            this.IsChecked.Width = 30;
            // 
            // DateOfTransaction
            // 
            this.DateOfTransaction.DataPropertyName = "DateOfTransaction";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateOfTransaction.DefaultCellStyle = dataGridViewCellStyle2;
            this.DateOfTransaction.HeaderText = "Date";
            this.DateOfTransaction.Name = "DateOfTransaction";
            this.DateOfTransaction.ReadOnly = true;
            this.DateOfTransaction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DateOfTransaction.Width = 78;
            // 
            // CheckNumber
            // 
            this.CheckNumber.DataPropertyName = "CheckNumber";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckNumber.DefaultCellStyle = dataGridViewCellStyle3;
            this.CheckNumber.HeaderText = "Ck#";
            this.CheckNumber.Name = "CheckNumber";
            this.CheckNumber.ReadOnly = true;
            this.CheckNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CheckNumber.Width = 42;
            // 
            // Payee
            // 
            this.Payee.DataPropertyName = "Payee";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Payee.DefaultCellStyle = dataGridViewCellStyle4;
            this.Payee.HeaderText = "Payee";
            this.Payee.Name = "Payee";
            this.Payee.ReadOnly = true;
            this.Payee.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Category
            // 
            this.Category.DataPropertyName = "Category";
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Category.DefaultCellStyle = dataGridViewCellStyle5;
            this.Category.HeaderText = "Category";
            this.Category.Name = "Category";
            this.Category.ReadOnly = true;
            this.Category.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Category.Width = 84;
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Amount.DefaultCellStyle = dataGridViewCellStyle6;
            this.Amount.HeaderText = "Amount";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            this.Amount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Amount.Width = 84;
            // 
            // Debit
            // 
            this.Debit.DataPropertyName = "Debit";
            this.Debit.HeaderText = "Debit";
            this.Debit.Name = "Debit";
            this.Debit.ReadOnly = true;
            this.Debit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Debit.Width = 84;
            // 
            // Credit
            // 
            this.Credit.DataPropertyName = "Credit";
            this.Credit.HeaderText = "Credit";
            this.Credit.Name = "Credit";
            this.Credit.ReadOnly = true;
            this.Credit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Credit.Width = 84;
            // 
            // Balance
            // 
            this.Balance.DataPropertyName = "Balance";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Balance.DefaultCellStyle = dataGridViewCellStyle7;
            this.Balance.HeaderText = "Balance";
            this.Balance.Name = "Balance";
            this.Balance.ReadOnly = true;
            this.Balance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Balance.Width = 90;
            // 
            // IsCleared
            // 
            this.IsCleared.DataPropertyName = "IsCleared";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsCleared.DefaultCellStyle = dataGridViewCellStyle8;
            this.IsCleared.HeaderText = "x";
            this.IsCleared.Name = "IsCleared";
            this.IsCleared.ReadOnly = true;
            this.IsCleared.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IsCleared.Width = 20;
            // 
            // DateCleared
            // 
            this.DateCleared.DataPropertyName = "DateCleared";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateCleared.DefaultCellStyle = dataGridViewCellStyle9;
            this.DateCleared.HeaderText = "Cleared";
            this.DateCleared.Name = "DateCleared";
            this.DateCleared.ReadOnly = true;
            this.DateCleared.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DateCleared.Width = 78;
            // 
            // Memo
            // 
            this.Memo.DataPropertyName = "Memo";
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Memo.DefaultCellStyle = dataGridViewCellStyle10;
            this.Memo.HeaderText = "Memo";
            this.Memo.Name = "Memo";
            this.Memo.ReadOnly = true;
            this.Memo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Memo.Width = 80;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status.DefaultCellStyle = dataGridViewCellStyle11;
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Status.Visible = false;
            // 
            // DateModified
            // 
            this.DateModified.DataPropertyName = "DateModified";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateModified.DefaultCellStyle = dataGridViewCellStyle12;
            this.DateModified.HeaderText = "Modified";
            this.DateModified.Name = "DateModified";
            this.DateModified.ReadOnly = true;
            this.DateModified.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DateModified.Width = 78;
            // 
            // ModifiedBy
            // 
            this.ModifiedBy.DataPropertyName = "ModifiedBy";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModifiedBy.DefaultCellStyle = dataGridViewCellStyle13;
            this.ModifiedBy.HeaderText = "By";
            this.ModifiedBy.Name = "ModifiedBy";
            this.ModifiedBy.ReadOnly = true;
            this.ModifiedBy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ModifiedBy.Width = 50;
            // 
            // NewEntryRow
            // 
            this.NewEntryRow.DataPropertyName = "NewEntryRow";
            this.NewEntryRow.HeaderText = "NewEntryRow";
            this.NewEntryRow.Name = "NewEntryRow";
            this.NewEntryRow.ReadOnly = true;
            this.NewEntryRow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.NewEntryRow.Visible = false;
            // 
            // Entry
            // 
            this.Entry.DataPropertyName = "Entry";
            this.Entry.HeaderText = "Entry";
            this.Entry.Name = "Entry";
            this.Entry.ReadOnly = true;
            this.Entry.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Entry.Visible = false;
            // 
            // Color
            // 
            this.Color.DataPropertyName = "Color";
            this.Color.HeaderText = "Color";
            this.Color.Name = "Color";
            this.Color.ReadOnly = true;
            this.Color.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Color.Visible = false;
            // 
            // EntryBeforeEdit
            // 
            this.EntryBeforeEdit.DataPropertyName = "EntryBeforeEdit";
            this.EntryBeforeEdit.HeaderText = "EntryBeforeEdit";
            this.EntryBeforeEdit.Name = "EntryBeforeEdit";
            this.EntryBeforeEdit.ReadOnly = true;
            this.EntryBeforeEdit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.EntryBeforeEdit.Visible = false;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            this.Id.Width = 250;
            // 
            // ShowSplits
            // 
            this.ShowSplits.DataPropertyName = "ShowSplits";
            this.ShowSplits.HeaderText = "ShowSplits";
            this.ShowSplits.Name = "ShowSplits";
            this.ShowSplits.ReadOnly = true;
            this.ShowSplits.Visible = false;
            // 
            // BankInfo
            // 
            this.BankInfo.DataPropertyName = "BankInfo";
            this.BankInfo.HeaderText = "BankInfo";
            this.BankInfo.Name = "BankInfo";
            this.BankInfo.ReadOnly = true;
            this.BankInfo.Width = 80;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menuStrip1.BackgroundImage")));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.toolStripMenuItemDueNotice,
            this.superToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(622, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newAcctToolStripMenuItem,
            this.openAcctToolStripMenuItem,
            this.saveAcctToolStripMenuItem,
            this.openBackupToolStripMenuItem,
            this.acctSettingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.importQifToolStripMenuItem,
            this.exportQifToolStripMenuItem,
            this.toolStripSeparator2,
            this.importCsvToolStripMenuItem,
            this.exportCsvToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newAcctToolStripMenuItem
            // 
            this.newAcctToolStripMenuItem.Name = "newAcctToolStripMenuItem";
            this.newAcctToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.newAcctToolStripMenuItem.Text = "&New Acct";
            this.newAcctToolStripMenuItem.Click += new System.EventHandler(this.newAcctToolStripMenuItem_Click);
            // 
            // openAcctToolStripMenuItem
            // 
            this.openAcctToolStripMenuItem.Name = "openAcctToolStripMenuItem";
            this.openAcctToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.openAcctToolStripMenuItem.Text = "&Open Acct";
            this.openAcctToolStripMenuItem.Click += new System.EventHandler(this.openAcctToolStripMenuItem_Click);
            // 
            // saveAcctToolStripMenuItem
            // 
            this.saveAcctToolStripMenuItem.Name = "saveAcctToolStripMenuItem";
            this.saveAcctToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.saveAcctToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveAcctToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.saveAcctToolStripMenuItem.Text = "&Save Acct";
            this.saveAcctToolStripMenuItem.Click += new System.EventHandler(this.saveAcctToolStripMenuItem_Click);
            // 
            // openBackupToolStripMenuItem
            // 
            this.openBackupToolStripMenuItem.Name = "openBackupToolStripMenuItem";
            this.openBackupToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.openBackupToolStripMenuItem.Text = "Open &Backup File";
            this.openBackupToolStripMenuItem.Click += new System.EventHandler(this.openBackupToolStripMenuItem_Click);
            // 
            // acctSettingsToolStripMenuItem
            // 
            this.acctSettingsToolStripMenuItem.Name = "acctSettingsToolStripMenuItem";
            this.acctSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.acctSettingsToolStripMenuItem.Text = "&Acct Settings";
            this.acctSettingsToolStripMenuItem.Click += new System.EventHandler(this.acctSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // importQifToolStripMenuItem
            // 
            this.importQifToolStripMenuItem.Name = "importQifToolStripMenuItem";
            this.importQifToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.importQifToolStripMenuItem.Text = "Import &QIF";
            this.importQifToolStripMenuItem.Click += new System.EventHandler(this.importQifToolStripMenuItem_Click);
            // 
            // exportQifToolStripMenuItem
            // 
            this.exportQifToolStripMenuItem.Name = "exportQifToolStripMenuItem";
            this.exportQifToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.exportQifToolStripMenuItem.Text = "&Export QIF";
            this.exportQifToolStripMenuItem.Click += new System.EventHandler(this.exportQifToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
            // 
            // importCsvToolStripMenuItem
            // 
            this.importCsvToolStripMenuItem.Name = "importCsvToolStripMenuItem";
            this.importCsvToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.importCsvToolStripMenuItem.Text = "Import &CSV";
            this.importCsvToolStripMenuItem.Click += new System.EventHandler(this.importCsvToolStripMenuItem_Click);
            // 
            // exportCsvToolStripMenuItem
            // 
            this.exportCsvToolStripMenuItem.Name = "exportCsvToolStripMenuItem";
            this.exportCsvToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.exportCsvToolStripMenuItem.Text = "Expo&rt CSV";
            this.exportCsvToolStripMenuItem.Click += new System.EventHandler(this.exportCsvToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(196, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(199, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator4,
            this.copyToolStripMenuItem,
            this.newEntryToolStripMenuItem,
            this.deleteEntryToolStripMenuItem,
            this.toolStripSeparator5,
            this.renamePayeeToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.copyToolStripMenuItem.Text = "&Copy Entry";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // newEntryToolStripMenuItem
            // 
            this.newEntryToolStripMenuItem.Name = "newEntryToolStripMenuItem";
            this.newEntryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newEntryToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.newEntryToolStripMenuItem.Text = "&New Entry";
            this.newEntryToolStripMenuItem.Click += new System.EventHandler(this.newEntryToolStripMenuItem_Click);
            // 
            // deleteEntryToolStripMenuItem
            // 
            this.deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
            this.deleteEntryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.deleteEntryToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.deleteEntryToolStripMenuItem.Text = "&Delete Entry";
            this.deleteEntryToolStripMenuItem.Click += new System.EventHandler(this.deleteEntryToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(213, 6);
            // 
            // renamePayeeToolStripMenuItem
            // 
            this.renamePayeeToolStripMenuItem.Name = "renamePayeeToolStripMenuItem";
            this.renamePayeeToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.renamePayeeToolStripMenuItem.Text = "Rename &Payee";
            this.renamePayeeToolStripMenuItem.Click += new System.EventHandler(this.renamePayeeToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.searchToolStripMenuItem.Text = "&Search Entries";
            this.searchToolStripMenuItem.ToolTipText = "Search Entries";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byDateToolStripMenuItem,
            this.byPayeeToolStripMenuItem,
            this.byCategoryToolStripMenuItem,
            this.byCheckNumberToolStripMenuItem,
            this.byMatchToolStripMenuItem,
            this.byReconcileToolStripMenuItem,
            this.toolStripSeparator13,
            this.itemizeSplitsToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // byDateToolStripMenuItem
            // 
            this.byDateToolStripMenuItem.Name = "byDateToolStripMenuItem";
            this.byDateToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byDateToolStripMenuItem.Text = "Sort By &Date";
            this.byDateToolStripMenuItem.Click += new System.EventHandler(this.byDateToolStripMenuItem_Click);
            // 
            // byPayeeToolStripMenuItem
            // 
            this.byPayeeToolStripMenuItem.Name = "byPayeeToolStripMenuItem";
            this.byPayeeToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byPayeeToolStripMenuItem.Text = "Sort By &Payee";
            this.byPayeeToolStripMenuItem.Click += new System.EventHandler(this.byPayeeToolStripMenuItem_Click);
            // 
            // byCategoryToolStripMenuItem
            // 
            this.byCategoryToolStripMenuItem.Name = "byCategoryToolStripMenuItem";
            this.byCategoryToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byCategoryToolStripMenuItem.Text = "Sort By &Category";
            this.byCategoryToolStripMenuItem.Click += new System.EventHandler(this.byCategoryToolStripMenuItem_Click);
            // 
            // byCheckNumberToolStripMenuItem
            // 
            this.byCheckNumberToolStripMenuItem.Name = "byCheckNumberToolStripMenuItem";
            this.byCheckNumberToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byCheckNumberToolStripMenuItem.Text = "Sort By Check &Number";
            this.byCheckNumberToolStripMenuItem.Click += new System.EventHandler(this.byCheckNumberToolStripMenuItem_Click);
            // 
            // byMatchToolStripMenuItem
            // 
            this.byMatchToolStripMenuItem.Enabled = false;
            this.byMatchToolStripMenuItem.Name = "byMatchToolStripMenuItem";
            this.byMatchToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byMatchToolStripMenuItem.Text = "Sort By Match";
            // 
            // byReconcileToolStripMenuItem
            // 
            this.byReconcileToolStripMenuItem.Enabled = false;
            this.byReconcileToolStripMenuItem.Name = "byReconcileToolStripMenuItem";
            this.byReconcileToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.byReconcileToolStripMenuItem.Text = "Sort By Reconcile";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(229, 6);
            // 
            // itemizeSplitsToolStripMenuItem
            // 
            this.itemizeSplitsToolStripMenuItem.Name = "itemizeSplitsToolStripMenuItem";
            this.itemizeSplitsToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.itemizeSplitsToolStripMenuItem.Text = "&Itemize Splits";
            this.itemizeSplitsToolStripMenuItem.Click += new System.EventHandler(this.itemizeSplitsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scheduledTransactionsToolStripMenuItem,
            this.toolStripSeparator6,
            this.reconcileToolStripMenuItem,
            this.yearEndToolStripMenuItem,
            this.toolStripSeparator7,
            this.categoryReportToolStripMenuItem1,
            this.printRegisterToolStripMenuItem1});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(56, 24);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // scheduledTransactionsToolStripMenuItem
            // 
            this.scheduledTransactionsToolStripMenuItem.Name = "scheduledTransactionsToolStripMenuItem";
            this.scheduledTransactionsToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.scheduledTransactionsToolStripMenuItem.Text = "&Scheduled Events";
            this.scheduledTransactionsToolStripMenuItem.Click += new System.EventHandler(this.scheduledTransactionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(219, 6);
            // 
            // reconcileToolStripMenuItem
            // 
            this.reconcileToolStripMenuItem.Name = "reconcileToolStripMenuItem";
            this.reconcileToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.reconcileToolStripMenuItem.Text = "&Reconcile (Monthly)";
            this.reconcileToolStripMenuItem.Click += new System.EventHandler(this.reconcileToolStripMenuItem_Click);
            // 
            // yearEndToolStripMenuItem
            // 
            this.yearEndToolStripMenuItem.Name = "yearEndToolStripMenuItem";
            this.yearEndToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.yearEndToolStripMenuItem.Text = "&Year-End Wrap-Up";
            this.yearEndToolStripMenuItem.Click += new System.EventHandler(this.yearEndToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(219, 6);
            // 
            // categoryReportToolStripMenuItem1
            // 
            this.categoryReportToolStripMenuItem1.Name = "categoryReportToolStripMenuItem1";
            this.categoryReportToolStripMenuItem1.Size = new System.Drawing.Size(222, 26);
            this.categoryReportToolStripMenuItem1.Text = "C&ategory Report";
            this.categoryReportToolStripMenuItem1.Click += new System.EventHandler(this.categoryReportToolStripMenuItem1_Click);
            // 
            // printRegisterToolStripMenuItem1
            // 
            this.printRegisterToolStripMenuItem1.Name = "printRegisterToolStripMenuItem1";
            this.printRegisterToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printRegisterToolStripMenuItem1.Size = new System.Drawing.Size(222, 26);
            this.printRegisterToolStripMenuItem1.Text = "&Print Register";
            this.printRegisterToolStripMenuItem1.Click += new System.EventHandler(this.printRegisterToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpContentsToolStripMenuItem,
            this.toolStripSeparator14,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator8,
            this.activateStripMenuItem,
            this.diagnosticsToolStripMenuItem,
            this.adminModeToolStripMenuItem});
            this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpContentsToolStripMenuItem
            // 
            this.helpContentsToolStripMenuItem.Name = "helpContentsToolStripMenuItem";
            this.helpContentsToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.helpContentsToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.helpContentsToolStripMenuItem.Text = "&Help Contents";
            this.helpContentsToolStripMenuItem.Click += new System.EventHandler(this.helpContentsToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(187, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(187, 6);
            // 
            // activateStripMenuItem
            // 
            this.activateStripMenuItem.Name = "activateStripMenuItem";
            this.activateStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.activateStripMenuItem.Text = "Activate &License";
            this.activateStripMenuItem.Click += new System.EventHandler(this.activateStripMenuItem_Click);
            // 
            // diagnosticsToolStripMenuItem
            // 
            this.diagnosticsToolStripMenuItem.Name = "diagnosticsToolStripMenuItem";
            this.diagnosticsToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.diagnosticsToolStripMenuItem.Text = "&Diagnostics";
            this.diagnosticsToolStripMenuItem.Visible = false;
            this.diagnosticsToolStripMenuItem.Click += new System.EventHandler(this.diagnosticsToolStripMenuItem_Click_1);
            // 
            // adminModeToolStripMenuItem
            // 
            this.adminModeToolStripMenuItem.Name = "adminModeToolStripMenuItem";
            this.adminModeToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.adminModeToolStripMenuItem.Text = "Admin &Mode";
            this.adminModeToolStripMenuItem.Visible = false;
            this.adminModeToolStripMenuItem.Click += new System.EventHandler(this.adminModeToolStripMenuItem_Click);
            // 
            // toolStripMenuItemDueNotice
            // 
            this.toolStripMenuItemDueNotice.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItemDueNotice.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemDueNotice.BackgroundImage")));
            this.toolStripMenuItemDueNotice.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.toolStripMenuItemDueNotice.ForeColor = System.Drawing.Color.Red;
            this.toolStripMenuItemDueNotice.Name = "toolStripMenuItemDueNotice";
            this.toolStripMenuItemDueNotice.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
            this.toolStripMenuItemDueNotice.Padding = new System.Windows.Forms.Padding(4, 0, 12, 0);
            this.toolStripMenuItemDueNotice.Size = new System.Drawing.Size(20, 24);
            this.toolStripMenuItemDueNotice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripMenuItemDueNotice.Click += new System.EventHandler(this.toolStripMenuItemDueNotice_Click);
            // 
            // superToolStripMenuItem
            // 
            this.superToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userMgmtToolStripMenuItem,
            this.readLogToolStripMenuItem});
            this.superToolStripMenuItem.Name = "superToolStripMenuItem";
            this.superToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.superToolStripMenuItem.Text = "&Super";
            // 
            // userMgmtToolStripMenuItem
            // 
            this.userMgmtToolStripMenuItem.Name = "userMgmtToolStripMenuItem";
            this.userMgmtToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.userMgmtToolStripMenuItem.Text = "&User Mgmt";
            this.userMgmtToolStripMenuItem.Click += new System.EventHandler(this.userMgmtToolStripMenuItem_Click);
            // 
            // readLogToolStripMenuItem
            // 
            this.readLogToolStripMenuItem.Name = "readLogToolStripMenuItem";
            this.readLogToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.readLogToolStripMenuItem.Text = "&Log Analysis";
            this.readLogToolStripMenuItem.Click += new System.EventHandler(this.readLogToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonPrintRegister,
            this.toolStripSeparator9,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo,
            this.toolStripButtonCopy,
            this.toolStripButtonNewEntry,
            this.toolStripButtonDeleteEntry,
            this.toolStripSeparator11,
            this.toolStripTextBoxSearchForPayee,
            this.toolStripButtonSearchMemo,
            this.toolStripButtonScheduled,
            this.toolStripButtonCategoryReport,
            this.toolStripSeparator10,
            this.toolStripButtonPreferences,
            this.toolStripSeparator12,
            this.toolStripButtonHelp,
            this.toolStripTextBoxHelp,
            this.toolStripLabelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(622, 27);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.ToolTipText = "Save";
            // 
            // toolStripButtonPrintRegister
            // 
            this.toolStripButtonPrintRegister.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintRegister.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPrintRegister.Image")));
            this.toolStripButtonPrintRegister.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintRegister.Name = "toolStripButtonPrintRegister";
            this.toolStripButtonPrintRegister.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonPrintRegister.Text = "Print Register";
            this.toolStripButtonPrintRegister.Click += new System.EventHandler(this.toolStripButtonPrintRegister_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonUndo.Text = "Undo";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // toolStripButtonRedo
            // 
            this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRedo.Image")));
            this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRedo.Name = "toolStripButtonRedo";
            this.toolStripButtonRedo.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonRedo.Text = "Redo";
            this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
            // 
            // toolStripButtonCopy
            // 
            this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopy.Image")));
            this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopy.Name = "toolStripButtonCopy";
            this.toolStripButtonCopy.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonCopy.Text = "Copy";
            this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
            // 
            // toolStripButtonNewEntry
            // 
            this.toolStripButtonNewEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNewEntry.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNewEntry.Image")));
            this.toolStripButtonNewEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewEntry.Name = "toolStripButtonNewEntry";
            this.toolStripButtonNewEntry.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonNewEntry.Text = "toolStripButton1";
            this.toolStripButtonNewEntry.Click += new System.EventHandler(this.toolStripButtonNewEntry_Click);
            // 
            // toolStripButtonDeleteEntry
            // 
            this.toolStripButtonDeleteEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteEntry.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteEntry.Image")));
            this.toolStripButtonDeleteEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteEntry.Name = "toolStripButtonDeleteEntry";
            this.toolStripButtonDeleteEntry.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonDeleteEntry.ToolTipText = "Delete Entry";
            this.toolStripButtonDeleteEntry.Click += new System.EventHandler(this.toolStripButtonDeleteEntry_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripTextBoxSearchForPayee
            // 
            this.toolStripTextBoxSearchForPayee.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.toolStripTextBoxSearchForPayee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBoxSearchForPayee.Name = "toolStripTextBoxSearchForPayee";
            this.toolStripTextBoxSearchForPayee.Size = new System.Drawing.Size(120, 27);
            this.toolStripTextBoxSearchForPayee.ToolTipText = "Search For Payee...";
            this.toolStripTextBoxSearchForPayee.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxSearchForPayee_KeyUp);
            // 
            // toolStripButtonSearchMemo
            // 
            this.toolStripButtonSearchMemo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSearchMemo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSearchMemo.Image")));
            this.toolStripButtonSearchMemo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearchMemo.Name = "toolStripButtonSearchMemo";
            this.toolStripButtonSearchMemo.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonSearchMemo.Text = "Search Memos";
            this.toolStripButtonSearchMemo.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // toolStripButtonScheduled
            // 
            this.toolStripButtonScheduled.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonScheduled.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonScheduled.Image")));
            this.toolStripButtonScheduled.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonScheduled.Name = "toolStripButtonScheduled";
            this.toolStripButtonScheduled.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonScheduled.Text = "Scheduled Events";
            // 
            // toolStripButtonCategoryReport
            // 
            this.toolStripButtonCategoryReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCategoryReport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCategoryReport.Image")));
            this.toolStripButtonCategoryReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCategoryReport.Name = "toolStripButtonCategoryReport";
            this.toolStripButtonCategoryReport.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonCategoryReport.Text = "Category Report";
            this.toolStripButtonCategoryReport.Click += new System.EventHandler(this.toolStripButtonCategoryReport_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonPreferences
            // 
            this.toolStripButtonPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPreferences.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPreferences.Image")));
            this.toolStripButtonPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPreferences.Name = "toolStripButtonPreferences";
            this.toolStripButtonPreferences.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonPreferences.Text = "Preferences";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHelp.Image")));
            this.toolStripButtonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonHelp.Text = "Help";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // toolStripTextBoxHelp
            // 
            this.toolStripTextBoxHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBoxHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.toolStripTextBoxHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBoxHelp.Name = "toolStripTextBoxHelp";
            this.toolStripTextBoxHelp.Size = new System.Drawing.Size(120, 27);
            this.toolStripTextBoxHelp.ToolTipText = "Help Topic...";
            this.toolStripTextBoxHelp.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxHelp_KeyUp);
            // 
            // toolStripLabelStatus
            // 
            this.toolStripLabelStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLabelStatus.ForeColor = System.Drawing.Color.Red;
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Size = new System.Drawing.Size(0, 24);
            // 
            // textBoxPrevReconBalance
            // 
            this.textBoxPrevReconBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxPrevReconBalance.Location = new System.Drawing.Point(119, 366);
            this.textBoxPrevReconBalance.Name = "textBoxPrevReconBalance";
            this.textBoxPrevReconBalance.Size = new System.Drawing.Size(80, 22);
            this.textBoxPrevReconBalance.TabIndex = 8;
            this.textBoxPrevReconBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxPrevReconBalance.Enter += new System.EventHandler(this.textBoxLastBalance_Enter);
            this.textBoxPrevReconBalance.Leave += new System.EventHandler(this.textBoxLastBalance_Leave);
            // 
            // textBoxThisReconBalance
            // 
            this.textBoxThisReconBalance.AcceptsReturn = true;
            this.textBoxThisReconBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxThisReconBalance.Location = new System.Drawing.Point(119, 395);
            this.textBoxThisReconBalance.Name = "textBoxThisReconBalance";
            this.textBoxThisReconBalance.Size = new System.Drawing.Size(80, 22);
            this.textBoxThisReconBalance.TabIndex = 9;
            this.textBoxThisReconBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxThisReconBalance.Leave += new System.EventHandler(this.textBoxThisBalance_Leave);
            // 
            // dateTimePickerPrevRecon
            // 
            this.dateTimePickerPrevRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimePickerPrevRecon.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerPrevRecon.Location = new System.Drawing.Point(13, 366);
            this.dateTimePickerPrevRecon.Name = "dateTimePickerPrevRecon";
            this.dateTimePickerPrevRecon.Size = new System.Drawing.Size(100, 22);
            this.dateTimePickerPrevRecon.TabIndex = 44;
            this.dateTimePickerPrevRecon.Enter += new System.EventHandler(this.dateTimePickerLastRecon_Enter);
            // 
            // dateTimePickerThisRecon
            // 
            this.dateTimePickerThisRecon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimePickerThisRecon.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerThisRecon.Location = new System.Drawing.Point(13, 395);
            this.dateTimePickerThisRecon.Name = "dateTimePickerThisRecon";
            this.dateTimePickerThisRecon.Size = new System.Drawing.Size(100, 22);
            this.dateTimePickerThisRecon.TabIndex = 4;
            this.dateTimePickerThisRecon.ValueChanged += new System.EventHandler(this.dateTimePickerThisRecon_ValueChanged);
            this.dateTimePickerThisRecon.Leave += new System.EventHandler(this.dateTimePickerThisRecon_Leave);
            // 
            // buttonReconcileTips
            // 
            this.buttonReconcileTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReconcileTips.Enabled = false;
            this.buttonReconcileTips.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReconcileTips.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.buttonReconcileTips.Location = new System.Drawing.Point(515, 362);
            this.buttonReconcileTips.Name = "buttonReconcileTips";
            this.buttonReconcileTips.Size = new System.Drawing.Size(93, 27);
            this.buttonReconcileTips.TabIndex = 5;
            this.buttonReconcileTips.Text = "Tips";
            this.buttonReconcileTips.UseVisualStyleBackColor = true;
            this.buttonReconcileTips.Visible = false;
            this.buttonReconcileTips.Click += new System.EventHandler(this.buttonReconcileTips_Click);
            // 
            // buttonAllDone
            // 
            this.buttonAllDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAllDone.Location = new System.Drawing.Point(399, 390);
            this.buttonAllDone.Name = "buttonAllDone";
            this.buttonAllDone.Size = new System.Drawing.Size(209, 32);
            this.buttonAllDone.TabIndex = 6;
            this.buttonAllDone.Text = "Create Balance Adjustment";
            this.buttonAllDone.UseVisualStyleBackColor = true;
            this.buttonAllDone.Click += new System.EventHandler(this.buttonAllDone_Click);
            // 
            // textBoxReconDisparity
            // 
            this.textBoxReconDisparity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReconDisparity.Location = new System.Drawing.Point(399, 363);
            this.textBoxReconDisparity.Name = "textBoxReconDisparity";
            this.textBoxReconDisparity.ReadOnly = true;
            this.textBoxReconDisparity.Size = new System.Drawing.Size(103, 22);
            this.textBoxReconDisparity.TabIndex = 14;
            this.textBoxReconDisparity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxReconDisparity.Visible = false;
            // 
            // labelInstructions
            // 
            this.labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstructions.ForeColor = System.Drawing.Color.Red;
            this.labelInstructions.Location = new System.Drawing.Point(208, 368);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(386, 17);
            this.labelInstructions.TabIndex = 15;
            this.labelInstructions.Text = "Check off entries that are cleared in bank statement";
            // 
            // labelReconDisparity
            // 
            this.labelReconDisparity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReconDisparity.AutoSize = true;
            this.labelReconDisparity.Location = new System.Drawing.Point(213, 367);
            this.labelReconDisparity.Name = "labelReconDisparity";
            this.labelReconDisparity.Size = new System.Drawing.Size(175, 17);
            this.labelReconDisparity.TabIndex = 16;
            this.labelReconDisparity.Text = "Disparity (should be zero):";
            this.labelReconDisparity.Visible = false;
            // 
            // buttonAbandonReconcile
            // 
            this.buttonAbandonReconcile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbandonReconcile.Location = new System.Drawing.Point(211, 390);
            this.buttonAbandonReconcile.Name = "buttonAbandonReconcile";
            this.buttonAbandonReconcile.Size = new System.Drawing.Size(182, 32);
            this.buttonAbandonReconcile.TabIndex = 7;
            this.buttonAbandonReconcile.Text = "Abandon Reconcile";
            this.buttonAbandonReconcile.UseVisualStyleBackColor = true;
            this.buttonAbandonReconcile.Click += new System.EventHandler(this.buttonAbandonReconcile_Click);
            // 
            // labelLastClosing
            // 
            this.labelLastClosing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastClosing.AutoSize = true;
            this.labelLastClosing.Location = new System.Drawing.Point(116, 368);
            this.labelLastClosing.Name = "labelLastClosing";
            this.labelLastClosing.Size = new System.Drawing.Size(91, 17);
            this.labelLastClosing.TabIndex = 45;
            this.labelLastClosing.Text = "Prev Closing:";
            this.labelLastClosing.Visible = false;
            // 
            // labelThisClosing
            // 
            this.labelThisClosing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelThisClosing.AutoSize = true;
            this.labelThisClosing.Location = new System.Drawing.Point(116, 398);
            this.labelThisClosing.Name = "labelThisClosing";
            this.labelThisClosing.Size = new System.Drawing.Size(89, 17);
            this.labelThisClosing.TabIndex = 46;
            this.labelThisClosing.Text = "This Closing:";
            this.labelThisClosing.Visible = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.BackgroundImage")));
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxLogo.Location = new System.Drawing.Point(543, 99);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(272, 55);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 47;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Visible = false;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(622, 433);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.buttonAbandonReconcile);
            this.Controls.Add(this.textBoxReconDisparity);
            this.Controls.Add(this.buttonAllDone);
            this.Controls.Add(this.buttonReconcileTips);
            this.Controls.Add(this.dateTimePickerThisRecon);
            this.Controls.Add(this.dateTimePickerPrevRecon);
            this.Controls.Add(this.textBoxThisReconBalance);
            this.Controls.Add(this.textBoxPrevReconBalance);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelReconDisparity);
            this.Controls.Add(this.labelInstructions);
            this.Controls.Add(this.labelLastClosing);
            this.Controls.Add(this.labelThisClosing);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(632, 280);
            this.Name = "MainScreen";
            this.Opacity = 0.4D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainScreen_FormClosed);
            this.Load += new System.EventHandler(this.MainScreen_Load);
            this.ResizeEnd += new System.EventHandler(this.MainScreen_ResizeEnd);
            this.Click += new System.EventHandler(this.MainScreen_Click);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker BgWorkerThread;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newAcctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAcctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAcctToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scheduledTransactionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem reconcileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem categoryReportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activateStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintRegister;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSearchForPayee;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearchMemo;
        private System.Windows.Forms.ToolStripButton toolStripButtonScheduled;
        private System.Windows.Forms.ToolStripButton toolStripButtonCategoryReport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreferences;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStatus;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byPayeeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byCheckNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renamePayeeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDueNotice;
        private System.Windows.Forms.ToolStripMenuItem newEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewEntry;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteEntry;
        private System.Windows.Forms.ToolStripMenuItem superToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userMgmtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importQifToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportQifToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yearEndToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TextBox textBoxPrevReconBalance;
        private System.Windows.Forms.TextBox textBoxThisReconBalance;
        private System.Windows.Forms.DateTimePicker dateTimePickerPrevRecon;
        private System.Windows.Forms.DateTimePicker dateTimePickerThisRecon;
        private System.Windows.Forms.Button buttonReconcileTips;
        private System.Windows.Forms.Button buttonAllDone;
        private System.Windows.Forms.TextBox textBoxReconDisparity;
        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.Label labelReconDisparity;
        private System.Windows.Forms.Button buttonAbandonReconcile;
        private System.Windows.Forms.ToolStripMenuItem byMatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byReconcileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diagnosticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.Label labelLastClosing;
        private System.Windows.Forms.Label labelThisClosing;
        private System.Windows.Forms.ToolStripMenuItem adminModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printRegisterToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem itemizeSplitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acctSettingsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsChecked;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateOfTransaction;
        private System.Windows.Forms.DataGridViewTextBoxColumn CheckNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Payee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Debit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Credit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Balance;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsCleared;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateCleared;
        private System.Windows.Forms.DataGridViewTextBoxColumn Memo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifiedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewEntryRow;
        private System.Windows.Forms.DataGridViewTextBoxColumn Entry;
        private System.Windows.Forms.DataGridViewTextBoxColumn Color;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryBeforeEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowSplits;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankInfo;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
    }
}

