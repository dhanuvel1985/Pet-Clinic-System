namespace AuthService.Api.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Role { get; set; } = 2;
    }
}
