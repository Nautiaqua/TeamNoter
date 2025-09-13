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
            Initializing = false;

        }

        public void ProceedUnlock()
        {
            if (!Initializing)
            {
                proceedBtn.IsEnabled = (!string.IsNullOrWhiteSpace(databaseURI_Box.Text) && databaseURI_Box.Text != "Database URI" &&
                                        !string.IsNullOrWhiteSpace(emailBox.Text) && emailBox.Text != "Email" &&
                                        !string.IsNullOrWhiteSpace(passwordBox.Text) && passwordBox.Text != "Password");
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

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(passwordBox, "Password", true);
        }

        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(passwordBox, "Password", false);
        }

        private void generalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProceedUnlock();
        }
    }
}