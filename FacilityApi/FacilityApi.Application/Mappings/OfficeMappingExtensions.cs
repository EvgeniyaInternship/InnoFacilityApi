using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Domain.Entities;

namespace FacilityApi.Application.Mappings;

public static class OfficeMappingExtensions
{
    public static OfficeDto ToDto(this OfficeEntity entity)
    {
        return new OfficeDto(
            entity.Id,
            entity.Title
        );
    }

    public static OfficeEntity ToEntity(this CreateOfficeDto dto, Guid id)
    {
        return new OfficeEntity
        {
            Id = id,
            Title = dto.Title
        };
    }

    public static OfficeEntity UpdateEntity(this UpdateOfficeDto dto)
    {
        return new OfficeEntity
        {
            Id = dto.Id,
            Title = dto.Title
        };
    }
}
