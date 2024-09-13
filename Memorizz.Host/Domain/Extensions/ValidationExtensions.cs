using FluentValidation;

namespace Memorizz.Host.Domain.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidateGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().Must(x => Guid.TryParse(x, out _)).WithMessage("Must be a valid ID.");
    }
}