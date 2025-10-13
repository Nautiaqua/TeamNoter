using Dark.Net;
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
            if (dbConnect.Connect(serverBox.Text, portBox.Text, dbBox.Text,
                                  dbUsernameBox.Text, dbPasswordPassbox.Password,
                                  sslMode, caPathBox.Text) 
                                  == true)
            {
                Dashboard dashboard = new Dashboard(this);
                dashboard.Show();
                this.Hide();
            }
        }

        //public void ProceedUnlock()
        //{
        //    if (!Initializing)
        //    {
        //        proceedBtn.IsEnabled = (!string.IsNullOrWhiteSpace(databaseURI_Box.Text) && databaseURI_Box.Text != "Database URI" &&
        //                                !string.IsNullOrWhiteSpace(emailBox.Text) && emailBox.Text != "Email" &&
        //                                !string.IsNullOrWhiteSpace(userpassPassbox.Password) && userpassPassbox.Password != "Password");
        //    }
        //    ;
        //}




    }
}