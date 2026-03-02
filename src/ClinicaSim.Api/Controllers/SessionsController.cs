using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController(ISessionsService sessionsService, IPdfReportService pdfReportService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<StartSessionResponse>> Start([FromBody] StartSessionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionsService.StartAsync(request.CaseId, cancellationToken);
            return Ok(MapStart(result));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{sessionCode}/events")]
    public async Task<ActionResult<SessionEventResponse>> RegisterEvent(string sessionCode, [FromBody] RegisterEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionsService.RegisterEventAsync(sessionCode, request.QuestionId, cancellationToken);
            return Ok(new SessionEventResponse(result.EventId, result.OccurredAt, result.SectionName, result.CategoryName, result.QuestionText, result.AnswerText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{sessionCode}")]
    public async Task<ActionResult<SessionInfoResponse>> GetSession(string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionsService.GetSessionAsync(sessionCode, cancellationToken);
            var caseDto = new CaseListItemDto(result.Case.CaseId, result.Case.FullName, result.Case.Age, result.Case.Sex, result.Case.ChiefComplaint, result.Case.Triage);
            return Ok(new SessionInfoResponse(result.SessionCode, result.Status, result.StartedAt, result.FinishedAt, caseDto));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{sessionCode}/note")]
    public async Task<ActionResult<ClinicalNoteResponse>> GetNote(string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            var note = await sessionsService.GetNoteAsync(sessionCode, cancellationToken);
            return Ok(new ClinicalNoteResponse(note.SummaryText, note.ProbableDiagnosisText, note.ConductStudiesText, note.ConductTreatmentText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{sessionCode}/note")]
    public async Task<ActionResult<ClinicalNoteResponse>> UpsertNote(string sessionCode, [FromBody] ClinicalNoteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var note = await sessionsService.UpsertNoteAsync(sessionCode,
                new ClinicalNoteModel(request.SummaryText, request.ProbableDiagnosisText, request.ConductStudiesText, request.ConductTreatmentText),
                cancellationToken);

            return Ok(new ClinicalNoteResponse(note.SummaryText, note.ProbableDiagnosisText, note.ConductStudiesText, note.ConductTreatmentText));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpGet("{sessionCode}/differentials")]
    public async Task<ActionResult<IReadOnlyCollection<DifferentialItemResponse>>> GetDifferentials(string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            var items = await sessionsService.GetDifferentialsAsync(sessionCode, cancellationToken);
            return Ok(items.Select(x => new DifferentialItemResponse(x.Rank, x.Text)).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{sessionCode}/differentials")]
    public async Task<ActionResult<IReadOnlyCollection<DifferentialItemResponse>>> SaveDifferentials(string sessionCode, [FromBody] SaveDifferentialsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var items = request.Items.Select(x => new DifferentialModel(x.Rank, x.Text)).ToList();
            var saved = await sessionsService.SaveDifferentialsAsync(sessionCode, items, cancellationToken);
            return Ok(saved.Select(x => new DifferentialItemResponse(x.Rank, x.Text)).ToList());
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    [HttpPost("{sessionCode}/finalize")]
    public async Task<ActionResult<FinalizeResponse>> FinalizeSession(string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionsService.FinalizeAsync(sessionCode, cancellationToken);
            return Ok(new FinalizeResponse(result.SessionCode, result.Status, result.FinishedAt));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }


    [HttpGet("{sessionCode}/pdf")]
    public async Task<IActionResult> GetPdf(string sessionCode, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await pdfReportService.GenerateSessionPdfAsync(sessionCode, cancellationToken);
            return File(bytes, "application/pdf", $"ClinicaSim_{sessionCode}.pdf");
        }
        catch (AppException ex) when (ex.StatusCode == 404)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (AppException ex) when (ex.StatusCode == 409)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new ErrorResponse(ex.Errors));
        }
    }

    private static StartSessionResponse MapStart(SessionStartResult x)
    {
        var caseDto = new CaseListItemDto(x.Case.CaseId, x.Case.FullName, x.Case.Age, x.Case.Sex, x.Case.ChiefComplaint, x.Case.Triage);

        var sections = x.Sections.Select(s => new SessionSectionDto(
            s.SectionId,
            s.Name,
            s.Categories.Select(c => new SessionCategoryDto(
                c.CategoryId,
                c.Name,
                c.Questions.Select(q => new SessionQuestionDto(q.QuestionId, q.Text)).ToList())).ToList())).ToList();

        return new StartSessionResponse(x.SessionCode, x.Status, x.StartedAt, caseDto, sections);
    }
}
