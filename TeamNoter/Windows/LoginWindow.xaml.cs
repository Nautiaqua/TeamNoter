using Dark.Net;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TeamNoter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool Initializing = true;
        public string sslMode;
        string saveFile = "lastlogin.txt";
        public MainWindow()
        {

            InitializeComponent();
            //Class1.Connect();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            // initializing flag to prevent crashing for certain things
            Initializing = false;

            attemptRemember();
        }

        private void caFilterHandler(object sender, RoutedEventArgs e)
        {
            // yes, this could've been a radio button
            // but im too lazy to style the radio buttons, so you get this. works the same anyway lol ~ nautia
            if (sender is CheckBox checkbox)
            {
                if (checkbox == none)
                {
                    checkbox.IsChecked = true;
                    required.IsChecked = false;
                    verifyca.IsChecked = false;
                    sslMode = "None";

                    mainBorder.Height = 650;
                    mainBorder.MinHeight = 650;
                    mainBorder.MaxHeight = 650;
                    caPathBox.Visibility = System.Windows.Visibility.Collapsed;
                    caPathBox.Text = "ca.pem Filepath (Example: D:\\ca.pem)";
                }
                if (checkbox == required)
                {
                    checkbox.IsChecked = true;
                    none.IsChecked = false;
                    verifyca.IsChecked = false;
                    sslMode = "Required";

                    mainBorder.Height = 650;
                    mainBorder.MinHeight = 650;
                    mainBorder.MaxHeight = 650;
                    caPathBox.Visibility = System.Windows.Visibility.Collapsed;
                    caPathBox.Text = "ca.pem Filepath (Example: D:\\ca.pem)";
                }
                if (checkbox == verifyca)
                {
                    checkbox.IsChecked = true;
                    required.IsChecked = false;
                    none.IsChecked = false;
                    sslMode = "VerifyCA";

                    mainBorder.Height = 680;
                    mainBorder.MinHeight = 680;
                    mainBorder.MaxHeight = 680;
                    caPathBox.Visibility = System.Windows.Visibility.Visible;
                    caPathBox.Text = "ca.pem Filepath (Example: D:\\ca.pem)";
                }

                proceedUnlock();
            }
        }

        private void serverBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Server", e);
        }

        private void portBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Port", e);
        }

        private void dbBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Database", e);
        }

        private void dbUsernameBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Username / UID", e);
        }

        private void caPathBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "ca.pem Filepath (Example: D:\\ca.pem)", e);
        }

        private void emailBox_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Email", e);
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            bool verification = false;

            if (dbConnect.Connect(serverBox.Text, portBox.Text, "TASK_MANAGEMENT",
                dbUsernameBox.Text, dbPasswordPassbox.Password,
                sslMode, caPathBox.Text))
            {
                using (MySqlConnection conn = dbConnect.GetConnection())
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    bool loginSuccess = false;

                    try
                    {
                        // Verify schema
                        string[] queries =
                        {
                        "SELECT TASK_ID, TASK_NAME, TASK_DESCRIPTION, DATE_CREATED, DEADLINE, IS_COMPLETED FROM TASKS;",
                        "SELECT USER_ID, TASK_ID FROM USER_TASKS;",
                        "SELECT USER_ID, USERNAME, EMAIL, PASSWORD, CREATION_DATE, TASKS_COMPLETED, TASKS_ASSIGNED, ACCOUNT_TYPE FROM USERS;"
                    };

                        foreach (string q in queries)
                        {
                            using (MySqlCommand cmd = new MySqlCommand(q, conn))
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                // checks the structure
                            }
                        }

                        // for debug reasons, just uncomment if u wanna debug the connection code
                        // MessageBox.Show("Working — all queries executed successfully!");
                        verification = true;
                    }
                    catch (Exception error)
                    {
                        Utility.NoterMessage("Incomplete or wrong database", "Incomplete or wrong database" + error.Message);
                    }

                    if (verification)
                    {
                        try
                        {
                            string queryString = "SELECT * FROM USERS WHERE EMAIL = @email";
                            using (MySqlCommand query = new MySqlCommand(queryString, conn))
                            {
                                query.Parameters.AddWithValue("@email", emailBox.Text);
                                using (MySqlDataReader resultset = query.ExecuteReader())
                                {
                                    if (!resultset.HasRows)
                                    {
                                        Utility.NoterMessage("Login failed", "Invalid email or password.");
                                    }
                                    else if (resultset.Read())
                                    {
                                        if (resultset["PASSWORD"] != DBNull.Value &&
                                            userpassPassbox.Password.Equals(resultset.GetString("PASSWORD")))
                                        {
                                            // Utility.NoterMessage("Login successful", "Valid email and password.");
                                            loginSuccess = true;

                                            LoginData.UserID = resultset.GetInt32("USER_ID");
                                            LoginData.Username = resultset.GetString("USERNAME");
                                            LoginData.Email = resultset.GetString("EMAIL");
                                            LoginData.Password = resultset.GetString("PASSWORD");
                                            LoginData.TasksCompleted = resultset.GetInt32("TASKS_COMPLETED");
                                            LoginData.TasksAssigned = resultset.GetInt32("TASKS_ASSIGNED");
                                            LoginData.AccountType = resultset.GetString("ACCOUNT_TYPE");

                                            resultset.Close();

                                        }
                                        else
                                        {
                                            Utility.NoterMessage("Login failed", "Invalid email or password.");
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            conn.Close();

                            if (loginSuccess)
                            {
                                if (remCheck.IsChecked == true)
                                    rememberLastLogin();

                                Dashboard dashboard = new Dashboard(this);
                                dashboard.Show();
                                this.Hide();
                            }
                        }
                    }
                }
            }


        }

        private void rememberLastLogin()
        {
            string remValue = (remCheck.IsChecked == true).ToString();
            string basePath = Utility.getBaseDirectory();

            // MessageBox.Show(@$"{basePath}\testfile.txt");

            File.WriteAllLines(@"D:\testfile.txt", new string[]
                {
                    remValue,
                    serverBox.Text,
                    portBox.Text,
                    dbUsernameBox.Text,
                    dbPasswordPassbox.Password,
                    emailBox.Text,
                    userpassPassbox.Password
                });
        }

        private void attemptRemember()
        {
            
        }
        private void proceedUnlock()
        {
            if (verifyca.IsChecked == false)
            {
                proceedBtn.IsEnabled = (
                    !string.IsNullOrEmpty(serverBox.Text) && serverBox.Text != "Server" &&
                    !string.IsNullOrEmpty(portBox.Text) && portBox.Text != "Port" &&
                    !string.IsNullOrEmpty(dbUsernameBox.Text) && dbUsernameBox.Text != "Username / UID" &&
                    !string.IsNullOrEmpty(dbPasswordPassbox.Password) &&
                    (none.IsChecked == true || required.IsChecked == true) &&
                    !string.IsNullOrEmpty(emailBox.Text) && emailBox.Text != "Email" &&
                    !string.IsNullOrEmpty(userpassPassbox.Password)
                    );
            }
            else if (verifyca.IsChecked == true)
            {
                proceedBtn.IsEnabled = (
                    !string.IsNullOrEmpty(serverBox.Text) && serverBox.Text != "Server" &&
                    !string.IsNullOrEmpty(portBox.Text) && portBox.Text != "Port" &&
                    !string.IsNullOrEmpty(dbUsernameBox.Text) && dbUsernameBox.Text != "Username / UID" &&
                    !string.IsNullOrEmpty(dbPasswordPassbox.Password) &&
                    !string.IsNullOrEmpty(caPathBox.Text) && caPathBox.Text != "ca.pem Filepath (Example: D:\\ca.pem)" &&
                    !string.IsNullOrEmpty(emailBox.Text) && emailBox.Text != "Email" &&
                    !string.IsNullOrEmpty(userpassPassbox.Password)
                    );
            }
        }

        private void box_KeyPress(object sender, KeyEventArgs e)
        {
            proceedUnlock();
        }

        private void serverBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void mainTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // just for quick login! comment this out later.
            serverBox.Text = "ibestupid-teamnoter.j.aivencloud.com";
            portBox.Text = "28069";
            dbUsernameBox.Text = "avnadmin";
            dbPasswordPassbox.Password = "AVNS_qtIuKAzlGVweZJe6e-C";
            emailBox.Text = "NOTER.JONNY@UE.EDU.PH";
            userpassPassbox.Password = "TEAMNOTER";
            required.IsChecked = true;
            proceedBtn.IsEnabled = true;
        }

        private void noterLogo_TouchLeave(object sender, TouchEventArgs e)
        {
            
        }

        private void noterLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void accountdetailsLabel_Copy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessStartInfo linkOpener = new ProcessStartInfo
            {
                FileName = "https://github.com/Nautiaqua/TeamNoter",
                UseShellExecute = true
            };

            Process.Start(linkOpener);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}