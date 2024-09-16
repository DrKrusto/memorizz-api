using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// Represents an entry in a user's journal
/// </summary>
public record EntryView(Guid Id, string Content, DateOnly Date, EntryMetadata Metadata)
{
    public static EntryView From(Entry entry, IUserResolver userResolver)
    {
        var user = userResolver.TryResolveUser(entry.UserId.ToString());
        var userView = user != null ? UserView.From(user) : null;
        return new(entry.Id, entry.Content, entry.EntryDate, new(userView));
    }
}