namespace Tutor.Model.Data
{
    public class ProfileUpdateResponse
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public bool RequiresImageUpdate { get; set; }
    }
}
