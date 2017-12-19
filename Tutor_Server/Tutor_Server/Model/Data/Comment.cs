using System;

namespace Tutor_Server.Model.Data
{
    public class Comment
    {
        public string UserId { get; set; }
        public string VideoID { get; set; }
        public string Username { get; set; }
        public string Thumbnail { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime Date { get; set; }
        public string Uid { get; set; }
    }
}