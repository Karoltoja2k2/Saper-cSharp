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

namespace Saper.Windows

{
    public class Point
    {
        public int row;
        public int col;

        public Point(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Point Add_Point(Point p2)
        {
            return new Point(this.row - p2.row, this.col - p2.col);
        }

        public bool Inside_Boundries(int rowsParam, int colsParams)
        {
            if (row >= 0 && col >= 0 && row < rowsParam && col < colsParams)
                return true;
            else
                return false;
        }
    }

    // 
    //  (0,0) (0,1) (0,2)
    //  (1,0) (1,1) (1,2)
    //  (2,0) (2,1) (2,2)
    //


    public class Field
    {
        public Point point;
        public Button btn;

        public bool hidden;
        public bool flag;
        public bool bomb;
        public int nBombs;



        public Field(Point point, bool bomb = false, int nearBombs = 0)
        {
            this.point = point;
            this.bomb = bomb;
            this.flag = false;
            this.hidden = true;
            this.nBombs = nearBombs;
        }

    }

    public class MapGenerator
    {
        public List<Point> bombCords;

        public int rows;
        public int columns;
        public int bombs;
        public static Point[] offset = new Point[] { new Point(-1, -1), new Point(-1, 0), new Point(-1, +1),
                                                     new Point( 0, -1),                   new Point( 0, +1),
                                                     new Point(+1, -1), new Point(+1, 0), new Point(+1, +1)
                                                   };

        public MapGenerator(int rows, int columns, int bombs)
        {
            this.rows = rows;
            this.columns = columns;
            this.bombs = bombs;            
        }

        public Field[,] Generate_Map()
        {
            Field[,] map = new Field[rows, columns];
            List<Point> cordsForBombs = new List<Point>();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Point point = new Point(row, column);
                    map[row, column] = new Field(point);
                    cordsForBombs.Add(point);
                }
            }

            bombCords = Bomb_Cords_Generator(cordsForBombs);
            Console.WriteLine(bombCords);
            foreach (Point pkt in bombCords)
            {
                map[pkt.row, pkt.col].bomb = true;
                foreach(Point offsetPoint in offset)
                {
                    Point toAddBomb = pkt.Add_Point(offsetPoint);
                    if (toAddBomb.row >= 0 && toAddBomb.row < rows && toAddBomb.col >= 0 && toAddBomb.col < columns)
                        map[toAddBomb.row, toAddBomb.col].nBombs += 1;
                }
            }

            return map;
        }

        public List<Point> Bomb_Cords_Generator(List<Point> cords)
        {
            List<Point> chosenBomb = new List<Point>();
            var random = new Random();
            int index;
            for (int b = 0; b < bombs; b++)
            {
                index = random.Next(cords.Count);
                chosenBomb.Add(cords[index]);
                cords.RemoveAt(index);
            }
            return chosenBomb;
        }
    }

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Field[,] map;
        System.Diagnostics.Stopwatch watch;
        MapGenerator mapGenerator;

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


            mapGenerator = new MapGenerator(rows, col, bombs);
            map = mapGenerator.Generate_Map();
            renderGrid(rows, col);
        }

        public void renderGrid(int rows, int cols)
        {
            Grid grid = new Grid();

            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                for (int j = 0; j < cols; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(30);
                    grid.ColumnDefinitions.Add(column);

                    Button btn = new Button();
                    btn.FontSize = 18;
                    btn.BorderThickness = new Thickness(1);
                    btn.Background = Brushes.DodgerBlue;
                    btn.CommandParameter = new Point(i, j);
                    // btn.MouseDown += Click_Down;

                    btn.PreviewMouseLeftButtonUp += Click_Up;
                    btn.PreviewMouseLeftButtonDown += Click_Down;

                    btn.MouseRightButtonUp += Click_Up;
                    btn.MouseRightButtonDown += Click_Down;

                    btn.MouseLeave += Mouse_Leave;

                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    grid.Children.Add(btn);
                    map[i, j].btn = btn;
                }
            }
            this.Content = grid;
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

        public void Click_Up(object sender, MouseButtonEventArgs e)
        {
            Point clickedXY;
            if (mouse_Down != null)
                clickedXY = mouse_Down;
            else
                return;
            Field clickedField = map[clickedXY.row, clickedXY.col];

            // Button clicked = (Button)sender;
            // Point clickedXY = (Point)clicked.CommandParameter;
            // Field clickedField = map[clickedXY.row, clickedXY.col];

            if (e.ChangedButton == MouseButton.Left)
                LMB_Click(clickedField);
            else if (e.ChangedButton == MouseButton.Right)
                RMB_Click(clickedField);
            e.Handled = true;

        }

        public void LMB_Click(Field clickedField)
        {
            

            if (clickedField.hidden == true)
                LMB_Click_Hidden(clickedField);
            else
                LMB_Click_Shown(clickedField);


        }

        public void RMB_Click(Field clickedField)
        {
            if (clickedField.hidden == true)
            {
                clickedField.flag = !clickedField.flag;
                string txt = clickedField.flag ? "F" : "";
                clickedField.btn.Content = $"{txt}";
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
            }
            else
            {
                Start_Game(16, 30, 99);

            }

        }

    }

    
}

