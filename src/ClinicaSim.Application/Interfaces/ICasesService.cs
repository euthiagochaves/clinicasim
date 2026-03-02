using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface ICasesService
{
    Task<IReadOnlyCollection<CaseListItem>> GetCasesAsync(CancellationToken cancellationToken = default);
}
