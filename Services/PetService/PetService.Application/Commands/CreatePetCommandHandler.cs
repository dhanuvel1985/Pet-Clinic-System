using AutoMapper;
using MediatR;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Commands
{
    public class CreatePetCommandHandler : IRequestHandler<CreatePetCommand, Guid>
    {
        private readonly IPetRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        public CreatePetCommandHandler(IPetRepository repo, IMapper mapper, ICurrentUserService currentUser)
        {
            _repo = repo;
            _mapper = mapper; 
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreatePetCommand req, CancellationToken ct)
        {
            var ownerId = _currentUser.UserId ?? throw new InvalidOperationException("OwnerId missing");
            var pet = new Pet
            {
                Name = req.Name,
                Species = req.Species,
                Breed = req.Breed,
                Age = req.Age,
                OwnerId = ownerId
            };

            await _repo.AddAsync(pet);
            await _repo.SaveChangesAsync();

            return pet.Id;
        }
    }
}
