using Saper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.MapGenerator_and_support
{
    public class ClickEvent
    {
        public bool bomb;
        public Nullable<int>[] n;

        public ClickEvent(bool isBomb, Nullable<int>[] neighbours)
        {
            bomb = isBomb;
            n = neighbours;
        }

    }


}
