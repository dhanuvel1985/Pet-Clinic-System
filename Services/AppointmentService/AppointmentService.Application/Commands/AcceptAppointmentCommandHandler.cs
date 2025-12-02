using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class AcceptAppointmentCommandHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly IPetServiceClient _petClient;

        public AcceptAppointmentCommandHandler(
            IAppointmentRepository repo,
            IPetServiceClient petClient)
        {
            _repo = repo;
            _petClient = petClient;
        }

        public async Task<Appointment> Handle(Guid appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            var exists = await _petClient.PetExists(appointment.PetId);

            if (!exists)
                throw new Exception("Pet does not exist in PetService");

            appointment.Status = AppointmentStatus.Accepted;

            await _repo.UpdateAsync(appointmentId);

            return appointment;
        }
    }

}
