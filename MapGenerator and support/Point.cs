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
            return new Point(this.row + p2.row, this.col + p2.col);
        }

        public bool Inside_Boundries(int rowsParam, int colsParams)
        {
            if (row >= 0 && col >= 0 && row < rowsParam && col < colsParams)
                return true;
            else
                return false;
        }
    }

    
}

