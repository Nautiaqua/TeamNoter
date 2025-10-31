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

        private async void exportBtnSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "SQL files (*.sql)|*.sql",
                    Title = "Export SQL File from TeamNoter Database"
                };

                if (openDialog.ShowDialog() != true)
                    return;

                string filePath = openDialog.FileName;

                using (var conn = dbConnect.GetConnection())
                using (var cmd = conn.CreateCommand())
                using (var mb = new MySqlBackup(cmd))
                {
                    await Task.Run(() =>
                    {
                        conn.Open();
                        mb.ExportToFile(filePath);
                    });

                }

                Utility.NoterMessage("Export Complete", $"Database successfully exported from:\n{filePath}");
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Export Error", $"An error occurred:\n{ex.Message}");
            }
        }

        private async void importBtnSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "SQL files (*.sql)|*.sql",
                    Title = "Import SQL File into TeamNoter Database"
                };

                if (openDialog.ShowDialog() != true)
                    return;

                string filePath = openDialog.FileName;

                using (var conn = dbConnect.GetConnection())
                using (var cmd = conn.CreateCommand())
                using (var mb = new MySqlBackup(cmd))
                {
                    await Task.Run(() =>
                    { 
                        conn.Open();
                        mb.ImportFromFile(filePath);
                    });
                    
                }

                Utility.NoterMessage("Import Complete", $"Database successfully imported from:\n{filePath}");
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Import Error", $"An error occurred:\n{ex.Message}");
            }
        }
    }
}
