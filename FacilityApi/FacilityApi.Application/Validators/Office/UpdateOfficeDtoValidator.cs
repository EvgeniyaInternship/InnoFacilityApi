using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Office;

public sealed class UpdateOfficeDtoValidator : AbstractValidator<UpdateOfficeDto>
{
    public UpdateOfficeDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty();
    }
}
