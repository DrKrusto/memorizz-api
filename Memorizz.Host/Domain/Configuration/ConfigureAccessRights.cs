using Memorizz.Host.Domain.AccessRights;
using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureAccessRights
{
    public static IServiceCollection AddAccessRights(this IServiceCollection services)
        => services
            .AddTransient<IAccessRightsConfiguration<Entry>, EntriesRightsConfiguration>()
            .AddTransient<IAccessRightsService, AccessRightsService>();
}