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
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FacilityApi.Application.Services;

public sealed class ServiceService(
    IServiceRepository serviceRepository,
    IDbTransactionScope transactionScope,
    ILogger<ServiceService> logger) : IServiceService
{
    private static Expression<Func<ServiceEntity, bool>> BuildFilterExpression(ServiceFilterDto? filter)
    {
        if (filter is null)
            return x => true;

        if (string.IsNullOrWhiteSpace(filter.Title))
        {
            return x => (filter.IsActive == null || x.IsActive == filter.IsActive)
                     && (filter.DoctorId == null || x.DoctorId == filter.DoctorId)
                     && (filter.MinPrice == null || x.Price >= filter.MinPrice)
                     && (filter.MaxPrice == null || x.Price <= filter.MaxPrice);
        }

        var title = filter.Title;
        return x => x.Title.ToLower().Contains(title.ToLower())
                 && (filter.IsActive == null || x.IsActive == filter.IsActive)
                 && (filter.DoctorId == null || x.DoctorId == filter.DoctorId)
                 && (filter.MinPrice == null || x.Price >= filter.MinPrice)
                 && (filter.MaxPrice == null || x.Price <= filter.MaxPrice);
    }

    public async Task<ServiceDto> GetServiceByIdAsync(Guid id, CancellationToken ct = default)
    {
        var service = await serviceRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
        if (service is null)
        {
            logger.LogWarning("Service with id {ServiceId} does not exist.", id);
            throw new KeyNotFoundException($"Service with ID {id} was not found.");
        }

        logger.LogInformation("Loaded service {ServiceId}.", service.Id);
        return service.ToDto();
    }

    public async Task<PagedResultDto<ServiceDto>> GetServicesAsync(
        PaginationParamsDto paginationParams,
        ServiceFilterDto? filter = null,
        CancellationToken ct = default)
    {
        var filterExpression = BuildFilterExpression(filter);
        var services = await serviceRepository.GetAllAsync(
            filterExpression,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken: ct);
        var totalCount = await serviceRepository.CountAsync(filterExpression, cancellationToken: ct);
        var totalPages = PaginationHelper.CalculateTotalPages(totalCount, paginationParams.PageSize);
        var dtos = services.Select(service => service.ToDto()).ToList();

        logger.LogInformation("Loaded {Count} services.", dtos.Count);
        return new PagedResultDto<ServiceDto>(dtos, totalCount, totalPages, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        try
        {
            var service = dto.ToEntity(Guid.NewGuid());
            await serviceRepository.AddAsync(service, ct);
            logger.LogInformation("Created service {ServiceId} with title {Title}.", service.Id, service.Title);
            return service.ToDto();
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Creating service failed.");
            throw new ServerException("An error occurred while creating service.", ex);
        }
    }

    public async Task UpdateServiceAsync(UpdateServiceDto dto, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var service = await serviceRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: ct);
            if (service is null)
            {
                logger.LogWarning("Service with id {ServiceId} does not exist.", dto.Id);
                throw new KeyNotFoundException($"Service with ID {dto.Id} was not found.");
            }

            await serviceRepository.UpdateAsync(dto.UpdateEntity(), ct);
            await transaction.CommitAsync(ct);
            logger.LogInformation("Updated service {ServiceId} with title {Title}.", dto.Id, dto.Title);
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Service operation failed.");
            await transaction.RollbackAsync(ct);
            throw new ServerException("An error occurred while updating service.", ex);
        }
    }

    public async Task RemoveServiceAsync(Guid id, CancellationToken ct = default)
    {
        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            var service = await serviceRepository.GetAsync(x => x.Id == id, cancellationToken: ct);
            if (service is null)
            {
                logger.LogWarning("Service with id {ServiceId} does not exist.", id);
                throw new KeyNotFoundException($"Service with ID {id} was not found.");
            }

            await serviceRepository.RemoveAsync(service, ct);
            await transaction.CommitAsync(ct);
            logger.LogInformation("Deleted service {ServiceId} with title {Title}.", service.Id, service.Title);
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Service operation failed.");
            await transaction.RollbackAsync(ct);
            throw new ServerException("An error occurred while removing service.", ex);
        }
    }

    public async Task<List<ServiceDto>> CreateRangeAsync(List<CreateServiceDto> dtos, CancellationToken ct = default)
    {
        if (!dtos.Any())
            return [];

        try
        {
            var entities = dtos.Select(dto => dto.ToEntity(Guid.NewGuid())).ToList();
            await serviceRepository.AddRangeAsync(entities, ct);
            logger.LogInformation("Created {Count} services.", entities.Count);
            return entities.Select(service => service.ToDto()).ToList();
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Creating services failed.");
            throw new ServerException("An error occurred while creating services.", ex);
        }
    }

    public async Task UpdateRangeAsync(List<UpdateServiceDto> dtos, CancellationToken ct = default)
    {
        var uniqueDtos = dtos.DistinctBy(dto => dto.Id).ToList();
        if (uniqueDtos.Count == 0)
            return;

        await using var transaction = await transactionScope.BeginAsync(ct);
        try
        {
            await GetExistingAsync(uniqueDtos.Select(dto => dto.Id), ct);
            await serviceRepository.UpdateRangeAsync(uniqueDtos.Select(dto => dto.UpdateEntity()), ct);
            await transaction.CommitAsync(ct);
            logger.LogInformation("Updated {Count} services.", uniqueDtos.Count);
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Service operation failed.");
            await transaction.RollbackAsync(ct);
            throw new ServerException("An error occurred while updating services.", ex);
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
            var existingServices = await GetExistingAsync(distinctIds, ct);
            await serviceRepository.RemoveRangeAsync(existingServices, ct);
            await transaction.CommitAsync(ct);
            logger.LogInformation("Deleted {Count} services.", existingServices.Count);
        }
        catch (Exception ex) when (ex is not AppException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Service operation failed.");
            await transaction.RollbackAsync(ct);
            throw new ServerException("An error occurred while removing services.", ex);
        }
    }

    private async Task<List<ServiceEntity>> GetExistingAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var idSet = ids.ToHashSet();
        var services = await serviceRepository.GetAllAsync(x => true, cancellationToken: ct);
        var existing = services.Where(service => idSet.Contains(service.Id)).ToList();
        if (existing.Count != idSet.Count)
        {
            logger.LogWarning("One or more services do not exist.");
            throw new KeyNotFoundException("One or more services were not found.");
        }

        return existing;
    }
}
