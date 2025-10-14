using Dark.Net;
using System;
using System.Collections.Generic;
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

namespace TeamNoter.Windows.CustomPopups
{
    /// <summary>
    /// Interaction logic for NoterMessage.xaml
    /// </summary>
    public partial class NoterMessage : Window
    {
        public NoterMessage(string windowTitle, string content)
        {
            InitializeComponent();

            // Directly handles the dark mode for the titlebar
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

            mainWindow.Title = windowTitle;
            mainMessage.Content = content;
        }
    }
}
