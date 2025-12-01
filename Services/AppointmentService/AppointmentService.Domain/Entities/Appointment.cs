using AppointmentService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Reason { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
