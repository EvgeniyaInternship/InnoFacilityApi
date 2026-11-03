namespace FacilityApi.Domain.Entities;

public sealed class ServiceEntity
{
    public Guid Id { get; init; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
