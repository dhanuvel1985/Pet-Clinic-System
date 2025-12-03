using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface ITokenRepository
    {
        Task<string> GenerateAccessToken(User user);
        RefreshToken CreateRefreshToken();
        Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(User user, CancellationToken ct = default);
    }
}
