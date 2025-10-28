using Dark.Net;
using MySql.Data.MySqlClient;
using System;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using TeamNoter.Windows.UserControls;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class AddPopup : Window
    {
        bool initializing = true;
        bool isInvalidUsername = false;
        List<int> validList = new List<int>();

        Dashboard origin;
        public DataStorage dataStorage = new DataStorage();
        public AddPopup(Dashboard originDashboard)
        {
            InitializeComponent();

            this.origin = originDashboard;
            this.DataContext = dataStorage;

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            initializing = false;
        }

        private void proceedCheck()
        {
            if (!initializing)
            {
                proceedBtn.IsEnabled =
                (!string.IsNullOrWhiteSpace(noteName.Text) && noteName.Text != "Title") &&
                (calendar.Value.HasValue && (calendar.Value >= DateTime.Now)) &&
                !isInvalidUsername && priorityCB.SelectedIndex > 0;
            }
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            // kunin nya yung value sa textbox
            string taskName = noteName.Text.Trim();
            string taskDescription =
                noteDetails.Text == "Details (Optional)" || string.IsNullOrWhiteSpace(noteDetails.Text) ?
                "N/A" : noteDetails.Text.Trim();

            DateTime deadline = calendar.Value.HasValue ? calendar.Value.Value : DateTime.Now;


            // tatawagin yung database method na ginawa dito sa baba 
            AddTaskToDatabase(taskName, taskDescription, deadline, validList);
            //eto sa baba        
    }
        private void AddTaskToDatabase(string name, string description, DateTime deadline, List<int> assignedUsers)
        {
            try
            { 
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    string queryString = @"
                    INSERT INTO TASKS (TASK_NAME, TASK_DESCRIPTION, DATE_CREATED, DEADLINE, IS_COMPLETED, PRIORITY)
                    VALUES (@name, @description, @created, @deadline, @completed, @priority)";
                    using (MySqlCommand query = new MySqlCommand(queryString, conn))
                    {
                        query.Parameters.AddWithValue("@name", name);
                        query.Parameters.AddWithValue("@description", description);
                        query.Parameters.AddWithValue("@created", DateTime.Now);
                        query.Parameters.AddWithValue("@deadline", deadline);
                        query.Parameters.AddWithValue("@completed", false);
                        query.Parameters.AddWithValue("@priority", priorityCB.Text.ToUpper());
                        query.ExecuteNonQuery();
                    }

                    foreach (int userID in assignedUsers)
                    {
                        queryString = @"INSERT INTO USER_TASKS (USER_ID, TASK_ID) VALUES (@userID, LAST_INSERT_ID());";

                        using (MySqlCommand query = new MySqlCommand(queryString, conn))
                        {
                            query.Parameters.AddWithValue("@userID", userID);
                            query.ExecuteNonQuery();
                        }
                    }

                    // probs quickest way i can "refresh" the page.
                    if (this.origin.contentPane.Content is tasksContent)
                        this.origin.contentPane.Content = new tasksContent();

                    Utility.NoterMessage("Task addition successful", "Task has been added successfully");

                    // this just effectively "clears" it. .Clear() doesn't work due to how placeholder text is programmed.
                    noteName.Text = "Title";
                    noteName.Foreground = Utility.HexConvert("#FF777777");

                    noteDetails.Text = "Details (Optional)";
                    noteDetails.Foreground = Utility.HexConvert("#FF777777");

                    noteUsers.Text = "Assigned Users (Optional)";
                    noteUsers.Foreground = Utility.HexConvert("#FF777777");

                    calendar.Value = DateTime.Now;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Task addition failure", ex.Message);
            }
        }

        private void noteName_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Title", e);
        }

        private void noteDetails_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Details (Optional)", e);
        }

        private void noteUsers_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Assigned Users (Optional)", e);

            if (noteUsersWarning.Visibility == Visibility.Visible &&
                noteUsers.Text == "Assigned Users (Optional)")
            {
                noteUsersWarning.Visibility = Visibility.Collapsed;
                noteUsersWarning.Content = "";
                noteUsers.Margin = new Thickness(0, 10, 0, 0);
            }
        }

        private void textBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            proceedCheck();
        }

        private void calendar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            proceedCheck();
        }

        private void calendar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            proceedCheck();
        }

        private void noteUsers_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!initializing && noteUsers.Text != "Assigned Users (Optional)" && !string.IsNullOrWhiteSpace(noteUsers.Text))
            {
                string usersInput = noteUsers.Text.Trim();
                HashSet<string> inputs = new HashSet<string>(usersInput.Split(","));
                List<string> invalidList = new List<string>();
                validList.Clear();

                foreach (string inputUsername in inputs)
                {
                    string trimmedInput = inputUsername.Trim();
                    bool found = false;
                    foreach (DataStorage.UserItem user in dataStorage.users)
                    {
                        if (user.Username.ToLower() == trimmedInput.ToLower())
                        {
                            found = true;
                            validList.Add(user.UserID);
                            break;
                        }
                    }
                    if (!found)
                        invalidList.Add(trimmedInput);
                }

                if (invalidList.Count > 0)
                {
                    string invalidUsernames = "";
                    foreach (string username in invalidList)
                    {
                        if (string.IsNullOrWhiteSpace(invalidUsernames))
                            invalidUsernames = username;
                        else
                            invalidUsernames += $", {username}";
                    }

                    noteUsers.Margin = new Thickness(0, 0, 0, 0);
                    noteUsersWarning.Visibility = Visibility.Visible;
                    noteUsersWarning.Content = $"The usernames {invalidUsernames} could not be found";
                    isInvalidUsername = true;
                }
                else
                {
                    noteUsers.Margin = new Thickness(0, 10, 0, 0);
                    noteUsersWarning.Visibility = Visibility.Collapsed;
                    isInvalidUsername = false;
                }
                    proceedCheck();
            }
        }

        private void priorityCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolidColorBrush elBrush;
            switch (priorityCB.SelectedIndex)
            {
                case 0: // Low priority
                    elBrush = Utility.HexConvert("#6b9675");
                    break;

                case 1: // Medium priority
                    elBrush = Utility.HexConvert("#b98246");
                    break;

                case 2: // High priority
                    elBrush = Utility.HexConvert("#d26161");
                    break;

                default:
                    elBrush = Utility.HexConvert("#FFFFFF");
                    break;
            }

            priorityCB.Foreground = elBrush;
        }
    }
}

