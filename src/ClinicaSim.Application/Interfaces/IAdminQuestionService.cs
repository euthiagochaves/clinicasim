using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface IAdminQuestionService
{
    Task<IReadOnlyCollection<AdminQuestionItem>> GetQuestionsAsync(AdminQuestionFilters filters, CancellationToken cancellationToken = default);
    Task<AdminQuestionItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AdminQuestionItem> CreateAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default);
    Task<AdminQuestionItem> UpdateAsync(Guid id, UpdateQuestionCommand command, CancellationToken cancellationToken = default);
    Task<AdminQuestionItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default);
}
