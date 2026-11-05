using FacilityApi.Api.Middlewares;
using FacilityApi.Api.Options;
using FacilityApi.Application;
using FacilityApi.Infrastructure;
using Microsoft.OpenApi;

namespace FacilityApi.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplicationServices();

        return services;
    }

    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = KeycloakAuthority(configuration);
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Facility API", Version = "v1" });
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri($"{authority}/.well-known/openid-configuration")
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Keycloak", document)] = ["openid"]
            });
        });

        return services;
    }

    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    private static string KeycloakAuthority(IConfiguration configuration)
    {
        var options = configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>() ?? new KeycloakOptions();
        if (string.IsNullOrWhiteSpace(options.BaseUrl) || string.IsNullOrWhiteSpace(options.Realm))
            throw new InvalidOperationException("Keycloak authentication settings are incomplete.");

        return $"{options.BaseUrl.TrimEnd('/')}/realms/{options.Realm}";
    }
}
