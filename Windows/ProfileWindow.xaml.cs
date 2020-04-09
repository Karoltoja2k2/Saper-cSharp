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

namespace Saper.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        public void logOffBtn(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.logged = false;
            Properties.Settings.Default.remember = false;
            Properties.Settings.Default.Id = 0;
            Properties.Settings.Default.NickName = "";
            Properties.Settings.Default.Token = "";

            this.Close();
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
