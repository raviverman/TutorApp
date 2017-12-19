namespace Tutor_Server.Model.Data
{
    
    public class CreateAccount
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
    }
}
