using AutoMapper;
using ConsultationService.Domain.Entities;
using ConsultationService.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Commands
{
    public class CreateConsultationCommandHandler : IRequestHandler<CreateConsultationCommand, Guid>
    {
        private readonly ConsultationDbContext _db;
        private readonly IMapper _mapper;

        public CreateConsultationCommandHandler(ConsultationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Consultation>(request);
            _db.Consultations.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }
    }
}
