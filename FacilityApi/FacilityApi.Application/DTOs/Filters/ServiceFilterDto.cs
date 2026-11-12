namespace FacilityApi.Application.DTOs.Filters;

public sealed record ServiceFilterDto(
    string? Title = null,
    bool? IsActive = null,
    Guid? DoctorId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
);
