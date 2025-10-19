using Dark.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using TeamNoter.Windows.UserControls;

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for AddPopup.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        bool init = true;
        public ChangePassword()
        {
            InitializeComponent();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            init = false;
        }

        private void proceedCheck()
        {
            if (!init)
            {
                proceedBtn.IsEnabled =
                (!string.IsNullOrWhiteSpace(noteName.Text) && noteName.Text != "Username") &&
                (!string.IsNullOrWhiteSpace(newPass.Text) && newPass.Text != "New Password") &&
                (!string.IsNullOrWhiteSpace(newPassAgain.Text) && newPassAgain.Text != "New Password Again");
            }
        }

        private void noteName_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "Old Password", e);
        }

        private void newPass_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "New Password", e);
        }

        private void newPassAgain_Placeholder(object sender, RoutedEventArgs e)
        {
            Utility.PlaceholderText(sender, "New Password Again", e);
        }

        private void noteName_TextChanged(object sender, TextChangedEventArgs e)
        {
            proceedCheck();
        }

        private void proceedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LoginData.Password == noteName.Text)
            {
                if (newPass.Text == newPassAgain.Text)
                {
                    using (MySqlConnection conn = dbConnect.GetConnection())
                    {
                        conn.Open();

                        string queryString = @"UPDATE USERS SET PASSWORD = @newPassword WHERE USER_ID = @loggedinUser";
                        using (MySqlCommand query = new MySqlCommand(queryString, conn))
                        {
                            query.Parameters.AddWithValue("@newPassword", newPass.Text);
                            query.Parameters.AddWithValue("@loggedinUser", LoginData.UserID);
                            query.ExecuteNonQuery();
                        }

                        Utility.NoterMessage("Password change successful", "Your password has been changed successfully.");

                        // this just effectively "clears" it. .Clear() doesn't work due to how placeholder text is programmed.
                        noteName.Text = "Old Password";
                        noteName.Foreground = Utility.HexConvert("#FF777777");

                        newPass.Text = "New Password";
                        newPass.Foreground = Utility.HexConvert("#FF777777");

                        newPassAgain.Text = "New Password Again";
                        newPassAgain.Foreground = Utility.HexConvert("#FF777777");

                        conn.Close();
                        this.Close();
                    }
                }
                else
                    Utility.NoterMessage("Invalid new password", "Please repeat the password correctly.");
            }
            else
                Utility.NoterMessage("Invalid password", "Please input the correct password.");
        }
    }
}
