using MediatR;
using PetService.Application.DTOs;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Queries
{
    public class GetPetQueryHandler : IRequestHandler<GetPagedAsync, List<PetDto?>>
    {
        private readonly IPetRepository _repo;

        public GetPetQueryHandler(IPetRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PetDto>> Handle(GetPagedAsync request, CancellationToken cancellationToken)
        {
            return await _repo.GetPagedAsync(request.page, request.pageSize);
        }
    }
}
