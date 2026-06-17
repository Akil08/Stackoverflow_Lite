using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Auth;

public record LoginQuery(string Email, string Password) : IRequest<Result<string>>;