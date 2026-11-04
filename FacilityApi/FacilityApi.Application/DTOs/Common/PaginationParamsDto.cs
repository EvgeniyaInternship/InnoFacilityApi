namespace FacilityApi.Application.DTOs.Common;

public sealed record PaginationParamsDto(
    int PageNumber = 0,
    int PageSize = 10
);
