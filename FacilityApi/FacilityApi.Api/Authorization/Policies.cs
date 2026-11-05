using Microsoft.AspNetCore.Authorization;

namespace FacilityApi.Api.Authorization;

public static class Policies
{
    public const string Admin = "Admin";
    public const string Receptionist = "Receptionist";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";

    public static IReadOnlyList<string> AccountRoles = [Admin, Receptionist, Doctor, Patient];

    public static void AddFacilityPolicies(this AuthorizationOptions options)
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy(Admin, policy => policy.RequireRole(Admin));
    }
}
