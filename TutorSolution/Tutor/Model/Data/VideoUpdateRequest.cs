using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class VideoUpdateRequest
    {
        public string VideoID { get; set; }
        public string  Title { get; set; }
        public string Description { get; set; }
        public bool IsThumbnailUpdateRequired { get; set; }
        public List<string> Tags { get; set; }
    }
}
