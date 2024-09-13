using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Memorizz.Host.Domain.Configuration;

public static class ConfigureSwagger
{
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        => services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Assembly.GetExecutingAssembly());
            options.SwaggerDoc("v1", new() { Title = "Memorizz API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
}