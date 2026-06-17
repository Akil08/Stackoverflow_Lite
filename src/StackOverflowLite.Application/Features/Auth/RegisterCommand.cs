using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Auth;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<Result<string>>;