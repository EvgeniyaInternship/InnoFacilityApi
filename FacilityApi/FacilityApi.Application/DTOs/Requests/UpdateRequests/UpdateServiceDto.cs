namespace FacilityApi.Application.DTOs.Requests.UpdateRequests;

public sealed record UpdateServiceDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    bool IsActive,
    Guid DoctorId
);
