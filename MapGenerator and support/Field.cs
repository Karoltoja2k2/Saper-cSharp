using System.Windows.Controls;

namespace Saper.Windows

{
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

    
}

