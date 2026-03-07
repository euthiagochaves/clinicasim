using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/admin/findings")]
public class AdminFindingsController(IAdminFindingService adminFindingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FindingAdminListItemDto>>> Get(
        [FromQuery] bool? active,
        [FromQuery] string? system,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await adminFindingService.GetFindingsAsync(new AdminFindingFilters(active, system, search), cancellationToken);
            return Ok(items.Select(Map).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FindingAdminListItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminFindingService.GetByIdAsync(id, cancellationToken);
            return Ok(Map(item));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost]
    public async Task<ActionResult<FindingAdminListItemDto>> Create([FromBody] CreateFindingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await adminFindingService.CreateAsync(new CreateFindingCommand(request.Name, request.System, request.Tags), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FindingAdminListItemDto>> Update(Guid id, [FromBody] UpdateFindingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminFindingService.UpdateAsync(id, new UpdateFindingCommand(request.Name, request.System, request.Tags, request.Active), cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<ActionResult<FindingAdminListItemDto>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminFindingService.SetActiveAsync(id, false, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<ActionResult<FindingAdminListItemDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminFindingService.SetActiveAsync(id, true, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    private static FindingAdminListItemDto Map(AdminFindingItem x)
        => new(x.Id, x.Name, x.System, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt);
}
