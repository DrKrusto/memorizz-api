using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// Represents a simple entry in a user's journal
/// </summary>
/// <param name="Id"></param>
/// <param name="Content"></param>
/// <param name="Date"></param>
public record SimpleEntryView(Guid Id, string Content, DateOnly Date)
{
    public static SimpleEntryView From(Entry entry)
        => new(entry.Id, entry.Content, entry.EntryDate);
}