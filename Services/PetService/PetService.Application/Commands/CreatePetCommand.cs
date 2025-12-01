using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Commands
{
    public record CreatePetCommand(
    string Name,
    string Species,
    string Breed,
    int Age,
    Guid OwnerId
    ) : IRequest<Guid>;
}
