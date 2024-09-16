namespace Memorizz.Host.Domain.Models;

public class AccessRights<TEntity>(bool canRead, bool canWrite, bool canDelete)
{
    public bool CanRead { get; init; } = canRead;
    public bool CanWrite { get; init; } = canWrite;
    public bool CanDelete { get; init; } = canDelete;
    
    public bool HasAllRights => CanRead && CanWrite && CanDelete;
}