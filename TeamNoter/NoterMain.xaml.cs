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
using static TeamNoter.DataStorage;

namespace TeamNoter
{
    /// <summary>
    /// Interaction logic for NoterMain.xaml
    /// </summary>
    public partial class NoterMain : Window
    {
        public NoterMain()
        {
            InitializeComponent();
            DataContext = new DataStorage();
        }

        private void sidebarListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Item task)
            {
                task.Status = "Completed";
            }
        }
    }
}
