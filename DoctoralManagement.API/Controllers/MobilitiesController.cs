using DoctoralManagement.Application.ActivityDocuments;
using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ConferenceParticipations.Queries;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Application.Mobilities.Commands;
using DoctoralManagement.Application.Mobilities.Queries;
using DoctoralManagement.Application.Publications.Commands;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currenUserService;
        private readonly IStudentRepository _studentRepository;

        public MobilitiesController(IMediator mediator, ICurrentUserService currenUserService, IStudentRepository studentRepository)
        {
            _mediator = mediator;
            _currenUserService = currenUserService;
            _studentRepository = studentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> AddMobility([FromBody] AddMobilityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<IActionResult> GetStudentMobilities(int studentId)
        {
            var query = new GetStudentMobilitiesQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<MobilityResponse>>> GetMyMobilities()
        {
            var userId = _currenUserService.UserId;
            var student = await _studentRepository.GetByUserIdAsync(userId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var query = new GetStudentMobilitiesQuery { StudentId = student.Id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        public async Task<ActionResult<GetMobilityByIdResponse>> GetMobilityById(int id)
        {
            var result = await _mediator.Send(new GetMobilityByIdQuery { MobilityId = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> UpdateMobility(int id, [FromBody] UpdateMobilityCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> DeleteMobility(int id)
        {
            await _mediator.Send(new DeleteMobilityCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{mobilityId}/upload-document")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMobilityDocument(int mobilityId, [FromForm] UploadActivityDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadMobilityDocumentCommand
            {
                MobilityId = mobilityId,
                File = request.File,
                FileName = request.File.FileName,
                Type = request.Type
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{mobilityId}/review")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> ReviewMobility(int mobilityId, [FromBody] ReviewMobilityCommand command)
        {
            if (mobilityId != command.MobilityId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //[HttpGet("{mobilityid}/download")]
        //[Authorize(Roles = "Student,Admin")]
        //public async Task<IActionResult> DownloadDocument(int mobilityId, int documentId)
        //{
        //    var command = new DownloadActivityDocumentQuery
        //    {
        //        DocumentId = documentId,
        //        ActivityId = mobilityId,
        //        ActivityType = ActivityType.Mobility
        //    };
        //    var result = await _mediator.Send(command);

        //    return File(result.FileBytes, result.ContentType, result.FileName);
        //}

        [HttpGet("{mobilityId}/download")]
        [Authorize(Roles = "Student,Admin,Mentor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DownloadDocument(int mobilityId, int documentId)
        {
            try
            {
                var command = new DownloadActivityDocumentQuery
                {
                    DocumentId = documentId,
                    ActivityId = mobilityId,
                    ActivityType = ActivityType.Mobility
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


        //[HttpGet("{mobilityId}/download")]
        //[Authorize(Roles = "Student,Admin")]
        //public async Task<IActionResult> DownloadDocument(int mobilityId)
        //{
        //    try
        //    {
        //        var query = new DownloadActivityDocumentQuery
        //        {
        //            ActivityId = mobilityId,
        //            ActivityType = ActivityType.Mobility
        //        };

        //        var result = await _mediator.Send(query);
        //        return File(result.FileBytes, result.ContentType, result.FileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message, mobilityId = mobilityId });
        //    }
        //}


        [HttpDelete("{mobilityId}/delete")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> DeleteDocument(int mobilityId, int documentId)
        {
            var command = new DeleteActivityDocumentCommand 
            {
                ActivityDocumentId = documentId,
                ActivityType = ActivityType.Mobility,
                ActivityId = mobilityId
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
