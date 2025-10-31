using Dark.Net;
using SharpVectors.Converters;
using SharpVectors.Renderers.Utils;
using SharpVectors.Renderers.Wpf;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeamNoter.Windows.CustomPopups;
using TeamNoter.Windows.UserControls;

namespace TeamNoter
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>

    public partial class Dashboard : Window
    {
        bool sidebarExpanded;
        bool isLogout = false;

        // Sizing for the sidebar itself
        static int expandedSidebarSize = 210;
        static int collapsedSidebarSize = 50;

        // Sizing for the sidebar buttons
        static int expandedSize = expandedSidebarSize - 10;
        static int collapsedSize = collapsedSidebarSize - 10;

        MainWindow Origin;
        public Dashboard(MainWindow LoginWindow)
        {
            InitializeComponent();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            // Let's us track the original instance of the app this dashboard came from.
            this.Origin = LoginWindow;

            // Sets the content pane (THE RIGHT SIDE) to tasks, which should be the default.
            contentPane.Content = new tasksContent();
            taskBtn.Background = Utility.HexConvert("#FF403F3D");

            

            // hides manageBtn if a user is logged in
            if (LoginData.AccountType == "USER")
            {
                manageBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        

        private void selectedHighlight(object sender)
        {
            Button[] sidebarButtons = new Button[] { accBtn, manageBtn, taskBtn, logoutBtn };

            if (sender is Button target)
            {
                foreach (Button button in sidebarButtons)
                {
                    if (button == target)
                        button.Background = Utility.HexConvert("#FF403F3D");
                    else
                        button.Background = sidebarDock.Background;
                }
            }
        }

        private void expandBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!sidebarExpanded) // Sidebar ISN'T expanded
            {
                // Expands the buttons and reveals their respective titles.
                mainGrid.ColumnDefinitions[0].Width = new GridLength(expandedSidebarSize);
                sidebarExpanded = true;

                accLabel.Visibility = Visibility.Visible;
                accBtn.Width = expandedSize;
                
                manageLabel.Visibility = Visibility.Visible;
                manageBtn.Width = expandedSize;
                
                taskLabel.Visibility = Visibility.Visible;
                taskBtn.Width = expandedSize;
                
                logoutLabel.Visibility = Visibility.Visible;
                logoutBtn.Width = expandedSize;
            }
            else // Sidebar IS expanded
            {
                // Hides the buttons and their respective titles.
                mainGrid.ColumnDefinitions[0].Width = new GridLength(collapsedSidebarSize);
                sidebarExpanded = false;

                accLabel.Visibility = Visibility.Hidden;
                accBtn.Width = collapsedSize;

                manageLabel.Visibility = Visibility.Hidden;
                manageBtn.Width = collapsedSize;

                taskLabel.Visibility = Visibility.Hidden;
                taskBtn.Width = collapsedSize;

                logoutLabel.Visibility = Visibility.Hidden;
                logoutBtn.Width = collapsedSize;
            }
        }

        private void accBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new profileContent();
            selectedHighlight(sender);
        }

        private void dbBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new databaseContent();
            selectedHighlight(sender);

        }

        private void manageBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new manageContent(this);
            selectedHighlight(sender);

        }

        private void taskBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new tasksContent();
            selectedHighlight(sender);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (isLogout == false) // closes the app itself if we're NOT logging out.
            {
                this.Origin.Close();
            }
            
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            isLogout = true;
            this.Origin.Show();
            this.Close();
        }

        private void addTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPopup addTask = new AddPopup(this);
            addTask.Show();
        }
    }
}
