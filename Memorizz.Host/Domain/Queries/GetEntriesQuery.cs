using MediatR;
using Memorizz.Host.Persistence;
using Memorizz.Host.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Domain.Queries;

public record GetEntriesQuery(string UserId, DateTime? From, DateTime? To) : IRequest<IEnumerable<Entry>>
{
    internal class Handler : IRequestHandler<GetEntriesQuery, IEnumerable<Entry>>
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<GetEntriesQuery> logger;
        
        public Handler(AppDbContext dbContext, ILogger<GetEntriesQuery> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Entry>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
        {
            var parsedId = Guid.Parse(request.UserId);
            
            logger.LogInformation("Get entries for user {UserId}", parsedId);
            var entries = dbContext.Entries.Where(e => e.UserId == parsedId);
            if (request.Month.HasValue)
            {
                entries = entries.Where(e => e.EntryDate.Month == request.Month.Value);
            }
            
            return await entries.ToListAsync(cancellationToken);
        }
    }
}