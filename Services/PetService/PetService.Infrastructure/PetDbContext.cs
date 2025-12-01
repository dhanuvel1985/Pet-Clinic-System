using Microsoft.EntityFrameworkCore;
using PetService.Domain.Entities;
using PetService.Infrastructure.Configurations;
using PetService.Infrastructure.Seeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Infrastructure
{
    public class PetDbContext : DbContext
    {
        public PetDbContext(DbContextOptions<PetDbContext> opts) : base(opts) { }

        public DbSet<Pet> Pets => Set<Pet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PetConfiguration());
            PetSeeder.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
