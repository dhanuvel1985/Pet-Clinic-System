using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.Seeders
{
    public static class AppointmentSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    PetId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    PreferredDate = DateTime.UtcNow.AddDays(1),
                    Reason = "General Checkup",
                    Status = 0,
                    CreatedAt = DateTime.UtcNow
    }
);

        }
    }
}
