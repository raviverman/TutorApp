using System;
using System.Collections.Generic;

namespace Tutor_Server.Model.Data
{
    public class Video
    {
        public string VideoID { get; set; }
        public string Path { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TimeSpan Duration { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string Course { get; set; }
        public string CourseID { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public List<Comment> CommentList {get; set;}
        public List<string> Tags { get; set; }
        public float CourseRating { get; set; }
        public bool IsFavorite { get; set; }
    }
}