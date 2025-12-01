using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure
{
    public class AppointmentDbContextFactory : IDesignTimeDbContextFactory<AppointmentDbContext>
    {
        public AppointmentDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppointmentDbContext>();

            builder.UseSqlServer(
                "Server=localhost,1433;Database=AppointmentDb;User Id=sa;Password=StrongPass@1234;TrustServerCertificate=True;");

            return new AppointmentDbContext(builder.Options);
        }
    }
}
