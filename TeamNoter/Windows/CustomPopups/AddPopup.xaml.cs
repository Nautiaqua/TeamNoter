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
using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class AddPopup : Window
    {
        public AddPopup()
        {
            InitializeComponent();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            // kunin nya yung value sa textbox
            string taskName = noteName.Text.Trim();
            string taskDescription = noteDetails.Text.Trim();
            string assignedUser = assignedUsers.Text.Trim();

            // check nya kung may date ba kayo :)
            if (!calendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a deadline date!");
                return;
            }

            DateTime deadline = calendar.SelectedDate.Value;

            // dapat may laman yung taskName box
            if (string.IsNullOrEmpty(taskName))
            {
                MessageBox.Show("Task name cannot be empty!");
                return;
            }

            // tatawagin yung database method na ginawa dito sa baba 
            AddTaskToDatabase(taskName, taskDescription, deadline, assignedUser);
            //eto sa baba        
    }
        private void AddTaskToDatabase(string name, string description, DateTime deadline, string assignedUser)
        {
            try
            {
                // open the connection this is the connection that krystoff made
                MySqlConnection conn = dbConnect.GetConnection();//<--------

                if (conn.State != System.Data.ConnectionState.Open)//<---- checks if its opwn
                    conn.Open();

                // make a query 
                string query = @"
                    INSERT INTO TASKS (TASK_NAME, TASK_DESCRIPTION, DATE_CREATED, DEADLINE, IS_COMPLETED)
                    VALUES (@name, @description, @created, @deadline, @completed)";
                // insert that query in the Mysqlcommand function
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@deadline", deadline);
                    cmd.Parameters.AddWithValue("@completed", false);
                    //execute the query that you inserted
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Task added successfully!");

                // Optional: clear inputs
                noteName.Clear();
                noteDetails.Clear();
                assignedUsers.Clear();
                calendar.SelectedDate = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error adding task: " + ex.Message);
            }
        }
    }
}

