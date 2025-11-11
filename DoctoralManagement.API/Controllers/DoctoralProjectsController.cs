using DoctoralManagement.Application.DoctoralProjects.Commands;
using MediatR;
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

        // POST: api/DoctoralProjects/submit
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitDoctoralProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
