namespace Tutor.Model.Data
{
    //AccountDetails
    //AccountDetailsResponse
    //CreateAccount
    //VideoDetails
    public class CreateAccount
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
    }
}
