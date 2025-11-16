using DoctoralManagement.Application.ThesisDefenses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThesisDefensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ThesisDefensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleThesisDefenseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteThesisDefenseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
