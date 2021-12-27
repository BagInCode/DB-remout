using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBMSClient.ServiceReference1;


namespace DBMSClient
{
    public partial class Form1 : Form
    {
        private DBMSServiceClient dBMSServiceClient = new DBMSServiceClient();
        string currentFilePath = "";
        string cellOldValue = "";
        string cellNewValue = "";
        bool readOnly = true;

        public Form1()
        {     
            InitializeComponent();
            comboBoxColumnsTypes.SelectedIndex = 0;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dBMSServiceClient.AddColumn(tablesControl.SelectedIndex, textBoxAddColumn.Text, comboBoxColumnsTypes.Text))
            {
                int tableIndex = tablesControl.SelectedIndex;
                if (tableIndex != -1)
                {
                    ShowTable(dBMSServiceClient.GetTable(tableIndex));
                }
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void butCreateDatabase_Click(object sender, EventArgs e)
        {
            if (currentFilePath != String.Empty)
            {
                DialogResult dialogResult = MessageBox.Show("Save changings?", "Attention!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dBMSServiceClient.SaveCurrentDatabase();
                }
            }
            if (dBMSServiceClient.CreateDatabase(textBoxCreateDatabase.Text))
            {
                currentFilePath = "";
                tablesControl.TabPages.Clear();
                dataGridView.Rows.Clear();
                dataGridView.Columns.Clear();
            }
        }

        private void butLoadDatabase_Click(object sender, EventArgs e)
        {
            if (currentFilePath != String.Empty)
            {
                DialogResult dialogResult = MessageBox.Show("Save changings?", "Attention!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dBMSServiceClient.SaveCurrentDatabase();
                }
            }
            var databaseName = textBoxLoadDatabase.Text;
            if (!dBMSServiceClient.LoadDatabase(databaseName))
            {
                return;
            }
            currentFilePath = databaseName;
            tablesControl.TabPages.Clear();
            List<string> tablesNames = new List<string>(dBMSServiceClient.GetTablesNameList());
            foreach (string tableName in tablesNames)
            {
                tablesControl.TabPages.Add(tableName);
            }
            int selectedTableIndex = tablesControl.SelectedIndex;
            if (selectedTableIndex != -1)
            {
                ShowTable(dBMSServiceClient.GetTable(selectedTableIndex));
            }
        }

        private void ShowTable(Table table)
        {
            try
            {
                dataGridView.Rows.Clear();
                dataGridView.Columns.Clear();

                foreach (Column column in table.columnsList)
                {
                    DataGridViewColumn dataGridColumn;
                    dataGridColumn = new DataGridViewTextBoxColumn();
                    dataGridColumn.Name = column.name;
                    dataGridColumn.HeaderText = column.name;
                    dataGridView.Columns.Add(dataGridColumn);
                }

                foreach (Row row in table.rowsList)
                {
                    DataGridViewRow dataGridRow = new DataGridViewRow();
                    for (int i = 0; i < row.valuesList.Length; ++i)
                    {
                        DataGridViewCell cell;
                        var stringCellValue = row.valuesList[i];
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = stringCellValue;
                        dataGridRow.Cells.Add(cell);
                    }
                    try
                    {
                        dataGridView.Rows.Add(dataGridRow);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void butSaveDatabase_Click(object sender, EventArgs e)
        {
            dBMSServiceClient.SaveCurrentDatabase();
        }

        private void butCreateTable_Click(object sender, EventArgs e)
        {
            if (dBMSServiceClient.AddEmptyTableWithName(textBoxCreateTable.Text))
            {
                tablesControl.TabPages.Add(textBoxCreateTable.Text);
            }
        }

        private void butDeleteTable_Click(object sender, EventArgs e)
        {
            if (tablesControl.TabCount == 0)
            {
                return;
            }
            try
            {
                dBMSServiceClient.DeleteTable(tablesControl.SelectedIndex);
                tablesControl.TabPages.RemoveAt(tablesControl.SelectedIndex);
            }
            catch { }
            if (tablesControl.TabCount == 0)
            {
                return;
            }

            int tableIndex = tablesControl.SelectedIndex;
            if (tableIndex != -1)
            {
                ShowTable(dBMSServiceClient.GetTable(tableIndex));
            }
        }

        private void butAddRow_Click(object sender, EventArgs e)
        {
            if (dBMSServiceClient.AddRow(tablesControl.SelectedIndex))
            {
                int tableIndex = tablesControl.SelectedIndex;
                if (tableIndex != -1)
                {
                    ShowTable(dBMSServiceClient.GetTable(tableIndex));
                }
            }
        }

        private void butDeleteRow_Click(object sender, EventArgs e)
        {

            if (dataGridView.Rows.Count == 0)
            {
                return;
            }
            try
            {
                dBMSServiceClient.DeleteRow(tablesControl.SelectedIndex, dataGridView.CurrentCell.RowIndex);
            }
            catch { }

            int tableIndex = tablesControl.SelectedIndex;
            if (tableIndex != -1)
            {
                ShowTable(dBMSServiceClient.GetTable(tableIndex));
            }
        }

        private void butDeleteColumn_Click(object sender, EventArgs e)
        {
            if (dataGridView.Columns.Count == 0) return;
            try
            {
                dBMSServiceClient.DeleteColumn(tablesControl.SelectedIndex, dataGridView.CurrentCell.ColumnIndex);
            }
            catch { }

            int tableIndex = tablesControl.SelectedIndex;
            if (tableIndex != -1)
            {
                ShowTable(dBMSServiceClient.GetTable(tableIndex));
            }
        }

        private bool validateFilter(string filter)
        {
            var nameValue = filter.Split('=');

            return (nameValue.Length == 2);
        }

        private void butTemplateSearch_Click(object sender, EventArgs e)
        {
            int tableIndex = tablesControl.SelectedIndex;
            string templateSearch = textBoxTemplateSearch.Text;

            if (templateSearch == string.Empty)
            {
                readOnly = false;
                ShowTable(dBMSServiceClient.GetTable(tableIndex));
                return;
            }

            readOnly = true;

            var filters = templateSearch.Split(';');

            if (filters.Length != 0)
            {
                List<string[]> Filters = new List<string[]>();

                foreach (string filter in filters)
                {
                    if (!validateFilter(filter))
                    {
                        return;
                    }

                    var nameValue = filter.Split('=');
                    Filters.Add(nameValue);
                }

                ShowTable(dBMSServiceClient.TemplateSearch(tableIndex, Filters));
            }

            return;
        }

        private void butIntersection_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int selectedTableIndex = tablesControl.SelectedIndex;
            if (selectedTableIndex == -1)
            {
                return;
            }
            var table = dBMSServiceClient.GetTable(selectedTableIndex);
            return;
        }

        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int selectedTableIndex = tablesControl.SelectedIndex;
            if (selectedTableIndex == -1)
            {
                return;
            }
            var table = dBMSServiceClient.GetTable(selectedTableIndex);
            var currentCellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            cellOldValue = currentCellValue == null ? String.Empty : currentCellValue.ToString();
            return;
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (cellValue == null)
            {
                return;
            }
            cellNewValue = cellValue.ToString();
            if (!dBMSServiceClient.ChangeValue(cellNewValue, tablesControl.SelectedIndex, e.ColumnIndex, e.RowIndex))
            {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellOldValue;
                return;
            }
            int selectedTableIndex = tablesControl.SelectedIndex;
            if (selectedTableIndex == -1)
            {
                return;
            }
            var table = dBMSServiceClient.GetTable(selectedTableIndex);
            ShowTable(dBMSServiceClient.GetTable(selectedTableIndex));
        }

        private void tablesControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tablesIndex = tablesControl.SelectedIndex;
            if (tablesIndex != -1)
            {
                ShowTable(dBMSServiceClient.GetTable(tablesIndex));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBoxLoadDatabase_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxCreateTable_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxTemplateSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
