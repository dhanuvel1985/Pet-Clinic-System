using ConsultationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure
{
    public class ConsultationDbContext : DbContext
    {
        public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options)
            : base(options) { }

        public DbSet<Consultation> Consultations { get; set; }
    }
}
