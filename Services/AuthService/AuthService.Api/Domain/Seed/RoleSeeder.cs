using AuthService.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Domain.Seed
{
    namespace AuthService.Data.Seed
    {
        public static class RoleSeeder
        {
            public static void Seed(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Role>().HasData(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" },
                    new Role { Id = 3, Name = "Receptionist" }
                );
            }
        }
    }

}
