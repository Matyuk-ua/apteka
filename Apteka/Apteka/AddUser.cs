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
    public partial class AddUser : Form
    {
        Main Main;
        string BDconnect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Desktop\ЛАБ\Apteka\Apteka\Database1.mdf;Integrated Security=True;";
        string sqlQuery;
        public AddUser()
        {
            InitializeComponent();
            this.ControlBox = false;
        }
        //вихід до головного меню
        private void button1_Click(object sender, EventArgs e)
        {
            Main = new Main();
            Main.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                sqlQuery = "INSERT INTO Users (Username, password) VALUES (@Username, @password)";
                //додавання користувача
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Додавання значень параметрів
                    command.Parameters.AddWithValue("@Username", textBox1.Text);
                    command.Parameters.AddWithValue("@password", textBox2.Text);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {

                        if (ex.Number == 2627) // Код помилки унікальності
                        {
                            MessageBox.Show(" ім'я вже зайнято ");
                            return;
                        }
                        else
                        {
                            // Інші типи помилок SqlException
                            MessageBox.Show(ex.Message);
                        }

                    }
                    MessageBox.Show("Новий користувач успішно створений");
                }
            }
            Main = new Main();
            Main.Show();
            this.Close();
        }
    }
}
