using Dark.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class AddUserPopup : Window
    {
        public AddUserPopup()
        {
            InitializeComponent();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);
        }


        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            
            string taskName = noteName.Text.Trim();   
            string taskEmail = noteEmail.Text.Trim(); 
            string taskPass = notePass.Text.Trim();   

           
            string accountType = "";
            if (adminBtn.IsChecked == true)
                accountType = "ADMIN";
            else if (userBtn.IsChecked == true)
                accountType = "USER";
            else
            {
                MessageBox.Show("Please select an account type!");
                return;
            }

            
            if (string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(taskEmail) || string.IsNullOrEmpty(taskPass))
            {
                MessageBox.Show("All fields must be filled!");
                return;
            }

            AddTaskToDatabase(taskName, taskEmail, taskPass, accountType);
        }
        private void AddTaskToDatabase(string name, string email, string password, string accountType)
        {
            {
                try
                {
                  
                    MySqlConnection conn = dbConnect.GetConnection();

                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    
                    string query = @"
            INSERT INTO USERS (USERNAME, EMAIL, PASSWORD, CREATION_DATE, TASKS_COMPLETED, TASKS_ASSIGNED, ACCOUNT_TYPE)
            VALUES (@name, @email, @password, @creation, 0, 0, @accountType);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@creation", DateTime.Now);
                        cmd.Parameters.AddWithValue("@accountType", accountType);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("User added successfully!");
                    noteName.Clear();
                    noteEmail.Clear();
                    notePass.Clear();
                    adminBtn.IsChecked = false;
                    userBtn.IsChecked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message);
                }
            }
        }
    }
}
