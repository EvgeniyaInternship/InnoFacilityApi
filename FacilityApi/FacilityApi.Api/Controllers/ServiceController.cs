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
public sealed class ServiceController(IServiceService serviceService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServiceById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var service = await serviceService.GetServiceByIdAsync(id, cancellationToken);
        return Ok(service);
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServices(
        [FromQuery] ServiceFilterDto? filter,
        [FromQuery] PaginationParamsDto paginationParams,
        CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServicesAsync(paginationParams, filter, cancellationToken);
        return Ok(services);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto, CancellationToken cancellationToken)
    {
        var service = await serviceService.CreateServiceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateService([FromBody] UpdateServiceDto dto, CancellationToken cancellationToken)
    {
        await serviceService.UpdateServiceAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveService([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await serviceService.RemoveServiceAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRange([FromBody] List<CreateServiceDto> dtos, CancellationToken cancellationToken)
    {
        var services = await serviceService.CreateRangeAsync(dtos, cancellationToken);
        return Ok(services);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPut("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRange([FromBody] List<UpdateServiceDto> dtos, CancellationToken cancellationToken)
    {
        await serviceService.UpdateRangeAsync(dtos, cancellationToken);
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
        await serviceService.RemoveRangeAsync(ids, cancellationToken);
        return NoContent();
    }
}
