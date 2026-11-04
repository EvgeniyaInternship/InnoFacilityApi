using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Specialization;

public sealed class CreateSpecializationDtoValidator : AbstractValidator<CreateSpecializationDto>
{
    public CreateSpecializationDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();
        RuleFor(x => x.Description)
            .NotEmpty();
    }
}
