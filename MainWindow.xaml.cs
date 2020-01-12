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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper

{
    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class Field
    {
        public int X;
        public int Y;

        public bool bomb;
        public int nearBombs;
    }

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Field[,] map;

        public MainWindow()
        {
            InitializeComponent();

            grid(10);
        }

        public void grid(int length)
        {
            Grid grid = new Grid();

            for (int i = 0; i < length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(20);
                grid.RowDefinitions.Add(row);

                for (int j = 0; j < length; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(20);
                    grid.ColumnDefinitions.Add(column);

                    Button btn = new Button();
                    btn.CommandParameter = new Point(i,j);
                    btn.Click += clicked;
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    grid.Children.Add(btn);
                }
            }
            this.Content = grid;
        }

        public void mapGenerator()
        {

        }

        public void clicked(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            Point clickedXY = (Point)clicked.CommandParameter;
        }
    }

    
}
