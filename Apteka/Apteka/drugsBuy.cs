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
    public partial class drugsBuy : Form
    {
        UserForm UserForm;
        newDrug newDrug;
        string BDconnect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Desktop\ЛАБ\Apteka\Apteka\Database1.mdf;Integrated Security=True;";
        string sqlQuery;
        public drugsBuy()
        {          
            InitializeComponent();
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            this.ControlBox = false;
        }
        //виведення даних з БД
        private void drugsBuy_Load(object sender, EventArgs e)
        {
            //Вивід даних в таблицю
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                sqlQuery = "SELECT * FROM Drugs";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    // Встановлюємо дані в DataGridView
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Dock = DockStyle.Fill;

                }
                //Вивід рахунку користувача
                sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    label2.Text = (string)command.ExecuteScalar().ToString();
                }
            }
        }
        //повернення на головну форму
        private void button1_Click(object sender, EventArgs e)
        {
            UserForm = new UserForm();
            UserForm.Show();
            this.Close();
        }
        //замовлення ліків
        private void button2_Click(object sender, EventArgs e)
        {
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
            //перевірка рахунку
            int cost = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["Cost"].Value);
            int money = Convert.ToInt32(label2.Text);
            if ((money-cost)<0)
            {
                MessageBox.Show("Недостатньо грошей на рахунку. Поповніть рахунок на сторінці користувача.");
                return;
            }
            string id = dataGridView1.Rows[rowIndex].Cells["Id_drug"].Value.ToString();
            DateTime currentDateAndTime = DateTime.Now;

            // Додавання Замовлення в БД
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                sqlQuery = "INSERT INTO Orders (id_user, id_drug, Order_date) VALUES (@id_user, @id_drug, @Order_date)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    command.Parameters.AddWithValue("@id_drug", id);
                    command.Parameters.AddWithValue("@Order_date", currentDateAndTime);

                    command.ExecuteNonQuery();
                }
                //Вивід грошей з рахунку
                money -= cost;
                sqlQuery = "UPDATE Users SET Money = @Money WHERE id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    command.Parameters.AddWithValue("@Money", money);
                    command.ExecuteNonQuery();
                }
                sqlQuery = "SELECT Money FROM Users WHERE id_user = @id_user";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@id_user", GlobalVariables.user_id);
                    label2.Text = (string)command.ExecuteScalar().ToString();
                }

            }

            MessageBox.Show("Замовлення успішно створене. Перевірте ваші замовлення на сторінці користувача.");
        }
        //Відкриття вікна створення нових ліків
        private void button3_Click(object sender, EventArgs e)
        {
            newDrug = new newDrug();
            newDrug.Show();
            this.Close();
        }
    }
}
