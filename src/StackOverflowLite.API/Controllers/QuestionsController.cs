using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Questions;
using StackOverflowLite.Application.Features.Answers;

namespace StackOverflowLite.API.Controllers;

/// <summary>
/// Provides endpoints for question management and question-related answer operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    /// <summary>
    /// Request body for accepting or unaccepting an answer for a question.
    /// </summary>
    /// <param name="AnswerId">Answer identifier to accept, or null to clear accepted answer.</param>
    public sealed record AcceptAnswerRequest(Guid? AnswerId);

    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns paginated questions, optionally filtered by tag.
    /// </summary>
    /// <param name="tagName">Optional tag name to filter by.</param>
    /// <param name="page">Page number starting from 1.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated question summaries.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? tagName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetQuestionsQuery(tagName, page, pageSize));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Returns details for a specific question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <returns>Question details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetQuestionByIdQuery(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Returns answers for a specific question.
    /// </summary>
    /// <param name="questionId">Question identifier.</param>
    /// <returns>Ordered list of answers.</returns>
    [HttpGet("{questionId}/answers")]
    public async Task<IActionResult> GetAnswers(Guid questionId)
    {
        var result = await _mediator.Send(new GetAnswersByQuestionQuery(questionId));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Accepts an answer for a question or clears acceptance when answerId is null.
    /// </summary>
    /// <param name="questionId">Question identifier.</param>
    /// <param name="request">Accept answer payload.</param>
    /// <returns>Operation status.</returns>
    [Authorize]
    [HttpPut("{questionId}/accept")]
    public async Task<IActionResult> AcceptAnswer(Guid questionId, [FromBody] AcceptAnswerRequest request)
    {
        var result = await _mediator.Send(new AcceptAnswerCommand(questionId, request.AnswerId));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new answer for a question.
    /// </summary>
    /// <param name="questionId">Question identifier.</param>
    /// <param name="command">Answer creation payload.</param>
    /// <returns>Created answer identifier.</returns>
    [Authorize]
    [HttpPost("{questionId}/answers")]
    public async Task<IActionResult> CreateAnswer(Guid questionId, CreateAnswerCommand command)
    {
        if (questionId != command.QuestionId) return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new question.
    /// </summary>
    /// <param name="command">Question creation payload.</param>
    /// <returns>Created question identifier.</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing question.
    /// </summary>
    /// <param name="id">Question identifier from route.</param>
    /// <param name="command">Question update payload.</param>
    /// <returns>Operation status.</returns>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionCommand command)
    {
        if (id != command.QuestionId) return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <returns>Operation status.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteQuestionCommand(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }
}