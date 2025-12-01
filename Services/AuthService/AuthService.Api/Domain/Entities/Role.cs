using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Domain.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
