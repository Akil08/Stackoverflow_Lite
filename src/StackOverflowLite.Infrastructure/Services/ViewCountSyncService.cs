using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackOverflowLite.Infrastructure.Persistence;
using StackExchange.Redis;

namespace StackOverflowLite.Infrastructure.Services;

public class ViewCountSyncService : IHostedService
{
    private const string ViewsHashKey = "question:views";

    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private CancellationTokenSource? _stoppingCts;
    private Task? _backgroundTask;

    public ViewCountSyncService(IConnectionMultiplexer connectionMultiplexer, IServiceScopeFactory serviceScopeFactory)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _backgroundTask = RunLoopAsync(_stoppingCts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_stoppingCts is null || _backgroundTask is null)
        {
            return;
        }

        _stoppingCts.Cancel();

        await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }

    private async Task RunLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await SyncViewCountsAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
                // Ignore background sync failures; next cycle will retry.
            }
        }
    }

    private async Task SyncViewCountsAsync(CancellationToken cancellationToken)
    {
        var redisDb = _connectionMultiplexer.GetDatabase();
        var entries = await redisDb.HashGetAllAsync(ViewsHashKey);

        if (entries.Length == 0)
        {
            return;
        }

        var increments = new Dictionary<Guid, long>();
        foreach (var entry in entries)
        {
            if (!Guid.TryParse(entry.Name, out var questionId))
            {
                continue;
            }

            if (!long.TryParse(entry.Value, out var value) || value <= 0)
            {
                continue;
            }

            increments[questionId] = value;
        }

        if (increments.Count == 0)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var questionIds = increments.Keys.ToList();
        var questions = await dbContext.Questions
            .Where(question => questionIds.Contains(question.Id))
            .ToListAsync(cancellationToken);

        foreach (var question in questions)
        {
            if (!increments.TryGetValue(question.Id, out var increment))
            {
                continue;
            }

            question.ViewCount = Math.Min(int.MaxValue, question.ViewCount + (int)increment);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var resetEntries = increments
            .Select(pair => new HashEntry(pair.Key.ToString(), 0))
            .ToArray();

        await redisDb.HashSetAsync(ViewsHashKey, resetEntries);
    }
}