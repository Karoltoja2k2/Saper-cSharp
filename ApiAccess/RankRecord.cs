using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class RankRecord
    {
        public UserAuthenticated User { get; set; }
        public string NickName { get; set; }
        public float Time { get; set; }
        public int Level { get; set; }

        public RankRecord()
        {
        }

        public RankRecord(UserAuthenticated user, float time, int level)
        {
            User = user;
            Time = time;
            Level = level;
        }
    }
}
