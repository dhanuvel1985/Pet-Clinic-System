using ConsultationService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Queries
{
    public class GetConsultationsPagedQuery : IRequest<IEnumerable<ConsultationDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
