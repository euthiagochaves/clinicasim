using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface ISessionsService
{
    Task<SessionStartResult> StartAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<SessionEventResult> RegisterEventAsync(string sessionCode, Guid questionId, CancellationToken cancellationToken = default);
    Task<SessionInfoResult> GetSessionAsync(string sessionCode, CancellationToken cancellationToken = default);
    Task<ClinicalNoteModel> GetNoteAsync(string sessionCode, CancellationToken cancellationToken = default);
    Task<ClinicalNoteModel> UpsertNoteAsync(string sessionCode, ClinicalNoteModel model, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DifferentialModel>> GetDifferentialsAsync(string sessionCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DifferentialModel>> SaveDifferentialsAsync(string sessionCode, IReadOnlyCollection<DifferentialModel> items, CancellationToken cancellationToken = default);
    Task<FinalizeResult> FinalizeAsync(string sessionCode, CancellationToken cancellationToken = default);
}
