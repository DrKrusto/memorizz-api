using Microsoft.AspNetCore.Identity;

namespace Memorizz.Host.Domain;

public class RequestContext 
{
    public IdentityUser? Requester { get; set; }
}