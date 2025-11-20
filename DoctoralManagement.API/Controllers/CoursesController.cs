using DoctoralManagement.Application.Courses.Commands;
using DoctoralManagement.Application.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var query = new GetCourseByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("semester/{semester}")]
        public async Task<IActionResult> GetCoursesBySemester(int semester)
        {
            var query = new GetCoursesBySemesterQuery { Semester = semester };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
