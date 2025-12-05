using AuthService.Application.Commands;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Test
{
    public class RegisterHandlerTests
    {
        private readonly Mock<IUserRepository> _users;
        private readonly Mock<ITokenRepository> _tokens;
        private readonly Mock<IPasswordHasher<User>> _hasher;
        private readonly RegisterHandler _handler;

        public RegisterHandlerTests()
        {
            _users = new Mock<IUserRepository>();
            _tokens = new Mock<ITokenRepository>();
            _hasher = new Mock<IPasswordHasher<User>>();

            _handler = new RegisterHandler(_users.Object, _tokens.Object, _hasher.Object);
        }

        [Fact]
        public async Task Handle_ShouldRegisterUser_WhenEmailNotExists()
        {
            var cmd = new RegisterCommand("new@example.com", "password", 2);

            _users.Setup(r => r.GetByEmailAsync(cmd.Email, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

            _tokens.Setup(t => t.GenerateTokensAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(("access",
                    new RefreshToken
                    {
                        Token = "ref",
                        ExpiresAt = DateTime.UtcNow
                    }));

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.Equal("access", result.AccessToken);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
        {
            var cmd = new RegisterCommand("exists@example.com", "password", 1);
            var existingUser = new User { Email = cmd.Email };

            _users.Setup(r => r.GetByEmailAsync(cmd.Email, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(cmd, CancellationToken.None));
        }
    }
}
