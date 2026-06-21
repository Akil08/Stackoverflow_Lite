using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StackOverflowLite.Application.Common.Behaviours;
using StackOverflowLite.Application.Common.Services;

namespace StackOverflowLite.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ReputationService>();

        return services;
    }
}