using DoctoralManagement.Application.Applications.Commands;
using DoctoralManagement.Application.Applications.Queries;
using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Applications
        [HttpGet]
        [Authorize(Roles = "Secretary,Committee,Mentor,Admin")]
        public async Task<ActionResult<IEnumerable<GetAllApplicationResponse>>> GetAllApplications(
            [FromQuery] ApplicationStatus? status,
            [FromQuery] int? programId,
            [FromQuery] int? studentId)
        {
            var query = new GetAllApplicationsQuery
            {
                Status = status,
                ProgramId = programId,
                StudentId = studentId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/Applications/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Secretary,Committee,Mentor,Admin,Student")]
        public async Task<ActionResult<GetApplicationByIdResponse>> GetApplicationById(int id)
        {
            try
            {
                var query = new GetApplicationByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Applications/student/5
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Secretary,Committee,Mentor")]
        public async Task<ActionResult<IEnumerable<GetStudentApplicationsResponse>>> GetStudentApplications(int studentId)
        {
            var query = new GetStudentApplicationsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/Applications/program/5
        [HttpGet("program/{programId}")]
        [Authorize(Roles = "Secretary,Committee,Admin")]
        public async Task<ActionResult<IEnumerable<GetProgramApplicationsResponse>>> GetProgramApplications(int programId,
            [FromQuery] ApplicationStatus? status)
        {
            var query = new GetProgramApplicationsQuery
            {
                ProgramId = programId,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST: api/Applications
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<SubmitApplicationResponse>> SubmitApplication(SubmitApplicationCommand command)
        {
            try
            {
                var result = await _mediator.Send(command); 
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Applications/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<ActionResult<UpdateApplicationResponse>> UpdateApplication(int id, UpdateApplicationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Applications/5/review
        [HttpPut("{id}/review")]
        [Authorize(Roles = "Secretary,Committee,Mentor")]
        public async Task<ActionResult<ReviewApplicationResponse>> ReviewApplication(int id, ReviewApplicationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student,Secretary")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            try
            {
                var command = new DeleteApplicationCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
