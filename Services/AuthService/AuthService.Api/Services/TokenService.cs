using AuthService.Api.Domain.Entities;
using AuthService.Api.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _users;

        public TokenService(IConfiguration config, IUserRepository users)
        {
            _config = config;
            _users = users;
        }

        public string GenerateAccessToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("AccessTokenExpiresMinutes")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken()
        {
            // Strong random token + expiry handled by config
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpiresDays"));

            return new RefreshToken
            {
                Token = token,
                ExpiresAt = expires
            };
        }

        public async Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(User user, CancellationToken ct = default)
        {
            var access = GenerateAccessToken(user);
            var refresh = CreateRefreshToken();
            // Persist refresh token
            refresh.UserId = user.Id;
            await _users.AddRefreshTokenAsync(refresh, ct);
            return (access, refresh);
        }
    }
}
