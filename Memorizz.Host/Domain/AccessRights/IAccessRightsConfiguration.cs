namespace Memorizz.Host.Domain.AccessRights;

public interface IAccessRightsConfiguration<TEntity>
{
    public Task<bool> CanRead(TEntity entity, RequestContext requestContext);
    public Task<bool> CanWrite(TEntity entity,RequestContext requestContext);
    public Task<bool> CanDelete(TEntity entity,RequestContext requestContext);
}