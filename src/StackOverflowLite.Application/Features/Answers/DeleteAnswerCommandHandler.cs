using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Answers;

public class DeleteAnswerCommandHandler : IRequestHandler<DeleteAnswerCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteAnswerCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<bool>.Failure("Not authenticated");
        }

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
        {
            return Result<bool>.Failure("Invalid user id");
        }

        var answer = await _db.Answers.FirstOrDefaultAsync(x => x.Id == request.AnswerId, cancellationToken);
        if (answer is null)
        {
            return Result<bool>.Failure("Answer not found");
        }

        if (answer.AuthorId != userId)
        {
            return Result<bool>.Failure("Forbidden");
        }

        if (answer.IsAccepted)
        {
            return Result<bool>.Failure("Remove accepted status before deleting this answer");
        }

        _db.Answers.Remove(answer);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}