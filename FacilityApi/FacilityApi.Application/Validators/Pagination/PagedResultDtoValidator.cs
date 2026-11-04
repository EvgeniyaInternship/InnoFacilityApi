using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.Validators.Constants;
using FluentValidation;

namespace FacilityApi.Application.Validators.Pagination;

public sealed class PagedResultDtoValidator<T> : AbstractValidator<PagedResultDto<T>>
{
    public PagedResultDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page number must be greater than or equal to 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(ValidationConstants.MaxPageSize)
            .WithMessage($"Page size cannot exceed {ValidationConstants.MaxPageSize}.");
    }
}
