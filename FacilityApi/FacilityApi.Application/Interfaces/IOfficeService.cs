using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;

namespace FacilityApi.Application.Interfaces;

public interface IOfficeService
{
    Task<OfficeDto> GetOfficeByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResultDto<OfficeDto>> GetOfficesAsync(
        PaginationParamsDto paginationParams,
        OfficeFilterDto? filter = null,
        CancellationToken ct = default);
    Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto dto, CancellationToken ct = default);
    Task UpdateOfficeAsync(UpdateOfficeDto dto, CancellationToken ct = default);
    Task RemoveOfficeAsync(Guid id, CancellationToken ct = default);
    Task<List<OfficeDto>> CreateRangeAsync(List<CreateOfficeDto> dtos, CancellationToken ct = default);
    Task UpdateRangeAsync(List<UpdateOfficeDto> dtos, CancellationToken ct = default);
    Task RemoveRangeAsync(List<Guid> ids, CancellationToken ct = default);
}
