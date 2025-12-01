using Microsoft.EntityFrameworkCore;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Infrastructure.Seeder
{
    public static class PetSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<Pet>().HasData(
                new Pet
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Tommy",
                    Species = "Dog",
                    Breed = "Labrador",
                    Age = 3,
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                },
                new Pet
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Kitty",
                    Species = "Cat",
                    Breed = "Persian",
                    Age = 2,
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
