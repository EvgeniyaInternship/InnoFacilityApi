using FacilityApi.Application.DTOs.Responses;
using FluentValidation;

namespace FacilityApi.Application.Validators.Service;

public sealed class ServiceDtoValidator : AbstractValidator<ServiceDto>
{
    public ServiceDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty();
        RuleFor(x => x.Description)
            .NotEmpty();
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}
