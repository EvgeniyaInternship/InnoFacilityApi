using FacilityApi.Api.Options;

namespace FacilityApi.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            var keycloak = app.Configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Facility API v1");
                options.ConfigObject.AdditionalItems["withCredentials"] = true;
                options.OAuthClientId(keycloak?.Audience);
                options.OAuthUsePkce();
                options.OAuthScopes("openid");
            });
        }

        return app;
    }

    public static WebApplication UseApiMiddlewares(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
