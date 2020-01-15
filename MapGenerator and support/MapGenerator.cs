using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Saper.Windows

{
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
        public Field[,] map;
        public static List<Point> cordsForBombs_CONST;


        public MapGenerator(int rows, int columns, int bombs)
        {
            this.rows = rows;
            this.columns = columns;
            this.bombs = bombs;

            map = new Field[rows, columns];
            cordsForBombs_CONST = new List<Point>();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Point point = new Point(row, column);
                    Field field = new Field(point);
                    field.btn = new Button();

                    map[row, column] = field;

                    cordsForBombs_CONST.Add(point);
                }
            }
        }

        public Field[,] Generate_Map(Point clickedPoint)
        {
            List<Point> cordsForBombs = cordsForBombs_CONST;
            Field[,] tempMap = map;


            cordsForBombs[clickedPoint.row * columns + clickedPoint.col] = new Point(-1, -1);
            foreach (Point offsetPoint in offset)
            {
                Point tempPoint = clickedPoint.Add_Point(offsetPoint);
                if (tempPoint.Inside_Boundries(rows, columns))
                    cordsForBombs[tempPoint.row * columns + tempPoint.col] = new Point(-1, -1);
            }

            cordsForBombs.RemoveAll(check);

            bombCords = Bomb_Cords_Generator(cordsForBombs);
            foreach (Point pkt in bombCords)
            {
                tempMap[pkt.row, pkt.col].bomb = true;
                foreach(Point offsetPoint in offset)
                {
                    Point toAddBomb = pkt.Add_Point(offsetPoint);
                    if (toAddBomb.Inside_Boundries(rows, columns))
                        tempMap[toAddBomb.row, toAddBomb.col].nBombs += 1;
                }
            }

            return tempMap;
        }

        public bool check(Point p)
        {
            if (p.col == -1 || p.row == -1)
                return true;
            else
                return false;
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

    
}

