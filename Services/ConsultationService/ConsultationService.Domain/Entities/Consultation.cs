using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Domain.Entities
{
    public class Consultation
    {
        public Guid Id { get; set; }
        public Guid AppointmentId {  get; set; }
        public Guid PetId { get; set; }
        public Guid VetId { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string Symptoms { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Prescription { get; set; } = "";
    }
}
