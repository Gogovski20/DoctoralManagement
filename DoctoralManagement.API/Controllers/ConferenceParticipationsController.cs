using DoctoralManagement.Application.ActivityDocuments;
using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ConferenceParticipations.Commands;
using DoctoralManagement.Application.ConferenceParticipations.Queries;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
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
        private readonly ICurrentUserService _currenUserService;
        private readonly IStudentRepository _studentRepository;

        public ConferenceParticipationsController(IMediator mediator, ICurrentUserService currenUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currenUserService = currenUserService;
            _studentRepository = studentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> AddConferenceParticipation([FromBody] AddConferenceParticipationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> GetAllConferences()
        {
            var query = new GetAllConferenceParticipationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<IActionResult> GetStudentConferences(int studentId)
        {
            var query = new GetStudentConferencesQuery {StudentId = studentId};
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ConferenceParticipationResponse>>> GetMyConferences()
        {
            var userId = _currenUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentConferencesQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<ActionResult<GetConferenceParticipationByIdResponse>> GetConferenceById(int id)
        {
            var result = await _mediator.Send(new GetConferenceParticipationByIdQuery { ConferenceId = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConferenceParticipation(int id)
        {
            await _mediator.Send(new DeleteConferenceParticipationCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{conferenceId}/upload-document")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadConferenceDocument(int conferenceId, [FromForm] UploadActivityDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadConferenceDocumentCommand
            {
                ConferenceId = conferenceId,
                File = request.File,
                FileName = request.File.FileName,
                Type = request.Type
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{conferenceId}/review")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> ReviewConference(int conferenceId, [FromBody] ReviewConferenceCommand command)
        {
            if (conferenceId != command.ConferenceId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //[HttpGet("{conferenceId}/download")]
        //[Authorize(Roles = "Student,Admin")]
        //public async Task<IActionResult> DownloadDocument(int conferenceId, int documentId)
        //{
        //    var command = new DownloadActivityDocumentQuery 
        //    {
        //        DocumentId = documentId,
        //        ActivityId = conferenceId,
        //        ActivityType = ActivityType.Conference
        //    };
        //    var result = await _mediator.Send(command);

        //    return File(result.FileBytes, result.ContentType, result.FileName);
        //}

        [HttpGet("{conferenceId}/download")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DownloadDocument(int conferenceId, int documentId)
        {
            try
            {
                var command = new DownloadActivityDocumentQuery
                {
                    DocumentId = documentId,
                    ActivityId = conferenceId,
                    ActivityType = ActivityType.Conference
                };

                var result = await _mediator.Send(command);

                // Check if download was successful
                if (!result.Success)
                {
                    return NotFound(new { message = result.Message });
                }

                // Return file for download
                return File(result.FileBytes, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error downloading document: {ex.Message}" });
            }
        }

        [HttpDelete("{conferenceId}/delete")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> DeleteDocument(int conferenceId, int documentId)
        {
            var command = new DeleteActivityDocumentCommand 
            { 
                ActivityDocumentId = documentId,
                ActivityType = ActivityType.Conference,
                ActivityId = conferenceId
            };
            var result = await _mediator.Send(command);
            return Ok(new { message = "Document deleted successfully", success = result });
        }
    }
}
