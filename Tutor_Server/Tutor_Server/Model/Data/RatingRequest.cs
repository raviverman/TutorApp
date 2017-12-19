namespace Tutor_Server.Model.Data
{
    public class RatingRequest
    {
        public string CourseID { get; set; }
        public int Rating { get; set; }
        public string UserID { get; set; }
    }
}
