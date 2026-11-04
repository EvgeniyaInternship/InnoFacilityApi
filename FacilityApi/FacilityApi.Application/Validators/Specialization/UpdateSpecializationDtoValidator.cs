using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Specialization;

public sealed class UpdateSpecializationDtoValidator : AbstractValidator<UpdateSpecializationDto>
{
    public UpdateSpecializationDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty();
        RuleFor(x => x.Description)
            .NotEmpty();
    }
}
