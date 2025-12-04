using ConsultationService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<AppointmentDto?> GetAppointment(Guid appointmentId);
    }
}
