using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Domain.Entities;

namespace FacilityApi.Application.Mappings;

public static class SpecializationMappingExtensions
{
    public static SpecializationDto ToDto(this SpecializationEntity entity)
    {
        return new SpecializationDto(
            entity.Id,
            entity.Title,
            entity.Description
        );
    }

    public static SpecializationEntity ToEntity(this CreateSpecializationDto dto, Guid id)
    {
        return new SpecializationEntity
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description
        };
    }

    public static SpecializationEntity UpdateEntity(this UpdateSpecializationDto dto)
    {
        return new SpecializationEntity
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description
        };
    }
}
