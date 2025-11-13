using DoctoralManagement.Application.DoctoralProjects.Commands;
using DoctoralManagement.Application.DoctoralProjects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoralProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctoralProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/DoctoralProjects/submit
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitDoctoralProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDoctoralProjectsQuery());
            return Ok(result);
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var result = await _mediator.Send(new GetDoctoralProjectsByStudentQuery(studentId));
            return Ok(result);
        }

        [HttpGet("by-mentor/{mentorId}")]
        public async Task<IActionResult> GetByMentor(int mentorId)
        {
            var result = await _mediator.Send(new GetDoctoralProjectsByMentorQuery(mentorId));
            return Ok(result);
        }

        [HttpPost("review")]
        public async Task<IActionResult> Review([FromBody] ReviewDoctoralProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
