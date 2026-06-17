using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public record CreateQuestionCommand(string Title, string Description, List<string> Tags) : IRequest<Result<Guid>>;
