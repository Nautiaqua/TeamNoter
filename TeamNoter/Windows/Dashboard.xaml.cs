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
using System.Windows.Shapes;
using TeamNoter.Windows.UserControls;

namespace TeamNoter
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        bool sidebarExpanded;
        int expandedSize = 190;
        int collapsedSize = 40;
        public Dashboard()
        {
            InitializeComponent();

            // Sets the content pane (THE RIGHT ONE) to tasks, which should be the default.
            contentPane.Content = new tasksContent(); 
        }

        private void expandBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!sidebarExpanded) // Sidebar ISN'T expanded
            {
                // Expands the buttons and reveals their respective titles.
                mainGrid.ColumnDefinitions[0].Width = new GridLength(200);
                sidebarExpanded = true;

                accLabel.Visibility = Visibility.Visible;
                accBtn.Width = expandedSize;
                
                dbLabel.Visibility = Visibility.Visible;
                dbBtn.Width = expandedSize;
                
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
                mainGrid.ColumnDefinitions[0].Width = new GridLength(50);
                sidebarExpanded = false;

                accLabel.Visibility = Visibility.Hidden;
                accBtn.Width = collapsedSize;

                dbLabel.Visibility = Visibility.Hidden;
                dbBtn.Width = collapsedSize;

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
        }

        private void dbBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new databaseContent();
        }

        private void manageBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new manageContent();
        }

        private void taskBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPane.Content = new tasksContent();
        }
    }
}
