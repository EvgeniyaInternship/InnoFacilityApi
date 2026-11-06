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
public sealed class OfficeController(IOfficeService officeService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OfficeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOfficeById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var office = await officeService.GetOfficeByIdAsync(id, cancellationToken);
        return Ok(office);
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<OfficeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOffices(
        [FromQuery] OfficeFilterDto? filter,
        [FromQuery] PaginationParamsDto paginationParams,
        CancellationToken cancellationToken)
    {
        var offices = await officeService.GetOfficesAsync(paginationParams, filter, cancellationToken);
        return Ok(offices);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(OfficeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOffice([FromBody] CreateOfficeDto dto, CancellationToken cancellationToken)
    {
        var office = await officeService.CreateOfficeAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetOfficeById), new { id = office.Id }, office);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOffice([FromBody] UpdateOfficeDto dto, CancellationToken cancellationToken)
    {
        await officeService.UpdateOfficeAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveOffice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await officeService.RemoveOfficeAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<OfficeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRange([FromBody] List<CreateOfficeDto> dtos, CancellationToken cancellationToken)
    {
        var offices = await officeService.CreateRangeAsync(dtos, cancellationToken);
        return Ok(offices);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRange([FromBody] List<UpdateOfficeDto> dtos, CancellationToken cancellationToken)
    {
        await officeService.UpdateRangeAsync(dtos, cancellationToken);
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
        await officeService.RemoveRangeAsync(ids, cancellationToken);
        return NoContent();
    }
}
