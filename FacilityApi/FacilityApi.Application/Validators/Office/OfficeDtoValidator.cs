using FacilityApi.Application.DTOs.Responses;
using FluentValidation;

namespace FacilityApi.Application.Validators.Office;

public sealed class OfficeDtoValidator : AbstractValidator<OfficeDto>
{
    public OfficeDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty();
    }
}
