using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public Guid OwnerId { get; set; } // FK to AuthService User

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
