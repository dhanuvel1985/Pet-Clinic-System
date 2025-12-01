using AppointmentService.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyReceptionistAsync(AppointmentCreatedEvent evt);
    }
}
