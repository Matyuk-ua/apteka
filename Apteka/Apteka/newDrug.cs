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
    public partial class newDrug : Form
    {
        drugsBuy drugsBuy;
        string BDconnect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Desktop\ЛАБ\Apteka\Apteka\Database1.mdf;Integrated Security=True;";
        string sqlQuery;
        public newDrug()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drugsBuy = new drugsBuy();
            drugsBuy.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(BDconnect))
            {
                connection.Open();
                sqlQuery = "INSERT INTO Drugs (Name, Strg_period, Ingridients, Instructions, Cost, Amount) VALUES (@Name, @Strg_period, @Ingridients, @Instructions, @Cost, @Amount)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", textBox1.Text);
                    command.Parameters.AddWithValue("@Strg_period",Convert.ToInt32(textBox4.Text));
                    command.Parameters.AddWithValue("@Ingridients", textBox5.Text);
                    command.Parameters.AddWithValue("@Instructions", textBox6.Text);
                    command.Parameters.AddWithValue("@Cost", Convert.ToInt32(textBox2.Text));
                    command.Parameters.AddWithValue("@Amount", Convert.ToInt32(textBox3.Text));

                    command.ExecuteNonQuery();
                }

            }
            MessageBox.Show("Ліки додані.");
        }
    }
}
