using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Apteka
{
    public partial class UserForm : Form
    {
        Main main;
        drugsBuy drugsBuy;
        //підключення до БД
        string BDconnect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Desktop\ЛАБ\Apteka\Apteka\Database1.mdf;Integrated Security=True;";
        string sqlQuery;
        bool check = true;
        public UserForm()
        {
            InitializeComponent();
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            this.ControlBox = false;
        }
        //вихід до сторінки входу
        private void button1_Click(object sender, EventArgs e)
        {
            main = new Main();
            main.Show();
            this.Close();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            //заповнення таблиці
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                string sqlQuery = "SELECT O.id_order, O.Order_date, D.Name, O.id_drug, D.Strg_period " +
                                  "FROM Orders O " +
                                  "JOIN Drugs D ON O.id_drug = D.id_drug " +
                                  "WHERE O.id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
                // Встановленння даних в DataGridView
                dataGridView1.DataSource = dataTable;
                dataGridView1.Dock = DockStyle.Fill;
                //заповнення даних про користувача (ім'я та рахунок)
                sqlQuery = "SELECT Username, Money FROM Users WHERE id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        label3.Text = reader["Username"].ToString();
                        label4.Text = reader["Money"].ToString();
                    }
                    reader.Close();
                }
            }
            //перевірка на просрочені ліки
            //прочрочені ліки виділяються ЧЕРВОНИМ кольором
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DateTime date = Convert.ToDateTime(row.Cells["Order_date"].Value);
                int storage =Convert.ToInt32(row.Cells["Strg_period"].Value);
                DateTime lastDay = date.AddDays(storage);
                if (DateTime.Now > lastDay)
                {
                    dataGridView1.CurrentCell = null;
                    row.Selected = true;
                    row.DefaultCellStyle.BackColor = Color.Red;
                    if (check == true)
                    {
                        MessageBox.Show("У вас є просрочені ліки. ПРИБЕРІТЬ ЇХ НЕГАЙНО. Просрочені ліки будуть виділені червоним.");
                        check = false;
                    }
                }
            }
        }
        //Елементи щоб поповнити рахунок
        private void button3_Click(object sender, EventArgs e)
        {
            button4.Show();
            label5.Show();
            label7.Show();
            textBox1.Show();
            textBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int money;
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                //вивід поточного рахунку
                connection.Open();
                sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    money = (int)command.ExecuteScalar();
                    //поповнення грошей
                    try
                    {
                        money += int.Parse(textBox1.Text);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Введені неправильні дані");
                        return;
                    }

                    // Оновлення данниї в базі даних
                    sqlQuery = "UPDATE Users SET Money = @Money WHERE id_user = @id_user";
                    command.CommandText = sqlQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    command.Parameters.AddWithValue("@Money", money);
                    command.ExecuteNonQuery();

                    // вивід ононвленого рахунку
                    sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                    command.CommandText = sqlQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    label4.Text = command.ExecuteScalar().ToString();
                }
            }
            button4.Hide();
            label5.Hide();
            label7.Hide();
            textBox1.Hide();
        }
        //вікно замовлення ліків
        private void button2_Click(object sender, EventArgs e)
        {
            drugsBuy = new drugsBuy();
            drugsBuy.Show();
            this.Close();
        }
        //поверненння грошей
        private void button5_Click(object sender, EventArgs e)
        {
            //виділення одного рядка
            int rowIndex = 0;
            try
            {
                rowIndex = dataGridView1.SelectedRows[0].Index;
            }
            catch (Exception)
            {
                MessageBox.Show("Помилка. виберіть один рядок");
                return;
            }
            //перевірка чи просрочені ліки позначені червоним кольором
            //за просрочені ліки не повертаються гроші
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            Color rowColor = row.DefaultCellStyle.BackColor;
            if (rowColor == Color.Red)
            {
                MessageBox.Show("гроші за просрочені ліки не повертаються.");
                return;
            }
            //зберігання айді замовлення та ліків
            int Delete = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["id_order"].Value);
            int Cost = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["id_drug"].Value);
            int money;
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                //повернення грошей за покупку ліків
                connection.Open();
                sqlQuery = "SELECT Cost FROM Drugs WHERE Id_drug = @Id_drug";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_drug", Cost);
                    money = (int)command.ExecuteScalar();

                    sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                    command.CommandText = sqlQuery;
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    money += (int)command.ExecuteScalar();

                    // Оновлення БД
                    sqlQuery = "UPDATE Users SET Money = @Money WHERE id_user = @id_user";
                    command.CommandText = sqlQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    command.Parameters.AddWithValue("@Money", money);
                    command.ExecuteNonQuery();

                    // Вивід оновленного рахунку
                    sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                    command.CommandText = sqlQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    label4.Text = command.ExecuteScalar().ToString();
                }
                //видалення замовлення з БД
                sqlQuery = "DELETE FROM Orders WHERE id_order = @id_order";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_order", Delete);
                    command.ExecuteNonQuery();
                }
            }
            // Видалення рядку
            dataGridView1.Rows.RemoveAt(rowIndex);

        }
        //елементи пошуку ліків
        private void button6_Click(object sender, EventArgs e)
        {
            button7.Show();
            textBox2.Show();
            radioButton1.Show();
            radioButton2.Show();
            radioButton3.Show();

        }
        //сховати елементи
        private void button7_Click(object sender, EventArgs e)
        {
            button7.Hide();
            textBox2.Hide();
            textBox2.Text = "";
            dataGridView1.CurrentCell = null;
            radioButton1.Hide();
            radioButton2.Hide();
            radioButton3.Hide();
        }
        //пошук ліків
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Visible == true)
            {
                dataGridView1.CurrentCell = null;
                string searchText = textBox2.Text;

                // Пройтися по кожному рядку у DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string cellValue = row.Cells["Name"].Value.ToString();
                    if (radioButton1.Checked == true) 
                    {
                        //якщо пошук за номером замовлення
                        cellValue = row.Cells["Id_order"].Value.ToString();
                    }
                    else if (radioButton1.Checked == true) 
                    {
                        //якщо пошук за назвою
                        cellValue = row.Cells["Name"].Value.ToString();
                    }
                    else if (radioButton1.Checked == true) 
                    {
                        //якщо пошук за датою замовлення
                        cellValue = row.Cells["Order_date"].Value.ToString();
                    }
                    // Перевірити, чи співпадає значення зі шуканим текстом
                    if (cellValue.Equals(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        row.Selected = true; // Виділити рядок
                        dataGridView1.FirstDisplayedScrollingRowIndex = row.Index; // Прокрутити до видимого рядка
                    }
                }
            }
        }
        //скидає дані пошуку при зміні. Щоб не виникало помилки
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
            textBox2.Text = "";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
            textBox2.Text = "";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
            textBox2.Text = "";
        }
        //видаленнія просрочених ліків
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {
                    int deleteId = Convert.ToInt32(selectedRow.Cells["id_order"].Value);

                    using (SqlConnection connection = new SqlConnection(BDconnect))
                    {
                        connection.Open();
                        string sqlQuery = "DELETE FROM Orders WHERE id_order = @id_order";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id_order", deleteId);
                            command.ExecuteNonQuery();
                        }
                    }

                    dataGridView1.Rows.Remove(selectedRow);
                }
            }
            else
            {
                MessageBox.Show("Немає вибраних рядків для видалення.");
            }
        }
    }
}
