
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Votes;

namespace StackOverflowLite.API.Controllers;

[ApiController]
[Route("api/votes")]
public class VotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Vote(VoteCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}