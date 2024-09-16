using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Domain.AccessRights;

public class EntriesRightsConfiguration : IAccessRightsConfiguration<Entry>
{
    private readonly IUserResolver userResolver;
    
    public EntriesRightsConfiguration(IUserResolver userResolver)
    {
        this.userResolver = userResolver ?? throw new ArgumentNullException(nameof(userResolver));
    }

    public async Task<bool> CanRead(Entry entry, RequestContext requestContext)
        => IsOwner(entry, requestContext) || await IsAdmin(requestContext);

    public async Task<bool> CanWrite(Entry entry, RequestContext requestContext)
        => IsOwner(entry, requestContext) || await IsAdmin(requestContext);

    public async Task<bool> CanDelete(Entry entry, RequestContext requestContext)
        => IsOwner(entry, requestContext) || await IsAdmin(requestContext);
    
    private bool IsOwner(Entry entry, RequestContext requestContext)
        => requestContext.Requester?.Id == entry.UserId.ToString();
    
    private async Task<bool> IsAdmin(RequestContext requestContext)
    {
        if (requestContext.Requester == null)
            return false;
        
        return await userResolver.IsAdmin(requestContext.Requester.Id);
    }
}