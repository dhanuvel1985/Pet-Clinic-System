using AppointmentService.Application.Events;
using AppointmentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.Notifications
{
    public class NotificationService : INotificationService
    {
        public Task NotifyReceptionistAsync(AppointmentCreatedEvent evt)
        {
            // TODO: Send Email / SMS / Push Notification / Teams message
            Console.WriteLine($"🔔 Appointment created for Pet {evt.PetId}");
            return Task.CompletedTask;
        }
    }
}
