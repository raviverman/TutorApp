namespace Tutor_Server.Model.Data
{
    public class VideoUpdateResponse
    {
        public bool IsThumbnailUpdateRequired { get; set; }
        public string ThumbnailImage { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
