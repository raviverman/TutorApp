namespace Tutor.Model.Data
{
    public class ProfileUpdateRequest
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public bool RequiresPasswordUpdate { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public bool RequiresImageUpdate { get; set; }
    }
}
