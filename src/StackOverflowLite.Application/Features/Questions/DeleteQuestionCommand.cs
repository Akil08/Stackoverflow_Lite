using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public record DeleteQuestionCommand(Guid QuestionId) : IRequest<Result<bool>>;
