using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Data
{
    public class SignIn
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Challenge { get; set; }
    }
}
