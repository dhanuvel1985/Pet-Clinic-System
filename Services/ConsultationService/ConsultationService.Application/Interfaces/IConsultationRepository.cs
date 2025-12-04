using ConsultationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Interfaces
{
    public interface IConsultationRepository
    {
        Task<Consultation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Consultation>> GetAllAsync();
        Task AddAsync(Consultation consultation);
    }
}
