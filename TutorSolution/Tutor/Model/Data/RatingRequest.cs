using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class RatingRequest
    {
        public string CourseID { get; set; }
        public int Rating { get; set; }
        public string UserID { get; set; }
    }
}
