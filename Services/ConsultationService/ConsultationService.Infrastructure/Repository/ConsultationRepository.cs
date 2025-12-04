using ConsultationService.Domain.Entities;
using ConsultationService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure.Repository
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly ConsultationDbContext _db;

        public ConsultationRepository(ConsultationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Consultation consultation)
        {
            await _db.Consultations.AddAsync(consultation);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Consultation>> GetAllAsync()
        {
            return await _db.Consultations.ToListAsync();
        }

        public async Task<Consultation?> GetByIdAsync(Guid id)
        {
            return await _db.Consultations.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
