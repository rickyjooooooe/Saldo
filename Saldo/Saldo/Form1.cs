using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Saldo
{
    public partial class Form1 : Form
    {
        private MySqlConnection mySqlConnection;
        private MySqlCommand mySqlCommand;
        private MySqlDataAdapter mySqlDataAdapter;
        private string YourConnectionString = "server=localhost;uid=root;pwd=titi020504;database=latian";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            string customerName = label2.Text;
            decimal emoneyBalance = GetEmoneyBalance(customerName);
            label3.Text = emoneyBalance.ToString();
            button10.Click += MoneyButton_Click;
            button20.Click += MoneyButton_Click;
            button50.Click += MoneyButton_Click;
            button100.Click += MoneyButton_Click;
            button150.Click += MoneyButton_Click;
            button200.Click += MoneyButton_Click;
            button500.Click += MoneyButton_Click;
            button1000.Click += MoneyButton_Click;



        }
        private void MoneyButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string buttonText = button.Text;

            // Show a confirmation dialog
            DialogResult result = MessageBox.Show("Do you want to top up " + buttonText + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Extract the nominal value from the button text
                string nominal = buttonText.Replace(".", "").Replace(",", "").Replace("Rp", "");
                decimal topUpAmount = decimal.Parse(nominal);

                // Update the e-money balance in the database
                string customerName = label2.Text;
                UpdateEmoneyBalance(customerName, topUpAmount);

                // Refresh the e-money balance display
                decimal emoneyBalance = GetEmoneyBalance(customerName);
                label3.Text = emoneyBalance.ToString();

                // Show a success message
                MessageBox.Show("Successfully topped up " + buttonText + " to e-money balance.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            panel1.Visible = false;
        }
        private void UpdateEmoneyBalance(string customerName, decimal topUpAmount)
        {
            using (mySqlConnection = new MySqlConnection(YourConnectionString))
            {
                mySqlConnection.Open();

                string updateQuery = "UPDATE Customer SET e_money = e_money + @TopUpAmount WHERE Nama_customer = @CustomerName";

                using (mySqlCommand = new MySqlCommand(updateQuery, mySqlConnection))
                {
                    mySqlCommand.Parameters.AddWithValue("@TopUpAmount", topUpAmount);
                    mySqlCommand.Parameters.AddWithValue("@CustomerName", customerName);

                    mySqlCommand.ExecuteNonQuery();
                }
            }
        }

        private decimal GetEmoneyBalance(string customerName)
        {
            decimal emoneyBalance = 0;

            using (mySqlConnection = new MySqlConnection(YourConnectionString))
            {
                mySqlConnection.Open();

                string query = "SELECT e_money FROM Customer WHERE Nama_customer = @CustomerName";

                using (mySqlCommand = new MySqlCommand(query, mySqlConnection))
                {
                    mySqlCommand.Parameters.AddWithValue("@CustomerName", customerName);

                    using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emoneyBalance = reader.GetDecimal(0); // Assuming the e_money column is of DECIMAL data type
                        }
                    }
                }
            }

            return emoneyBalance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }
    }
}