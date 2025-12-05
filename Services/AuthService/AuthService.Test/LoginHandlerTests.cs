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
    public class LoginHandlerTests
    {
        private readonly Mock<IUserRepository> _users;
        private readonly Mock<ITokenRepository> _tokens;
        private readonly Mock<IPasswordHasher<User>> _hasher;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _users = new Mock<IUserRepository>();
            _tokens = new Mock<ITokenRepository>();
            _hasher = new Mock<IPasswordHasher<User>>();

            _handler = new LoginHandler(_users.Object, _tokens.Object, _hasher.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTokenResponse_WhenUserExists()
        {
            var cmd = new LoginCommand("test@example.com", "password");
            var user = new User { Email = cmd.Email };

            _users.Setup(r => r.GetByEmailAsync(cmd.Email, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);

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
            Assert.Equal("ref", result.RefreshToken);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            var cmd = new LoginCommand("missing@example.com", "password");

            _users.Setup(r => r.GetByEmailAsync(cmd.Email, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(cmd, CancellationToken.None));
        }
    }
}
