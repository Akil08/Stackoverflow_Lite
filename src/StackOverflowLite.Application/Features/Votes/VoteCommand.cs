using MediatR;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Votes;

public record VoteCommand(Guid TargetId, VoteTarget TargetType, VoteType VoteType) : IRequest<Result<bool>>;