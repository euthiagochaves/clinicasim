using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class CaseMappingService(ClinicaSimDbContext dbContext) : ICaseMappingService
{
    private const string DefaultMissingAnswer = "No tengo ese dato.";

    public async Task<IReadOnlyCollection<CaseQuestionAnswerItem>> GetCaseAnswersAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.QuestionBanks
            .AsNoTracking()
            .Where(x => x.Active)
            .OrderBy(x => x.Text)
            .Select(x => new
            {
                x.Id,
                x.Text,
                Override = x.CaseOverrides.Where(o => o.CaseId == caseId).Select(o => new { o.Id, o.AnswerText, o.IsCaseSpecific, o.IsHighlighted }).FirstOrDefault(),
                Default = x.DefaultAnswers.Where(d => d.Active).Select(d => d.AnswerText).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return items
            .Select(x => new CaseQuestionAnswerItem(
                x.Override?.Id,
                x.Id,
                x.Text,
                x.Override?.AnswerText ?? x.Default ?? DefaultMissingAnswer,
                x.Override is null,
                x.Override?.IsCaseSpecific ?? false,
                x.Override?.IsHighlighted ?? false))
            .ToList();
    }

    public async Task<IReadOnlyCollection<CasePhysicalFindingItem>> GetCaseFindingsAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .Where(x => x.Active)
            .OrderBy(x => x.System)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.System,
                Override = x.CaseOverrides.Where(o => o.CaseId == caseId)
                    .Select(o => new { o.Id, o.Present, o.DetailText, o.IsCaseSpecific, o.IsHighlighted })
                    .FirstOrDefault(),
                Default = x.Defaults.Where(d => d.Active).Select(d => new { d.Present, d.DetailText }).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return items
            .Select(x => new CasePhysicalFindingItem(
                x.Override?.Id,
                x.Id,
                x.Name,
                x.System,
                x.Override?.Present ?? x.Default?.Present ?? false,
                x.Override?.DetailText ?? x.Default?.DetailText,
                x.Override is null,
                x.Override?.IsCaseSpecific ?? false,
                x.Override?.IsHighlighted ?? false))
            .ToList();
    }

    public async Task<ResolvedQuestionAnswer> ResolveQuestionAnswerAsync(Guid caseId, Guid questionId, CancellationToken cancellationToken = default)
    {
        var overrideAnswer = await dbContext.CaseQuestionOverrides
            .AsNoTracking()
            .Where(x => x.CaseId == caseId && x.QuestionId == questionId)
            .Select(x => x.AnswerText)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(overrideAnswer))
        {
            return new ResolvedQuestionAnswer(caseId, questionId, overrideAnswer);
        }

        var defaultAnswer = await dbContext.QuestionDefaultAnswers
            .AsNoTracking()
            .Where(x => x.QuestionId == questionId && x.Active)
            .Select(x => x.AnswerText)
            .FirstOrDefaultAsync(cancellationToken);

        return new ResolvedQuestionAnswer(caseId, questionId, defaultAnswer ?? DefaultMissingAnswer);
    }

    public async Task<ResolvedFinding> ResolveFindingAsync(Guid caseId, Guid findingId, CancellationToken cancellationToken = default)
    {
        var overrideFinding = await dbContext.CasePhysicalFindingOverrides
            .AsNoTracking()
            .Where(x => x.CaseId == caseId && x.FindingId == findingId)
            .Select(x => new { x.Present, x.DetailText })
            .FirstOrDefaultAsync(cancellationToken);

        if (overrideFinding is not null)
        {
            return new ResolvedFinding(caseId, findingId, overrideFinding.Present, overrideFinding.DetailText);
        }

        var defaultFinding = await dbContext.PhysicalFindingDefaults
            .AsNoTracking()
            .Where(x => x.FindingId == findingId && x.Active)
            .Select(x => new { x.Present, x.DetailText })
            .FirstOrDefaultAsync(cancellationToken);

        return defaultFinding is null
            ? new ResolvedFinding(caseId, findingId, false, null)
            : new ResolvedFinding(caseId, findingId, defaultFinding.Present, defaultFinding.DetailText);
    }
}
