using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class QuestionBankService(ClinicaSimDbContext dbContext) : IQuestionBankService
{
    public async Task<IReadOnlyCollection<QuestionBankItem>> GetQuestionsAsync(string? section, CancellationToken cancellationToken = default)
    {
        var query = dbContext.QuestionBanks
            .AsNoTracking()
            .Where(x => x.Active);

        if (!string.IsNullOrWhiteSpace(section))
        {
            var sectionFilter = section.Trim();
            query = query.Where(x => x.Section == sectionFilter);
        }

        return await query
            .OrderBy(x => x.Text)
            .Select(x => new QuestionBankItem(x.Id, x.Text, x.Section, x.Category, x.Tags, x.Active))
            .ToListAsync(cancellationToken);
    }
}
