using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IUserRepository _users;
        private readonly ITokenRepository _tokens;

        public RefreshTokenHandler(IUserRepository users, ITokenRepository tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        public async Task<TokenResponse> Handle(RefreshTokenCommand req, CancellationToken ct)
        {
            var rt = await _users.GetRefreshTokenAsync(req.RefreshToken);
            if (rt == null || rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException();

            var user = rt.User!;
            await _users.RevokeRefreshTokenAsync(rt);

            var (access, newRt) = await _tokens.GenerateTokensAsync(user);

            return new TokenResponse(access, newRt.Token, newRt.ExpiresAt);
        }
    }

}
