using AuthService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
    {
        private readonly IUserRepository _users;

        public RevokeTokenHandler(IUserRepository users)
        {
            _users = users;
        }

        public async Task<Unit> Handle(RevokeTokenCommand req, CancellationToken ct)
        {
            var rt = await _users.GetRefreshTokenAsync(req.RefreshToken);
            if (rt == null) return Unit.Value;

            await _users.RevokeRefreshTokenAsync(rt);

            return Unit.Value;
        }
    }

}
