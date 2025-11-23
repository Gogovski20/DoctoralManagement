using DoctoralManagement.Application.Mobilities.Commands;
using DoctoralManagement.Application.Mobilities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MobilitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> AddMobility([FromBody] AddMobilityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> GetStudentMobilities(int studentId)
        {
            var query = new GetStudentMobilitiesQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> UpdateMobility(int id, [FromBody] UpdateMobilityCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Secretary")]
        public async Task<IActionResult> DeleteMobility(int id)
        {
            await _mediator.Send(new DeleteMobilityCommand { Id = id });
            return NoContent();
        }
    }
}
