using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetAppointmentQueryHandler : IRequestHandler<GetAppointmentQuery, List<AppointmentDto?>>
    {
        private readonly IAppointmentRepository _repo;

        public GetAppointmentQueryHandler(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AppointmentDto>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken = default)
        {
            return await _repo.GetAppointmentsAsync(request.page, request.pageSize);
        }
    }
}
