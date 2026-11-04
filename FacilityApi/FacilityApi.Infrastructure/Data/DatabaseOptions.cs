namespace FacilityApi.Infrastructure.Data;

public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string FacilityDb { get; set; } = string.Empty;
}
