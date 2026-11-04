using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;

namespace FacilityApi.Application.Interfaces;

public interface ISpecializationService
{
    Task<SpecializationDto> GetSpecializationByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResultDto<SpecializationDto>> GetSpecializationsAsync(
        PaginationParamsDto paginationParams,
        SpecializationFilterDto? filter = null,
        CancellationToken ct = default);
    Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationDto dto, CancellationToken ct = default);
    Task UpdateSpecializationAsync(UpdateSpecializationDto dto, CancellationToken ct = default);
    Task RemoveSpecializationAsync(Guid id, CancellationToken ct = default);
    Task<List<SpecializationDto>> CreateRangeAsync(List<CreateSpecializationDto> dtos, CancellationToken ct = default);
    Task UpdateRangeAsync(List<UpdateSpecializationDto> dtos, CancellationToken ct = default);
    Task RemoveRangeAsync(List<Guid> ids, CancellationToken ct = default);
}
