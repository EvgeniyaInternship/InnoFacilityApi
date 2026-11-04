using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Service;

public sealed class CreateServiceDtoValidator : AbstractValidator<CreateServiceDto>
{
    public CreateServiceDtoValidator()
    {
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
