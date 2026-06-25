using StackOverflowLite.Application.Common.Interfaces;
using StackExchange.Redis;

namespace StackOverflowLite.Infrastructure.Services;

public class ViewTrackingService : IViewTrackingService
{
    private const string ViewsHashKey = "question:views";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public ViewTrackingService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task IncrementAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.HashIncrementAsync(ViewsHashKey, questionId.ToString(), 1);
    }
}