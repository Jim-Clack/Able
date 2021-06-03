using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class SearchEntriesForm : Form
    {

        private UiBackend _backend = null;

        private List<Guid> _matches = new List<Guid>();

        private bool _showAll = true;

        public SearchEntriesForm(UiBackend backend)
        {
            _backend = backend;
            InitializeComponent();
        }

        public List<Guid> Matches
        {
            get
            {
                return _matches;
            }
        }

        public bool ListAll
        {
            get
            {
                return _showAll;
            }
        }

        private void SearchEntriesForm_Load(object sender, EventArgs e)
        {
            this.Text = Strings.Get("Search Entries");
            label2.Text = Strings.Get("Search only those Entries before or on:");
            label8.Text = Strings.Get("Matches");
            buttonPayeeSubstring.Text = Strings.Get("Search for substring in Payee");
            buttonPayeeMatch.Text = Strings.Get("Search for Payee match");
            buttonCategoryMatch.Text = Strings.Get("Search for Category match");
            buttonMemoSubstring.Text = Strings.Get("Search for substring in Memo");
            buttonCheckNumberRange.Text = Strings.Get("Search for Check Numbers between");
            buttonShowLast.Text = Strings.Get("Go To Most Recent");
            buttonShowAll.Text = Strings.Get("Sort Matches First");
            buttonCancel.Text = Strings.Get("Cancel");
            label1.Text = Strings.Get("=> Thru =>");
            dateTimePickerBeforeDate.Value = DateTime.Now.AddYears(1);
            comboBoxPayee.DataSource = _backend.Payees;
            comboBoxCategory.DataSource = _backend.Categories;
            comboBoxPayee.Text = "";
            comboBoxPayee.SelectedIndex = -1;
            comboBoxCategory.Text = "";
            comboBoxCategory.SelectedIndex = -1;
            AdjustVisibilities();
        }

        private void AdjustVisibilities()
        {
            buttonPayeeSubstring.Enabled = textBoxPayeeSubstring.Text.Trim().Length > 0;
            buttonPayeeMatch.Enabled = comboBoxPayee.Text.Trim().Length > 0;
            buttonCategoryMatch.Enabled = comboBoxCategory.Text.Trim().Length > 0;
            buttonMemoSubstring.Enabled = textBoxMemo.Text.Trim().Length > 0;
            buttonCheckNumberRange.Enabled = textBoxCheckNumberMin.Text.Trim().Length > 0 && textBoxCheckNumberMin.Text.Trim().Length > 0;
        }

        private void UpdateForm()
        {
            textBoxMatchCount.Text = "" + _matches.Count;
            AdjustVisibilities();
        }

        private void CompleteSearch()
        {
            if (textBoxMatchCount.Text.Trim().Length < 1)
            {
                buttonPayeeMatch_Click(null, null);
                buttonCategoryMatch_Click(null, null);
                buttonPayeeSubstring_Click(null, null);
                buttonMemoSubstring_Click(null, null);
                buttonCheckNumberRange_Click(null, null);
            }
            DialogResult = (_matches.Count > 0) ? DialogResult.OK : DialogResult.Cancel;
        }

        private void buttonPayeeMatch_Click(object sender, EventArgs e)
        {
            if(comboBoxPayee.Text.Trim().Length < 1)
            {
                return;
            }
            _matches = UtilityMethods.SearchDb(_backend.Db, UtilityMethods.EntryField.Payee,
                comboBoxPayee.Text, Guid.NewGuid(), 0, 0, false, dateTimePickerBeforeDate.Value);
            UpdateForm();
        }

        private void buttonPayeeSubstring_Click(object sender, EventArgs e)
        {
            if (textBoxPayeeSubstring.Text.Trim().Length < 1)
            {
                return;
            }
            _matches = UtilityMethods.SearchDb(_backend.Db, UtilityMethods.EntryField.PayeeSubstring,
                textBoxPayeeSubstring.Text, Guid.NewGuid(), 0, 0, false, dateTimePickerBeforeDate.Value);
            UpdateForm();
        }

        private void buttonCategoryMatch_Click(object sender, EventArgs e)
        {
            if (comboBoxCategory.Text.Trim().Length < 1)
            {
                return;
            }
            FinancialCategory category = UtilityMethods.GetCategoryOrUnknown(_backend.Db, comboBoxCategory.Text.Trim());
            if (category != null)
            {
                _matches = UtilityMethods.SearchDb(_backend.Db, UtilityMethods.EntryField.Category,
                    "", category.Id, 0, 0, false, dateTimePickerBeforeDate.Value);
            }
            UpdateForm();
        }

        private void buttonMemoSubstring_Click(object sender, EventArgs e)
        {
            if (textBoxMemo.Text.Trim().Length < 1)
            {
                return;
            }
            _matches = UtilityMethods.SearchDb(_backend.Db, UtilityMethods.EntryField.MemoSubstring,
                textBoxMemo.Text, Guid.NewGuid(), 0, 0, false, dateTimePickerBeforeDate.Value);
            UpdateForm();
        }

        private void buttonCheckNumberRange_Click(object sender, EventArgs e)
        {
            if (textBoxCheckNumberMin.Text.Trim().Length < 1 || textBoxCheckNumberMax.Text.Trim().Length < 1)
            {
                return;
            }
            int min = 0, max = 99999;
            if (int.TryParse(textBoxCheckNumberMin.Text.Trim(), out min) &&
                int.TryParse(textBoxCheckNumberMax.Text.Trim(), out max))
            {
                _matches = UtilityMethods.SearchDb(_backend.Db, UtilityMethods.EntryField.CheckNumberRange,
                    textBoxMemo.Text, Guid.NewGuid(), min, max, false, dateTimePickerBeforeDate.Value);
                UpdateForm();
            }
        }

        private void comboBoxPayee_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxCategory.Text = "";
            textBoxPayeeSubstring.Text = "";
            textBoxMemo.Text = "";
            textBoxCheckNumberMin.Text = "";
            textBoxCheckNumberMin.Text = "";
            AdjustVisibilities();
        }

        private void textBoxPayeeSubstring_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxPayee.Text = "";
            comboBoxCategory.Text = "";
            textBoxMemo.Text = "";
            textBoxCheckNumberMin.Text = "";
            textBoxCheckNumberMin.Text = "";
            AdjustVisibilities();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxPayee.Text = "";
            textBoxPayeeSubstring.Text = "";
            textBoxMemo.Text = "";
            textBoxCheckNumberMin.Text = "";
            textBoxCheckNumberMin.Text = "";
            AdjustVisibilities();
        }

        private void textBoxMemo_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxPayee.Text = "";
            comboBoxCategory.Text = "";
            textBoxPayeeSubstring.Text = "";
            textBoxCheckNumberMin.Text = "";
            textBoxCheckNumberMin.Text = "";
            AdjustVisibilities();
        }

        private void textBoxCheckNumberMin_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxPayee.Text = "";
            comboBoxCategory.Text = "";
            textBoxPayeeSubstring.Text = "";
            textBoxMemo.Text = "";
            AdjustVisibilities();
        }

        private void textBoxCheckNumberMax_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxPayee.Text = "";
            comboBoxCategory.Text = "";
            textBoxPayeeSubstring.Text = "";
            textBoxMemo.Text = "";
            AdjustVisibilities();
        }

        private void dateTimePickerBeforeDate_KeyUp(object sender, KeyEventArgs e)
        {
            AdjustVisibilities();
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {

        }

        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            CompleteSearch();
            _showAll = true;
            Close();
        }

        private void buttonShowLast_Click(object sender, EventArgs e)
        {
            CompleteSearch();
            _showAll = false;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
