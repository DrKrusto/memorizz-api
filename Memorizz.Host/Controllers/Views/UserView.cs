using Microsoft.AspNetCore.Identity;

namespace Memorizz.Host.Controllers.Views;

/// <summary>
/// User view
/// </summary>
public record UserView(string Id, string Email)
{
    public static UserView From(IdentityUser user) => new(user.Id, user.Email ?? string.Empty);
}