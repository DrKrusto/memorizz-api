namespace Memorizz.Host.Persistence.Models;

public class Entry : AuditedEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly EntryDate { get; set; }
    public string Content { get; set; }
}