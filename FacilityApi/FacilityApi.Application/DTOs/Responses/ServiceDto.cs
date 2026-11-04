namespace FacilityApi.Application.DTOs.Responses;

public sealed record ServiceDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    bool IsActive,
    Guid DoctorId
);
