using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Queries
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _users;

        public GetUserByIdHandler(IUserRepository users)
        {
            _users = users;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery req, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(req.Id);
            return new UserDto(user.Id, user.Email, user.RoleId, user.Role.Name);
        }
    }
}
