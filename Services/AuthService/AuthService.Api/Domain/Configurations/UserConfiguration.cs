using AuthService.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Api.Domain.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)     // Role has many Users
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
