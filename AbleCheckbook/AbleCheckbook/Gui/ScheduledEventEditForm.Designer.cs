namespace AbleCheckbook.Gui
{
    partial class ScheduledEventEditForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMonthly = new System.Windows.Forms.TabPage();
            this.labelExample1 = new System.Windows.Forms.Label();
            this.listBoxDaysOfMonth1 = new System.Windows.Forms.ListBox();
            this.tabPageAnnually = new System.Windows.Forms.TabPage();
            this.labelExample2 = new System.Windows.Forms.Label();
            this.listBoxMonth2 = new System.Windows.Forms.ListBox();
            this.listBoxDaysOfMonth2 = new System.Windows.Forms.ListBox();
            this.tabPageWeekly = new System.Windows.Forms.TabPage();
            this.labelExample3 = new System.Windows.Forms.Label();
            this.listBoxDaysOfWeek3 = new System.Windows.Forms.ListBox();
            this.tabPageMonthlySsa = new System.Windows.Forms.TabPage();
            this.labelExample4 = new System.Windows.Forms.Label();
            this.listBoxNthOccurrence = new System.Windows.Forms.ListBox();
            this.listBoxDayOfWeek4 = new System.Windows.Forms.ListBox();
            this.tabPageBiWeekly = new System.Windows.Forms.TabPage();
            this.labelExample5 = new System.Windows.Forms.Label();
            this.labelNextOccurrence5 = new System.Windows.Forms.Label();
            this.dateTimePickerNextOccurrence5 = new System.Windows.Forms.DateTimePicker();
            this.listBoxDayOfWeek5 = new System.Windows.Forms.ListBox();
            this.comboBoxPayee = new System.Windows.Forms.ComboBox();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.comboBoxDebitCredit = new System.Windows.Forms.ComboBox();
            this.textBoxAmount = new System.Windows.Forms.TextBox();
            this.checkBoxEstimate = new System.Windows.Forms.CheckBox();
            this.groupBoxOccurrences = new System.Windows.Forms.GroupBox();
            this.dateTimePickerFinal = new System.Windows.Forms.DateTimePicker();
            this.numericUpDownOccurrences = new System.Windows.Forms.NumericUpDown();
            this.radioButtonFinal = new System.Windows.Forms.RadioButton();
            this.radioButtonOccurrences = new System.Windows.Forms.RadioButton();
            this.radioButtonForever = new System.Windows.Forms.RadioButton();
            this.labelPayee = new System.Windows.Forms.Label();
            this.labelCategory = new System.Windows.Forms.Label();
            this.labelKind = new System.Windows.Forms.Label();
            this.labelAmount = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.textBoxFinalPayment = new System.Windows.Forms.TextBox();
            this.labelFinalPayment = new System.Windows.Forms.Label();
            this.labelNotice = new System.Windows.Forms.Label();
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.labelMemo = new System.Windows.Forms.Label();
            this.checkBoxReminder = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabPageMonthly.SuspendLayout();
            this.tabPageAnnually.SuspendLayout();
            this.tabPageWeekly.SuspendLayout();
            this.tabPageMonthlySsa.SuspendLayout();
            this.tabPageBiWeekly.SuspendLayout();
            this.groupBoxOccurrences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurrences)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageMonthly);
            this.tabControl.Controls.Add(this.tabPageAnnually);
            this.tabControl.Controls.Add(this.tabPageWeekly);
            this.tabControl.Controls.Add(this.tabPageMonthlySsa);
            this.tabControl.Controls.Add(this.tabPageBiWeekly);
            this.tabControl.Location = new System.Drawing.Point(13, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(538, 176);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageMonthly
            // 
            this.tabPageMonthly.Controls.Add(this.labelExample1);
            this.tabPageMonthly.Controls.Add(this.listBoxDaysOfMonth1);
            this.tabPageMonthly.Location = new System.Drawing.Point(4, 25);
            this.tabPageMonthly.Name = "tabPageMonthly";
            this.tabPageMonthly.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMonthly.Size = new System.Drawing.Size(530, 147);
            this.tabPageMonthly.TabIndex = 0;
            this.tabPageMonthly.Text = "Monthly";
            this.tabPageMonthly.UseVisualStyleBackColor = true;
            // 
            // labelExample1
            // 
            this.labelExample1.AutoSize = true;
            this.labelExample1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExample1.ForeColor = System.Drawing.Color.Red;
            this.labelExample1.Location = new System.Drawing.Point(262, 114);
            this.labelExample1.Name = "labelExample1";
            this.labelExample1.Size = new System.Drawing.Size(246, 17);
            this.labelExample1.TabIndex = 2;
            this.labelExample1.Text = "i.e. Pay bill on the 17th of each month";
            // 
            // listBoxDaysOfMonth1
            // 
            this.listBoxDaysOfMonth1.FormattingEnabled = true;
            this.listBoxDaysOfMonth1.ItemHeight = 16;
            this.listBoxDaysOfMonth1.Items.AddRange(new object[] {
            "1st (first day of month)",
            "2nd",
            "3rd",
            "4th",
            "5th",
            "6th",
            "7th",
            "8th",
            "9th",
            "10th",
            "11th",
            "12th",
            "13th",
            "14th",
            "15th",
            "16th",
            "17th",
            "18th",
            "19th",
            "20th",
            "21st",
            "22nd",
            "23rd",
            "24th",
            "25th",
            "26th",
            "27th",
            "28th",
            "29th (28th in short Feb)",
            "30th (or last day in Feb)",
            "31st (same as last, below)",
            "Last day of month"});
            this.listBoxDaysOfMonth1.Location = new System.Drawing.Point(16, 16);
            this.listBoxDaysOfMonth1.Name = "listBoxDaysOfMonth1";
            this.listBoxDaysOfMonth1.Size = new System.Drawing.Size(179, 116);
            this.listBoxDaysOfMonth1.TabIndex = 0;
            this.listBoxDaysOfMonth1.SelectedIndexChanged += new System.EventHandler(this.listBoxDaysOfMonth1_SelectedIndexChanged);
            // 
            // tabPageAnnually
            // 
            this.tabPageAnnually.Controls.Add(this.labelExample2);
            this.tabPageAnnually.Controls.Add(this.listBoxMonth2);
            this.tabPageAnnually.Controls.Add(this.listBoxDaysOfMonth2);
            this.tabPageAnnually.Location = new System.Drawing.Point(4, 25);
            this.tabPageAnnually.Name = "tabPageAnnually";
            this.tabPageAnnually.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAnnually.Size = new System.Drawing.Size(530, 147);
            this.tabPageAnnually.TabIndex = 1;
            this.tabPageAnnually.Text = "Yearly";
            this.tabPageAnnually.UseVisualStyleBackColor = true;
            // 
            // labelExample2
            // 
            this.labelExample2.AutoSize = true;
            this.labelExample2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExample2.ForeColor = System.Drawing.Color.Red;
            this.labelExample2.Location = new System.Drawing.Point(299, 116);
            this.labelExample2.Name = "labelExample2";
            this.labelExample2.Size = new System.Drawing.Size(215, 17);
            this.labelExample2.TabIndex = 4;
            this.labelExample2.Text = "i.e. Pay bill on April 25 each year";
            // 
            // listBoxMonth2
            // 
            this.listBoxMonth2.FormattingEnabled = true;
            this.listBoxMonth2.ItemHeight = 16;
            this.listBoxMonth2.Items.AddRange(new object[] {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"});
            this.listBoxMonth2.Location = new System.Drawing.Point(16, 16);
            this.listBoxMonth2.Name = "listBoxMonth2";
            this.listBoxMonth2.Size = new System.Drawing.Size(152, 116);
            this.listBoxMonth2.TabIndex = 3;
            this.listBoxMonth2.SelectedIndexChanged += new System.EventHandler(this.listBoxMonth2_SelectedIndexChanged);
            // 
            // listBoxDaysOfMonth2
            // 
            this.listBoxDaysOfMonth2.FormattingEnabled = true;
            this.listBoxDaysOfMonth2.ItemHeight = 16;
            this.listBoxDaysOfMonth2.Items.AddRange(new object[] {
            "1st (first day of month)",
            "2nd",
            "3rd",
            "4th",
            "5th",
            "6th",
            "7th",
            "8th",
            "9th",
            "10th",
            "11th",
            "12th",
            "13th",
            "14th",
            "15th",
            "16th",
            "17th",
            "18th",
            "19th",
            "20th",
            "21st",
            "22nd",
            "23rd",
            "24th",
            "25th",
            "26th",
            "27th",
            "28th",
            "29th (28th in short Feb)",
            "30th (or last day in Feb)",
            "31st (same as last, below)",
            "Last day of month"});
            this.listBoxDaysOfMonth2.Location = new System.Drawing.Point(185, 16);
            this.listBoxDaysOfMonth2.Name = "listBoxDaysOfMonth2";
            this.listBoxDaysOfMonth2.Size = new System.Drawing.Size(174, 84);
            this.listBoxDaysOfMonth2.TabIndex = 2;
            this.listBoxDaysOfMonth2.SelectedIndexChanged += new System.EventHandler(this.listBoxDaysOfMonth2_SelectedIndexChanged);
            // 
            // tabPageWeekly
            // 
            this.tabPageWeekly.Controls.Add(this.labelExample3);
            this.tabPageWeekly.Controls.Add(this.listBoxDaysOfWeek3);
            this.tabPageWeekly.Location = new System.Drawing.Point(4, 25);
            this.tabPageWeekly.Name = "tabPageWeekly";
            this.tabPageWeekly.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWeekly.Size = new System.Drawing.Size(530, 147);
            this.tabPageWeekly.TabIndex = 2;
            this.tabPageWeekly.Text = "Weekly";
            this.tabPageWeekly.UseVisualStyleBackColor = true;
            // 
            // labelExample3
            // 
            this.labelExample3.AutoSize = true;
            this.labelExample3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExample3.ForeColor = System.Drawing.Color.Red;
            this.labelExample3.Location = new System.Drawing.Point(270, 115);
            this.labelExample3.Name = "labelExample3";
            this.labelExample3.Size = new System.Drawing.Size(245, 17);
            this.labelExample3.TabIndex = 3;
            this.labelExample3.Text = "i.e. Paycheck deposit every Thursday";
            // 
            // listBoxDaysOfWeek3
            // 
            this.listBoxDaysOfWeek3.FormattingEnabled = true;
            this.listBoxDaysOfWeek3.ItemHeight = 16;
            this.listBoxDaysOfWeek3.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.listBoxDaysOfWeek3.Location = new System.Drawing.Point(16, 16);
            this.listBoxDaysOfWeek3.Name = "listBoxDaysOfWeek3";
            this.listBoxDaysOfWeek3.Size = new System.Drawing.Size(137, 116);
            this.listBoxDaysOfWeek3.TabIndex = 0;
            this.listBoxDaysOfWeek3.SelectedIndexChanged += new System.EventHandler(this.listBoxDaysOfWeek3_SelectedIndexChanged);
            // 
            // tabPageMonthlySsa
            // 
            this.tabPageMonthlySsa.Controls.Add(this.labelExample4);
            this.tabPageMonthlySsa.Controls.Add(this.listBoxNthOccurrence);
            this.tabPageMonthlySsa.Controls.Add(this.listBoxDayOfWeek4);
            this.tabPageMonthlySsa.Location = new System.Drawing.Point(4, 25);
            this.tabPageMonthlySsa.Name = "tabPageMonthlySsa";
            this.tabPageMonthlySsa.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMonthlySsa.Size = new System.Drawing.Size(530, 147);
            this.tabPageMonthlySsa.TabIndex = 3;
            this.tabPageMonthlySsa.Text = "Nth Wkday";
            this.tabPageMonthlySsa.UseVisualStyleBackColor = true;
            // 
            // labelExample4
            // 
            this.labelExample4.AutoSize = true;
            this.labelExample4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExample4.ForeColor = System.Drawing.Color.Red;
            this.labelExample4.Location = new System.Drawing.Point(173, 116);
            this.labelExample4.Name = "labelExample4";
            this.labelExample4.Size = new System.Drawing.Size(335, 17);
            this.labelExample4.TabIndex = 3;
            this.labelExample4.Text = "i.e. SSA check deposit 2nd Thursday of each month";
            // 
            // listBoxNthOccurrence
            // 
            this.listBoxNthOccurrence.FormattingEnabled = true;
            this.listBoxNthOccurrence.ItemHeight = 16;
            this.listBoxNthOccurrence.Items.AddRange(new object[] {
            "1st occurrence in month",
            "2nd occurrence in month",
            "3rd occurence in month",
            "4th occurrence in month"});
            this.listBoxNthOccurrence.Location = new System.Drawing.Point(173, 17);
            this.listBoxNthOccurrence.Name = "listBoxNthOccurrence";
            this.listBoxNthOccurrence.Size = new System.Drawing.Size(186, 68);
            this.listBoxNthOccurrence.TabIndex = 1;
            this.listBoxNthOccurrence.SelectedIndexChanged += new System.EventHandler(this.listBoxNthOccurrence_SelectedIndexChanged);
            // 
            // listBoxDayOfWeek4
            // 
            this.listBoxDayOfWeek4.FormattingEnabled = true;
            this.listBoxDayOfWeek4.ItemHeight = 16;
            this.listBoxDayOfWeek4.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.listBoxDayOfWeek4.Location = new System.Drawing.Point(17, 17);
            this.listBoxDayOfWeek4.Name = "listBoxDayOfWeek4";
            this.listBoxDayOfWeek4.Size = new System.Drawing.Size(137, 116);
            this.listBoxDayOfWeek4.TabIndex = 0;
            this.listBoxDayOfWeek4.SelectedIndexChanged += new System.EventHandler(this.listBoxDayOfWeek4_SelectedIndexChanged);
            // 
            // tabPageBiWeekly
            // 
            this.tabPageBiWeekly.Controls.Add(this.labelExample5);
            this.tabPageBiWeekly.Controls.Add(this.labelNextOccurrence5);
            this.tabPageBiWeekly.Controls.Add(this.dateTimePickerNextOccurrence5);
            this.tabPageBiWeekly.Controls.Add(this.listBoxDayOfWeek5);
            this.tabPageBiWeekly.Location = new System.Drawing.Point(4, 25);
            this.tabPageBiWeekly.Name = "tabPageBiWeekly";
            this.tabPageBiWeekly.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBiWeekly.Size = new System.Drawing.Size(530, 147);
            this.tabPageBiWeekly.TabIndex = 4;
            this.tabPageBiWeekly.Text = "Biweekly";
            this.tabPageBiWeekly.UseVisualStyleBackColor = true;
            // 
            // labelExample5
            // 
            this.labelExample5.AutoSize = true;
            this.labelExample5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExample5.ForeColor = System.Drawing.Color.Red;
            this.labelExample5.Location = new System.Drawing.Point(245, 116);
            this.labelExample5.Name = "labelExample5";
            this.labelExample5.Size = new System.Drawing.Size(268, 17);
            this.labelExample5.TabIndex = 4;
            this.labelExample5.Text = "i.e. Mortgage payment every other Friday";
            // 
            // labelNextOccurrence5
            // 
            this.labelNextOccurrence5.AutoSize = true;
            this.labelNextOccurrence5.Location = new System.Drawing.Point(174, 15);
            this.labelNextOccurrence5.Name = "labelNextOccurrence5";
            this.labelNextOccurrence5.Size = new System.Drawing.Size(155, 17);
            this.labelNextOccurrence5.TabIndex = 3;
            this.labelNextOccurrence5.Text = "Next occurrence date...";
            // 
            // dateTimePickerNextOccurrence5
            // 
            this.dateTimePickerNextOccurrence5.Location = new System.Drawing.Point(174, 38);
            this.dateTimePickerNextOccurrence5.Name = "dateTimePickerNextOccurrence5";
            this.dateTimePickerNextOccurrence5.Size = new System.Drawing.Size(233, 22);
            this.dateTimePickerNextOccurrence5.TabIndex = 2;
            this.dateTimePickerNextOccurrence5.ValueChanged += new System.EventHandler(this.dateTimePickerNextOccurrence5_ValueChanged);
            // 
            // listBoxDayOfWeek5
            // 
            this.listBoxDayOfWeek5.FormattingEnabled = true;
            this.listBoxDayOfWeek5.ItemHeight = 16;
            this.listBoxDayOfWeek5.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.listBoxDayOfWeek5.Location = new System.Drawing.Point(18, 16);
            this.listBoxDayOfWeek5.Name = "listBoxDayOfWeek5";
            this.listBoxDayOfWeek5.Size = new System.Drawing.Size(137, 116);
            this.listBoxDayOfWeek5.TabIndex = 1;
            this.listBoxDayOfWeek5.SelectedIndexChanged += new System.EventHandler(this.listBoxDayOfWeek5_SelectedIndexChanged);
            // 
            // comboBoxPayee
            // 
            this.comboBoxPayee.AllowDrop = true;
            this.comboBoxPayee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPayee.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxPayee.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPayee.FormattingEnabled = true;
            this.comboBoxPayee.Location = new System.Drawing.Point(105, 200);
            this.comboBoxPayee.Name = "comboBoxPayee";
            this.comboBoxPayee.Size = new System.Drawing.Size(174, 24);
            this.comboBoxPayee.TabIndex = 2;
            this.comboBoxPayee.SelectedIndexChanged += new System.EventHandler(this.comboBoxPayee_SelectedIndexChanged);
            this.comboBoxPayee.Leave += new System.EventHandler(this.comboBoxPayee_Leave);
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxCategory.FormattingEnabled = true;
            this.comboBoxCategory.Location = new System.Drawing.Point(105, 233);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(174, 24);
            this.comboBoxCategory.TabIndex = 3;
            this.comboBoxCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxCategory_SelectedIndexChanged);
            this.comboBoxCategory.Leave += new System.EventHandler(this.comboBoxCategory_Leave);
            // 
            // comboBoxDebitCredit
            // 
            this.comboBoxDebitCredit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDebitCredit.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxDebitCredit.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxDebitCredit.FormattingEnabled = true;
            this.comboBoxDebitCredit.Location = new System.Drawing.Point(105, 266);
            this.comboBoxDebitCredit.Name = "comboBoxDebitCredit";
            this.comboBoxDebitCredit.Size = new System.Drawing.Size(174, 24);
            this.comboBoxDebitCredit.TabIndex = 4;
            this.comboBoxDebitCredit.SelectedIndexChanged += new System.EventHandler(this.comboBoxDebitCredit_SelectedIndexChanged);
            this.comboBoxDebitCredit.Leave += new System.EventHandler(this.comboBoxDebitCredit_Leave);
            // 
            // textBoxAmount
            // 
            this.textBoxAmount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAmount.Location = new System.Drawing.Point(190, 300);
            this.textBoxAmount.Name = "textBoxAmount";
            this.textBoxAmount.Size = new System.Drawing.Size(89, 22);
            this.textBoxAmount.TabIndex = 5;
            this.textBoxAmount.TextChanged += new System.EventHandler(this.textBoxAmount_TextChanged);
            this.textBoxAmount.Enter += new System.EventHandler(this.textBoxAmount_Enter);
            this.textBoxAmount.Leave += new System.EventHandler(this.textBoxAmount_Leave);
            // 
            // checkBoxEstimate
            // 
            this.checkBoxEstimate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxEstimate.AutoSize = true;
            this.checkBoxEstimate.Location = new System.Drawing.Point(23, 300);
            this.checkBoxEstimate.Name = "checkBoxEstimate";
            this.checkBoxEstimate.Size = new System.Drawing.Size(84, 21);
            this.checkBoxEstimate.TabIndex = 6;
            this.checkBoxEstimate.Text = "Estimate";
            this.checkBoxEstimate.UseVisualStyleBackColor = true;
            // 
            // groupBoxOccurrences
            // 
            this.groupBoxOccurrences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOccurrences.Controls.Add(this.dateTimePickerFinal);
            this.groupBoxOccurrences.Controls.Add(this.numericUpDownOccurrences);
            this.groupBoxOccurrences.Controls.Add(this.radioButtonFinal);
            this.groupBoxOccurrences.Controls.Add(this.radioButtonOccurrences);
            this.groupBoxOccurrences.Controls.Add(this.radioButtonForever);
            this.groupBoxOccurrences.Location = new System.Drawing.Point(304, 192);
            this.groupBoxOccurrences.Name = "groupBoxOccurrences";
            this.groupBoxOccurrences.Size = new System.Drawing.Size(243, 104);
            this.groupBoxOccurrences.TabIndex = 7;
            this.groupBoxOccurrences.TabStop = false;
            this.groupBoxOccurrences.Text = "Duration";
            // 
            // dateTimePickerFinal
            // 
            this.dateTimePickerFinal.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerFinal.Location = new System.Drawing.Point(98, 73);
            this.dateTimePickerFinal.Name = "dateTimePickerFinal";
            this.dateTimePickerFinal.Size = new System.Drawing.Size(130, 22);
            this.dateTimePickerFinal.TabIndex = 11;
            this.dateTimePickerFinal.ValueChanged += new System.EventHandler(this.dateTimePickerFinal_ValueChanged);
            // 
            // numericUpDownOccurrences
            // 
            this.numericUpDownOccurrences.Location = new System.Drawing.Point(146, 46);
            this.numericUpDownOccurrences.Name = "numericUpDownOccurrences";
            this.numericUpDownOccurrences.Size = new System.Drawing.Size(82, 22);
            this.numericUpDownOccurrences.TabIndex = 10;
            this.numericUpDownOccurrences.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownOccurrences.ValueChanged += new System.EventHandler(this.numericUpDownOccurrences_ValueChanged);
            // 
            // radioButtonFinal
            // 
            this.radioButtonFinal.AutoSize = true;
            this.radioButtonFinal.Location = new System.Drawing.Point(16, 73);
            this.radioButtonFinal.Name = "radioButtonFinal";
            this.radioButtonFinal.Size = new System.Drawing.Size(63, 21);
            this.radioButtonFinal.TabIndex = 8;
            this.radioButtonFinal.Text = "Final:";
            this.radioButtonFinal.UseVisualStyleBackColor = true;
            this.radioButtonFinal.CheckedChanged += new System.EventHandler(this.radioButtonFinal_CheckedChanged);
            // 
            // radioButtonOccurrences
            // 
            this.radioButtonOccurrences.AutoSize = true;
            this.radioButtonOccurrences.Location = new System.Drawing.Point(16, 46);
            this.radioButtonOccurrences.Name = "radioButtonOccurrences";
            this.radioButtonOccurrences.Size = new System.Drawing.Size(114, 21);
            this.radioButtonOccurrences.TabIndex = 9;
            this.radioButtonOccurrences.Text = "Occurrences:";
            this.radioButtonOccurrences.UseVisualStyleBackColor = true;
            this.radioButtonOccurrences.CheckedChanged += new System.EventHandler(this.radioButtonOccurrences_CheckedChanged);
            // 
            // radioButtonForever
            // 
            this.radioButtonForever.AutoSize = true;
            this.radioButtonForever.Checked = true;
            this.radioButtonForever.Location = new System.Drawing.Point(16, 20);
            this.radioButtonForever.Name = "radioButtonForever";
            this.radioButtonForever.Size = new System.Drawing.Size(145, 21);
            this.radioButtonForever.TabIndex = 8;
            this.radioButtonForever.TabStop = true;
            this.radioButtonForever.Text = "Continues Forever";
            this.radioButtonForever.UseVisualStyleBackColor = true;
            this.radioButtonForever.CheckedChanged += new System.EventHandler(this.radioButtonForever_CheckedChanged);
            // 
            // labelPayee
            // 
            this.labelPayee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelPayee.AutoSize = true;
            this.labelPayee.Location = new System.Drawing.Point(17, 200);
            this.labelPayee.Name = "labelPayee";
            this.labelPayee.Size = new System.Drawing.Size(48, 17);
            this.labelPayee.TabIndex = 8;
            this.labelPayee.Text = "Payee";
            // 
            // labelCategory
            // 
            this.labelCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCategory.AutoSize = true;
            this.labelCategory.Location = new System.Drawing.Point(17, 233);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(65, 17);
            this.labelCategory.TabIndex = 9;
            this.labelCategory.Text = "Category";
            // 
            // labelKind
            // 
            this.labelKind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelKind.AutoSize = true;
            this.labelKind.Location = new System.Drawing.Point(17, 266);
            this.labelKind.Name = "labelKind";
            this.labelKind.Size = new System.Drawing.Size(77, 17);
            this.labelKind.TabIndex = 10;
            this.labelKind.Text = "Trans Kind";
            // 
            // labelAmount
            // 
            this.labelAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAmount.AutoSize = true;
            this.labelAmount.Location = new System.Drawing.Point(127, 302);
            this.labelAmount.Name = "labelAmount";
            this.labelAmount.Size = new System.Drawing.Size(56, 17);
            this.labelAmount.TabIndex = 11;
            this.labelAmount.Text = "Amount";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(450, 368);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(97, 32);
            this.buttonOk.TabIndex = 12;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(335, 368);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(95, 32);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.Location = new System.Drawing.Point(20, 368);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(100, 32);
            this.buttonDelete.TabIndex = 14;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // textBoxFinalPayment
            // 
            this.textBoxFinalPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxFinalPayment.Location = new System.Drawing.Point(190, 332);
            this.textBoxFinalPayment.Name = "textBoxFinalPayment";
            this.textBoxFinalPayment.Size = new System.Drawing.Size(89, 22);
            this.textBoxFinalPayment.TabIndex = 15;
            this.textBoxFinalPayment.TextChanged += new System.EventHandler(this.textBoxFinalPayment_TextChanged);
            this.textBoxFinalPayment.Enter += new System.EventHandler(this.textBoxFinalPayment_Enter);
            this.textBoxFinalPayment.Leave += new System.EventHandler(this.textBoxFinalPayment_Leave);
            // 
            // labelFinalPayment
            // 
            this.labelFinalPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFinalPayment.AutoSize = true;
            this.labelFinalPayment.Location = new System.Drawing.Point(18, 333);
            this.labelFinalPayment.Name = "labelFinalPayment";
            this.labelFinalPayment.Size = new System.Drawing.Size(166, 17);
            this.labelFinalPayment.TabIndex = 16;
            this.labelFinalPayment.Text = "Final Payment if Different";
            // 
            // labelNotice
            // 
            this.labelNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelNotice.AutoSize = true;
            this.labelNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotice.ForeColor = System.Drawing.Color.Red;
            this.labelNotice.Location = new System.Drawing.Point(124, 375);
            this.labelNotice.Name = "labelNotice";
            this.labelNotice.Size = new System.Drawing.Size(56, 17);
            this.labelNotice.TabIndex = 17;
            this.labelNotice.Text = "--------";
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMemo.Location = new System.Drawing.Point(369, 332);
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.Size = new System.Drawing.Size(178, 22);
            this.textBoxMemo.TabIndex = 18;
            // 
            // labelMemo
            // 
            this.labelMemo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMemo.AutoSize = true;
            this.labelMemo.Location = new System.Drawing.Point(306, 335);
            this.labelMemo.Name = "labelMemo";
            this.labelMemo.Size = new System.Drawing.Size(46, 17);
            this.labelMemo.TabIndex = 19;
            this.labelMemo.Text = "Memo";
            // 
            // checkBoxReminder
            // 
            this.checkBoxReminder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxReminder.AutoSize = true;
            this.checkBoxReminder.Location = new System.Drawing.Point(308, 303);
            this.checkBoxReminder.Name = "checkBoxReminder";
            this.checkBoxReminder.Size = new System.Drawing.Size(218, 21);
            this.checkBoxReminder.TabIndex = 20;
            this.checkBoxReminder.Text = "Highlight Entry as a Reminder";
            this.checkBoxReminder.UseVisualStyleBackColor = true;
            // 
            // ScheduledEventEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 415);
            this.Controls.Add(this.checkBoxReminder);
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxFinalPayment);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxOccurrences);
            this.Controls.Add(this.checkBoxEstimate);
            this.Controls.Add(this.textBoxAmount);
            this.Controls.Add(this.comboBoxDebitCredit);
            this.Controls.Add(this.comboBoxCategory);
            this.Controls.Add(this.comboBoxPayee);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.labelAmount);
            this.Controls.Add(this.labelKind);
            this.Controls.Add(this.labelCategory);
            this.Controls.Add(this.labelPayee);
            this.Controls.Add(this.labelFinalPayment);
            this.Controls.Add(this.labelNotice);
            this.Controls.Add(this.labelMemo);
            this.MinimumSize = new System.Drawing.Size(582, 430);
            this.Name = "ScheduledEventEditForm";
            this.Text = "Edit Scheduled Event";
            this.Load += new System.EventHandler(this.ScheduledEventEditForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageMonthly.ResumeLayout(false);
            this.tabPageMonthly.PerformLayout();
            this.tabPageAnnually.ResumeLayout(false);
            this.tabPageAnnually.PerformLayout();
            this.tabPageWeekly.ResumeLayout(false);
            this.tabPageWeekly.PerformLayout();
            this.tabPageMonthlySsa.ResumeLayout(false);
            this.tabPageMonthlySsa.PerformLayout();
            this.tabPageBiWeekly.ResumeLayout(false);
            this.tabPageBiWeekly.PerformLayout();
            this.groupBoxOccurrences.ResumeLayout(false);
            this.groupBoxOccurrences.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOccurrences)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageMonthly;
        private System.Windows.Forms.TabPage tabPageAnnually;
        private System.Windows.Forms.ListBox listBoxDaysOfMonth1;
        private System.Windows.Forms.ListBox listBoxDaysOfMonth2;
        private System.Windows.Forms.TabPage tabPageWeekly;
        private System.Windows.Forms.TabPage tabPageMonthlySsa;
        private System.Windows.Forms.TabPage tabPageBiWeekly;
        private System.Windows.Forms.ListBox listBoxMonth2;
        private System.Windows.Forms.ListBox listBoxDaysOfWeek3;
        private System.Windows.Forms.ListBox listBoxNthOccurrence;
        private System.Windows.Forms.ListBox listBoxDayOfWeek4;
        private System.Windows.Forms.ListBox listBoxDayOfWeek5;
        private System.Windows.Forms.DateTimePicker dateTimePickerNextOccurrence5;
        private System.Windows.Forms.Label labelNextOccurrence5;
        private System.Windows.Forms.ComboBox comboBoxPayee;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.ComboBox comboBoxDebitCredit;
        private System.Windows.Forms.TextBox textBoxAmount;
        private System.Windows.Forms.CheckBox checkBoxEstimate;
        private System.Windows.Forms.GroupBox groupBoxOccurrences;
        private System.Windows.Forms.DateTimePicker dateTimePickerFinal;
        private System.Windows.Forms.NumericUpDown numericUpDownOccurrences;
        private System.Windows.Forms.RadioButton radioButtonFinal;
        private System.Windows.Forms.RadioButton radioButtonOccurrences;
        private System.Windows.Forms.RadioButton radioButtonForever;
        private System.Windows.Forms.Label labelPayee;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.Label labelKind;
        private System.Windows.Forms.Label labelAmount;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelExample1;
        private System.Windows.Forms.Label labelExample2;
        private System.Windows.Forms.Label labelExample3;
        private System.Windows.Forms.Label labelExample4;
        private System.Windows.Forms.Label labelExample5;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textBoxFinalPayment;
        private System.Windows.Forms.Label labelFinalPayment;
        private System.Windows.Forms.Label labelNotice;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.Label labelMemo;
        private System.Windows.Forms.CheckBox checkBoxReminder;
    }
}