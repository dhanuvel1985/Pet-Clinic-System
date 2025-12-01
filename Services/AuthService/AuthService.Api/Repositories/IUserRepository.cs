using AuthService.Api.Domain.Entities;

namespace AuthService.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);
        Task AddRefreshTokenAsync(RefreshToken rt, CancellationToken ct = default);
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
        Task RevokeRefreshTokenAsync(RefreshToken rt, CancellationToken ct = default);
    }
}
