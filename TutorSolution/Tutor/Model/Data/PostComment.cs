using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class PostComment
    {
        public string VideoID { get; set; }
        public string Username { get; set; }
        public string UserID { get; set; }
        public string Thumbnail { get; set; }
        public string Content { get; set; }
    }
}
