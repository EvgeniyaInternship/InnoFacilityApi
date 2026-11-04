namespace FacilityApi.Application.DTOs.Requests.UpdateRequests;

public sealed record UpdateSpecializationDto(
    Guid Id,
    string Title,
    string Description
);
