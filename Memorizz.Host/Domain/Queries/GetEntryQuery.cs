using FluentValidation;
using MediatR;
using Memorizz.Host.Domain.Behaviors;
using Memorizz.Host.Domain.Extensions;
using Memorizz.Host.Domain.Models;
using Memorizz.Host.Domain.Services;
using Memorizz.Host.Persistence;
using Memorizz.Host.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Domain.Queries;

public record GetEntryQuery(string EntryId) : DomainRequest<RequestResult<Entry>>
{
    internal class Validator : AbstractValidator<GetEntryQuery>
    {
        public Validator()
        {
            RuleFor(x => x.EntryId).ValidateGuid();
        }
    }

    internal class Handler : IRequestHandler<GetEntryQuery, RequestResult<Entry>>
    {
        private readonly AppDbContext dbContext;
        private readonly IAccessRightsService accessRights;
        private readonly ILogger<GetEntryQuery> logger;

        public Handler(AppDbContext dbContext, IAccessRightsService accessRights, ILogger<GetEntryQuery> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.accessRights = accessRights ?? throw new ArgumentNullException(nameof(accessRights));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<RequestResult<Entry>> Handle(GetEntryQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get entry {EntryId}", request.EntryId);
            var entry = await dbContext.Entries
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.EntryId), cancellationToken);
            if (entry == null)
                return RequestResult<Entry>.NotFound();
            
            logger.LogInformation("Check access rights");
            var rights = await accessRights.HasRights(entry, request.RequestContext);
            if (!rights.CanRead)
            {
                logger.LogWarning("Access denied");
                return RequestResult<Entry>.Forbidden();
            }
            
            return RequestResult<Entry>.Success(entry);
        }
    }
}