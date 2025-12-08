using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Queries;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECTSTrackingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStudentRepository _studentRepository;

        public ECTSTrackingsController(IMediator mediator, ICurrentUserService currentUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _studentRepository = studentRepository;
        }

        [HttpGet("{studentId}/status")]
        [Authorize(Roles = "Admin,Secretary,Student,Mentor")]
        public async Task<IActionResult> GetEctsStatus(int studentId)
        {
            var query = new GetStudentEctsStatusQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{studentId}/detailed")]
        [Authorize(Roles = "Admin,Secretary,Student,Mentor")]
        public async Task<IActionResult> GetEctsDetailed(int studentId)
        {
            var query = new GetStudentEctsDetailedQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my/status")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEctsStatus()
        {
            var userId = _currentUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null) 
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentEctsStatusQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my/detailed")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEctsDetailed()
        {
            var userId = _currentUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentEctsDetailedQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
