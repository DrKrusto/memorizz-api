using MediatR;
using Memorizz.Host.Domain.Behaviors;
using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence;
using Memorizz.Host.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Domain.Commands;

public record UpsertEntry(string UserId, DateOnly Date, string Content) : DomainRequest<Entry>
{
    internal class Handler : IRequestHandler<UpsertEntry, Entry>
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<UpsertEntry> logger;

        public Handler(AppDbContext dbContext, ILogger<UpsertEntry> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Entry> Handle(UpsertEntry request, CancellationToken cancellationToken)
        {
            var parsedId = Guid.Parse(request.UserId);
            
            logger.LogInformation("Upsert entry for user {UserId} on date {Date}", parsedId, request.Date);
            var entry = await dbContext.Entries.FirstOrDefaultAsync(e => e.UserId == parsedId && e.EntryDate == request.Date, cancellationToken);
            if (entry == null)
            {
                entry = new Entry
                {
                    Id = Guid.NewGuid(),
                    UserId = parsedId,
                    EntryDate = request.Date,
                    Content = request.Content
                };
                await dbContext.Entries.AddAsync(entry, cancellationToken);
            }
            else
            {
                entry.Content = request.Content;
            }
            
            logger.LogInformation("Update audited entities");
            dbContext.UpdateAuditedEntities(request.RequestContext.Requester);
            
            logger.LogInformation("Save changes");
            await dbContext.SaveChangesAsync(cancellationToken);
            return entry;
        }
    }
}