using Memorizz.Host.Domain.Services;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddTransient<IUserResolver, UserResolver>();
}