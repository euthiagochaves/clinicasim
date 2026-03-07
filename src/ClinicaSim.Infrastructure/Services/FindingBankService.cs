using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class FindingBankService(ClinicaSimDbContext dbContext) : IFindingBankService
{
    public async Task<IReadOnlyCollection<FindingBankItem>> GetFindingsAsync(string? system, CancellationToken cancellationToken = default)
    {
        var query = dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .Where(x => x.Active);

        if (!string.IsNullOrWhiteSpace(system))
        {
            var systemFilter = system.Trim();
            query = query.Where(x => x.System == systemFilter);
        }

        return await query
            .OrderBy(x => x.System)
            .ThenBy(x => x.Name)
            .Select(x => new FindingBankItem(x.Id, x.Name, x.System, x.Tags, x.Active))
            .ToListAsync(cancellationToken);
    }
}
