using FacilityApi.Application.Interfaces;
using FacilityApi.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace FacilityApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddFluentValidationAutoValidation(configuration =>
        {
            configuration.EnableCustomBindingSourceAutomaticValidation = true;
            configuration.EnableNullBindingSourceAutomaticValidation = true;
        });

        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ISpecializationService, SpecializationService>();

        return services;
    }
}
