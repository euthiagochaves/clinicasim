using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/cases/{caseId:guid}")]
public class CaseMappingsController(ICaseMappingService caseMappingService) : ControllerBase
{
    [HttpGet("answers")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CaseQuestionAnswerItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CaseQuestionAnswerItemDto>>> GetCaseAnswers(Guid caseId, CancellationToken cancellationToken)
    {
        var items = await caseMappingService.GetCaseAnswersAsync(caseId, cancellationToken);
        return Ok(items.Select(x => new CaseQuestionAnswerItemDto(x.Id, x.QuestionId, x.QuestionText, x.AnswerText)).ToList());
    }

    [HttpGet("findings")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CasePhysicalFindingItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CasePhysicalFindingItemDto>>> GetCaseFindings(Guid caseId, CancellationToken cancellationToken)
    {
        var items = await caseMappingService.GetCaseFindingsAsync(caseId, cancellationToken);
        return Ok(items.Select(x => new CasePhysicalFindingItemDto(x.Id, x.FindingId, x.FindingName, x.System, x.Present, x.DetailText)).ToList());
    }

    [HttpGet("resolve-question/{questionId:guid}")]
    [ProducesResponseType(typeof(ResolvedQuestionAnswerDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResolvedQuestionAnswerDto>> ResolveQuestion(Guid caseId, Guid questionId, CancellationToken cancellationToken)
    {
        var resolved = await caseMappingService.ResolveQuestionAnswerAsync(caseId, questionId, cancellationToken);
        return Ok(new ResolvedQuestionAnswerDto(resolved.CaseId, resolved.QuestionId, resolved.AnswerText));
    }

    [HttpGet("resolve-finding/{findingId:guid}")]
    [ProducesResponseType(typeof(ResolvedFindingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResolvedFindingDto>> ResolveFinding(Guid caseId, Guid findingId, CancellationToken cancellationToken)
    {
        var resolved = await caseMappingService.ResolveFindingAsync(caseId, findingId, cancellationToken);
        return Ok(new ResolvedFindingDto(resolved.CaseId, resolved.FindingId, resolved.Present, resolved.DetailText));
    }
}
