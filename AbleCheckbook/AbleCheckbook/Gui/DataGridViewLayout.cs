﻿using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public class DataGridViewLayout
    {

        /// <summary>
        /// Row styles for payment v deposits, cleared v future, etc.
        /// </summary>
        private DataGridViewCellStyle[] _styles = null;

        /// <summary>
        /// Row styles "amount" column.
        /// </summary>
        private DataGridViewCellStyle[] _boldStyles = null;

        /// <summary>
        /// Use this style to alert the user.
        /// </summary>
        private DataGridViewCellStyle _alertStyle = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        public DataGridViewLayout()
        {
            CreateStyles();
        }

        /// <summary>
        /// Create the cell/row styles and fonts.
        /// </summary>
        public void CreateStyles()
        { 
            int fontSize = Configuration.Instance.HighVisibility ? 10 : 9;
            _styles = new DataGridViewCellStyle[(int)EntryColor.Count];
            _boldStyles = new DataGridViewCellStyle[(int)EntryColor.Count];
            for (int colorIndex = 0; colorIndex < (int)EntryColor.Count; colorIndex++)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(FontFamily.GenericSansSerif, fontSize, 
                    (colorIndex == (int)EntryColor.NewEntryRow) ? FontStyle.Bold : FontStyle.Regular); 
                style.ForeColor = RowOfCheckbook.CellFgColor(colorIndex);
                style.BackColor = RowOfCheckbook.CellBgColor(colorIndex);
                _styles[colorIndex] = style;
                _boldStyles[colorIndex] = style.Clone();
                Font font = _boldStyles[colorIndex].Font;
                font = new Font(font, FontStyle.Bold);
                _boldStyles[colorIndex].Font = font;
            }
            _alertStyle = new DataGridViewCellStyle();
            _alertStyle.Font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
            _alertStyle.ForeColor = Color.Yellow;
            _alertStyle.BackColor = Color.Red;
        }

        /// <summary>
        /// Get a cell style for alerts.
        /// </summary>
        public DataGridViewCellStyle AlertStyle
        {
            get
            {
                return _alertStyle;
            }
        }

        /// <summary>
        /// Get a row style.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isBold">true for BOLD font</param>
        /// <returns>the cell style</returns>
        public DataGridViewCellStyle Style(int index, bool isBold = false)
        {
            if(isBold)
            {
                return _boldStyles[index];
            }
            return _styles[index];
        }

        /// <summary>
        /// Call this immediately AFTER binding a datasource to the grid, even though the error occurs when binding. 
        /// </summary>
        /// <param name="dataGridView">DataGridView immediately AFTER it has been bound to a data source.</param>
        /// <param name="diagsEnabled">true if diags are enabled</param>
        /// <param name="reconcile">true to layout per reconciliation column sequence</param>
        public void LayoutColumns(DataGridView dataGridView, bool diagsEnabled, bool reconcile)
        {
            CreateStyles();
            dataGridView.Columns["IsChecked"].DisplayIndex = 0;
            dataGridView.Columns["Color"].DisplayIndex = 1;
            if (reconcile)
            {
                dataGridView.Columns["Amount"].DisplayIndex = 2;
                dataGridView.Columns["Debit"].DisplayIndex = 3;
                dataGridView.Columns["Credit"].DisplayIndex = 4;
                dataGridView.Columns["Payee"].DisplayIndex = 5;
                dataGridView.Columns["CheckNumber"].DisplayIndex = 6;
                dataGridView.Columns["DateOfTransaction"].DisplayIndex = 7;
                dataGridView.Columns["Category"].DisplayIndex = 8;
            }
            else
            {
                dataGridView.Columns["DateOfTransaction"].DisplayIndex = 2;
                dataGridView.Columns["CheckNumber"].DisplayIndex = 3;
                dataGridView.Columns["Payee"].DisplayIndex = 4;
                dataGridView.Columns["Category"].DisplayIndex = 5;
                dataGridView.Columns["Amount"].DisplayIndex = 6;
                dataGridView.Columns["Debit"].DisplayIndex = 7;
                dataGridView.Columns["Credit"].DisplayIndex = 8;
            }
            dataGridView.Columns["Balance"].DisplayIndex = 9;
            dataGridView.Columns["Memo"].DisplayIndex = 10;
            dataGridView.Columns["Status"].DisplayIndex = 11;
            dataGridView.Columns["IsCleared"].DisplayIndex = 12;
            dataGridView.Columns["DateCleared"].DisplayIndex = 13;
            dataGridView.Columns["DateModified"].DisplayIndex = 14;
            dataGridView.Columns["ModifiedBy"].DisplayIndex = 15;
            dataGridView.Columns["NewEntryRow"].DisplayIndex = 16;
            dataGridView.Columns["Id"].DisplayIndex = 17;
            dataGridView.Columns["ShowSplits"].DisplayIndex = 18;
            dataGridView.Columns["DateOfTransaction"].HeaderText = Strings.Get("Date");
            dataGridView.Columns["CheckNumber"].HeaderText = Strings.Get("Chk#");
            dataGridView.Columns["Payee"].HeaderText = Strings.Get("Payee");
            dataGridView.Columns["Category"].HeaderText = Strings.Get("Category");
            dataGridView.Columns["Amount"].HeaderText = Strings.Get("Amount");
            dataGridView.Columns["Debit"].HeaderText = Strings.Get("Debit");
            dataGridView.Columns["Credit"].HeaderText = Strings.Get("Credit");
            dataGridView.Columns["Balance"].HeaderText = Strings.Get("Balance");
            dataGridView.Columns["IsCleared"].HeaderText = "x";
            dataGridView.Columns["Memo"].HeaderText = Strings.Get("Memo");
            dataGridView.Columns["Status"].HeaderText = Strings.Get("Status");
            dataGridView.Columns["DateCleared"].HeaderText = Strings.Get("Cleared");
            dataGridView.Columns["DateModified"].HeaderText = Strings.Get("Modified");
            dataGridView.Columns["ModifiedBy"].HeaderText = Strings.Get("By");
            dataGridView.Columns["Id"].HeaderText = Strings.Get("Id");
            dataGridView.Columns["Amount"].Visible = !Configuration.Instance.TwoAmountColumns;
            dataGridView.Columns["Debit"].Visible = Configuration.Instance.TwoAmountColumns;
            dataGridView.Columns["Credit"].Visible = Configuration.Instance.TwoAmountColumns;
            dataGridView.Columns["Id"].Visible = diagsEnabled;
            dataGridView.Columns["NewEntryRow"].Visible = false;
            dataGridView.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["Memo"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView.Columns["Category"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.RowHeadersVisible = false;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// Adjust the data grid view by changing critical columns' width to suit a resized area.
        /// </summary>
        /// <param name="dataGridView"></param>
        public void AdjustWidths(DataGridView dataGridView)
        {
            int rectWidth = dataGridView.Width + 38; 
            int rowsWidth = dataGridView.RowHeadersWidth;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Visible)
                {
                    rowsWidth += column.Width;
                }
            }
            int plus = Configuration.Instance.HighVisibility ? 10 : 0;
            int difference = rectWidth - rowsWidth;
            int rowsAdjust = difference / 10;
            int residual = difference - rowsAdjust * 10;
            dataGridView.Columns["IsChecked"].Width = 30;
            dataGridView.Columns["Payee"].Width = Math.Max(100 + plus, dataGridView.Columns["Payee"].Width + rowsAdjust * 2 + residual);
            dataGridView.Columns["Category"].Width = Math.Max(128 + plus, dataGridView.Columns["Category"].Width + rowsAdjust * 2);
            dataGridView.Columns["Memo"].Width = Math.Max(120 + plus, dataGridView.Columns["Memo"].Width + rowsAdjust * 2);
            dataGridView.Columns["Amount"].Width = Math.Max(84 + plus, dataGridView.Columns["Amount"].Width + rowsAdjust);
            dataGridView.Columns["Debit"].Width = Math.Max(84 + plus, dataGridView.Columns["Debit"].Width + rowsAdjust);
            dataGridView.Columns["Balance"].Width = Math.Max(84 + plus, dataGridView.Columns["Balance"].Width + rowsAdjust);
            dataGridView.Columns["BankInfo"].Width = Math.Max(84 + plus, dataGridView.Columns["BankInfo"].Width + rowsAdjust * 2);
        }

    }

}
