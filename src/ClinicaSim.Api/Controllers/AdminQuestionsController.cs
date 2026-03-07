using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/admin/questions")]
public class AdminQuestionsController(IAdminQuestionService adminQuestionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<QuestionAdminListItemDto>>> Get(
        [FromQuery] bool? active,
        [FromQuery] string? section,
        [FromQuery] string? category,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await adminQuestionService.GetQuestionsAsync(new AdminQuestionFilters(active, section, category, search), cancellationToken);
            return Ok(items.Select(Map).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuestionAdminListItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminQuestionService.GetByIdAsync(id, cancellationToken);
            return Ok(Map(item));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuestionAdminListItemDto>> Create([FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await adminQuestionService.CreateAsync(new CreateQuestionCommand(request.Text, request.Section, request.Category, request.Tags), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<QuestionAdminListItemDto>> Update(Guid id, [FromBody] UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminQuestionService.UpdateAsync(id, new UpdateQuestionCommand(request.Text, request.Section, request.Category, request.Tags, request.Active), cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<ActionResult<QuestionAdminListItemDto>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminQuestionService.SetActiveAsync(id, false, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<ActionResult<QuestionAdminListItemDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminQuestionService.SetActiveAsync(id, true, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    private static QuestionAdminListItemDto Map(AdminQuestionItem x)
        => new(x.Id, x.Text, x.Section, x.Category, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt);
}
