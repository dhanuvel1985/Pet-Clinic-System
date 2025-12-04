using AutoMapper;
using ConsultationService.Application.DTOs;
using ConsultationService.Application.Interfaces;
using ConsultationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Queries
{
    public class GetConsultationByIdQueryHandler
    : IRequestHandler<GetConsultationByIdQuery, Consultation?>
    {
        private readonly IConsultationRepository _repo;

        public GetConsultationByIdQueryHandler(IConsultationRepository repo)
        {
            _repo = repo;
        }

        public async Task<Consultation?> Handle(GetConsultationByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id);
        }
    }
}
