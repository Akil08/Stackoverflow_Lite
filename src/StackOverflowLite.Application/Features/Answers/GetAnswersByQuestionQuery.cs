using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public record GetAnswersByQuestionQuery(Guid QuestionId) : IRequest<Result<List<AnswerDto>>>;