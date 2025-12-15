using DoctoralManagement.Application.ApplicationDocuments;
using DoctoralManagement.Application.Applications.Commands;
using DoctoralManagement.Application.Applications.Queries;
using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IStudentRepository _studentRepository;

        public ApplicationsController(IMediator mediator, ICurrentUserService currentUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _studentRepository = studentRepository;
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

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<GetStudentApplicationsResponse>>> GetMyApplications()
        {
            var userId = _currentUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentApplicationsQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        // GET: api/Applications/student/5
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Secretary,Committee,Mentor,Admin")]
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

        [HttpPost("create-draft")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<CreateApplicationResponse>> CreateApplicationDraft(CreateApplicationCommand command)
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

        // POST: api/Applications
        [HttpPost("submit")]
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
        [Authorize(Roles = "Student,Admin")]
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
        [Authorize(Roles = "Admin,Secretary,Committee,Mentor")]
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
        [Authorize(Roles = "Student,Admin")]
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

        [HttpPost("{id}/upload-document")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UploadApplicationDocumentResponse>> UploadApplicationDocument(
            int id,
            [FromForm] UploadApplicationDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var command = new UploadApplicationDocumentCommand
            {
                ApplicationId = id,
                File = request.File,
                FileName = request.FileName,
                Type = request.Type
            };

            try
            {
                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{applicationId}/documents/{documentId}/download")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<IActionResult> DownloadApplicationDocument(int applicationId, int documentId)
        {
            var query = new DownloadApplicationDocumentQuery
            {
                ApplicationId = applicationId,
                DocumentId = documentId
            };

            var response = await _mediator.Send(query);

            if (response.FileBytes == null || response.FileBytes.Length == 0)
            {
                return NotFound("Document not found.");
            }

            return File(response.FileBytes, response.ContentType, response.FileName);
        }

        [HttpDelete("{applicationId}/documents/{documentId}")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> DeleteApplicationDocument(int applicationId, int documentId)
        {
            var command = new DeleteApplicationDocumentCommand
            {
                ApplicationId = applicationId,
                DocumentId = documentId
            };
            try
            {
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return BadRequest("Failed to delete document.");
                }
                return Ok("Document deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
