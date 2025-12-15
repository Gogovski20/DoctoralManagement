using DoctoralManagement.Application.ActivityDocuments;
using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ConferenceParticipations.Queries;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Application.Publications.Commands;
using DoctoralManagement.Application.Publications.Queries;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currenUserService;
        private readonly IStudentRepository _studentRepository;

        public PublicationsController(IMediator mediator, ICurrentUserService currenUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currenUserService = currenUserService;
            _studentRepository = studentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> AddPublication([FromBody] CreatePublicationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> GetAllPublications()
        {
            var query = new GetAllPublicationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<IActionResult> GetStudentPublications(int studentId)
        {
            var query = new GetStudentPublicationsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<Application.Publications.Queries.PublicationResponse>>> GetMyPublications()
        {
            var userId = _currenUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentPublicationsQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<ActionResult<GetPublicationByIdResponse>> GetPublicationById(int id)
        {
            var result = await _mediator.Send(new GetPublicationByIdQuery { PublicationId = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> UpdatePublication(int id, [FromBody] UpdatePublicationCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            await _mediator.Send(new DeletePublicationCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{publicationId}/upload-document")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPublicationDocument(int publicationId, [FromForm] UploadActivityDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadPublicationDocumentCommand
            {
                PublicationId = publicationId,
                File = request.File,
                FileName = request.File.FileName,
                Type = request.Type
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{publicationId}/review")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> ReviewPublication(int publicationId, [FromBody] ReviewPublicationCommand command)
        {
            if (publicationId != command.PublicationId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //[HttpGet("{publicationId}/download")]
        //[Authorize(Roles = "Student,Admin")]
        //public async Task<IActionResult> DownloadDocument(int publicationId, int documentId)
        //{
        //    var command = new DownloadActivityDocumentQuery 
        //    { 
        //        DocumentId = documentId,
        //        ActivityId = publicationId,
        //        ActivityType = ActivityType.Publication
        //    };
        //    var result = await _mediator.Send(command);

        //    return File(result.FileBytes, result.ContentType, result.FileName);
        //}

        [HttpGet("{publicationId}/download")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DownloadDocument(int publicationId, int documentId)
        {
            try
            {
                var command = new DownloadActivityDocumentQuery
                {
                    DocumentId = documentId,
                    ActivityId = publicationId,
                    ActivityType = ActivityType.Publication
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

        [HttpDelete("{publicationId}/delete")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> DeleteDocument(int publicationId, int documentId)
        {
            var command = new DeleteActivityDocumentCommand 
            { 
                ActivityDocumentId = documentId,
                ActivityType = ActivityType.Publication,
                ActivityId = publicationId
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
