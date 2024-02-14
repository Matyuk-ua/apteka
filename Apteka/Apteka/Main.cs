using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Apteka
{
    
    public partial class Main : Form
    {
        bool alreadyClicked1;
        bool alreadyClicked2;
        AddUser AddUser;
        UserForm userForm;
        //підключення до бази даних(БД)
        string BDconnect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Desktop\ЛАБ\Apteka\Apteka\Database1.mdf;Integrated Security=True;";
        string sqlQuery;
        
        public Main()
        {
            InitializeComponent();
            alreadyClicked1 = false;
            alreadyClicked2 = false;
        }
        //прибирає текст з поля один раз
        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!alreadyClicked1)
            {
                textBox1.Text = "";

                alreadyClicked1 = true;
            }
        }
        //прибирає текст з поля один раз
        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (!alreadyClicked2)
            {
                textBox2.Text = "";

                alreadyClicked2 = true;
            }
        }
        //відкриває вікно реєстрації
        private void button2_Click(object sender, EventArgs e)
        {
            AddUser = new AddUser();
            AddUser.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                int result;
                //перевіряє пароль і ім'я
                sqlQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND password = @Password";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", textBox1.Text);
                    command.Parameters.AddWithValue("@Password", textBox2.Text);
                    result = Convert.ToInt32(command.ExecuteScalar());
                }
                if (result == 1)
                {
                    //зберігає айді в глобальну змінну
                    sqlQuery = "SELECT Id_user FROM Users WHERE Username = @Username AND password = @Password";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", textBox1.Text);
                        command.Parameters.AddWithValue("@Password", textBox2.Text);
                        GlobalVariables.user_id = Convert.ToInt32(command.ExecuteScalar());
                    }
                    //відкриває віно користувача
                    userForm = new UserForm();
                    userForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("помилка: неправильні ім'я або пароль");
                }
            }                
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
