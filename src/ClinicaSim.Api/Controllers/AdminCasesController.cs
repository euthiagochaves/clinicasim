using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/admin/cases")]
public class AdminCasesController(IAdminCaseService adminCaseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CaseAdminListItemDto>>> Get(
        [FromQuery] bool? active,
        [FromQuery] string? search,
        [FromQuery] string? triage,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await adminCaseService.GetCasesAsync(new AdminCaseFilters(active, search, triage), cancellationToken);
            return Ok(items.Select(Map).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CaseAdminListItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminCaseService.GetByIdAsync(id, cancellationToken);
            return Ok(Map(item));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost]
    public async Task<ActionResult<CaseAdminListItemDto>> Create([FromBody] CreateCaseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await adminCaseService.CreateAsync(new CreateCaseCommand(request.FullName, request.Age, request.Sex, request.ChiefComplaint, request.Triage), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CaseAdminListItemDto>> Update(Guid id, [FromBody] UpdateCaseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminCaseService.UpdateAsync(id, new UpdateCaseCommand(request.FullName, request.Age, request.Sex, request.ChiefComplaint, request.Triage, request.Active), cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<ActionResult<CaseAdminListItemDto>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminCaseService.SetActiveAsync(id, false, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<ActionResult<CaseAdminListItemDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminCaseService.SetActiveAsync(id, true, cancellationToken);
            return Ok(Map(updated));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{caseId:guid}/answers")]
    public async Task<ActionResult<IReadOnlyCollection<CaseQuestionAnswerDto>>> GetAnswers(Guid caseId, CancellationToken cancellationToken)
    {
        try
        {
            var items = await adminCaseService.GetAnswersAsync(caseId, cancellationToken);
            return Ok(items.Select(x => new CaseQuestionAnswerDto(x.Id, x.CaseId, x.QuestionId, x.QuestionText, x.Section, x.Category, x.AnswerText)).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{caseId:guid}/answers")]
    public async Task<ActionResult<CaseQuestionAnswerDto>> AddAnswer(Guid caseId, [FromBody] CreateCaseQuestionAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminCaseService.AddAnswerAsync(caseId, new CreateCaseQuestionAnswerCommand(request.QuestionId, request.AnswerText), cancellationToken);
            return Ok(new CaseQuestionAnswerDto(item.Id, item.CaseId, item.QuestionId, item.QuestionText, item.Section, item.Category, item.AnswerText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPut("{caseId:guid}/answers/{mappingId:guid}")]
    public async Task<ActionResult<CaseQuestionAnswerDto>> UpdateAnswer(Guid caseId, Guid mappingId, [FromBody] UpdateCaseQuestionAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminCaseService.UpdateAnswerAsync(caseId, mappingId, new UpdateCaseQuestionAnswerCommand(request.QuestionId, request.AnswerText), cancellationToken);
            return Ok(new CaseQuestionAnswerDto(item.Id, item.CaseId, item.QuestionId, item.QuestionText, item.Section, item.Category, item.AnswerText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpDelete("{caseId:guid}/answers/{mappingId:guid}")]
    public async Task<ActionResult> DeleteAnswer(Guid caseId, Guid mappingId, CancellationToken cancellationToken)
    {
        try
        {
            await adminCaseService.DeleteAnswerAsync(caseId, mappingId, cancellationToken);
            return NoContent();
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{caseId:guid}/findings")]
    public async Task<ActionResult<IReadOnlyCollection<CasePhysicalFindingDto>>> GetFindings(Guid caseId, CancellationToken cancellationToken)
    {
        try
        {
            var items = await adminCaseService.GetFindingsAsync(caseId, cancellationToken);
            return Ok(items.Select(x => new CasePhysicalFindingDto(x.Id, x.CaseId, x.FindingId, x.FindingName, x.System, x.Present, x.DetailText)).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{caseId:guid}/findings")]
    public async Task<ActionResult<CasePhysicalFindingDto>> AddFinding(Guid caseId, [FromBody] CreateCasePhysicalFindingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminCaseService.AddFindingAsync(caseId, new CreateCasePhysicalFindingCommand(request.FindingId, request.Present, request.DetailText), cancellationToken);
            return Ok(new CasePhysicalFindingDto(item.Id, item.CaseId, item.FindingId, item.FindingName, item.System, item.Present, item.DetailText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPut("{caseId:guid}/findings/{mappingId:guid}")]
    public async Task<ActionResult<CasePhysicalFindingDto>> UpdateFinding(Guid caseId, Guid mappingId, [FromBody] UpdateCasePhysicalFindingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await adminCaseService.UpdateFindingAsync(caseId, mappingId, new UpdateCasePhysicalFindingCommand(request.FindingId, request.Present, request.DetailText), cancellationToken);
            return Ok(new CasePhysicalFindingDto(item.Id, item.CaseId, item.FindingId, item.FindingName, item.System, item.Present, item.DetailText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpDelete("{caseId:guid}/findings/{mappingId:guid}")]
    public async Task<ActionResult> DeleteFinding(Guid caseId, Guid mappingId, CancellationToken cancellationToken)
    {
        try
        {
            await adminCaseService.DeleteFindingAsync(caseId, mappingId, cancellationToken);
            return NoContent();
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    private static CaseAdminListItemDto Map(AdminCaseItem x)
        => new(x.Id, x.FullName, x.Age, x.Sex, x.ChiefComplaint, x.Triage, x.Active, x.CreatedAt, x.UpdatedAt);
}
