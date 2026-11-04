using FacilityApi.Application.DTOs.Responses;
using FluentValidation;

namespace FacilityApi.Application.Validators.Specialization;

public sealed class SpecializationDtoValidator : AbstractValidator<SpecializationDto>
{
    public SpecializationDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty();
        RuleFor(x => x.Description)
            .NotEmpty();
    }
}
