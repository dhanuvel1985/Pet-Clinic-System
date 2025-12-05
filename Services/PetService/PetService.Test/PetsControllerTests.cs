using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetService.Api.Controllers;
using PetService.Application.Commands;
using PetService.Application.DTOs;
using PetService.Application.Queries;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Test
{
    public class PetsControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly PetsController _controller;

        public PetsControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _controller = new PetsController(_mediator.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var command = new CreatePetCommand("Buddy", "Dog", "Lab", 3, Guid.NewGuid());
            var newId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);

            // Act
            var result = await _controller.Create(command);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PetsController.GetById), created.ActionName);
            Assert.Equal(newId, created.RouteValues["id"]);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenPetsFound()
        {
            // Arrange
            var pets = new List<PetDto>()
        {
            new PetDto { Id = Guid.NewGuid(), Name = "Buddy" }
        };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetPagedAsync>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pets);

            // Act
            var result = await _controller.Get(1, 10);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(pets, ok.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenNoPets()
        {
            _mediator
                .Setup(x => x.Send(It.IsAny<GetPagedAsync>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<PetDto>)null);

            var result = await _controller.Get(1, 10);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk()
        {

            var pet = new Pet
            {
                Id = Guid.NewGuid()
            };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetPetByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pet);  // Pet, NOT PetDto

            var result = await _controller.GetById(pet.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(pet, ok.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound()
        {
            var pet = new Pet
            {
                Id = Guid.NewGuid(),
                Name = "Kutty",
                Age = 10,
                OwnerId = Guid.NewGuid(),
            };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetPetByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pet);

            var result = await _controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
