using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public record GetQuestionsQuery(string? TagName, int Page = 1, int PageSize = 10) : IRequest<Result<PagedResult<QuestionSummaryDto>>>;
