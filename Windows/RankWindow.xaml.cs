using Saper.ApiAccess;
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
    /// Logika interakcji dla klasy RankWindow.xaml
    /// </summary>
    public partial class RankWindow : Window
    {
        RankRecord[] rankArray;

        public RankWindow(RankRecord[] rankArray)
        {
            this.rankArray = rankArray;
            InitializeComponent();
            Loaded += Render_Data;
        }

        public void Render_Data(object sender, RoutedEventArgs e)
        {
            Label label;
            foreach (RankRecord rR in rankArray)
            {
                label = new Label();
                label.Foreground = Brushes.White;
                label.Content = $"{rR.NickName.Trim()} -- {rR.Time}";
                if (rR.Level == 1)
                    easyPanel.Children.Add(label);
                else if (rR.Level == 2)
                    mediumPanel.Children.Add(label);
                else if (rR.Level == 3)
                    expertPanel.Children.Add(label);
            }
        }        

        private void Change_Cart(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int chosen = Int32.Parse((string)btn.CommandParameter);

            if(chosen == 1)
            {
                easyPanel.Width = Double.NaN;
                mediumPanel.Width = 0;
                expertPanel.Width = 0;
            }
            else if (chosen == 2)
            {
                easyPanel.Width = 0;
                mediumPanel.Width = Double.NaN;
                expertPanel.Width = 0;
            }
            else if (chosen == 3)
            {
                easyPanel.Width = 0;
                mediumPanel.Width = 0;
                expertPanel.Width = Double.NaN;
            }
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
