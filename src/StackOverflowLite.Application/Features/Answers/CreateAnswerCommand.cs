using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public record CreateAnswerCommand(Guid QuestionId, string Content) : IRequest<Result<Guid>>;