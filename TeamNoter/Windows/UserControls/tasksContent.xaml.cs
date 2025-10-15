using Dark.Net;
using Microsoft.VisualBasic.ApplicationServices;
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

namespace TeamNoter.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ContentTemplate.xaml
    /// </summary>
    public partial class tasksContent : UserControl
    {
        public CollectionView taskView;
        public bool initializing = true;
        public tasksContent()
        {
            InitializeComponent();

            DataStorage dataStorage = new DataStorage();
            this.DataContext = dataStorage;
            taskView = dataStorage.getTaskView();

            initializing = false;
        }

        static string searchBoxPlaceholder = "Search for task titles";
        static string userSearchPlaceholder = "Search for users";

        private void searchBox_Placeholder(object sender, RoutedEventArgs e)   
        {
            Utility.PlaceholderText(sender, searchBoxPlaceholder, e);
        }

        private void userSearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, userSearchPlaceholder, e);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs eB)
        {
            if (!initializing)
            {
                string titleSearchText = searchBox.Text.ToLower();
                string userSearchText = userSearchBox.Text.ToLower();

                bool titleSearch_IsDefault = searchBox.Text == searchBoxPlaceholder || string.IsNullOrWhiteSpace(searchBox.Text);
                bool userSearch_IsDefault = userSearchBox.Text == userSearchPlaceholder || string.IsNullOrWhiteSpace(userSearchBox.Text);

                if (titleSearch_IsDefault && userSearch_IsDefault)
                {
                    taskView.Filter = null;
                }
                else
                {
                    taskView.Filter = item =>
                    {
                        var taskItem = item as DataStorage.TaskItem;

                        if (!titleSearch_IsDefault && !userSearch_IsDefault)
                        {
                            if (taskItem != null)
                                return taskItem.Title.ToLower().Contains(titleSearchText) &&
                                       taskItem.Users.ToLower().Contains(userSearchText);
                            else
                                return false;
                        }
                        else if (!titleSearch_IsDefault && userSearch_IsDefault)
                        {
                            if (taskItem != null)
                                return taskItem.Title.ToLower().Contains(titleSearchText);
                            else
                                return false;
                        }
                        else if (!userSearch_IsDefault && titleSearch_IsDefault)
                        {
                            if (taskItem != null)
                                return taskItem.Users.ToLower().Contains(userSearchText);
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }

                taskView.Refresh();

                if (userSearchBox.Text == LoginData.Username)
                    filter2.IsChecked = true;

                else
                    filter2.IsChecked = false;
            }
        }

        private void filter2_Click(object sender, RoutedEventArgs e)
        {
            if (filter2.IsChecked == true)
            {
                userSearchBox.Text = LoginData.Username;
                userSearchBox.Foreground = Utility.HexConvert("#FFFFFFFF");
            }
            else
            {
                userSearchBox.Text = userSearchPlaceholder;
                userSearchBox.Foreground = Utility.HexConvert("#FF777777");
            }
        }

        
    }
}
