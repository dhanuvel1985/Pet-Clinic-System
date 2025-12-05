using Moq;
using PetService.Application.Commands;
using PetService.Application.Interfaces;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Test
{
    public class CreatePetCommandHandlerTests
    {
        private readonly Mock<IPetRepository> _repo;
        private readonly Mock<ICurrentUserService> _currentUser;
        private readonly CreatePetCommandHandler _handler;

        public CreatePetCommandHandlerTests()
        {
            _repo = new Mock<IPetRepository>();
            _currentUser = new Mock<ICurrentUserService>();

            _handler = new CreatePetCommandHandler(_repo.Object, null, _currentUser.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreatePet()
        {
            // Arrange
            _currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());

            var command = new CreatePetCommand("Leo", "Cat", "Persian", 2, Guid.NewGuid());

            // Act
            var id = await _handler.Handle(command, default);

            // Assert
            _repo.Verify(x => x.AddAsync(It.IsAny<Pet>()), Times.Once);
            _repo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOwnerMissing()
        {
            _currentUser.Setup(x => x.UserId).Returns((Guid?)null);

            var command = new CreatePetCommand("Leo", "Cat", "Persian", 2, Guid.NewGuid());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, default));
        }
    }
}
