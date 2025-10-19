using Dark.Net;
using MySql.Data.MySqlClient;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeamNoter.Windows.UserControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class DeleteUser : Window
    {
        string regexPattern = @"^[^@]+@[^@]+\.[^@]+$";
        bool init = true;
        Dashboard dashboard;
        public DeleteUser(Dashboard dashboard)
        {
            InitializeComponent();

            this.dashboard = dashboard;
            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            init = false;
        }

        private void proceedCheck()
        {
            if (!init)
                proceedBtn.IsEnabled =
                (!string.IsNullOrWhiteSpace(noteName.Text) && noteName.Text != "ID Number");
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            int num = int.Parse(noteName.Text);

            using (MySqlConnection conn = dbConnect.GetConnection())
            {
                conn.Open();

                string queryString = "DELETE FROM USERS WHERE USER_ID = @currentUserID";
                using (MySqlCommand query = new MySqlCommand(queryString, conn))
                {
                    query.Parameters.AddWithValue("@currentUserID", num);
                    query.ExecuteNonQuery();
                }

                conn.Close();

                this.dashboard.contentPane.Content = new manageContent(this.dashboard);
            }
        }

        private void noteName_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "ID Number", e);
        }

        private void textboxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            proceedCheck();
        }

        private void noteName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                e.Handled = false;
        }

        private void noteName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
