using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Data;
using FacilityApi.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.FacilityDb), "ConnectionStrings:FacilityDb is required.")
            .ValidateOnStart();

        services.AddSingleton<NpgsqlConnectionFactory>();
        services.AddScoped<IDbTransactionScope, DatabaseTransactionScope>();
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();

        return services;
    }
}
