using DoctoralManagement.Application.ActivityDocuments;
using DoctoralManagement.Application.DoctoralProjects.Commands;
using DoctoralManagement.Application.DoctoralProjects.Queries;
using DoctoralManagement.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoralProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctoralProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-draft")]
        [Authorize(Roles = "Student,Mentor")]
        public async Task<IActionResult> CreateDraft([FromBody] CreateDoctoralProjectDraftCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // POST: api/DoctoralProjects/submit
        [HttpPost("submit")]
        [Authorize(Roles = "Student,Mentor")]
        public async Task<IActionResult> Submit([FromBody] SubmitDoctoralProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Secretary,Committee,Mentor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDoctoralProjectsQuery());
            return Ok(result);
        }

        [HttpGet("by-student/{studentId}")]
        [Authorize(Roles = "Student,Secretary,Mentor,Committee,Admin")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var result = await _mediator.Send(new GetDoctoralProjectsByStudentQuery(studentId));
            return Ok(result);
        }

        [HttpGet("by-mentor/{mentorId}")]
        [Authorize(Roles = "Student,Secretary,Mentor,Committee,Admin")]
        public async Task<IActionResult> GetByMentor(int mentorId)
        {
            var result = await _mediator.Send(new GetDoctoralProjectsByMentorQuery(mentorId));
            return Ok(result);
        }

        [HttpPost("review")]
        [Authorize(Roles = "Mentor,Committee")]
        public async Task<IActionResult> Review([FromBody] ReviewDoctoralProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Mentor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctoralProjectCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Secretary,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _mediator.Send(new DeleteDoctoralProjectCommand { Id = id });
            if (!ok)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpPost("{projectId}/upload-proposal")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProposalDocument(int projectId, [FromForm] UploadActivityDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadDoctoralProjectProposalCommand
            {
                DoctoralProjectId = projectId,
                File = request.File,
                FileName = request.File.FileName,
                DocumentType = request.Type
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{projectId}/complete")]
        [Authorize(Roles = "Admin,Mentor,Committee")]
        public async Task<IActionResult> CompleteDoctoralProject(int projectId)
        {
            var command = new CompleteDoctoralProjectCommand { ProjectId = projectId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //[HttpGet("{projectId}/download")]
        //[Authorize(Roles = "Student,Admin")]
        //public async Task<IActionResult> DownloadDocument(int projectId, int documentId)
        //{
        //    var command = new DownloadActivityDocumentQuery 
        //    { 
        //        DocumentId = documentId,
        //        ActivityId = projectId,
        //        ActivityType = ActivityType.DoctoralProject
        //    };
        //    var result = await _mediator.Send(command);

        //    return File(result.FileBytes, result.ContentType, result.FileName);
        //}

        [HttpGet("{projectId}/download")]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DownloadDocument(int projectId, int documentId)
        {
            try
            {
                var command = new DownloadActivityDocumentQuery
                {
                    DocumentId = documentId,
                    ActivityId = projectId,
                    ActivityType = ActivityType.DoctoralProject
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

        [HttpDelete("{projectId}/delete")]
        public async Task<IActionResult> DeleteDocument(int projectId, int documentId)
        {
            var command = new DeleteActivityDocumentCommand 
            {
                ActivityDocumentId = documentId,
                ActivityType = ActivityType.DoctoralProject,
                ActivityId = projectId
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
