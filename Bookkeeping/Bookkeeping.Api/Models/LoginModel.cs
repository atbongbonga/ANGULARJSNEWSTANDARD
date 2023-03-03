namespace Bookkeeping.Api.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string? UserIP { get; set; }
    }
}
