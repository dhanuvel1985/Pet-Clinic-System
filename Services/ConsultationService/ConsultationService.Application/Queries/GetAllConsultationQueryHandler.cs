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
    public class GetAllConsultationQueryHandler : IRequestHandler<GetAllConsultationsQuery, IEnumerable<Consultation>>
    {
        private readonly IConsultationRepository _repo;

        public GetAllConsultationQueryHandler(IConsultationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Consultation>> Handle(GetAllConsultationsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync();
        }
    }
}
