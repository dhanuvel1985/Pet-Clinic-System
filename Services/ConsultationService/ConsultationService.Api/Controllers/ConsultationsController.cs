using ConsultationService.Application.Commands;
using ConsultationService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationService.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConsultationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateConsultationCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new { id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllConsultationsQuery());
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetConsultationByIdQuery());
            return Ok(result);
        }
    }
}
