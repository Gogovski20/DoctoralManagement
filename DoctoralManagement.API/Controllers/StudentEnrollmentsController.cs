using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Courses.Commands;
using DoctoralManagement.Application.Courses.Queries;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/students/{studentId}/[controller]")]
    [ApiController]
    public class StudentEnrollmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStudentRepository _studentRepository;

        public StudentEnrollmentsController(IMediator mediator, ICurrentUserService currentUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _studentRepository = studentRepository;
        }

        [HttpPost("courses/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollInCourse(int studentId, int courseId)
        {
            var command = new EnrollStudentInCourseCommand { StudentId = studentId, CourseId = courseId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Student,Secretary,Mentor")]
        public async Task<IActionResult> GetEnrollments(int studentId)
        {
            var query = new GetStudentEnrollmentsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Secretary,Mentor")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var query = new GetAllCourseEnrollmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("/api/StudentEnrollments/all")]
        [Authorize(Roles = "Admin,Secretary,Mentor")]
        public async Task<IActionResult> GetAllEnrollmentsGlobal()
        {
            var query = new GetAllCourseEnrollmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPut("{enrollmentId}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteCourse(int studentId, int enrollmentId,
            [FromBody] CompleteCourseEnrollmentCommand command)
        {
            command.StudentId = studentId;
            command.EnrollmentId = enrollmentId;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<StudentEnrollmentResponse>>> GetMyEnrollments()
        {
            var userId = _currentUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentEnrollmentsQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
