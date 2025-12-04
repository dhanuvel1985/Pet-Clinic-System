using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
