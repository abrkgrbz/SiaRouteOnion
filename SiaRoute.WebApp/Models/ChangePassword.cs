namespace SiaRoute.WebApp.Models
{
    public class ChangePassword
    {
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
