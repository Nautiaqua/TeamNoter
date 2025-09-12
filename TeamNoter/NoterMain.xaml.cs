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
        public bool initializing;
        public NoterMain()
        {
            initializing = true;
            InitializeComponent();
            DataContext = new DataStorage();

            dateLabel.Content = DateTime.Today;

            initializing = false;
        }

        private void sidebarListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initializing)
            {
                switch (sidebarListbox.SelectedIndex)
                {
                    case 0:
                        mainTab.SelectedIndex = 0;
                        break;

                    case 2:
                        mainTab.SelectedIndex = 1;
                        break;

                    case 4:
                        mainTab.SelectedIndex = 2;
                        break;

                    case 6:
                        mainTab.SelectedIndex = 3;
                        break;
                }
            }
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
