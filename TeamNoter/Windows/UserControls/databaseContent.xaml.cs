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
                                var deadline = (DateTime)reader["DEADLINE"];
                                writer.WriteLine($"{reader["TASK_ID"]},{deadline:yyyy-MM-dd HH:mm:ss},{reader["TASK_NAME"]},{reader["TASK_DESCRIPTION"]},{reader["IS_COMPLETED"]}");
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

        private void importBtnCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Select a CSV file to import"
                };

                if (openFileDialog.ShowDialog() != true)
                    return;

                string filePath = openFileDialog.FileName;

                using (var conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    using (var reader = new StreamReader(filePath))
                    {
                        string? line;
                        bool isFirstLine = true;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (isFirstLine)
                            {
                                isFirstLine = false;
                                continue;
                            }

                            string[] values = line.Split(',');
                            if (values.Length < 5) continue;

                            string taskId = values[0];
                            string deadline = values[1];
                            string taskName = values[2];
                            string taskDescription = values[3];
                            string isCompleted = values[4];

                            string query = @"
                            INSERT INTO TASKS (TASK_ID, DEADLINE, TASK_NAME, TASK_DESCRIPTION, IS_COMPLETED)
                            VALUES (@TASK_ID, @DEADLINE, @TASK_NAME, @TASK_DESCRIPTION, @IS_COMPLETED)
                            ON DUPLICATE KEY UPDATE 
                            DEADLINE=@DEADLINE,
                            TASK_NAME=@TASK_NAME,
                            TASK_DESCRIPTION=@TASK_DESCRIPTION,
                            IS_COMPLETED=@IS_COMPLETED;";

                            using (var cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@TASK_ID", taskId);
                                cmd.Parameters.AddWithValue("@DEADLINE", deadline);
                                cmd.Parameters.AddWithValue("@TASK_NAME", taskName);
                                cmd.Parameters.AddWithValue("@TASK_DESCRIPTION", taskDescription);
                                cmd.Parameters.AddWithValue("@IS_COMPLETED", isCompleted);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    Utility.NoterMessage("Import Complete", $"Successfully imported CSV file:\n{filePath}");
                }
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Import Failed", $"An error has been occured: {ex.Message}");
            }
        }
    }
}
