using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Features.Auth;

public record GetProfileQuery() : IRequest<Result<ProfileDto>>;