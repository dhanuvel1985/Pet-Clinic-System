using ConsultationService.Application.DTOs;
using ConsultationService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Queries
{
    public class GetAllConsultationsQuery : IRequest<IEnumerable<Consultation>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
