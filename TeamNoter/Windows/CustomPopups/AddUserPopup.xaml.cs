using Dark.Net;
using MySql.Data.MySqlClient;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeamNoter.Windows.UserControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class AddUserPopup : Window
    {
        string regexPattern = @"^[^@]+@[^@]+\.[^@]+$";
        bool init = true;
        Dashboard dashboard;
        public AddUserPopup(Dashboard dashboard)
        {
            InitializeComponent();

            this.dashboard = dashboard;
            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            init = false;
        }

        private void proceedCheck()
        {

            if (!init)
            {
                proceedBtn.IsEnabled =
                    (!string.IsNullOrWhiteSpace(noteName.Text) && noteName.Text != "Username") &&
                    (!string.IsNullOrWhiteSpace(noteEmail.Text) && noteEmail.Text != "Email") &&
                    Regex.IsMatch(noteEmail.Text, regexPattern, RegexOptions.IgnoreCase) &&
                    (!string.IsNullOrWhiteSpace(notePass.Text) && notePass.Text != "Password");
            }
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            
            string taskName = noteName.Text.Trim();   
            string taskEmail = noteEmail.Text.Trim(); 
            string taskPass = notePass.Text.Trim();   

           
            string accountType = "";
            if (typeCB.SelectedIndex == 0)
                accountType = "ADMIN";
            else if (typeCB.SelectedIndex == 1)
                accountType = "USER";
            
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

                    Utility.NoterMessage("Added user", "User added successfully!");
                    noteName.Text = "Username";
                    noteName.Foreground = Utility.HexConvert("#FF777777");

                    noteEmail.Text = "Email";
                    noteEmail.Foreground = Utility.HexConvert("#FF777777");

                    notePass.Text = "Password";
                    notePass.Foreground = Utility.HexConvert("#FF777777");

                    typeCB.SelectedIndex = 0;

                    this.dashboard.contentPane.Content = new manageContent(this.dashboard);
                    this.Close();

                }
                catch (Exception ex)
                {
                    Utility.NoterMessage("Error adding user: ", ex.Message);
                }


            }
        }

        private void noteName_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Username", e);
        }

        private void noteEmail_Placholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Email", e);

            
        }

        private void notePass_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Password", e);

        }
       
        private void textboxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            proceedCheck();
        }

        private void noteEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(noteEmail.Text, regexPattern, RegexOptions.IgnoreCase) &&
                noteEmail.Text != "Email" || string.IsNullOrWhiteSpace(noteEmail.Text))
            {
                noteEmailWarning.Visibility = Visibility.Visible;
                noteEmail.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                noteEmailWarning.Visibility = Visibility.Collapsed;
                noteEmail.Margin = new Thickness(0, 10, 0, 0);
            }

            proceedCheck();
        }
    }
}
