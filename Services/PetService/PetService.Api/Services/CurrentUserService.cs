using PetService.Application.Interfaces;
using System.Security.Claims;

namespace PetService.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        public Guid? UserId
        {
            get
            {
                var sub = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? _http.HttpContext?.User?.FindFirst("sub")?.Value;
                return sub is null ? null : Guid.Parse(sub);
            }
        }

        public string? Email => _http.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
}
