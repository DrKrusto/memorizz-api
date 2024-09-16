using FluentValidation;
using MediatR;
using Memorizz.Host.Domain.Behaviors;
using Memorizz.Host.Domain.Extensions;
using Memorizz.Host.Domain.Models;
using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence;
using Memorizz.Host.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Domain.Commands;

public record UpsertEntry(string UserId, DateOnly Date, string Content) : DomainRequest<RequestResult<Entry>>
{
    internal class Validator : AbstractValidator<UpsertEntry>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).ValidateGuid();
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content must be set.");
        }
    }
    
    internal class Handler : IRequestHandler<UpsertEntry, RequestResult<Entry>>
    {
        private readonly AppDbContext dbContext;
        private readonly IAccessRightsService accessRights;
        private readonly ILogger<UpsertEntry> logger;

        public Handler(AppDbContext dbContext, IAccessRightsService accessRights, ILogger<UpsertEntry> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.accessRights = accessRights ?? throw new ArgumentNullException(nameof(accessRights));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RequestResult<Entry>> Handle(UpsertEntry request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create entry for user {UserId} on date {Date}", request.UserId, request.Date);
            var parsedId = Guid.Parse(request.UserId);
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
            
            logger.LogInformation("Check access rights");
            var rights = await accessRights.HasRights(entry, request.RequestContext);
            if (!rights.CanWrite)
            {
                logger.LogWarning("User {UserId} has no rights to write entry {EntryId}", request.UserId, entry.Id);
                return RequestResult<Entry>.Forbidden("You have no rights to write this entry.");
            }
            
            logger.LogInformation("Update audited entities");
            dbContext.UpdateAuditedEntities(request.RequestContext.Requester);
            
            logger.LogInformation("Save changes");
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return RequestResult<Entry>.Success(entry);
        }
    }
}