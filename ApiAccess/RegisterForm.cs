using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class RegisterForm
    {
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public RegisterForm(string NickName, string Email, string Password)
        {
            this.NickName = NickName;
            this.Email = Email;
            this.Password = Password;
        }

    }
}
