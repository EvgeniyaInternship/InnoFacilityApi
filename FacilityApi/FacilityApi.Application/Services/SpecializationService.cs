using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Application.Helpers;
using FacilityApi.Application.Interfaces;
using FacilityApi.Application.Mappings;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Exceptions;
using FacilityApi.Domain.Interfaces;
using System.Linq.Expressions;

namespace FacilityApi.Application.Services;

public sealed class SpecializationService(ISpecializationRepository specializationRepository, IDbTransactionScope transactionScope) : ISpecializationService
{
    private static Expression<Func<SpecializationEntity, bool>> BuildFilterExpression(SpecializationFilterDto? filter)
    {
        if (filter is null)
            return x => true;

        return x => filter.Title == null || x.Title == filter.Title;
    }

    public async Task<SpecializationDto> GetSpecializationByIdAsync(Guid id, CancellationToken ct = default)
    {
        var specialization = await specializationRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
        if (specialization is null)
            throw new KeyNotFoundException($"Specialization with ID {id} was not found.");

        return specialization.ToDto();
    }

    public async Task<PagedResultDto<SpecializationDto>> GetSpecializationsAsync(
        PaginationParamsDto paginationParams,
        SpecializationFilterDto? filter = null,
        CancellationToken ct = default)
    {
        var filterExpression = BuildFilterExpression(filter);
        var specializations = await specializationRepository.GetAllAsync(
            filterExpression,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken: ct);
        var totalCount = await specializationRepository.CountAsync(filterExpression, cancellationToken: ct);
        var totalPages = PaginationHelper.CalculateTotalPages(totalCount, paginationParams.PageSize);
        var dtos = specializations.Select(specialization => specialization.ToDto()).ToList();

        return new PagedResultDto<SpecializationDto>(dtos, totalCount, totalPages, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationDto dto, CancellationToken ct = default)
    {
        try
        {
            var specialization = dto.ToEntity(Guid.NewGuid());
            await specializationRepository.AddAsync(specialization, ct);
            return specialization.ToDto();
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            throw new ServerException("An error occurred while creating specialization.", ex);
        }
    }

    public async Task UpdateSpecializationAsync(UpdateSpecializationDto dto, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var specialization = await specializationRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: ct);
            if (specialization is null)
                throw new KeyNotFoundException($"Specialization with ID {dto.Id} was not found.");

            await specializationRepository.UpdateAsync(dto.UpdateEntity(), ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex) when (ex is not AppException)
        {
            await transaction.RollbackAsync(ct);
            if (ex is KeyNotFoundException)
                throw;

            throw new ServerException("An error occurred while updating specialization.", ex);
        }
    }

    public async Task RemoveSpecializationAsync(Guid id, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var specialization = await specializationRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
            if (specialization is null)
                throw new KeyNotFoundException($"Specialization with ID {id} was not found.");

            await specializationRepository.RemoveAsync(specialization, ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex) when (ex is not AppException)
        {
            await transaction.RollbackAsync(ct);
            if (ex is KeyNotFoundException)
                throw;

            throw new ServerException("An error occurred while removing specialization.", ex);
        }
    }

    public async Task<List<SpecializationDto>> CreateRangeAsync(List<CreateSpecializationDto> dtos, CancellationToken ct = default)
    {
        if (!dtos.Any())
            return [];

        try
        {
            var entities = dtos.Select(dto => dto.ToEntity(Guid.NewGuid())).ToList();
            await specializationRepository.AddRangeAsync(entities, ct);
            return entities.Select(specialization => specialization.ToDto()).ToList();
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            throw new ServerException("An error occurred while creating specializations.", ex);
        }
    }

    public async Task UpdateRangeAsync(List<UpdateSpecializationDto> dtos, CancellationToken ct = default)
    {
        var uniqueDtos = dtos.DistinctBy(dto => dto.Id).ToList();
        if (uniqueDtos.Count == 0)
            return;

        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            await GetExistingAsync(uniqueDtos.Select(dto => dto.Id), ct);
            await specializationRepository.UpdateRangeAsync(uniqueDtos.Select(dto => dto.UpdateEntity()), ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex) when (ex is not AppException)
        {
            await transaction.RollbackAsync(ct);
            if (ex is KeyNotFoundException)
                throw;

            throw new ServerException("An error occurred while updating specializations.", ex);
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
            var existingSpecializations = await GetExistingAsync(distinctIds, ct);
            await specializationRepository.RemoveRangeAsync(existingSpecializations, ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex) when (ex is not AppException)
        {
            await transaction.RollbackAsync(ct);
            if (ex is KeyNotFoundException)
                throw;

            throw new ServerException("An error occurred while removing specializations.", ex);
        }
    }

    private async Task<List<SpecializationEntity>> GetExistingAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var idSet = ids.ToHashSet();
        var specializations = await specializationRepository.GetAllAsync(x => true, cancellationToken: ct);
        var existing = specializations.Where(specialization => idSet.Contains(specialization.Id)).ToList();
        if (existing.Count != idSet.Count)
            throw new KeyNotFoundException("One or more specializations were not found.");

        return existing;
    }
}
