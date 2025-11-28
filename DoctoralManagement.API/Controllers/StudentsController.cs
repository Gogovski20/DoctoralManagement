using DoctoralManagement.Application.ECTS.Commands;
using DoctoralManagement.Application.Students.Commands;
using DoctoralManagement.Application.Students.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Students
        [HttpGet]
        [Authorize(Roles = "Admin,Secretary,Mentor")]
        public async Task<ActionResult<IEnumerable<GetAllStudentsResponse>>> GetStudents()
        {
            var query = new GetAllStudentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretary,Mentor")]
        public async Task<ActionResult<GetStudentByIdResponse>> GetStudentById(int id)
        {
            var query = new GetStudentByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the URL does not match ID in the body.");
            }

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CreateStudentResponse>> CreateStudent(CreateStudentCommand command)
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

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var command = new DeleteStudentCommand { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{studentId}/semester")]
        [Authorize(Roles = "Secretary")]
        public async Task<IActionResult> AdvanceStudentSemester(int studentId,
            [FromBody] AdvanceStudentSemesterCommand command)
        {
            command.StudentId = studentId;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
