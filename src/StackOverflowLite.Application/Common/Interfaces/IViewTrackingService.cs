namespace StackOverflowLite.Application.Common.Interfaces;

public interface IViewTrackingService
{
    Task IncrementAsync(Guid questionId, CancellationToken cancellationToken = default);
}