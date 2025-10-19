using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeamNoter.Windows.CustomPopups;

namespace TeamNoter.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ContentTemplate.xaml
    /// </summary>
    public partial class profileContent : UserControl
    {
        public profileContent()
        {
            InitializeComponent();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            usernameBox.Text = LoginData.Username;
            emailBox.Text = LoginData.Email;
            uidBox.Text = LoginData.UserID.ToString();

            try
            {
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            (SELECT COUNT(TASK_ID) 
                            FROM USER_TASKS 
                            WHERE USER_ID = @currentUserID) AS ASSIGNED_TASKS,
                            (SELECT COUNT(*) 
                            FROM USER_TASKS 
                            JOIN TASKS ON USER_TASKS.TASK_ID = TASKS.TASK_ID
                            WHERE USER_TASKS.USER_ID = @currentUserID 
                            AND TASKS.IS_COMPLETED = TRUE) AS COMPLETED_TASKS;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@currentUserID", LoginData.UserID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LoginData.TasksAssigned = reader.GetInt32("ASSIGNED_TASKS");
                                LoginData.TasksCompleted = reader.GetInt32("COMPLETED_TASKS");
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading task data: " + ex.Message);
            }

            tasksAssignedBox.Text = LoginData.TasksAssigned.ToString();
            tasksAssignedBox1.Text = LoginData.TasksCompleted.ToString();
        }
    
private void recoverBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changepass = new ChangePassword();
            changepass.Show();
        }
    }
}
