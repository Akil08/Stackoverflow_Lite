using MediatR;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Answers;

public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateAnswerCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<Guid>.Failure("Not authenticated");
        }

        if (!Guid.TryParse(_currentUser.UserId, out var authorId))
        {
            return Result<Guid>.Failure("Invalid user id");
        }

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            QuestionId = request.QuestionId,
            Content = request.Content,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsAccepted = false
        };

        _db.Answers.Add(answer);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(answer.Id);
    }
}