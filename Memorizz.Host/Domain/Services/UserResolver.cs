using System.Security.Claims;
using Memorizz.Host.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Memorizz.Host.Domain.Services;

/// <summary>
/// Interface for resolving users
/// </summary>
public interface IUserResolver
{
    /// <summary>
    /// Resolve a user or return the default
    /// </summary>
    /// <param name="userId">User's ID to resolve the user</param>
    /// <param name="email">User's mail to resolve the user</param>
    /// <param name="user">Returned user if it exists</param>
    /// <returns>True if user exists else false</returns>
    bool ResolveUserOrDefault(string? userId, string? email, out IdentityUser? user);
    
    /// <summary>
    /// Try to resolve a user
    /// </summary>
    /// <param name="userId">User's ID to resolve the user</param>
    /// <param name="email">User's mail to resolve the user</param>
    /// <returns></returns>
    IdentityUser? TryResolveUser(string? userId, string? email = null);
    
    /// <summary>
    /// Try to resolve the user from the HTTP context
    /// </summary>
    /// <returns></returns>
    IdentityUser? TryResolveContextUser();
}


/// <summary>
/// Resolves users
/// </summary>
public class UserResolver : IUserResolver
{
    private readonly AppDbContext dbContext;
    
    /// <summary>
    /// Constructor for the user resolver
    /// </summary>
    /// <param name="dbContext"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserResolver(AppDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    /// <summary>
    /// Resolve a user or return the default
    /// </summary>
    /// <param name="userId">User's ID to resolve the user</param>
    /// <param name="email">User's mail to resolve the user</param>
    /// <param name="user">Returned user if it exists</param>
    /// <returns>True if user exists else false</returns>
    public bool ResolveUserOrDefault(string? userId, string? email, out IdentityUser? user)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            return user != null;
        }
        if (!string.IsNullOrEmpty(email))
        {
            user = dbContext.Users.FirstOrDefault(u => u.Email == email);
            return user != null;
        }
        user = null;
        return false;
    }
    
    /// <summary>
    /// Try to resolve a user
    /// </summary>
    /// <param name="userId">User's ID to resolve the user</param>
    /// <param name="email">User's mail to resolve the user</param>
    /// <returns></returns>
    public IdentityUser? TryResolveUser(string? userId, string? email = null) 
        => ResolveUserOrDefault(userId, email, out var user) ? user : null;

    /// <summary>
    /// Try to resolve the user from the HTTP context
    /// </summary>
    /// <returns></returns>
    public IdentityUser? TryResolveContextUser()
    {
        var userId = ClaimsPrincipal.Current?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return TryResolveUser(userId);
    }
}