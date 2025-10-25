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
using System.Xml.Linq;
using TeamNoter.Windows.CustomPopups;

namespace TeamNoter.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ContentTemplate.xaml
    /// </summary>
    public partial class manageContent : UserControl
    {
        DataStorage dataStorage = new DataStorage();

        Dashboard dashboard;
        public manageContent(Dashboard dashboard)
        {
            InitializeComponent();

            this.dashboard = dashboard;

            this.DataContext = dataStorage;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void searchBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Search for users", e);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            AddUserPopup adduser = new AddUserPopup(this.dashboard);
            adduser.Show();
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteUser del = new DeleteUser(this.dashboard);
            del.Show();
        }

        private void exportBtnSQL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importBtnSQL_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
