using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public enum UserType
    {
        Student,
        Professional
    }
    class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public UserType Type { get; set; }
        public string Name { get; set; }
    }
}
