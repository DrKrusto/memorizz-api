using MediatR;
using Memorizz.Host.Domain.Behaviors;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureRequestContext
{
    public static IServiceCollection AddRequestContextBehavior(this IServiceCollection services)
        => services.AddHttpContextAccessor()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestContextBehavior<,>));
}