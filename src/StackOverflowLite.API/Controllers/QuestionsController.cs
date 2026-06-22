using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Questions;
using StackOverflowLite.Application.Features.Answers;

namespace StackOverflowLite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    public sealed record AcceptAnswerRequest(Guid? AnswerId);

    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? tagName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetQuestionsQuery(tagName, page, pageSize));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetQuestionByIdQuery(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{questionId}/answers")]
    public async Task<IActionResult> GetAnswers(Guid questionId)
    {
        var result = await _mediator.Send(new GetAnswersByQuestionQuery(questionId));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("{questionId}/accept")]
    public async Task<IActionResult> AcceptAnswer(Guid questionId, [FromBody] AcceptAnswerRequest request)
    {
        var result = await _mediator.Send(new AcceptAnswerCommand(questionId, request.AnswerId));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("{questionId}/answers")]
    public async Task<IActionResult> CreateAnswer(Guid questionId, CreateAnswerCommand command)
    {
        if (questionId != command.QuestionId) return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionCommand command)
    {
        if (id != command.QuestionId) return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteQuestionCommand(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }
}