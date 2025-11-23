using DoctoralManagement.Application.ECTS.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/students/{studentId}/[controller]")]
    [ApiController]
    public class ECTSTrackingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ECTSTrackingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("status")]
        [Authorize(Roles = "Secretary,Student,Mentor")]
        public async Task<IActionResult> GetEctsStatus(int studentId)
        {
            var query = new GetStudentEctsStatusQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("detailed")]
        [Authorize(Roles = "Secretary,Student,Mentor")]
        public async Task<IActionResult> GetEctsDetailed(int studentId)
        {
            var query = new GetStudentEctsDetailedQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
