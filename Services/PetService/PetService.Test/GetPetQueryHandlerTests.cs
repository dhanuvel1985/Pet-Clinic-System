using Moq;
using PetService.Application.DTOs;
using PetService.Application.Interfaces;
using PetService.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Test
{
    public class GetPetQueryHandlerTests
    {
        private readonly Mock<IPetRepository> _repo;
        private readonly GetPetQueryHandler _handler;

        public GetPetQueryHandlerTests()
        {
            _repo = new Mock<IPetRepository>();
            _handler = new GetPetQueryHandler(_repo.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPets()
        {
            var pets = new List<PetDto>
        {
            new PetDto { Id = Guid.NewGuid(), Name = "Max" }
        };

            _repo.Setup(x => x.GetPagedAsync(1, 10)).ReturnsAsync(pets);

            var query = new GetPagedAsync(1, 10);

            var result = await _handler.Handle(query, default);

            Assert.Single(result);
            Assert.Equal("Max", result[0].Name);
        }
    }
}
