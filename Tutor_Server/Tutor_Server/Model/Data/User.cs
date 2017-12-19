using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Data
{
    public enum UserType
    {
        Student,
        Professional
    }
    class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public UserType Type { get; set; }
        public string Name { get; set; }
    }
}
