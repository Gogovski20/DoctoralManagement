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

        [HttpPost("schedule")]
        [Authorize(Roles = "Mentor,Secretary")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleThesisDefenseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("complete")]
        [Authorize(Roles = "Secretary")]
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
        [Authorize(Roles = "Secretary")]
        public async Task<IActionResult> FinalizeReviews(int defenseId)
        {
            var result = await _mediator.Send(new FinalizeCommitteeReviewsCommand
            {
                DefenseId = defenseId
            });

            return Ok(result);
        }
    }
}
