using Memorizz.Host.Domain.Services;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureGlobalExceptionHandler
{
    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        => services.AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
}