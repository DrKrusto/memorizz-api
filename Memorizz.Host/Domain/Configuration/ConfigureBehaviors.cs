using MediatR;
using Memorizz.Host.Domain.Behaviors;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureBehaviors
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services)
        => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestContextBehavior<,>));
}