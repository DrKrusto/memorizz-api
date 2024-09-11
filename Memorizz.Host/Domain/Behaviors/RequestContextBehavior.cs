using System.Security.Claims;
using MediatR;
using Memorizz.Host.Domain.Services;

namespace Memorizz.Host.Domain.Behaviors;

public class RequestContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : DomainRequest<TResponse>
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IUserResolver userResolver;
    private readonly ILogger<TRequest> logger;

    public RequestContextBehavior(IHttpContextAccessor contextAccessor, IUserResolver userResolver, ILogger<TRequest> logger)
    {
        this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        this.userResolver = userResolver ?? throw new ArgumentNullException(nameof(userResolver));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = contextAccessor.HttpContext;
        if (httpContext == null)
        {
            logger.LogWarning("No HTTP context available");
            return await next();
        }
        
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            logger.LogWarning("No user ID available in the HTTP context");
            return await next();
        }
        
        var user = userResolver.TryResolveUser(userId);
        if (user == null)
        {
            logger.LogCritical("User {UserId} not found", userId);
            return await next();
        }
        
        request.RequestContext = new RequestContext
        {
            Requester = user
        };
        
        return await next();
    }
}