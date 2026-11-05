namespace FacilityApi.Api.Options;

public sealed class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string? BaseUrl { get; set; }
    public string? Realm { get; set; }
    public string? Audience { get; set; }
    public bool RequireHttpsMetadata { get; set; }
}
