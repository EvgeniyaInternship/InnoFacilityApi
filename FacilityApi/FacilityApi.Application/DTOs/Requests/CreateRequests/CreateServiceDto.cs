namespace FacilityApi.Application.DTOs.Requests.CreateRequests;

public sealed record CreateServiceDto(
    string Title,
    string Description,
    decimal Price,
    bool IsActive,
    Guid DoctorId
);
