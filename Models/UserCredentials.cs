using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserCredentials
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class UserCredentialsWithToken
    {
        public UserCredentials User { get; set; }
        public string Token { get; set; }
    }
}
