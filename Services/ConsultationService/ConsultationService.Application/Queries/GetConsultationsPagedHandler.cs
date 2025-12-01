using AutoMapper;
using ConsultationService.Application.DTOs;
using ConsultationService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Queries
{
    public class GetConsultationsPagedHandler
    : IRequestHandler<GetConsultationsPagedQuery, IEnumerable<ConsultationDto>>
    {
        private readonly ConsultationDbContext _db;
        private readonly IMapper _mapper;

        public GetConsultationsPagedHandler(ConsultationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ConsultationDto>> Handle(GetConsultationsPagedQuery request, CancellationToken cancellationToken)
        {
            var data = await _db.Consultations
                .OrderByDescending(x => x.ConsultationDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ConsultationDto>>(data);
        }
    }
}
