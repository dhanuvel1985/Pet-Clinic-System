using Microsoft.EntityFrameworkCore;
using PetService.Application.DTOs;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetService.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDbContext _db;

        public PetRepository(PetDbContext db)
        {
            _db = db;
        }

        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            return await _db.Pets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<PetDto>> GetPagedAsync(int page, int pageSize)
        {
            return await _db.Pets
                        .AsNoTracking()
                        .Select(p => new PetDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Species = p.Species,
                            Breed = p.Breed,
                            Age = p.Age
                        })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
        }

        public async Task AddAsync(Pet pet)
        {
            await _db.Pets.AddAsync(pet).AsTask();
        }
           

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }

}
