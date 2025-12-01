using AutoMapper;
using NSubstitute;
using PetService.Application.Commands;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Test.UnitTest
{
    public class CreatePetCommandTests
    {
        [Fact]
        public async Task Should_Create_Pet_Successfully()
        {
            var repo = Substitute.For<IPetRepository>();
            var mapper = Substitute.For<IMapper>();
            var currentUser = Substitute.For<ICurrentUserService>();
            var handler = new CreatePetCommandHandler(repo,mapper,currentUser);

            var cmd = new CreatePetCommand("Tommy", "Dog", "Labrador", 4, Guid.NewGuid());

            var result = await handler.Handle(cmd, default);

            Assert.NotEqual(Guid.Empty, result);
            await repo.Received(1).AddAsync(Arg.Any<Pet>());
        }
    }
}
