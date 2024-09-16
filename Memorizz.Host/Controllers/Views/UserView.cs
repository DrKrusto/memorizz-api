using Microsoft.AspNetCore.Identity;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// Represents a user
/// </summary>
public record UserView(string Id, string Email)
{
    public static UserView From(IdentityUser user) => new(user.Id, user.Email ?? string.Empty);
}