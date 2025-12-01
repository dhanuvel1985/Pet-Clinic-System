using AppointmentService.Application.Commands;
using AppointmentService.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly CreateAppointmentCommandHandler _create;
        private readonly AcceptAppointmentCommandHandler _accept;
        private readonly GetAppointmentQueryHandler _query;

        public AppointmentsController(
            CreateAppointmentCommandHandler create,
            AcceptAppointmentCommandHandler accept,
            GetAppointmentQueryHandler query)
        {
            _create = create;
            _accept = accept;
            _query = query;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentCommand cmd)
        {
            return Ok(await _create.Handle(cmd));
        }

        [HttpPut("{id}/accept")]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> Accept(Guid id)
            => Ok(await _accept.Handle(id));

        [HttpGet]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> Get(int page, int pageSize)
        {
            var appointments = await _query.Handle(new GetAppointmentQuery(page, pageSize));
            return appointments is null ? NotFound() : Ok(appointments);
        }
    }

}
