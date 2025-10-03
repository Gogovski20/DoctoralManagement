using DoctoralManagement.Application.DoctoralPrograms.Commands;
using DoctoralManagement.Application.DoctoralPrograms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctoralManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoralProgramsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctoralProgramsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/DoctoralPrograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllDoctoralProgramsResponse>>> GetDoctoralPrograms()
        {
            var query = new GetAllDoctoralProgramsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/DoctoralPrograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetDoctoralProgramByIdResponse>> GetDoctoralProgram(int id)
        {
            try
            {
                var query = new GetDoctoralProgramByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/DoctoralPrograms
        [HttpPost]
        public async Task<ActionResult<CreateDoctoralProgramResponse>> CreateDoctoralProgram(CreateDoctoralProgramCommand command)
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

        // PUT: api/DoctoralPrograms/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateDoctoralProgramResponse>> UpdateDoctoralProgram(int id, UpdateDoctoralProgramCommand command)
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

        // DELETE: api/DoctoralPrograms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctoralProgram(int id)
        {
            try
            {
                var command = new DeleteDoctoralProgramCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
