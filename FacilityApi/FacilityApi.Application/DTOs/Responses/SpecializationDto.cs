namespace FacilityApi.Application.DTOs.Responses;

public sealed record SpecializationDto(
    Guid Id,
    string Title,
    string Description
);
