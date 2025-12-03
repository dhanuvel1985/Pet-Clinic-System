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
    public class LoginHandler : IRequestHandler<LoginCommand, TokenResponse>
    {
        private readonly IUserRepository _users;
        private readonly ITokenRepository _tokens;
        private readonly IPasswordHasher<User> _hasher;

        public LoginHandler(IUserRepository users, ITokenRepository tokens,
            IPasswordHasher<User> hasher)
        {
            _users = users;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<TokenResponse> Handle(LoginCommand req, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(req.Email);
            if (user == null)
                throw new UnauthorizedAccessException();

            // var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
            // if (verify == PasswordVerificationResult.Failed)
            //     throw new UnauthorizedAccessException();

            var (access, refresh) = await _tokens.GenerateTokensAsync(user);

            return new TokenResponse(access, refresh.Token, refresh.ExpiresAt);
        }
    }

}
