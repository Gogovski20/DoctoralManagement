using DoctoralManagement.Application.Mentors.Commands;
using DoctoralManagement.Application.Mentors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MentorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetMentorResponse>>> GetAll()
        {
            var mentors = await _mediator.Send(new GetAllMentorsQuery());
            return Ok(mentors);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<ActionResult<GetMentorResponse>> GetById(int id)
        {
            var mentor = await _mediator.Send(new GetMentorByIdQuery { Id = id });
            if (mentor == null)
            {
                return NotFound();
            }
            return Ok(mentor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MentorResponse>> Create([FromBody] CreateMentorCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MentorResponse>> Update(int id, [FromBody] UpdateMentorCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteMentorCommand { Id = id });
            return NoContent();
        }
    }
}
