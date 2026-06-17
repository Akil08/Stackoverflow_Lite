using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public record UpdateQuestionCommand(Guid QuestionId, string Title, string Description, List<string> Tags) : IRequest<Result<bool>>;
