using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public record UpdateAnswerCommand(Guid AnswerId, string Content) : IRequest<Result<bool>>;