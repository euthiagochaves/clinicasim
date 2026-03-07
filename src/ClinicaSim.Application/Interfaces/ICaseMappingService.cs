using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface ICaseMappingService
{
    Task<IReadOnlyCollection<CaseQuestionAnswerItem>> GetCaseAnswersAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CasePhysicalFindingItem>> GetCaseFindingsAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<ResolvedQuestionAnswer> ResolveQuestionAnswerAsync(Guid caseId, Guid questionId, CancellationToken cancellationToken = default);
    Task<ResolvedFinding> ResolveFindingAsync(Guid caseId, Guid findingId, CancellationToken cancellationToken = default);
}
