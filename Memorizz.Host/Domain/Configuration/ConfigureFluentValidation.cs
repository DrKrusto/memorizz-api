using FluentValidation;
using MediatR;
using Memorizz.Host.Domain.Behaviors;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureFluentValidation
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
}