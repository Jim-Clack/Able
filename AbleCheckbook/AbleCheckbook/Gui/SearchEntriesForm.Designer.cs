namespace AbleCheckbook.Gui
{
    partial class SearchEntriesForm
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
            this.buttonPayeeMatch = new System.Windows.Forms.Button();
            this.buttonPayeeSubstring = new System.Windows.Forms.Button();
            this.buttonCategoryMatch = new System.Windows.Forms.Button();
            this.buttonMemoSubstring = new System.Windows.Forms.Button();
            this.buttonCheckNumberRange = new System.Windows.Forms.Button();
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.textBoxCheckNumberMin = new System.Windows.Forms.TextBox();
            this.textBoxCheckNumberMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerBeforeDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonShowAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPayeeSubstring = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxPayee = new System.Windows.Forms.ComboBox();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxMatchCount = new System.Windows.Forms.TextBox();
            this.buttonShowLast = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonPayeeMatch
            // 
            this.buttonPayeeMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPayeeMatch.Location = new System.Drawing.Point(187, 55);
            this.buttonPayeeMatch.Name = "buttonPayeeMatch";
            this.buttonPayeeMatch.Size = new System.Drawing.Size(211, 32);
            this.buttonPayeeMatch.TabIndex = 2;
            this.buttonPayeeMatch.Text = "Search for Payee match";
            this.buttonPayeeMatch.UseVisualStyleBackColor = true;
            this.buttonPayeeMatch.Click += new System.EventHandler(this.buttonPayeeMatch_Click);
            // 
            // buttonPayeeSubstring
            // 
            this.buttonPayeeSubstring.Location = new System.Drawing.Point(188, 95);
            this.buttonPayeeSubstring.Name = "buttonPayeeSubstring";
            this.buttonPayeeSubstring.Size = new System.Drawing.Size(210, 32);
            this.buttonPayeeSubstring.TabIndex = 4;
            this.buttonPayeeSubstring.Text = "Search for substring in Payee";
            this.buttonPayeeSubstring.UseVisualStyleBackColor = true;
            this.buttonPayeeSubstring.Click += new System.EventHandler(this.buttonPayeeSubstring_Click);
            // 
            // buttonCategoryMatch
            // 
            this.buttonCategoryMatch.Location = new System.Drawing.Point(188, 136);
            this.buttonCategoryMatch.Name = "buttonCategoryMatch";
            this.buttonCategoryMatch.Size = new System.Drawing.Size(210, 32);
            this.buttonCategoryMatch.TabIndex = 6;
            this.buttonCategoryMatch.Text = "Search for Category match";
            this.buttonCategoryMatch.UseVisualStyleBackColor = true;
            this.buttonCategoryMatch.Click += new System.EventHandler(this.buttonCategoryMatch_Click);
            // 
            // buttonMemoSubstring
            // 
            this.buttonMemoSubstring.Location = new System.Drawing.Point(188, 177);
            this.buttonMemoSubstring.Name = "buttonMemoSubstring";
            this.buttonMemoSubstring.Size = new System.Drawing.Size(210, 32);
            this.buttonMemoSubstring.TabIndex = 8;
            this.buttonMemoSubstring.Text = "Search for substring in Memo";
            this.buttonMemoSubstring.UseVisualStyleBackColor = true;
            this.buttonMemoSubstring.Click += new System.EventHandler(this.buttonMemoSubstring_Click);
            // 
            // buttonCheckNumberRange
            // 
            this.buttonCheckNumberRange.Location = new System.Drawing.Point(304, 218);
            this.buttonCheckNumberRange.Name = "buttonCheckNumberRange";
            this.buttonCheckNumberRange.Size = new System.Drawing.Size(260, 32);
            this.buttonCheckNumberRange.TabIndex = 11;
            this.buttonCheckNumberRange.Text = "Search for Check Numbers between";
            this.buttonCheckNumberRange.UseVisualStyleBackColor = true;
            this.buttonCheckNumberRange.Click += new System.EventHandler(this.buttonCheckNumberRange_Click);
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.Location = new System.Drawing.Point(21, 182);
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.Size = new System.Drawing.Size(129, 22);
            this.textBoxMemo.TabIndex = 7;
            this.textBoxMemo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxMemo_KeyUp);
            // 
            // textBoxCheckNumberMin
            // 
            this.textBoxCheckNumberMin.Location = new System.Drawing.Point(21, 223);
            this.textBoxCheckNumberMin.Name = "textBoxCheckNumberMin";
            this.textBoxCheckNumberMin.Size = new System.Drawing.Size(69, 22);
            this.textBoxCheckNumberMin.TabIndex = 9;
            this.textBoxCheckNumberMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCheckNumberMin_KeyUp);
            // 
            // textBoxCheckNumberMax
            // 
            this.textBoxCheckNumberMax.Location = new System.Drawing.Point(188, 222);
            this.textBoxCheckNumberMax.Name = "textBoxCheckNumberMax";
            this.textBoxCheckNumberMax.Size = new System.Drawing.Size(69, 22);
            this.textBoxCheckNumberMax.TabIndex = 10;
            this.textBoxCheckNumberMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCheckNumberMax_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "=> Thru =>";
            // 
            // dateTimePickerBeforeDate
            // 
            this.dateTimePickerBeforeDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBeforeDate.Location = new System.Drawing.Point(302, 18);
            this.dateTimePickerBeforeDate.Name = "dateTimePickerBeforeDate";
            this.dateTimePickerBeforeDate.Size = new System.Drawing.Size(262, 22);
            this.dateTimePickerBeforeDate.TabIndex = 0;
            this.dateTimePickerBeforeDate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePickerBeforeDate_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 20);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(252, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Search only those Entries before or on";
            // 
            // buttonShowAll
            // 
            this.buttonShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowAll.Location = new System.Drawing.Point(407, 265);
            this.buttonShowAll.Name = "buttonShowAll";
            this.buttonShowAll.Size = new System.Drawing.Size(157, 32);
            this.buttonShowAll.TabIndex = 12;
            this.buttonShowAll.Text = "Sort Matches First";
            this.buttonShowAll.UseVisualStyleBackColor = true;
            this.buttonShowAll.Click += new System.EventHandler(this.buttonShowAll_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(157, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "=>";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(157, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "=>";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(157, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "=>";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(269, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "=>";
            // 
            // textBoxPayeeSubstring
            // 
            this.textBoxPayeeSubstring.Location = new System.Drawing.Point(21, 99);
            this.textBoxPayeeSubstring.Name = "textBoxPayeeSubstring";
            this.textBoxPayeeSubstring.Size = new System.Drawing.Size(129, 22);
            this.textBoxPayeeSubstring.TabIndex = 3;
            this.textBoxPayeeSubstring.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxPayeeSubstring_KeyUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(157, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 17);
            this.label7.TabIndex = 20;
            this.label7.Text = "=>";
            // 
            // comboBoxPayee
            // 
            this.comboBoxPayee.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxPayee.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPayee.DropDownWidth = 220;
            this.comboBoxPayee.FormattingEnabled = true;
            this.comboBoxPayee.Location = new System.Drawing.Point(21, 58);
            this.comboBoxPayee.Name = "comboBoxPayee";
            this.comboBoxPayee.Size = new System.Drawing.Size(129, 24);
            this.comboBoxPayee.TabIndex = 1;
            this.comboBoxPayee.SelectedIndexChanged += new System.EventHandler(this.comboBoxPayee_SelectedIndexChanged);
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxCategory.DropDownWidth = 220;
            this.comboBoxCategory.FormattingEnabled = true;
            this.comboBoxCategory.Location = new System.Drawing.Point(21, 139);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(129, 24);
            this.comboBoxCategory.TabIndex = 5;
            this.comboBoxCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxCategory_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(502, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 17);
            this.label8.TabIndex = 23;
            this.label8.Text = "Matches";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Location = new System.Drawing.Point(20, 265);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(110, 32);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxMatchCount
            // 
            this.textBoxMatchCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMatchCount.Location = new System.Drawing.Point(502, 125);
            this.textBoxMatchCount.Name = "textBoxMatchCount";
            this.textBoxMatchCount.ReadOnly = true;
            this.textBoxMatchCount.Size = new System.Drawing.Size(60, 22);
            this.textBoxMatchCount.TabIndex = 25;
            // 
            // buttonShowLast
            // 
            this.buttonShowLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowLast.Location = new System.Drawing.Point(226, 265);
            this.buttonShowLast.Name = "buttonShowLast";
            this.buttonShowLast.Size = new System.Drawing.Size(166, 32);
            this.buttonShowLast.TabIndex = 27;
            this.buttonShowLast.Text = "Go To Most Recent";
            this.buttonShowLast.UseVisualStyleBackColor = true;
            this.buttonShowLast.Click += new System.EventHandler(this.buttonShowLast_Click);
            // 
            // SearchEntriesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 313);
            this.Controls.Add(this.textBoxMatchCount);
            this.Controls.Add(this.buttonShowLast);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboBoxCategory);
            this.Controls.Add(this.comboBoxPayee);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPayeeSubstring);
            this.Controls.Add(this.buttonCheckNumberRange);
            this.Controls.Add(this.textBoxCheckNumberMax);
            this.Controls.Add(this.dateTimePickerBeforeDate);
            this.Controls.Add(this.buttonShowAll);
            this.Controls.Add(this.textBoxCheckNumberMin);
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.buttonMemoSubstring);
            this.Controls.Add(this.buttonCategoryMatch);
            this.Controls.Add(this.buttonPayeeSubstring);
            this.Controls.Add(this.buttonPayeeMatch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label8);
            this.MaximumSize = new System.Drawing.Size(680, 380);
            this.MinimumSize = new System.Drawing.Size(600, 360);
            this.Name = "SearchEntriesForm";
            this.Text = "Search Entries";
            this.Load += new System.EventHandler(this.SearchEntriesForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPayeeMatch;
        private System.Windows.Forms.Button buttonPayeeSubstring;
        private System.Windows.Forms.Button buttonCategoryMatch;
        private System.Windows.Forms.Button buttonMemoSubstring;
        private System.Windows.Forms.Button buttonCheckNumberRange;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.TextBox textBoxCheckNumberMin;
        private System.Windows.Forms.TextBox textBoxCheckNumberMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerBeforeDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonShowAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPayeeSubstring;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxPayee;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxMatchCount;
        private System.Windows.Forms.Button buttonShowLast;
    }
}