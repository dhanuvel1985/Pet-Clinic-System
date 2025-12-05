using AuthService.Api.Controllers;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using AuthService.Application.Queries;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Test
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _controller = new AuthController(_mediator.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnCreated()
        {
            // Arrange
            var cmd = new RegisterCommand("test@example.com", "pass123", 1);
            var tokenResponse = new TokenResponse("access", "refresh", DateTime.UtcNow);

            _mediator.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(tokenResponse);

            // Act
            var result = await _controller.Register(cmd);

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            Assert.Equal(tokenResponse, created.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk()
        {
            // Arrange
            var cmd = new LoginCommand("test@example.com", "pass123");
            var tokenResponse = new TokenResponse("access", "refresh", DateTime.UtcNow);

            _mediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(tokenResponse);

            // Act
            var result = await _controller.Login(cmd);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenResponse, ok.Value);
        }

        [Fact]
        public async Task Refresh_ShouldReturnOk()
        {
            var cmd = new RefreshTokenCommand("refresh-token-value");
            var tokenResponse = new TokenResponse("access", "refresh", DateTime.UtcNow);

            _mediator.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(tokenResponse);

            var result = await _controller.Refresh(cmd);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenResponse, ok.Value);
        }

        [Fact]
        public async Task Revoke_ShouldReturnNoContent()
        {
            var cmd = new RevokeTokenCommand("refresh-token-value");

            _mediator.Setup(m => m.Send(It.IsAny<RevokeTokenCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _controller.Revoke(cmd);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            var userDto = new UserDto
            (
                Guid.NewGuid(),
                "test@test.com",
                1,
                "Admin"
            );

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<GetUserByIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userDto);

            var controller = new AuthController(mediatorMock.Object);

            var userId = Guid.NewGuid();

            // Act
            var result = await controller.GetUserById(userId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);

            var returnedUser = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(userDto.Email, returnedUser.Email);
            Assert.Equal(userDto.Id, returnedUser.Id);
        }
    }
}
