using Dark.Net;
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
        public tasksContent()
        {
            InitializeComponent();
        }

        static string searchBoxPlaceholder = "Search for tasks, users, deadlines...";

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(searchBox, searchBoxPlaceholder, true);
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(searchBox, searchBoxPlaceholder, false);
        }

        private void filter1_Checked(object sender, RoutedEventArgs e)
        {
            Utility.checkboxBGHandler(sender, "#FFF7BB64");
        }

        private void filter2_Checked(object sender, RoutedEventArgs e)
        {
            Utility.checkboxBGHandler(sender, "#FFF7BB64");
        }
    }
}
