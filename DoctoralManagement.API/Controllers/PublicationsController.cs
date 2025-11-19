using DoctoralManagement.Application.Publications.Commands;
using DoctoralManagement.Application.Publications.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPublication([FromBody] CreatePublicationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentPublications(int studentId)
        {
            var query = new GetStudentPublicationsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublication(int id, [FromBody] UpdatePublicationCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            await _mediator.Send(new DeletePublicationCommand { Id = id });
            return NoContent();
        }
    }
}
