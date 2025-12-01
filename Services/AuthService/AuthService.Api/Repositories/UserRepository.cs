using AuthService.Api.Domain.Entities;
using AuthService.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _db;
        public UserRepository(AuthDbContext db) => _db = db;

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _db.Users.Include(u => u.Role).Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }
            

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddRefreshTokenAsync(RefreshToken rt, CancellationToken ct = default)
        {
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
        {
            return await _db.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken rt, CancellationToken ct = default)
        {
            rt.IsRevoked = true;
            _db.RefreshTokens.Update(rt);
            await _db.SaveChangesAsync(ct);
        }
    }
}
