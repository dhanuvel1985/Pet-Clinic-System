using AppointmentService.Application.DTOs;
using AppointmentService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Guid id);
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync();
        Task<List<AppointmentDto>> GetAppointmentsAsync(int page, int pageSize);
    }
}
