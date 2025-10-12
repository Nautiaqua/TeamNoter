using Dark.Net;
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

namespace TeamNoter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool Initializing = true;
        public MainWindow()
        {

            InitializeComponent();
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            Initializing = false;
        }

        public void debugCheck()
        {
            if (databaseURI_Box.Text == "debugmode" && emailBox.Text == "debugmode@gmail.com" && userpassPassbox.Password == "beholdTESTMODE123")
            {
                Dashboard dashboard = new Dashboard(this);
                dashboard.Show();
                this.Hide();
            }
        }

        public void ProceedUnlock()
        {
            if (!Initializing)
            {
                proceedBtn.IsEnabled = (!string.IsNullOrWhiteSpace(databaseURI_Box.Text) && databaseURI_Box.Text != "Database URI" &&
                                        !string.IsNullOrWhiteSpace(emailBox.Text) && emailBox.Text != "Email" &&
                                        !string.IsNullOrWhiteSpace(userpassPassbox.Password) && userpassPassbox.Password != "Password");
            }
            ;
        }

        private void databaseURI_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(databaseURI_Box, "Database URI", true);
        }

        private void databaseURI_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(databaseURI_Box, "Database URI", false);
        }

        private void emailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(emailBox, "Email", true);
        }

        private void emailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(emailBox, "Email", false);
        }

        private void generalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProceedUnlock();
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            debugCheck();
        }

        private void guideBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(this);
            dashboard.Show();
            this.Hide();
        }

        private void userpassPassbox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.PassPlaceholderText(userpassPassbox, "", true);
        }

        private void userpassPassbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PassPlaceholderText(userpassPassbox, "", false);
        }

        private void userpassPassbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ProceedUnlock();
        }
    }
}