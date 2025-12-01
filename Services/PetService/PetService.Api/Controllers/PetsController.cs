using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetService.Application.Commands;
using PetService.Application.Queries;

namespace PetService.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/pets")]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePetCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize)
        {
            var pet = await _mediator.Send(new GetPagedAsync(page, pageSize));
            return pet is null ? NotFound() : Ok(pet);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pet = await _mediator.Send(new GetPetByIdQuery(id));
            return pet is null ? NotFound() : Ok(pet);
        }
    }
}
