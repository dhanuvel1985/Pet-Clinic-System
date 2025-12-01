using AppointmentService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public record GetAppointmentQuery(int page, int pageSize) : IRequest<List<AppointmentDto?>>;
}
