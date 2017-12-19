namespace Tutor_Server.Model.Data
{

    public class AccountDetailsResponse
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AccType { get; set; }
        public UserType Type { get; set; }
        public string ProfileImage { get; set; }
    }
}
