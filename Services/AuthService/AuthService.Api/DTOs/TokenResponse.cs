namespace AuthService.Api.DTOs
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
