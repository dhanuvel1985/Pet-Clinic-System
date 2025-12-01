using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure
{
    public class ConsultationDbContextFactory : IDesignTimeDbContextFactory<ConsultationDbContext>
    {
        public ConsultationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ConsultationDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=ConsultationDb;User Id=sa;Password=StrongPass@1234;TrustServerCertificate=True;")
                .Options;

            return new ConsultationDbContext(options);
        }
    }
}
