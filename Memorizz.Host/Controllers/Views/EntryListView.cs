using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// Represents a list of entries in a user's journal
/// </summary>
/// <param name="Entries"></param>
/// <param name="Metadata"></param>
public record EntryListView(IEnumerable<SimpleEntryView> Entries, EntryMetadata Metadata)
{
    public static EntryListView From(IEnumerable<Entry> entries, IUserResolver userResolver, string userId)
    {
        var userResponse = userResolver.TryResolveUser(userId);
        var userView = userResponse != null ? UserView.From(userResponse) : null;
        var entriesList = entries.ToList();
        if (!entriesList.Any())
        {
            return new([], new(userView));
        }
        return new(entriesList.Select(SimpleEntryView.From), new(userView));
    }
}