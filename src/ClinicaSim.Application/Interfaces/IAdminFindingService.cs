using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface IAdminFindingService
{
    Task<IReadOnlyCollection<AdminFindingItem>> GetFindingsAsync(AdminFindingFilters filters, CancellationToken cancellationToken = default);
    Task<AdminFindingItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AdminFindingItem> CreateAsync(CreateFindingCommand command, CancellationToken cancellationToken = default);
    Task<AdminFindingItem> UpdateAsync(Guid id, UpdateFindingCommand command, CancellationToken cancellationToken = default);
    Task<AdminFindingItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default);
}
