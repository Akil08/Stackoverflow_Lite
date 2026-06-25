using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Answers;

namespace StackOverflowLite.API.Controllers;

/// <summary>
/// Provides endpoints for answer updates and deletion.
/// </summary>
[ApiController]
[Route("api/answers")]
public class AnswersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnswersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates an answer.
    /// </summary>
    /// <param name="id">Answer identifier from route.</param>
    /// <param name="command">Answer update payload.</param>
    /// <returns>Operation status.</returns>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAnswerCommand command)
    {
        if (id != command.AnswerId) return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes an answer.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <returns>Operation status.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteAnswerCommand(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }
}