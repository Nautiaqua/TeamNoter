using Dark.Net;
using Microsoft.VisualBasic.ApplicationServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static TeamNoter.DataStorage;

namespace TeamNoter.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ContentTemplate.xaml
    /// </summary>
    public partial class tasksContent : UserControl
    {
        public CollectionView taskView;
        public bool initializing = true;
        public DataStorage dataStorage = new DataStorage();
        bool init = true;
        public tasksContent()
        {
            InitializeComponent();
            init = false;

            this.DataContext = dataStorage;
            
            taskView = (CollectionView)CollectionViewSource.GetDefaultView(dataStorage.tasks);

            initializing = false;
            filterHandler();
        }

        static string searchBoxPlaceholder = "Search for task titles";
        static string userSearchPlaceholder = "Search for user";

        private void filterHandler()
        {
            if (LoginData.AccountType == "USER")
            {
                userSearchBox.Visibility = Visibility.Collapsed;
                userSearchBox.Text = LoginData.Username;
            }

            string titleSearchText = searchBox.Text.ToLower();
            string userSearchText = userSearchBox.Text.ToLower();

            bool titleSearch_IsDefault = searchBox.Text == searchBoxPlaceholder || string.IsNullOrWhiteSpace(searchBox.Text);
            bool userSearch_IsDefault = userSearchBox.Text == userSearchPlaceholder || string.IsNullOrWhiteSpace(userSearchBox.Text);

            taskView.Filter = item =>
            {
                if (item is DataStorage.TaskItem taskItem)
                {
                    // check for if the item matches the search query
                    bool isMatchingTitle = titleSearch_IsDefault || taskItem.Title.ToLower().Contains(titleSearchText);
                    bool isMatchingUser = userSearch_IsDefault || taskItem.Users.ToLower().Contains(userSearchText);
                    bool isDueToday = filter1.IsChecked == false || taskItem.Deadline.Date.Equals(DateTime.Now);
                    bool isIncomplete = taskItem.IsCompleted == false;

                    switch (mainViewCB.SelectedIndex)
                    {
                        case 0:
                            return isMatchingTitle && isMatchingUser && isIncomplete && isDueToday;
                        case 1:
                            return isMatchingTitle && isMatchingUser && !isIncomplete && isDueToday;
                        default:
                            return false;
                    }
                }
                else
                    return false;

            };

            taskView.Refresh();
        }

        private void searchBox_Placeholder(object sender, RoutedEventArgs e)   
        {
            Utility.PlaceholderText(sender, searchBoxPlaceholder, e);
        }

        private void userSearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, userSearchPlaceholder, e);
        }

        private void task_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is DataStorage.TaskItem task)
            {
                bool isComplete = checkBox.IsChecked == true;
                        
                
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    string queryString = " UPDATE TASKS SET IS_COMPLETED = @completionBool WHERE TASK_ID = @currentTaskID";
                    MySqlCommand query = new MySqlCommand(queryString, conn);
                    query.Parameters.AddWithValue("@currentTaskID", task.TaskID);
                    query.Parameters.AddWithValue("@completionBool", isComplete ? 1 : 0);

                    query.ExecuteNonQuery();

                    // Utility.NoterMessage("FINISHED", "COMPLETED TASK");
                    conn.Close();
                }

                taskView.Refresh();
            }
        }

        private void filter1_Click(object sender, RoutedEventArgs e)
        {
            filterHandler();
        }

        private void deleteBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button delBtn && delBtn.DataContext is DataStorage.TaskItem task)
            {
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    string queryString = "DELETE FROM TASKS WHERE TASK_ID = @currentTaskID";
                    MySqlCommand query = new MySqlCommand(queryString, conn);
                    query.Parameters.AddWithValue("@currentTaskID", task.TaskID);

                    dataStorage.tasks.Remove(task);

                    query.ExecuteNonQuery();
                    conn.Close();
                }

                taskView.Refresh();
            }
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            taskView.Refresh();
        }

        private void mainViewCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!init)
            {
                filterHandler();
            }
            
        }
    }
}
