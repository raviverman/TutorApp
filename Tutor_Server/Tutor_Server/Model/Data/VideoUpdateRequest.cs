using System.Collections.Generic;

namespace Tutor_Server.Model.Data
{
    public class VideoUpdateRequest
    {
        public string VideoID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsThumbnailUpdateRequired { get; set; }
        public List<string> Tags { get; set; }
    }
}
