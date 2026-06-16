using System.Reflection;
using FluentValidation;
using MediatR;
using StackOverflowLite.Application.Common.Models;

namespace StackOverflowLite.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToArray();

        if (failures.Length == 0)
        {
            return await next();
        }

        var error = string.Join("; ", failures.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));
        return CreateFailureResponse(error);
    }

    private static TResponse CreateFailureResponse(string error)
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod(
                nameof(Result<int>.Failure),
                BindingFlags.Public | BindingFlags.Static,
                new[] { typeof(string) });

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }
        }

        return default!;
    }
}