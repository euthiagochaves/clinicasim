using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface IAdminCaseService
{
    Task<IReadOnlyCollection<AdminCaseItem>> GetCasesAsync(AdminCaseFilters filters, CancellationToken cancellationToken = default);
    Task<AdminCaseItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AdminCaseItem> CreateAsync(CreateCaseCommand command, CancellationToken cancellationToken = default);
    Task<AdminCaseItem> UpdateAsync(Guid id, UpdateCaseCommand command, CancellationToken cancellationToken = default);
    Task<AdminCaseItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CaseQuestionAnswerAdminItem>> GetAnswersAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<CaseQuestionAnswerAdminItem> AddAnswerAsync(Guid caseId, CreateCaseQuestionAnswerCommand command, CancellationToken cancellationToken = default);
    Task<CaseQuestionAnswerAdminItem> UpdateAnswerAsync(Guid caseId, Guid mappingId, UpdateCaseQuestionAnswerCommand command, CancellationToken cancellationToken = default);
    Task DeleteAnswerAsync(Guid caseId, Guid mappingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CasePhysicalFindingAdminItem>> GetFindingsAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<CasePhysicalFindingAdminItem> AddFindingAsync(Guid caseId, CreateCasePhysicalFindingCommand command, CancellationToken cancellationToken = default);
    Task<CasePhysicalFindingAdminItem> UpdateFindingAsync(Guid caseId, Guid mappingId, UpdateCasePhysicalFindingCommand command, CancellationToken cancellationToken = default);
    Task DeleteFindingAsync(Guid caseId, Guid mappingId, CancellationToken cancellationToken = default);
}
