using AuthService.Api.Domain.Entities;

namespace AuthService.Api.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        RefreshToken CreateRefreshToken();
        Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(User user, CancellationToken ct = default);
    }
}
