using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface IFindingBankService
{
    Task<IReadOnlyCollection<FindingBankItem>> GetFindingsAsync(string? system, CancellationToken cancellationToken = default);
}
