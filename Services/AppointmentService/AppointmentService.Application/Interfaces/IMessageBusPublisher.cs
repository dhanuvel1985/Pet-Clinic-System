using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface IMessageBusPublisher
    {
        Task PublishAsync(string topic, object message);
    }
}
