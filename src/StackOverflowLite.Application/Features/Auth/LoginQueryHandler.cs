using MediatR;
using Microsoft.AspNetCore.Identity;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Auth;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtService;

    public LoginQueryHandler(UserManager<ApplicationUser> userManager, IJwtTokenService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result<string>.Failure("Invalid credentials");

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
            return Result<string>.Failure("Invalid credentials");

        var token = _jwtService.GenerateToken(user);
        return Result<string>.Success(token);
    }
}