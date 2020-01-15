using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Saper.Windows

{

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Field[,] map;
        System.Diagnostics.Stopwatch watch;
        public MapGenerator mapGenerator;

        int rBombs;
        bool firstClick;
        bool run;

        public int rows;
        public int cols;
        public int bombs;

        public Point mouse_Down;

        public static Point[] offset = new Point[] { new Point(-1, -1), new Point(-1, 0), new Point(-1, +1),
                                                     new Point( 0, -1),                   new Point( 0, +1),
                                                     new Point(+1, -1), new Point(+1, 0), new Point(+1, +1)
                                                   };

        public MainWindow()
        {
            InitializeComponent();

            Start_Game(16, 30, 99);

        }

        public void Start_Game(int rows, int col, int bombs)
        {
            // watch = System.Diagnostics.Stopwatch.StartNew();
            // watch.Stop();
            // var elapsedMs = watch.ElapsedMilliseconds;
            // Console.WriteLine($"end: {elapsedMs}");
            this.rows = rows;
            this.cols = col;
            this.bombs = bombs;
            rBombs = bombs;

            firstClick = true;
            this.mapGenerator = new MapGenerator(rows, cols, bombs);

            renderGrid(rows, col);
        }

        public void renderGrid(int rows, int cols)
        {
            // Grid grid = new Grid();
            Grid grid = mapGrid;
            int squareSize = 30;

            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(squareSize);
                grid.RowDefinitions.Add(row);

                for (int j = 0; j < cols; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(squareSize);
                    this.Width = cols * squareSize;
                    this.Height = rows * squareSize;
                    grid.ColumnDefinitions.Add(column);
                    
                    mapGenerator.map[i, j].btn.FontSize = 18;
                    mapGenerator.map[i, j].btn.BorderThickness = new Thickness(1);
                    mapGenerator.map[i, j].btn.Background = Brushes.DodgerBlue;
                    mapGenerator.map[i, j].btn.CommandParameter = new Point(i, j);
                    mapGenerator.map[i, j].btn.PreviewMouseLeftButtonUp += Click_Up;
                    mapGenerator.map[i, j].btn.PreviewMouseLeftButtonDown += Click_Down;
                    mapGenerator.map[i, j].btn.MouseRightButtonUp += Click_Up;
                    mapGenerator.map[i, j].btn.MouseRightButtonDown += Click_Down;
                    mapGenerator.map[i, j].btn.MouseLeave += Mouse_Leave;

                    Grid.SetColumn(mapGenerator.map[i, j].btn, j);
                    Grid.SetRow(mapGenerator.map[i, j].btn, i);
                    grid.Children.Add(mapGenerator.map[i, j].btn);
                }
            }
            // this.Content = grid;
        }

        public void Click_Down(object sender, MouseButtonEventArgs e)
        {
            Button clicked = (Button)sender;
            mouse_Down = (Point)clicked.CommandParameter;
            e.Handled = true;
        }

        public void Mouse_Leave(object sender, MouseEventArgs e)
        {
            mouse_Down = null;
        }

        public void WatchThread()
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
            TimeSpan elapsedTime;
            while (run)
            {
                elapsedTime = watch.Elapsed;
                try
                {
                    Dispatcher.Invoke(new Action(() => updateClock(elapsedTime)));
                }
                catch
                {
                    break;
                }

                Thread.Sleep(1000);
                }

            watch.Stop();

        }

        public void updateClock(TimeSpan time)
        {
            timer.Text = $"{time.Seconds}";
        }

        public void Click_Up(object sender, MouseButtonEventArgs e)
        {
            Point clickedXY;
            if (mouse_Down != null)
                clickedXY = mouse_Down;
            else
                return;

            if (firstClick)
            {
                run = true;
                rBombs = bombs;
                map = mapGenerator.Generate_Map(clickedXY);
                firstClick = false;

                Thread watchThread = new Thread(WatchThread);
                watchThread.Start();
            }

            Field clickedField = map[clickedXY.row, clickedXY.col];


            if (e.ChangedButton == MouseButton.Left)
                LMB_Click(clickedField);
            else if (e.ChangedButton == MouseButton.Right)
                RMB_Click(clickedField);
            e.Handled = true;

        }

        public void LMB_Click(Field clickedField)
        {
            

            if (clickedField.hidden == true && clickedField.flag == false)
                LMB_Click_Hidden(clickedField);
            else
                LMB_Click_Shown(clickedField);


        }

        public void RMB_Click(Field clickedField)
        {
            if (clickedField.hidden)
            {
                rBombs += clickedField.flag ? 1 : -1;
                remainingBombs.Text = $"{rBombs}";
                
                clickedField.flag = !clickedField.flag;
                string txt = clickedField.flag ? "F" : "";
                clickedField.btn.Content = $"{txt}";
                clickedField.btn.Foreground = Brushes.Orange;
            }
        }

        public void LMB_Click_Hidden(Field clickedField)
        {

            if (clickedField.bomb)
            {
                endGame(false);
                return;
            }

            clickedField.btn.Content = clickedField.nBombs == 0 ? "" : $"{clickedField.nBombs}";
            clickedField.btn.Foreground = Brushes.Black;
            clickedField.btn.Background = Brushes.LightGray;

            clickedField.hidden = false;

            if (clickedField.nBombs == 0)
            {
                foreach (Point offPoint in offset)
                {
                    Point tempPoint = clickedField.point.Add_Point(offPoint);
                    if (tempPoint.Inside_Boundries(16,30))
                    {
                        Field tempField = map[tempPoint.row, tempPoint.col];
                        if (tempField.hidden == true && tempField.flag == false)
                        {
                            LMB_Click_Hidden(tempField);
                        }
                    }
                }
            }
        }

        public void LMB_Click_Shown(Field clickedField)
        {
            int nearBombs = clickedField.nBombs;
            int flaggedFields = 0;
            List<Field> safeFields = new List<Field>();
            List<Field> fieldsToClick = new List<Field>();

            foreach (Point offPoint in offset)
            {
                Point tempPoint = clickedField.point.Add_Point(offPoint);
                if (!tempPoint.Inside_Boundries(16, 30))
                    continue;

                Field fTC = map[tempPoint.row, tempPoint.col];
                if (fTC.flag == true)
                    flaggedFields += 1;
                else if (fTC.hidden == true)
                    fieldsToClick.Add(fTC);
            }

            if (flaggedFields == nearBombs)
            {
                foreach (Field fTC in fieldsToClick)
                {
                    LMB_Click_Hidden(fTC);
                }
            }
        }



        public void endGame(bool success)
        {
            if (success)
            {
                Start_Game(16,30,99);
                run = false;
            }
            else
            {
                Start_Game(16, 30, 99);
                run = false;
            }

        }

    }

    
}

