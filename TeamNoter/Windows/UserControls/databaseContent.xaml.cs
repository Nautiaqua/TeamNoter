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
            LoadDatabaseStats();
        }

        private void LoadDatabaseStats()
        {

            try
            {
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            DATABASE() AS DATABASE_NAME,
                            (SELECT COUNT(*) 
                            FROM USER_TASKS 
                            JOIN TASKS ON USER_TASKS.TASK_ID = TASKS.TASK_ID
                            WHERE USER_TASKS.USER_ID = @currentUserID 
                            AND TASKS.IS_COMPLETED = TRUE) AS COMPLETED_TASKS,

                            (SELECT COUNT(*) 
                            FROM USER_TASKS 
                            JOIN TASKS ON USER_TASKS.TASK_ID = TASKS.TASK_ID
                            WHERE USER_TASKS.USER_ID = @currentUserID 
                            AND TASKS.IS_COMPLETED = FALSE) AS INCOMPLETE_TASKS,

                            (SELECT COUNT(TASK_ID) 
                            FROM USER_TASKS 
                            WHERE USER_ID = @currentUserID) AS TOTAL_TASKS;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@currentUserID", LoginData.UserID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                dbNameBox.Text = reader["DATABASE_NAME"].ToString();
                                completedBox.Text = reader["COMPLETED_TASKS"].ToString();
                                incompleteBox.Text = reader["INCOMPLETE_TASKS"].ToString();
                                totalBox.Text = reader["TOTAL_TASKS"].ToString();
                            }
                            else
                            {
                                dbNameBox.Text = "N/A";
                                completedBox.Text = "0";
                                incompleteBox.Text = "0";
                                totalBox.Text = "0";
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch
            {
            }
        }


        private void filepathExportBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Filepath for exports", e);
        }

        private void exportBtnCSV_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\";

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
            exportFilePath = "C:\\";
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

        private void exportBtnSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "SQL files (*.sql)|*.sql",
                    Title = "Export Database to SQL File"
                };

                if (saveDialog.ShowDialog() != true)
                    return;

                string filePath = saveDialog.FileName;
                string database = "your_database_name";

                using (var conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine($"-- SQL Backup of `{database}`");
                        writer.WriteLine($"-- Exported at {DateTime.Now}");
                        writer.WriteLine();


                        string tableQuery = "SHOW TABLES";
                        using (var tableCmd = new MySqlCommand(tableQuery, conn))
                        using (var tableReader = tableCmd.ExecuteReader())
                        {
                            var tables = new List<string>();
                            while (tableReader.Read())
                            {
                                tables.Add(tableReader.GetString(0));
                            }
                            tableReader.Close();

                            foreach (var table in tables)
                            {

                                var createCmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`", conn);
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

                                string dataQuery = $"SELECT * FROM `{table}`";
                                using (var dataCmd = new MySqlCommand(dataQuery, conn))
                                using (var dataReader = dataCmd.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        var values = new List<string>();
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
                    }
                }


                Utility.NoterMessage("Export Complete", $"Database successfully exported to:\n{filePath}");
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Export Error", ex.Message);
            }
        }

        private void importBtnSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "SQL files (*.sql)|*.sql",
                    Title = "Import Database from SQL File"
                };

                if (openDialog.ShowDialog() != true)
                    return;

                string filePath = openDialog.FileName;

                string sqlScript = File.ReadAllText(filePath);

                using (var conn = dbConnect.GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    string[] commands = sqlScript.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string command in commands)
                    {
                        string trimmed = command.Trim();
                        if (!string.IsNullOrEmpty(trimmed) && !trimmed.StartsWith("--"))
                        {
                            cmd.CommandText = trimmed;
                            try { cmd.ExecuteNonQuery(); } catch {  }
                        }
                    }

                    conn.Close();
                }

                Utility.NoterMessage("Import Complete", $"Successfully imported database from:\n{filePath}");
            }
            catch (Exception ex)
            {
                Utility.NoterMessage("Import Error", ex.Message);
            }
        }
    }
}