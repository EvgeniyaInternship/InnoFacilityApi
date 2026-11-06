using FacilityApi.Api.Authorization;
using FacilityApi.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace FacilityApi.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>() ?? new KeycloakOptions();
        var authority = $"{options.BaseUrl.TrimEnd('/')}/realms/{options.Realm}";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = authority;
                jwtOptions.Audience = options.Audience;
                jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
                jwtOptions.MapInboundClaims = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    AudienceValidator = IsExpectedAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "preferred_username",
                    RoleClaimType = "role"
                };
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("accessToken", out var cookieToken)
                            && !string.IsNullOrWhiteSpace(cookieToken))
                        {
                            context.Token = cookieToken;
                        }
                        else if (context.Request.Query.TryGetValue("access_token", out var queryToken)
                            && !string.IsNullOrWhiteSpace(queryToken))
                        {
                            context.Token = queryToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                            MapRoleClaims(identity);

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(authorization => authorization.AddFacilityPolicies());

        return services;
    }

    private static bool IsExpectedAudience(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        var expectedAudience = validationParameters.ValidAudience;
        if (string.IsNullOrWhiteSpace(expectedAudience))
            return false;

        if (audiences?.Contains(expectedAudience, StringComparer.Ordinal) == true)
            return true;

        var authorizedParty = securityToken switch
        {
            JsonWebToken jsonWebToken when jsonWebToken.TryGetPayloadValue("azp", out string? party) => party,
            JwtSecurityToken jwtSecurityToken => jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "azp")?.Value,
            _ => null
        };

        return string.Equals(authorizedParty, expectedAudience, StringComparison.Ordinal);
    }

    private static void MapRoleClaims(ClaimsIdentity identity)
    {
        var realmAccess = identity.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(realmAccess))
            return;

        try
        {
            using var document = JsonDocument.Parse(realmAccess);
            if (!document.RootElement.TryGetProperty("roles", out var realmRoles)
                || realmRoles.ValueKind != JsonValueKind.Array)
                return;

            foreach (var roleElement in realmRoles.EnumerateArray())
            {
                var roleName = roleElement.GetString();
                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                var knownRole = Policies.AccountRoles.FirstOrDefault(role => role.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                if (knownRole is null || identity.HasClaim("role", knownRole))
                    continue;

                identity.AddClaim(new Claim("role", knownRole));
            }
        }
        catch (JsonException)
        {
        }
    }
}
