using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK to user
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
