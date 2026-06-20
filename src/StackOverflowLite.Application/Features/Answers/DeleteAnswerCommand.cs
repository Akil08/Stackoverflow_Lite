using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public record DeleteAnswerCommand(Guid AnswerId) : IRequest<Result<bool>>;