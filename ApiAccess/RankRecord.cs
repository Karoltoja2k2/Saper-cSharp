using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class RankRecord
    {
        public string NickName { get; set; }
        public float Time { get; set; }
        public int Level { get; set; }

        public RankRecord()
        {
        }

        public RankRecord(string nn, float time, int level)
        {
            NickName = nn;
            Time = time;
            Level = level;
        }
    }
}
