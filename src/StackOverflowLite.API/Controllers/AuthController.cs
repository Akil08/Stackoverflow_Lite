using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Auth;

namespace StackOverflowLite.API.Controllers;

/// <summary>
/// Provides authentication endpoints for registration, login, and profile retrieval.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account and returns a JWT token.
    /// </summary>
    /// <param name="command">Registration data.</param>
    /// <returns>JWT token on success.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Authenticates an existing user and returns a JWT token.
    /// </summary>
    /// <param name="query">Login credentials.</param>
    /// <returns>JWT token on success.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query)
    {
        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Returns the current authenticated user's profile.
    /// </summary>
    /// <returns>Current user profile details.</returns>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var result = await _mediator.Send(new GetProfileQuery());
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }
}