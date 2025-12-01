using MediatR;
using PetService.Application.DTOs;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Queries
{
    public record GetPagedAsync(int page, int pageSize) : IRequest<List<PetDto?>>; 
}
