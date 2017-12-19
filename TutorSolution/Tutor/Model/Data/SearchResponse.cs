using System.Collections.Generic;

namespace Tutor.Model.Data
{
    class SearchResponse
    {
        public SearchType Type { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public List<VideoMini> VideoList { get; set; }
        public List<CourseDetails> CourseList { get; set; }
    }
}
