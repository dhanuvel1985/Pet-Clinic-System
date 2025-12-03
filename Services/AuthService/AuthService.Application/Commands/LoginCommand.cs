using AuthService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public record LoginCommand(string Email, string Password)
    : IRequest<TokenResponse>;

}
