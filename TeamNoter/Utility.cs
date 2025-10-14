using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TeamNoter.Windows.CustomPopups;

namespace TeamNoter
{
    public static class Utility
    {
        // This class contains methods to be used throughout the entire app.
        // ~ Nautia

        // Use this if you're gonna change the colors of ANY element. Trust me, WPF handles colors weirdly.
        public static SolidColorBrush HexConvert(string hexcode)
        {
            // Our "color" value is simply paint. This is like putting paint on a paintbrush so we can use it.
            var Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexcode));
            return Color;
        }

        // Does placeholder text behaviour for TextBoxes, dunno why this isn't a base feature.
        public static void PlaceholderText(object sender, string defaultText, RoutedEventArgs e)
        { 
            if (sender is TextBox box)
            {
                if (e.RoutedEvent == UIElement.GotFocusEvent) // Code to run if the user clicks on the box.
                {
                    if (box.Text == defaultText) // Prevents accidental clear of user inputs
                    {
                        box.Text = "";
                        box.Foreground = HexConvert("#FFFFFFFF");
                    }

                }

                else if (e.RoutedEvent == UIElement.LostFocusEvent) // Code to run when the user clicks out of the box.
                {
                    // Returns to default text is nothing has been inputted
                    if (string.IsNullOrWhiteSpace(box.Text) || box.Text == defaultText)
                    {
                        box.Text = defaultText;
                        box.Foreground = HexConvert("#FF777777");
                    }
                }
            }
        }

        // Sameish method as the one up top but it really just covers password boxes.
        public static void PassPlaceholderText(PasswordBox box, string defaultText, bool entering)
        {
            if (entering) // Code to run if the user clicks on the box.
            {
                if (box.Password == defaultText) // Prevents accidental clear of user inputs
                {
                    box.Password = "";
                    box.Foreground = HexConvert("#FFFFFFFF");
                }

            }
            else // Code to run when the user clicks out of the box.
            {
                // Returns to default text is nothing has been inputted
                if (string.IsNullOrWhiteSpace(box.Password) || box.Password == defaultText)
                {
                    box.Password = defaultText;
                    box.Foreground = HexConvert("#FF777777");
                }
            }
        }

        // for handling checkboxes with blank backgrounds
        public static void checkboxBGHandler(object sender, string hexcolor)
        {
            if (sender is CheckBox chkbox)
            {
                if (chkbox.IsChecked == true)
                    chkbox.Background = Brushes.Transparent;
                else
                    chkbox.Background = HexConvert(hexcolor);
            }
        }

        // this is literally a custom MessageBox
        public static void NoterMessage(string windowTitle, string content)
        {
            NoterMessage message = new NoterMessage(windowTitle, content);
            message.Show();
        }
        
    }
}
