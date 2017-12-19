namespace Tutor_Server.Model.Data
{
    public class SignInResponse
    {
        public bool IsAccepted { get; set; }
        public string Reason { get; set; }
        public string SessionID { get; set; }
    }
}
