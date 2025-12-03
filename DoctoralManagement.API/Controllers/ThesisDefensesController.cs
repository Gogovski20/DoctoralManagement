using DoctoralManagement.Application.ConferenceParticipations.Commands;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Application.ThesisDefenseReviews;
using DoctoralManagement.Application.ThesisDefenses;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThesisDefensesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICommitteeReviewRepository _committeeReviewRepository;

        public ThesisDefensesController(IMediator mediator, ICommitteeReviewRepository committeeReviewRepository)
        {
            _mediator = mediator;
            _committeeReviewRepository = committeeReviewRepository;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Mentor,Admin,Committee,Secretary")]
        public async Task<ActionResult<GetDefenseByIdResponse>> GetDefenseById(int defenseId)
        {
            var result = await _mediator.Send(new GetDefenseByIdQuery { DefenseId = defenseId });
            return Ok(result);
        }

        [HttpGet("scheduled")]
        public async Task<ActionResult<IEnumerable<ScheduledDefenseResponse>>> GetScheduledDefenses()
        {
            var result = await _mediator.Send(new GetScheduledDefensesQuery());
            return Ok(result);
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Mentor,Admin")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleThesisDefenseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Complete([FromBody] CompleteThesisDefenseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{defenseId}/committee-review")]
        [Authorize(Roles = "Committee")]
        public async Task<IActionResult> SubmitReview(
            int defenseId,
            [FromBody] SubmitCommitteeReviewCommand command)
        {
            command.DefenseId = defenseId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{defenseId}/reviews")]
        [Authorize(Roles = "Student,Secretary,Mentor,Committee")]
        public async Task<IActionResult> GetReviews(int defenseId)
        {
            var reviews = await _committeeReviewRepository.GetByDefenseIdAsync(defenseId);
            return Ok(reviews);
        }

        [HttpPost("{defenseId}/finalize-reviews")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FinalizeReviews(int defenseId)
        {
            var result = await _mediator.Send(new FinalizeCommitteeReviewsCommand
            {
                DefenseId = defenseId
            });

            return Ok(result);
        }

        [HttpPost("{projectId}/upload-document")]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadThesisDocument(int projectId, [FromForm] UploadActivityDocumentDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadThesisDocumentCommand
            {
                ProjectId = projectId,
                File = request.File,
                FileName = request.File.FileName,
                DocumentType = request.Type
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{documentId}/review-document")]
        [Authorize(Roles = "Admin,Mentor,Secretary,Committee")]
        public async Task<IActionResult> ReviewThesisDocument([FromBody] ReviewThesisDocumentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


    }
}
