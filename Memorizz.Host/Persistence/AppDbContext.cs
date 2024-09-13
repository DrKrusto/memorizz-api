using System.Security.Claims;
using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Entry> Entries { get; init; }
    
    public void UpdateAuditedEntities(IdentityUser? user)
    {
        var userGuid = Guid.TryParse(user?.Id, out var id) ? id : default;
        
        var entries = ChangeTracker.Entries().Where(e => e is { Entity: AuditedEntity });
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((AuditedEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                ((AuditedEntity)entry.Entity).CreatedBy = userGuid;
            }
            ((AuditedEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            ((AuditedEntity)entry.Entity).UpdatedBy = userGuid;
        }
    }
}