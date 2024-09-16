using System.Security.Claims;
using Memorizz.Host.Domain.Models;
using Memorizz.Host.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Memorizz.Host.Domain.Services;

public interface IUserResolver
{
    bool ResolveUserOrDefault(string? userId, string? email, out IdentityUser? user);
    
    IdentityUser? TryResolveUser(string? userId, string? email = null);

    IdentityUser? TryResolveContextUser();
    
    Task<List<UserRoles>> GetUserRoles(string userId);
    
    Task<bool> IsAdmin(string userId);
}

public class UserResolver : IUserResolver
{
    private readonly UserManager<IdentityUser> userManager;
    
    public UserResolver(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }
    
    public bool ResolveUserOrDefault(string? userId, string? email, out IdentityUser? user)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            user = userManager.Users.FirstOrDefault(u => u.Id == userId);
            return user != null;
        }
        if (!string.IsNullOrEmpty(email))
        {
            user = userManager.Users.FirstOrDefault(u => u.Email == email);
            return user != null;
        }
        user = null;
        return false;
    }
    
    public IdentityUser? TryResolveUser(string? userId, string? email = null) 
        => ResolveUserOrDefault(userId, email, out var user) ? user : null;

    public IdentityUser? TryResolveContextUser()
    {
        var userId = ClaimsPrincipal.Current?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return TryResolveUser(userId);
    }
    
    public async Task<List<UserRoles>> GetUserRoles(string userId)
    {
        var user = TryResolveUser(userId);
        return user != null ? (await userManager.GetRolesAsync(user))
            .Select(x => Enum.TryParse(x, true, out UserRoles result) ? result : UserRoles.Unknown).ToList() : [];
    }
    
    public async Task<bool> IsAdmin(string userId)
    {
        var roles = await GetUserRoles(userId);
        return roles.Contains(UserRoles.Admin);
    }
}