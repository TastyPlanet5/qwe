using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BusinessCalc
{
    public partial class Form1 : Form
    {
        public static SQLiteConnection connection = new SQLiteConnection("Data Source=../../../database.db");
        List<string[]> incomes;
        List<string[]> expenses;
        public Form1()
        {
            InitializeComponent();
            UpdateAll();
            dataGridView1.Columns[0].Name = "Theme";
            dataGridView1.Columns[1].Name = "Count";
            dataGridView1.Columns[2].Name = "Date";
            dataGridView2.Columns[0].Name = "Theme";
            dataGridView2.Columns[1].Name = "Count";
            dataGridView2.Columns[2].Name = "Date";
        }

        private void addIncomeBTN_Click(object sender, EventArgs e)
        {
            connection.Open();
            string request = $"INSERT INTO transactions (Theme, Count, Date) VALUES ('{comboBox1.Text}', {numericUpDown1.Value}, '{DateTime.Today.ToString("yyyy-MM-dd")}')";
            var command = new SQLiteCommand(request, connection);
            command.ExecuteNonQuery();
            connection.Close();
            UpdateAll();
        }

        private void addExpenseBTN_Click(object sender, EventArgs e)
        {
            connection.Open();
            string request = $"INSERT INTO transactions (Theme, Count, Date) VALUES ('{comboBox2.Text}', {numericUpDown2.Value}, '{DateTime.Today.ToString("yyyy-MM-dd")}')";
            var command = new SQLiteCommand(request, connection);
            command.ExecuteNonQuery();
            connection.Close();
            UpdateAll();
        }

        public void UpdateAll()
        {
            incomes = getData("SELECT * FROM transactions WHERE Theme IN (SELECT Name FROM themes WHERE Type = 'Income')");
            expenses = getData("SELECT * FROM transactions WHERE Theme IN (SELECT Name FROM themes WHERE Type = 'Expense')");
            dataGridView1.ColumnCount = incomes[0].Length;
            dataGridView1.RowCount = incomes.Count;
            dataGridView2.ColumnCount = expenses[0].Length;
            dataGridView2.RowCount = expenses.Count;

            ALL1.Text = $"All: {getData("SELECT SUM(Count) FROM transactions WHERE Theme IN (SELECT Name FROM themes WHERE Type = 'Income')")[0][0]}$";
            ALL2.Text = $"All: {getData("SELECT SUM(Count) FROM transactions WHERE Theme IN (SELECT Name FROM themes WHERE Type = 'Expense')")[0][0]}$";

            for (var i = 0; i < incomes.Count; i++)
            {
                for (var j = 0; j < incomes[0].Length; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = incomes[i][j];
                }
            }
            for (var i = 0; i < expenses.Count; i++)
            {
                for (var j = 0; j < expenses[0].Length; j++)
                {
                    dataGridView2.Rows[i].Cells[j].Value = expenses[i][j];
                }
            }
        }

        public List<string[]> getData(string request)
        {
            connection.Open();

            var command = new SQLiteCommand(String.Format(request), connection);
            var tempList = new List<string>();
            var result = new List<string[]>();

            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tempList.Clear();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            tempList.Add(Convert.ToString(reader.GetValue(i)));
                        }
                        var tempArray = new string[tempList.Count];
                        tempList.CopyTo(tempArray);
                        result.Add(tempArray);
                    }
                }
            }
            connection.Close();
            return result;
        }
    }
}