using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Application.Helpers;
using FacilityApi.Application.Interfaces;
using FacilityApi.Application.Mappings;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using System.Linq.Expressions;

namespace FacilityApi.Application.Services;

public sealed class OfficeService(IOfficeRepository officeRepository, IDbTransactionScope transactionScope) : IOfficeService
{
    private static Expression<Func<OfficeEntity, bool>> BuildFilterExpression(OfficeFilterDto? filter)
    {
        if (filter is null)
            return x => true;

        return x => filter.Title == null || x.Title == filter.Title;
    }

    public async Task<OfficeDto> GetOfficeByIdAsync(Guid id, CancellationToken ct = default)
    {
        var office = await officeRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
        if (office is null)
            throw new KeyNotFoundException($"Office with ID {id} was not found.");

        return office.ToDto();
    }

    public async Task<PagedResultDto<OfficeDto>> GetOfficesAsync(
        PaginationParamsDto paginationParams,
        OfficeFilterDto? filter = null,
        CancellationToken ct = default)
    {
        var filterExpression = BuildFilterExpression(filter);
        var offices = await officeRepository.GetAllAsync(
            filterExpression,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken: ct);
        var totalCount = await officeRepository.CountAsync(filterExpression, cancellationToken: ct);
        var totalPages = PaginationHelper.CalculateTotalPages(totalCount, paginationParams.PageSize);
        var dtos = offices.Select(office => office.ToDto()).ToList();

        return new PagedResultDto<OfficeDto>(dtos, totalCount, totalPages, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto dto, CancellationToken ct = default)
    {
        var office = dto.ToEntity(Guid.NewGuid());
        await officeRepository.AddAsync(office, ct);
        return office.ToDto();
    }

    public async Task UpdateOfficeAsync(UpdateOfficeDto dto, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var office = await officeRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: ct);
            if (office is null)
                throw new KeyNotFoundException($"Office with ID {dto.Id} was not found.");

            await officeRepository.UpdateAsync(dto.UpdateEntity(), ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task RemoveOfficeAsync(Guid id, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var office = await officeRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
            if (office is null)
                throw new KeyNotFoundException($"Office with ID {id} was not found.");

            await officeRepository.RemoveAsync(office, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<List<OfficeDto>> CreateRangeAsync(List<CreateOfficeDto> dtos, CancellationToken ct = default)
    {
        if (!dtos.Any())
            return [];

        var entities = dtos.Select(dto => dto.ToEntity(Guid.NewGuid())).ToList();
        await officeRepository.AddRangeAsync(entities, ct);
        return entities.Select(office => office.ToDto()).ToList();
    }

    public async Task UpdateRangeAsync(List<UpdateOfficeDto> dtos, CancellationToken ct = default)
    {
        var uniqueDtos = dtos.DistinctBy(dto => dto.Id).ToList();
        if (uniqueDtos.Count == 0)
            return;

        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            await GetExistingAsync(uniqueDtos.Select(dto => dto.Id), ct);
            await officeRepository.UpdateRangeAsync(uniqueDtos.Select(dto => dto.UpdateEntity()), ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task RemoveRangeAsync(List<Guid> ids, CancellationToken ct = default)
    {
        var distinctIds = ids.Distinct().ToList();
        if (distinctIds.Count == 0)
            return;

        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var existingOffices = await GetExistingAsync(distinctIds, ct);
            await officeRepository.RemoveRangeAsync(existingOffices, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<List<OfficeEntity>> GetExistingAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var idSet = ids.ToHashSet();
        var offices = await officeRepository.GetAllAsync(x => true, cancellationToken: ct);
        var existing = offices.Where(office => idSet.Contains(office.Id)).ToList();
        if (existing.Count != idSet.Count)
            throw new KeyNotFoundException("One or more offices were not found.");

        return existing;
    }
}
