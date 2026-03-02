using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class CasesService(ClinicaSimDbContext dbContext) : ICasesService
{
    public async Task<IReadOnlyCollection<CaseListItem>> GetCasesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ClinicalCases
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new CaseListItem(x.Id, x.FullName, x.Age, x.Sex, x.ChiefComplaint, x.Triage))
            .ToListAsync(cancellationToken);
    }
}
