using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class CreateCourseRequest
    {
        public string CourseName { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
    }
}
