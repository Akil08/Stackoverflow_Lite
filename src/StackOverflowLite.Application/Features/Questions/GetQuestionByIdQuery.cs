using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public record GetQuestionByIdQuery(Guid QuestionId) : IRequest<Result<QuestionDetailDto>>;
