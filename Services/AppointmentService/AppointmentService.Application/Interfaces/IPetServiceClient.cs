using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface IPetServiceClient
    {
        Task<bool> PetExists(Guid petId);
    }
}
