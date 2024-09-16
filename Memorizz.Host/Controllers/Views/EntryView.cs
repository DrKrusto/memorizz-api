using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// Represents an entry in a user's journal
/// </summary>
public record EntryView(Guid Id, UserView? User, string Content, DateOnly Date)
{
    /// <summary>
    /// Create an entry view from an entry
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="userResolver"></param>
    /// <returns></returns>
    public static EntryView From(Entry entry, IUserResolver userResolver)
    {
        var user = userResolver.TryResolveUser(entry.UserId.ToString());
        return new EntryView(entry.Id, user != null ? UserView.From(user) : null, entry.Content, entry.EntryDate);
    }
}