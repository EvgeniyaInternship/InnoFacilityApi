namespace FacilityApi.Domain.Entities;

public sealed class ServiceEntity
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid DoctorId { get; set; }
}
