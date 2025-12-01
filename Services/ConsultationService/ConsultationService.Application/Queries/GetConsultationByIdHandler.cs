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
    public class GetConsultationByIdHandler
    : IRequestHandler<GetConsultationByIdQuery, ConsultationDto>
    {
        private readonly ConsultationDbContext _db;
        private readonly IMapper _mapper;

        public GetConsultationByIdHandler(ConsultationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ConsultationDto> Handle(GetConsultationByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _db.Consultations.FirstOrDefaultAsync(x => x.Id == request.Id);
            return _mapper.Map<ConsultationDto>(entity);
        }
    }
}
