using MediatR;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Queries
{
    public class GetPetByIdQueryHandler : IRequestHandler<GetPetByIdQuery, Pet?>
    {
        private readonly IPetRepository _repo;

        public GetPetByIdQueryHandler(IPetRepository repo)
        {
            _repo = repo;
        }

        public async Task<Pet?> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id);
        } 
    }
}
