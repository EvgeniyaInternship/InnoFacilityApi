namespace FacilityApi.Application.DTOs.Common;

public sealed record PagedResultDto<T>(
    List<T> Items,
    int TotalCount,
    int TotalPages,
    int PageNumber,
    int PageSize);
