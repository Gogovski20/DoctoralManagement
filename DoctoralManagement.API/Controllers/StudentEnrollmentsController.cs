using DoctoralManagement.Application.Courses.Commands;
using DoctoralManagement.Application.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/students/{studentId}/[controller]")]
    [ApiController]
    public class StudentEnrollmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentEnrollmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("courses/{courseId}")]
        public async Task<IActionResult> EnrollInCourse(int studentId, int courseId)
        {
            var command = new EnrollStudentInCourseCommand { StudentId = studentId, CourseId = courseId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments(int studentId)
        {
            var query = new GetStudentEnrollmentsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{enrollmentId}/complete")]
        public async Task<IActionResult> CompleteCourse(int studentId, int enrollmentId,
            [FromBody] CompleteCourseEnrollmentCommand command)
        {
            command.StudentId = studentId;
            command.EnrollmentId = enrollmentId;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
