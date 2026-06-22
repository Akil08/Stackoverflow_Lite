using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public record AcceptAnswerCommand(Guid QuestionId, Guid? AnswerId) : IRequest<Result<bool>>;