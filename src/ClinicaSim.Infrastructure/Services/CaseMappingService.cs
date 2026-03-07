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
        return await dbContext.CaseQuestionAnswers
            .AsNoTracking()
            .Where(x => x.CaseId == caseId)
            .OrderBy(x => x.Question.Text)
            .Select(x => new CaseQuestionAnswerItem(x.Id, x.QuestionId, x.Question.Text, x.AnswerText))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CasePhysicalFindingItem>> GetCaseFindingsAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CasePhysicalFindings
            .AsNoTracking()
            .Where(x => x.CaseId == caseId)
            .OrderBy(x => x.Finding.System)
            .ThenBy(x => x.Finding.Name)
            .Select(x => new CasePhysicalFindingItem(x.Id, x.FindingId, x.Finding.Name, x.Finding.System, x.Present, x.DetailText))
            .ToListAsync(cancellationToken);
    }

    public async Task<ResolvedQuestionAnswer> ResolveQuestionAnswerAsync(Guid caseId, Guid questionId, CancellationToken cancellationToken = default)
    {
        var mapped = await dbContext.CaseQuestionAnswers
            .AsNoTracking()
            .Where(x => x.CaseId == caseId && x.QuestionId == questionId)
            .Select(x => x.AnswerText)
            .FirstOrDefaultAsync(cancellationToken);

        return new ResolvedQuestionAnswer(caseId, questionId, mapped ?? DefaultMissingAnswer);
    }

    public async Task<ResolvedFinding> ResolveFindingAsync(Guid caseId, Guid findingId, CancellationToken cancellationToken = default)
    {
        var mapped = await dbContext.CasePhysicalFindings
            .AsNoTracking()
            .Where(x => x.CaseId == caseId && x.FindingId == findingId)
            .Select(x => new { x.Present, x.DetailText })
            .FirstOrDefaultAsync(cancellationToken);

        return mapped is null
            ? new ResolvedFinding(caseId, findingId, false, null)
            : new ResolvedFinding(caseId, findingId, mapped.Present, mapped.DetailText);
    }
}
