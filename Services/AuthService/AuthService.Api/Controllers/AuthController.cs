using AuthService.Api.Domain.Entities;
using AuthService.Api.DTOs;
using AuthService.Api.Repositories;
using AuthService.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(IUserRepository users, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
        {
            _users = users;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var existing = await _users.GetByEmailAsync(req.Email);
            if (existing != null) return Problem(statusCode: 400, title: "User already exists");

            var user = new User { Email = req.Email, RoleId = req.Role };
            //user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);

            await _users.AddAsync(user);

            // create tokens
            var (access, refresh) = await _tokenService.GenerateTokensAsync(user);

            return Created(string.Empty, new TokenResponse { AccessToken = access, RefreshToken = refresh.Token, RefreshTokenExpiresAt = refresh.ExpiresAt });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _users.GetByEmailAsync(req.Email);
            if (user == null) return Unauthorized();

            //var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
            //if (verify == PasswordVerificationResult.Failed) return Unauthorized();

            var (access, refresh) = await _tokenService.GenerateTokensAsync(user);

            return Ok(new TokenResponse { AccessToken = access, RefreshToken = refresh.Token, RefreshTokenExpiresAt = refresh.ExpiresAt });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var rt = await _users.GetRefreshTokenAsync(refreshToken);
            if (rt == null || rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow) return Unauthorized();

            var user = rt.User!;
            // revoke old refresh token and issue a new one (rotate)
            await _users.RevokeRefreshTokenAsync(rt);

            var (access, newRt) = await _tokenService.GenerateTokensAsync(user);
            return Ok(new TokenResponse { AccessToken = access, RefreshToken = newRt.Token, RefreshTokenExpiresAt = newRt.ExpiresAt });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken)
        {
            var rt = await _users.GetRefreshTokenAsync(refreshToken);
            if (rt == null) return NotFound();

            await _users.RevokeRefreshTokenAsync(rt);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            return Ok(user);
        }
    }
}
