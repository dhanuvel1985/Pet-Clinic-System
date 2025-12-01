using PetService.Application.DTOs;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(Guid id);
        Task<List<PetDto>> GetPagedAsync(int page, int pageSize);
        Task AddAsync(Pet pet);
        Task SaveChangesAsync();
    }
}
