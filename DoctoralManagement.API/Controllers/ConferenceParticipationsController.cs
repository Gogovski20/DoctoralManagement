using DoctoralManagement.Application.ConferenceParticipations.Commands;
using DoctoralManagement.Application.ConferenceParticipations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConferenceParticipationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConferenceParticipationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> AddConferenceParticipation([FromBody] AddConferenceParticipationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> GetStudentConferences(int studentId)
        {
            var query = new GetStudentConferencesQuery {StudentId = studentId};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> UpdateConferenceParticipation(int id, 
            [FromBody] UpdateConferenceParticipationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Secretary")]
        public async Task<IActionResult> DeleteConferenceParticipation(int id)
        {
            await _mediator.Send(new DeleteConferenceParticipationCommand { Id = id });
            return NoContent();
        }
    }
}
