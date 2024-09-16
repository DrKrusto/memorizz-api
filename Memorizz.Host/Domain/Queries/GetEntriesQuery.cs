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

public record GetEntriesQuery(string UserId, DateTime? From, DateTime? To) : DomainRequest<RequestResult<IEnumerable<Entry>>>
{
    internal class Validator : AbstractValidator<GetEntriesQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).ValidateGuid();
            When(x => x.From.HasValue || x.To.HasValue, () =>
            {
                RuleFor(x => x.From).NotNull().WithMessage("'From' date must be set if 'To' date is set.");
                RuleFor(x => x.To).NotNull().WithMessage("'To' date must be set if 'From' date is set.");
                RuleFor(x => x.From).LessThanOrEqualTo(x => x.To)
                    .WithMessage("'From' date must be less than or equal to 'To' date.");
            });
        }
    }
    
    internal class Handler : IRequestHandler<GetEntriesQuery, RequestResult<IEnumerable<Entry>>>
    {
        private readonly AppDbContext dbContext;
        private readonly IAccessRightsService accessRights;
        private readonly ILogger<GetEntriesQuery> logger;
        
        public Handler(AppDbContext dbContext, IAccessRightsService accessRights, ILogger<GetEntriesQuery> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.accessRights = accessRights ?? throw new ArgumentNullException(nameof(accessRights));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RequestResult<IEnumerable<Entry>>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get entries for user {UserId}", request.UserId);
            var parsedId = Guid.Parse(request.UserId);
            var entries = dbContext.Entries.Where(e => e.UserId == parsedId);
            if (!entries.Any())
            {
                return RequestResult<IEnumerable<Entry>>.NotFound();
            }
            
            logger.LogInformation("Check access rights");
            var rights = await accessRights.HasRights(entries.First(), request.RequestContext);
            if (!rights.CanRead)
            {
                logger.LogWarning("Access denied");
                return RequestResult<IEnumerable<Entry>>.Forbidden();
            }
            
            logger.LogInformation("Filtering entries by date range");
            if (request is { From: not null, To: not null })
            {
                var dateFrom = DateOnly.FromDateTime(request.From.Value.Date);
                var dateTo = DateOnly.FromDateTime(request.To.Value.Date);
                entries = entries.Where(e => e.EntryDate >= dateFrom && e.EntryDate <= dateTo);
            }
            
            return RequestResult<IEnumerable<Entry>>.Success(await entries.OrderBy(x => x.EntryDate).ToListAsync(cancellationToken));
        }
    }
}