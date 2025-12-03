using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, TokenResponse>
    {
        private readonly IUserRepository _users;
        private readonly ITokenRepository _tokens;
        private readonly IPasswordHasher<User> _hasher;

        public RegisterHandler(IUserRepository users, ITokenRepository tokens,
            IPasswordHasher<User> hasher)
        {
            _users = users;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<TokenResponse> Handle(RegisterCommand req, CancellationToken ct)
        {
            var existing = await _users.GetByEmailAsync(req.Email);
            if (existing != null)
                throw new Exception("User already exists");

            var user = new User { Email = req.Email, RoleId = req.Role };

            // user.PasswordHash = _hasher.HashPassword(user, req.Password);

            await _users.AddAsync(user);

            var (access, refresh) = await _tokens.GenerateTokensAsync(user);

            return new TokenResponse(access, refresh.Token, refresh.ExpiresAt);
        }
    }

}
