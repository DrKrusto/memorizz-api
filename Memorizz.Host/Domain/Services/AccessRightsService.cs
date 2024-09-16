using Memorizz.Host.Domain.AccessRights;
using Memorizz.Host.Domain.Models;

namespace Memorizz.Host.Domain.Services;

public interface IAccessRightsService
{
    public Task<AccessRights<TEntity>> HasRights<TEntity>(TEntity entity, RequestContext requestContext);
}

public class AccessRightsService : IAccessRightsService
{
    private readonly IServiceProvider serviceProvider;
    
    public AccessRightsService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<AccessRights<TEntity>> HasRights<TEntity>(TEntity entity, RequestContext requestContext)
    {
        var configuration = serviceProvider.GetService<IAccessRightsConfiguration<TEntity>>();
        if (configuration == null)
        {
            throw new InvalidOperationException($"No access rights configuration found for {typeof(TEntity).Name}");
        }
        return new AccessRights<TEntity>(
            await configuration.CanRead(entity, requestContext), 
            await configuration.CanWrite(entity, requestContext), 
            await configuration.CanDelete(entity, requestContext));
    }
}