using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Infrastructure
{
    [Obsolete("This factory is for EF Core design-time only.")]
    public class PetDbContextFactory : IDesignTimeDbContextFactory<PetDbContext>
    {
        public PetDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PetDbContext>();

            // Same connection string used by your API
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=PetDb;User Id=sa;Password=YourStrong@1234;TrustServerCertificate=True;"
            );

            return new PetDbContext(optionsBuilder.Options);
        }
    }
}
