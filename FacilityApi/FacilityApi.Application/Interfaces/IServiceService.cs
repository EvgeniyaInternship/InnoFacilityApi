using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;

namespace FacilityApi.Application.Interfaces;

public interface IServiceService
{
    Task<ServiceDto> GetServiceByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResultDto<ServiceDto>> GetServicesAsync(
        PaginationParamsDto paginationParams,
        ServiceFilterDto? filter = null,
        CancellationToken ct = default);
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto, CancellationToken ct = default);
    Task UpdateServiceAsync(UpdateServiceDto dto, CancellationToken ct = default);
    Task RemoveServiceAsync(Guid id, CancellationToken ct = default);
    Task<List<ServiceDto>> CreateRangeAsync(List<CreateServiceDto> dtos, CancellationToken ct = default);
    Task UpdateRangeAsync(List<UpdateServiceDto> dtos, CancellationToken ct = default);
    Task RemoveRangeAsync(List<Guid> ids, CancellationToken ct = default);
}
