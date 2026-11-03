namespace FacilityApi.Domain.Entities;

public sealed class SpecializationEntity
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
