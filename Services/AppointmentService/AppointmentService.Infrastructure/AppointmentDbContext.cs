using AppointmentService.Domain.Entities;
using AppointmentService.Infrastructure.Configurations;
using AppointmentService.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            AppointmentSeeder.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
