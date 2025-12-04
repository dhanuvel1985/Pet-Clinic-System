using AutoMapper;
using ConsultationService.Application.Interfaces;
using ConsultationService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Commands
{
    public class CreateConsultationCommandHandler
    : IRequestHandler<CreateConsultationCommand, Guid>
    {
        private readonly IConsultationRepository _repository;
        private readonly IPetServiceClient _petService;
        private readonly IUserServiceClient _userService;
        private readonly IAppointmentServiceClient _appointmentService;

        public CreateConsultationCommandHandler(
            IConsultationRepository repository,
            IPetServiceClient petService,
            IUserServiceClient userService,
            IAppointmentServiceClient appointmentService)
        {
            _repository = repository;
            _petService = petService;
            _userService = userService;
            _appointmentService = appointmentService;
        }

        public async Task<Guid> Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Validate Pet exists
            if (!await _petService.PetExists(request.PetId))
                throw new Exception("Invalid PetId. Pet does not exist.");

            // 2️⃣ Validate Vet exists
            if (!await _userService.UserExists(request.VetId))
                throw new Exception("Invalid VetId. Vet(user) does not exist.");

            // 3️⃣ Validate Appointment and check Accepted == true
            var appointment = await _appointmentService.GetAppointment(request.AppointmentId);
            if (appointment == null)
                throw new Exception("Invalid AppointmentId. Appointment not found.");

            if (appointment.Status == 0)
                throw new Exception("Appointment is not accepted. Consultation is not allowed.");

            if (appointment.PetId != request.PetId)
                throw new Exception("PetId does not match the appointment.");

            // 4️⃣ Create consultation entity
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                AppointmentId = request.AppointmentId,
                PetId = request.PetId,
                VetId = request.VetId,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = request.Symptoms,
                Diagnosis = request.Diagnosis,
                Prescription = request.Prescription
            };

            // 5️⃣ Save to DB
            await _repository.AddAsync(consultation);

            return consultation.Id;
        }
    }

}
