using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class LoginForm
    {
        public string NickName { get; set; }
        public string Password { get; set; }

        public LoginForm(string NickName, string Password)
        {
            this.NickName = NickName;
            this.Password = Password;
        }
    }
}
