using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class CreateVideoRequest
    {
        public string Title { get; set; }
        public string Decription { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TimeSpan Duration { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public List<string> Tags { get; set; }
    }
}
