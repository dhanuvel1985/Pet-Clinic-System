using AppointmentService.Application.Events;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class CreateAppointmentCommandHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly IMessageBusPublisher _publisher;
        private readonly IPetServiceClient _petService;
        private readonly IUserServiceClient _userServiceClient;
        public CreateAppointmentCommandHandler(
            IAppointmentRepository repo,
            //IMessageBusPublisher publisher
            IPetServiceClient petService,
            IUserServiceClient userServiceClient
            )
        {
            _repo = repo;
            //_publisher = publisher;
            _userServiceClient = userServiceClient;
            _petService = petService;
        }

        public async Task<Appointment> Handle(CreateAppointmentCommand cmd)
        {
            var petexists = await _petService.PetExists(cmd.PetId);
            if (!petexists)
                throw new Exception("pet does not exist. cannot create appointment.");

            var userExists = await _userServiceClient.UserExists(cmd.UserId);
            if (!userExists)
                throw new Exception("User does not exist. Cannot create Appointment.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PetId = cmd.PetId,
                UserId = cmd.UserId,
                PreferredDate = cmd.PreferredDate,
                Reason = cmd.Reason
            };

            await _repo.AddAsync(appointment);

            // 🔥 Publish event to Azure Service Bus for receptionists
            //await _publisher.PublishAsync("appointment-created", new AppointmentCreatedEvent
            //{
            //    AppointmentId = appointment.Id,
            //    PetId = appointment.PetId,
            //    UserId = appointment.UserId,
            //    PreferredDate = appointment.PreferredDate,
            //    Reason = appointment.Reason,
            //    //CorrelationId = correlationIdFromContext // optional
            //});

            return appointment;
        }
    }

}
