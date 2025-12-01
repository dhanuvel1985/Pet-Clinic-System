using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class CreateAppointmentCommand
    {
        public Guid PetId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Reason { get; set; }
    }
}
