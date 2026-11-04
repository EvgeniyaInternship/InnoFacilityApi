using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Service;

public sealed class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
{
    public UpdateServiceDtoValidator()
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
