using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
            {
                try
                {
                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "SQL files (*.sql)|*.sql",
                        Title = "Export TeamNoter Database to SQL File"
                    };

                    if (saveDialog.ShowDialog() != true)
                        return;

                    string filePath = saveDialog.FileName;
                    string databaseName = "teamnoter"; 

                    using (var conn = dbConnect.GetConnection())
                    {
                        conn.Open();

                        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            writer.WriteLine($"-- TeamNoter Database Backup");
                            writer.WriteLine($"-- Database: `{databaseName}`");
                            writer.WriteLine($"-- Exported at {DateTime.Now}");
                            writer.WriteLine();

                            
                            List<string> tables = new List<string>();
                            using (var tableCmd = new MySqlCommand("SHOW TABLES", conn))
                            using (var reader = tableCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                    tables.Add(reader.GetString(0));
                            }

                            foreach (string table in tables)
                            {
                              
                                using (var createCmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`;", conn))
                                using (var createReader = createCmd.ExecuteReader())
                                {
                                    if (createReader.Read())
                                    {
                                        writer.WriteLine($"-- ----------------------------");
                                        writer.WriteLine($"-- Table structure for `{table}`");
                                        writer.WriteLine($"-- ----------------------------");
                                        writer.WriteLine($"DROP TABLE IF EXISTS `{table}`;");
                                        writer.WriteLine(createReader["Create Table"].ToString() + ";");
                                        writer.WriteLine();
                                    }
                                }

                                
                                writer.WriteLine($"-- ----------------------------");
                                writer.WriteLine($"-- Dumping data for table `{table}`");
                                writer.WriteLine($"-- ----------------------------");

                                using (var dataCmd = new MySqlCommand($"SELECT * FROM `{table}`;", conn))
                                using (var dataReader = dataCmd.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        List<string> values = new List<string>();
                                        for (int i = 0; i < dataReader.FieldCount; i++)
                                        {
                                            object val = dataReader.GetValue(i);
                                            if (val == DBNull.Value)
                                                values.Add("NULL");
                                            else
                                                values.Add("'" + val.ToString().Replace("'", "''") + "'");
                                        }

                                        writer.WriteLine($"INSERT INTO `{table}` VALUES ({string.Join(",", values)});");
                                    }
                                }

                                writer.WriteLine();
                            }
                        }

                        conn.Close();
                    }

                    Utility.NoterMessage("Export Complete", $"TeamNoter database exported to:\n{filePath}");
                }
                catch (Exception ex)
                {
                    Utility.NoterMessage("Export Error", $"An error occurred:\n{ex.Message}");
                }
            }
        }

        private void importBtnSQL_Click(object sender, RoutedEventArgs e)
        {
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
                    string sqlContent = File.ReadAllText(filePath);

                    using (var conn = dbConnect.GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;

                        
                        string[] sqlCommands = sqlContent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string command in sqlCommands)
                        {
                            string trimmed = command.Trim();
                            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("--") || trimmed.StartsWith("/*"))
                                continue;

                            try
                            {
                                cmd.CommandText = trimmed;
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                
                            }
                        }

                        conn.Close();
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
}
