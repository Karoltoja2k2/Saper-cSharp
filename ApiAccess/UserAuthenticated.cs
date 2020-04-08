using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class UserAuthenticated
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Token { get; set; }

        public UserAuthenticated(int id, string nickName, string token)
        {
            Id = id;
            NickName = nickName;
            Token = token;
        }
    }
}
