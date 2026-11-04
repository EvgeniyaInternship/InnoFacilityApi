using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Domain.Entities;

namespace FacilityApi.Application.Mappings;

public static class ServiceMappingExtensions
{
    public static ServiceDto ToDto(this ServiceEntity entity)
    {
        return new ServiceDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Price,
            entity.IsActive,
            entity.DoctorId
        );
    }

    public static ServiceEntity ToEntity(this CreateServiceDto dto, Guid id)
    {
        return new ServiceEntity
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            IsActive = dto.IsActive,
            DoctorId = dto.DoctorId
        };
    }

    public static ServiceEntity UpdateEntity(this UpdateServiceDto dto)
    {
        return new ServiceEntity
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            IsActive = dto.IsActive,
            DoctorId = dto.DoctorId
        };
    }
}
