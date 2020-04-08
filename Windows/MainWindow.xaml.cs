using Saper.MapGenerator_and_support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

using System.Net.Http;
using System.Net;
using Saper.ApiAccess;

namespace Saper.Windows

{

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        // meta map data
        public static Field[,] map;
        public MapGenerator mapGenerator;
        public Point mouse_Down;

        // styles atributes
        public int fontSize;
        public int squareSize;
        public bool vertical;
        public bool pVertical;
        Grid gridHorizontal;
        Grid gridVertical;

        // gameplay atributes
        public System.Diagnostics.Stopwatch watch;
        int level = 1;
        int rBombs;
        int shownFields;
        bool firstClick;
        bool run;

        public int rows;
        public int cols;
        public int bombs;

        // data catching for ML
        public List<ClickEvent> clickEvents;
        static string connetionString = ConfigurationManager.AppSettings["connectionString"];
        static SqlConnection cnn;

        public static RankRecord[] rankArray = new RankRecord[0];
        public static ApiRequests apiRequests = new ApiRequests();
        private float totalTimeSec;

        public LoginWin logWin;
        public RankWindow rankWin;
        public GameOverWindow endWin;


        // const values
        public static Point[] offset = new Point[] { new Point(-1, -1), new Point(-1, 0), new Point(-1, +1),
                                                     new Point( 0, -1),                   new Point( 0, +1),
                                                     new Point(+1, -1), new Point(+1, 0), new Point(+1, +1)
                                                    };

        public static Point[] offset2 = new Point[] {
                                                     new Point(-2, -2), new Point(-2, -1), new Point(-2, 0), new Point(-2, 1), new Point(-2, 2),
                                                     new Point(-1, -2), new Point(-1, -1), new Point(-1, 0), new Point(-1, 1), new Point(-1, 2),
                                                     new Point( 0, -2), new Point( 0, -1),                   new Point( 0, 1), new Point( 0, 2),
                                                     new Point( 1, -2), new Point( 1, -1), new Point( 1, 0), new Point( 1, 1), new Point( 1, 2),
                                                     new Point( 2, -2), new Point( 2, -1), new Point( 2, 0), new Point( 2, 1), new Point( 2, 2)
                                                    };


        public static Brush[] brushCollection = new Brush[] { Brushes.LightGray, Brushes.DarkCyan, Brushes.Green, Brushes.Red, Brushes.DarkSlateBlue, Brushes.Brown, Brushes.SeaGreen, Brushes.Orange, Brushes.Black };

        public GameWindow()
        {
            // titleBar.MouseLeftButtonDown += (o, e) => DragMove();
            App.Current.MainWindow = this;
            Loaded += Loaded_Event;
            InitializeComponent();
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void Loaded_Event(object sender, RoutedEventArgs e)
        {
            cnn = new SqlConnection(connetionString);

            SizeChanged += Window_Size_Changed;
            Set_Level(level);
            Start_Game();
        }

        public void Change_Level(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int lvl;
            Int32.TryParse((string)btn.CommandParameter, out lvl);

            if (level != lvl)
                Set_Level(lvl);
            Start_Game();
        }

        public void Set_Level(int lvl)
        {
            run = false;

            level = lvl;
            switch (level)
            {
                case 1:
                    this.rows = 9;
                    this.cols = 9;
                    this.bombs = 10;
                    break;
                case 2:
                    this.rows = 16;
                    this.cols = 16;
                    this.bombs = 40;
                    break;
                case 3:
                    this.rows = 16;
                    this.cols = 30;
                    this.bombs = 99;
                    break;
            }
            remainingBombs.Text = $"{bombs}";
            timer.Text = "00:00";

        }

        public void Start_Game()
        {
            // watch = System.Diagnostics.Stopwatch.StartNew();
            // watch.Stop();
            // var elapsedMs = watch.ElapsedMilliseconds;
            // Console.WriteLine($"end: {elapsedMs}");

            map = new Field[rows, cols];
            remainingBombs.Text = $"{bombs}";
            timer.Text = "00:00";
            rBombs = bombs;
            shownFields = 0;

            clickEvents = new List<ClickEvent>();

            firstClick = true;
            run = false;

            this.mapGenerator = new MapGenerator(rows, cols, bombs);


            Create_Grid(rows, cols);
        }

        #region Create grid and render methods

        public void Create_Grid(int rows, int cols)
        {
            var style = (Style)App.Current.TryFindResource("fieldButton");

            gridHorizontal = new Grid();
            Grid.SetColumn(gridHorizontal, 0);
            Grid.SetRow(gridHorizontal, 0);

            gridVertical = new Grid();
            Grid.SetColumn(gridVertical, 0);
            Grid.SetRow(gridVertical, 0);

            RowDefinition row;
            ColumnDefinition column;

            for (int i = 0; i < rows; i++)
            {
                row = new RowDefinition();
                row.Height = GridLength.Auto;
                gridHorizontal.HorizontalAlignment = HorizontalAlignment.Center;
                gridHorizontal.VerticalAlignment = VerticalAlignment.Center;
                gridHorizontal.RowDefinitions.Add(row);

                column = new ColumnDefinition();
                column.Width = GridLength.Auto;
                gridVertical.HorizontalAlignment = HorizontalAlignment.Center;
                gridVertical.VerticalAlignment = VerticalAlignment.Center;
                gridVertical.ColumnDefinitions.Add(column);


                for (int j = 0; j < cols; j++)
                {
                    column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    gridHorizontal.ColumnDefinitions.Add(column);

                    row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    gridVertical.RowDefinitions.Add(row);

                    map[i, j] = new Field(new Point(i, j));
                    map[i, j].btn = new Button();
                    map[i, j].btn.Style = style;
                    map[i, j].btn.FontSize = 18;
                    map[i, j].btn.Background = Brushes.DodgerBlue;
                    map[i, j].btn.CommandParameter = new Point(i, j);
                    map[i, j].btn.PreviewMouseLeftButtonUp += Click_Up;
                    map[i, j].btn.PreviewMouseLeftButtonDown += Click_Down;
                    map[i, j].btn.MouseRightButtonUp += Click_Up;
                    map[i, j].btn.MouseRightButtonDown += Click_Down;
                    map[i, j].btn.MouseLeave += Mouse_Leave;
                }
            }
            Initial_Grid_Render();
            // this.Content = grid;
        }

        public void Initial_Grid_Render()
        {
            gridContainer.Children.Clear();
            squareSize = Calc_SquareSize();
            if (vertical)
            {
                pVertical = true;
                foreach (Field field in map)
                {
                    field.btn.Height = squareSize;
                    field.btn.Width = squareSize;
                    Grid.SetColumn(field.btn, field.point.row);
                    Grid.SetRow(field.btn, field.point.col);
                    gridVertical.Children.Add(field.btn);
                }
                gridContainer.Children.Add(gridVertical);

            }
            else
            {
                pVertical = false;
                foreach (Field field in map)
                {
                    field.btn.Height = squareSize;
                    field.btn.Width = squareSize;
                    Grid.SetColumn(field.btn, field.point.col);
                    Grid.SetRow(field.btn, field.point.row);
                    gridHorizontal.Children.Add(field.btn);
                }
                gridContainer.Children.Add(gridHorizontal);
            }
        }

        public void Re_render_Grid()
        {
            squareSize = Calc_SquareSize();

            fontSize = squareSize > 20 ? 20 : 10;

            if (vertical)
            {
                if (pVertical)
                {
                    foreach (Field field in map)
                    {
                        field.btn.Height = squareSize;
                        field.btn.Width = squareSize;
                        field.btn.FontSize = FontSize;
                    }
                }
                else
                {
                    pVertical = true;
                    gridContainer.Children.Clear();
                    gridHorizontal.Children.Clear();
                    foreach (Field field in map)
                    {
                        field.btn.Height = squareSize;
                        field.btn.Width = squareSize;
                        field.btn.FontSize = FontSize;
                        Grid.SetColumn(field.btn, field.point.row);
                        Grid.SetRow(field.btn, field.point.col);
                        gridVertical.Children.Add(field.btn);
                    }
                    gridContainer.Children.Add(gridVertical);

                }
            }
            else
            {
                if (pVertical)
                {
                    pVertical = false;
                    gridContainer.Children.Clear();
                    gridVertical.Children.Clear();
                    foreach (Field field in map)
                    {
                        field.btn.Height = squareSize;
                        field.btn.Width = squareSize;
                        field.btn.FontSize = FontSize;
                        Grid.SetColumn(field.btn, field.point.col);
                        Grid.SetRow(field.btn, field.point.row);
                        gridHorizontal.Children.Add(field.btn);
                    }
                    gridContainer.Children.Add(gridHorizontal);
                }
                else
                {
                    foreach (Field field in map)
                    {
                        field.btn.Height = squareSize;
                        field.btn.Width = squareSize;
                        field.btn.FontSize = FontSize;
                    }
                }
            }

            // squareSize = Calc_SquareSize();
            // foreach (Field field in map)
            // {
            //     field.btn.Height = squareSize;
            //     field.btn.Width = squareSize;
            // }
        }

        #endregion

        #region click methods

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
                map = mapGenerator.Generate_Map(clickedXY, ref map);

                firstClick = false;

                Thread watchThread = new Thread(WatchThread);
                watchThread.Start();
            }

            Field clickedField = map[clickedXY.row, clickedXY.col];


            if (e.ChangedButton == MouseButton.Left)
            {
                if (clickedField.hidden == true && clickedField.flag == false)
                {
                    LMB_Click_Hidden(clickedField);
                }
                else
                    LMB_Click_Shown(clickedField);
            }
            else if (e.ChangedButton == MouseButton.Right)
                RMB_Click(clickedField);
            
            e.Handled = true;
        }

        public void RMB_Click(Field clickedField)
        {
            // COLLECTING DATA FOR ML
            // Collect_Data(clickedField);
            if (clickedField.hidden)
            {
                rBombs += clickedField.flag ? 1 : -1;
                remainingBombs.Text = $"{rBombs}";

                clickedField.flag = !clickedField.flag;
                Brush color = clickedField.flag ? Brushes.Orange : Brushes.DodgerBlue;
                clickedField.btn.Background = color;
            }
        }

        public void LMB_Click_Hidden(Field clickedField, bool recursion = false)
        {
            // COLLECTING DATA FOR ML
            // if (!recursion && clickedField.nBombs != 0)
            // {
            //     Collect_Data(clickedField);
            // }

            if (clickedField.bomb)
            {
                Game_OVer(false);
                return;
            }

            clickedField.btn.Content = clickedField.nBombs == 0 ? "" : $"{clickedField.nBombs}";
            clickedField.btn.Foreground = brushCollection[clickedField.nBombs];
            clickedField.btn.Background = brushCollection[0];

            clickedField.hidden = false;
            shownFields += 1;


            if (clickedField.nBombs == 0)
            {
                foreach (Point offPoint in offset)
                {
                    Point tempPoint = clickedField.point.Add_Point(offPoint);
                    if (tempPoint.Inside_Boundries(rows, cols))
                    {
                        Field tempField = map[tempPoint.row, tempPoint.col];
                        if (tempField.hidden == true && tempField.flag == false)
                        {
                            LMB_Click_Hidden(tempField, true);
                        }
                    }
                }
            }

            if (shownFields == cols * rows - bombs)
            {
                Game_OVer(true);
            }
        }

        List<Field> fieldsToClick;
        public void LMB_Click_Shown(Field clickedField)
        {
            int nearBombs = clickedField.nBombs;
            int flaggedFields = 0;
            List<Field> safeFields = new List<Field>();
            fieldsToClick = new List<Field>();

            foreach (Point offPoint in offset)
            {
                Point tempPoint = clickedField.point.Add_Point(offPoint);
                if (!tempPoint.Inside_Boundries(rows, cols))
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
                    if (fTC.hidden)
                        LMB_Click_Hidden(fTC);
                }
            }
        }

        #endregion

        #region ML stuff

        public void Collect_Data(Field clickedField)
        {
            Nullable<int>[] nFields = new Nullable<int>[24];
            int count = 0;
            int number;
            foreach (Point offPoint in offset2)
            {
                Point tempPoint = clickedField.point.Add_Point(offPoint);
                if (tempPoint.Inside_Boundries(rows, cols))
                {
                    Field threadedField = map[tempPoint.row, tempPoint.col];
                    if (threadedField.hidden)
                        number = 9;
                    else if (threadedField.flag)
                        number = 10;
                    else
                        number = threadedField.nBombs;
                    nFields[count] = number;
                }
                else
                {
                    nFields[count] = null;
                }
                count++;
            }
            bool bomb = clickedField.bomb;
            ClickEvent clc = new ClickEvent(bomb, nFields);
            clickEvents.Add(clc);
        }

        public void Save_ML_Data(List<ClickEvent> clickEvents)
        {
            cnn.Open();
            string q;
            SqlCommand cmd;
            foreach(ClickEvent click in clickEvents)
            {
                q = @"INSERT INTO ML_data(isBomb, LLUU, LUU, UU, RUU, RRUU, LLU, LU, U, RU, RRU, LL, L, R, RR, LLD, LD, D, RD, RRD, LLDD, LDD, DD, RDD, RRDD)
                      VALUES (@isBomb, @LLUU, @LUU, @UU, @RUU, @RRUU, @LLU, @LU, @U, @RU, @RRU, @LL, @L, @R, @RR, @LLD, @LD, @D, @RD, @RRD, @LLDD, @LDD, @DD, @RDD, @RRDD)";
                cmd = new SqlCommand(q, cnn);
                cmd.Parameters.AddWithValue("isBomb", click.bomb ? 1 : 0);
                cmd.Parameters.AddWithValue("LLUU", (object)click.n[0]  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("LUU",  (object)click.n[1]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("UU"  , (object)click.n[2]  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RUU",  (object)click.n[3]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("RRUU", (object)click.n[4]  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("LLU",  (object)click.n[5]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("LU" ,  (object)click.n[6]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("U"   , (object)click.n[7]  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RU" ,  (object)click.n[8]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("RRU",  (object)click.n[9]  ?? DBNull.Value );
                cmd.Parameters.AddWithValue("LL"  , (object)click.n[10] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("L"   , (object)click.n[11] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("R"   , (object)click.n[12] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RR"  , (object)click.n[13] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("LLD",  (object)click.n[14] ?? DBNull.Value );
                cmd.Parameters.AddWithValue("LD" ,  (object)click.n[15] ?? DBNull.Value );
                cmd.Parameters.AddWithValue("D"   , (object)click.n[16] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RD" ,  (object)click.n[17] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RRD",  (object)click.n[18] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("LLDD", (object)click.n[19] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("LDD",  (object)click.n[20] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("DD"  , (object)click.n[21] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RDD",  (object)click.n[22] ?? DBNull.Value);
                cmd.Parameters.AddWithValue("RRDD", (object)click.n[23] ?? DBNull.Value);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            cnn.Close();
        }

        #endregion

        public async Task Game_OVer(bool success)
        {
            run = false;
            // DATA FOR ML, ONLY WHEN CONN TO MY LOCAL DB
            // Save_ML_Data(clickEvents);

            endWin = new GameOverWindow();
            endWin.Owner = this;

            // int height = (int)this.ActualHeight / 2;
            // int Width = (int)this.ActualWidth / 2;
            // endWin.Height = height;
            // endWin.Width = Width;

            if (success)
            {
                endWin.successBlock.Text = "Congratulations!";
                endWin.timeBlock.Text = $"{timer.Text}";
                endWin.recordBlock.Text = "00:00";
            }
            else
            {
                endWin.successBlock.Text = $"Boom!";
                endWin.timeBlock.Text = $"{timer.Text}";
                endWin.recordBlock.Text = "00:00";
            }
            endWin.ShowDialog();
            Start_Game();
            if (success)
            {
                bool send = await apiRequests.Post_RankRecord("testAuto", totalTimeSec, level);
                if (!send)
                {
                    Connection_Error();
                }
            }
        }

        public void Connection_Error()
        {
        }

        public void Show_LoginWin(object sender, RoutedEventArgs e)
        {
            logWin = new LoginWin();
            logWin.Owner = this;
            logWin.ShowDialog();
            Connection_Error();
        }

        public void Show_Rank(object sender, RoutedEventArgs e) => Open_Rank_Win();
        async Task Open_Rank_Win()
        {
            rankArray = await apiRequests.Get_Ranking();
            if (rankArray == null || rankArray.Length == 0)
            {
                Connection_Error();
                return;
            }
            rankWin = new RankWindow(rankArray);
            rankWin.Owner = this;
            rankWin.ShowDialog();
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
            int min = time.Minutes;
            int sec = time.Seconds;
            string strMin;
            string strSec;

            totalTimeSec = min * 60 + sec;

            strMin = min < 10 ? $"0{min}" : $"{min}";
            strSec = sec < 10 ? $"0{sec}" : $"{sec}";

            timer.Text = $"{strMin}:{strSec}";
        }

        private void Close_Game_Window(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Game_Window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private bool fullscreen = false;
        private void Maximize_Win(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
            {
                if (!fullscreen)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else if (fullscreen)
                {
                    this.WindowState = WindowState.Normal;
                }
                fullscreen = !fullscreen;
            }
        }

        private void Window_Size_Changed(object sender, SizeChangedEventArgs e)
        {
            if (cols != 0 && rows != 0)
                if (cols * squareSize + 0 > gridContainer.ActualWidth || rows * squareSize + 0 > gridContainer.ActualHeight || cols * squareSize - 0 < gridContainer.ActualWidth || rows * squareSize - 0 < gridContainer.ActualHeight)
                    Re_render_Grid();
        }

        public int Calc_SquareSize()
        {
            int containerHeight = (int)gridContainer.ActualHeight;
            int containerWidth = (int)gridContainer.ActualWidth;

            vertical = containerHeight > 1.2 * containerWidth ? true : false;


            int size;
            if (containerHeight > containerWidth)
            {
                if (vertical)
                {
                    size = containerWidth / rows;

                    if (cols * size > containerHeight)
                    {
                        size = containerHeight / cols;
                    }

                    return size;
                }
                else
                {
                    size = containerWidth / cols;
                    if (rows * size > containerHeight)
                    {
                        size = containerHeight / rows;
                    }
                }
            }
            else
            {
                size = containerHeight / rows;
                if (cols * size > containerWidth)
                {
                    size = containerWidth / cols;
                }
            }
            return size;
        }

    }


}

