namespace FacilityApi.Domain.Entities;

public sealed class OfficeEntity
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
}
