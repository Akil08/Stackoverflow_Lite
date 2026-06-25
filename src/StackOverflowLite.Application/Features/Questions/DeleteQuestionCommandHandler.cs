using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Questions;

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public DeleteQuestionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser, ICacheService cacheService)
    {
        _db = db;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<Result<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result<bool>.Failure("Not authenticated");

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            return Result<bool>.Failure("Invalid user id");

        var question = await _db.Questions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);
        if (question == null) return Result<bool>.Failure("Question not found");

        if (question.AuthorId != userId) return Result<bool>.Failure("Forbidden");

        _db.Questions.Remove(question);
        await _db.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveByPrefixAsync("questions:list", cancellationToken);

        return Result<bool>.Success(true);
    }
}
