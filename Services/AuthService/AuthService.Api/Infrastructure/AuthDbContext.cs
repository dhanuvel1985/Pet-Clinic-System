using AuthService.Api.Domain.Configurations;
using AuthService.Api.Domain.Entities;
using AuthService.Api.Domain.Seed.AuthService.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Infrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasOne(rt => rt.User)
                 .WithMany(u => u.RefreshTokens)
                 .HasForeignKey(rt => rt.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // call separate seeders
            RoleSeeder.Seed(modelBuilder);
        }
    }

}
