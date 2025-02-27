namespace brain_back_domain.DTOs
{
    public class Login
    {
        public string EmailOrName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}