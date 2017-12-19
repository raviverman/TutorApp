using System.Collections.Generic;

namespace Tutor.Model.Data
{
    public class CourseListResponse
    {
        public string  Status { get; set; }
        public string Reason { get; set; }
        public List<CourseDetails> CourseList{ get; set; }
    }
}
