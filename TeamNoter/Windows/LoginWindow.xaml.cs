using Dark.Net;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public MainWindow()
        {

            InitializeComponent();
            //Class1.Connect();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            Initializing = false;
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

                    mainBorder.Height = 700;
                    mainBorder.MinHeight = 700;
                    mainBorder.MaxHeight = 700;
                    caPathBox.Visibility = System.Windows.Visibility.Collapsed;
                    caPathBox.Text = "ca.pem Filepath (Example: D:\\ca.pem)";
                }
                if (checkbox == required)
                {
                    checkbox.IsChecked = true;
                    none.IsChecked = false;
                    verifyca.IsChecked = false;
                    sslMode = "Required";

                    mainBorder.Height = 700;
                    mainBorder.MinHeight = 700;
                    mainBorder.MaxHeight = 700;
                    caPathBox.Visibility = System.Windows.Visibility.Collapsed;
                    caPathBox.Text = "ca.pem Filepath (Example: D:\\ca.pem)";
                }
                if (checkbox == verifyca)
                {
                    checkbox.IsChecked = true;
                    required.IsChecked = false;
                    none.IsChecked = false;
                    sslMode = "VerifyCA";

                    mainBorder.Height = 730;
                    mainBorder.MinHeight = 730;
                    mainBorder.MaxHeight = 730;
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

            if (dbConnect.Connect(serverBox.Text, portBox.Text, dbBox.Text,
                dbUsernameBox.Text, dbPasswordPassbox.Password,
                sslMode, caPathBox.Text))
            {
                bool loginSuccess = false;
                MySqlConnection conn = dbConnect.Connection;
                if (conn.State != ConnectionState.Open)
                    conn.Open();

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
                            // just checking structure — no need to read data
                        }
                    }

                    MessageBox.Show("Working — all queries executed successfully!");
                    verification = true;
                }
                catch (Exception error)
                {
                    MessageBox.Show("Incomplete or wrong database:\n" + error.Message);
                }

                if (verification)
                {
                    try
                    {
                        string queryString = "SELECT PASSWORD FROM USERS WHERE EMAIL = @email";
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
                                        Utility.NoterMessage("Login successful", "Valid email and password.");
                                        loginSuccess = true;
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
                    }
                }
            }


        }

        private void proceedUnlock()
        {
            if (verifyca.IsChecked == false)
            {
                proceedBtn.IsEnabled = (
                    !string.IsNullOrEmpty(serverBox.Text) && serverBox.Text != "Server" &&
                    !string.IsNullOrEmpty(portBox.Text) && portBox.Text != "Port" &&
                    !string.IsNullOrEmpty(dbBox.Text) && dbBox.Text != "Database" &&
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
                    !string.IsNullOrEmpty(dbBox.Text) && dbBox.Text != "Database" &&
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
        
    }
}