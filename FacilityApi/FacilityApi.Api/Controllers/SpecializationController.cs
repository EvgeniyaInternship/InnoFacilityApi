using FacilityApi.Api.Authorization;
using FacilityApi.Application.DTOs.Common;
using FacilityApi.Application.DTOs.Filters;
using FacilityApi.Application.DTOs.Requests.CreateRequests;
using FacilityApi.Application.DTOs.Requests.UpdateRequests;
using FacilityApi.Application.DTOs.Responses;
using FacilityApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilityApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class SpecializationController(ISpecializationService specializationService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SpecializationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecializationById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var specialization = await specializationService.GetSpecializationByIdAsync(id, cancellationToken);
        return Ok(specialization);
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<SpecializationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecializations(
        [FromQuery] SpecializationFilterDto? filter,
        [FromQuery] PaginationParamsDto paginationParams,
        CancellationToken cancellationToken)
    {
        var specializations = await specializationService.GetSpecializationsAsync(paginationParams, filter, cancellationToken);
        return Ok(specializations);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(SpecializationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSpecialization([FromBody] CreateSpecializationDto dto, CancellationToken cancellationToken)
    {
        var specialization = await specializationService.CreateSpecializationAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetSpecializationById), new { id = specialization.Id }, specialization);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSpecialization([FromBody] UpdateSpecializationDto dto, CancellationToken cancellationToken)
    {
        await specializationService.UpdateSpecializationAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveSpecialization([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await specializationService.RemoveSpecializationAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<SpecializationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRange([FromBody] List<CreateSpecializationDto> dtos, CancellationToken cancellationToken)
    {
        var specializations = await specializationService.CreateRangeAsync(dtos, cancellationToken);
        return Ok(specializations);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRange([FromBody] List<UpdateSpecializationDto> dtos, CancellationToken cancellationToken)
    {
        await specializationService.UpdateRangeAsync(dtos, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpDelete("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveRange([FromBody] List<Guid> ids, CancellationToken cancellationToken)
    {
        await specializationService.RemoveRangeAsync(ids, cancellationToken);
        return NoContent();
    }
}
