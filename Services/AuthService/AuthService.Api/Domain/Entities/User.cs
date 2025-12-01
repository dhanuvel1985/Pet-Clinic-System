using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // FK to Role
        [Required]
        public int RoleId { get; set; } = 2; // default User role

        public Role Role { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }

}
