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

namespace TeamNoter.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ContentTemplate.xaml
    /// </summary>
    public partial class databaseContent : UserControl
    {
        private string exportFilePath = string.Empty;
        public databaseContent()
        {
            InitializeComponent();
        }

        private void filepathExportBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Filepath for exports", e);
        }

        private void exportBtnCSV_Click(object sender, RoutedEventArgs e)
        {
            string filePath = filepathExportBox.Text;

            if (string.IsNullOrWhiteSpace(filePath) || filePath == "Filepath for exports")
            {
                Utility.NoterMessage("Missing File Path", "Please enter a valid export file path.");
                return;
            }

            using (MySqlConnection conn = dbConnect.GetConnection())
            {
                conn.Open();
                try
                {
                    Utility.NoterMessage("Export Started", "Exporting data from TASKS table...");

                    using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("TASK_ID,DEADLINE,TASK_NAME,TASK_DESCRIPTION,IS_COMPLETED");

                        string query = "SELECT TASK_ID, DEADLINE, TASK_NAME, TASK_DESCRIPTION, IS_COMPLETED FROM TASKS WHERE IS_COMPLETED = FALSE";

                        using (var cmd = new MySqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                writer.WriteLine($"{reader["TASK_ID"]},{reader["DEADLINE"]},{reader["TASK_NAME"]},{reader["TASK_DESCRIPTION"]},{reader["IS_COMPLETED"]}");
                            }
                        }
                    }

                    Utility.NoterMessage("Export Complete", $"CSV file successfully created at:\n{filePath}");
                }
                catch (Exception ex)
                {
                    Utility.NoterMessage("Error", $"An error occurred while exporting:\n{ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            bool mustQuote = value.Contains(",") || value.Contains("\"") || value.Contains("\n");
            if (mustQuote)
            {
                value = value.Replace("\"", "\"\"");
                value = $"\"{value}\"";
            }
            return value;
        }



        private void filepathExportBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            exportFilePath = filepathExportBox.Text;
            Utility.PlaceholderText(sender, "Filepath for exports", e);

        }
    }
}
