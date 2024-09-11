using MediatR;

namespace Memorizz.Host.Domain.Behaviors;

public abstract record DomainRequest<TResponse> : IRequest<TResponse>
{
    internal RequestContext RequestContext { get; set; }
}