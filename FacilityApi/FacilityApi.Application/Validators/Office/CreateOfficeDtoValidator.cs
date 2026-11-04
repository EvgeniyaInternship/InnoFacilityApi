using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FluentValidation;

namespace FacilityApi.Application.Validators.Office;

public sealed class CreateOfficeDtoValidator : AbstractValidator<CreateOfficeDto>
{
    public CreateOfficeDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();
    }
}
