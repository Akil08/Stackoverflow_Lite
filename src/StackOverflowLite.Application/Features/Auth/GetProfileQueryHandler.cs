using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Auth;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetProfileQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            return Result<ProfileDto>.Failure("Not authenticated");

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return Result<ProfileDto>.Failure("Invalid user id");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<ProfileDto>.Failure("User not found");

        var totalQuestions = await _dbContext.Questions.CountAsync(q => q.AuthorId == userId, cancellationToken);
        var totalAnswers = await _dbContext.Answers.CountAsync(a => a.AuthorId == userId, cancellationToken);

        var dto = new ProfileDto
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Reputation = user.Reputation,
            TotalQuestions = totalQuestions,
            TotalAnswers = totalAnswers
        };

        return Result<ProfileDto>.Success(dto);
    }
}