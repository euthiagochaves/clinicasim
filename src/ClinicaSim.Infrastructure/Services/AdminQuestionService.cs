using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Domain.Entities;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class AdminQuestionService(ClinicaSimDbContext dbContext) : IAdminQuestionService
{
    public async Task<IReadOnlyCollection<AdminQuestionItem>> GetQuestionsAsync(AdminQuestionFilters filters, CancellationToken cancellationToken = default)
    {
        var query = dbContext.QuestionBanks.AsNoTracking().AsQueryable();

        if (filters.Active.HasValue)
        {
            query = query.Where(x => x.Active == filters.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Section))
        {
            var section = filters.Section.Trim();
            query = query.Where(x => x.Section == section);
        }

        if (!string.IsNullOrWhiteSpace(filters.Category))
        {
            var category = filters.Category.Trim();
            query = query.Where(x => x.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(x => x.Text.ToLower().Contains(search));
        }

        return await query
            .OrderBy(x => x.Text)
            .Select(x => new AdminQuestionItem(x.Id, x.Text, x.Section, x.Category, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminQuestionItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.QuestionBanks
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AdminQuestionItem(x.Id, x.Text, x.Section, x.Category, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return item ?? throw new AppException("Pregunta no encontrada.", 404);
    }

    public async Task<AdminQuestionItem> CreateAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        var normalized = ValidateAndNormalize(command.Text, command.Section, command.Category, command.Tags);
        await EnsureNoDuplicateAsync(normalized.Text, normalized.Section, normalized.Category, null, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entity = new QuestionBank
        {
            Id = Guid.NewGuid(),
            Text = normalized.Text,
            Section = normalized.Section,
            Category = normalized.Category,
            Tags = normalized.Tags,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.QuestionBanks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminQuestionItem> UpdateAsync(Guid id, UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.QuestionBanks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        var normalized = ValidateAndNormalize(command.Text, command.Section, command.Category, command.Tags);
        await EnsureNoDuplicateAsync(normalized.Text, normalized.Section, normalized.Category, id, cancellationToken);

        entity.Text = normalized.Text;
        entity.Section = normalized.Section;
        entity.Category = normalized.Category;
        entity.Tags = normalized.Tags;
        if (command.Active.HasValue)
        {
            entity.Active = command.Active.Value;
        }

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminQuestionItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.QuestionBanks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        if (entity.Active == active)
        {
            return Map(entity);
        }

        entity.Active = active;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private async Task EnsureNoDuplicateAsync(string text, string section, string category, Guid? currentId, CancellationToken cancellationToken)
    {
        var normalizedText = text.ToLower();
        var normalizedSection = section.ToLower();
        var normalizedCategory = category.ToLower();

        var exists = await dbContext.QuestionBanks
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id != currentId &&
                x.Text.ToLower() == normalizedText &&
                x.Section.ToLower() == normalizedSection &&
                x.Category.ToLower() == normalizedCategory,
                cancellationToken);

        if (exists)
        {
            throw new AppException("Ya existe una pregunta con el mismo texto, sección y categoría.", 409);
        }
    }

    private static (string Text, string Section, string Category, string? Tags) ValidateAndNormalize(string text, string section, string category, string? tags)
    {
        var normalizedText = text?.Trim() ?? string.Empty;
        var normalizedSection = section?.Trim() ?? string.Empty;
        var normalizedCategory = category?.Trim() ?? string.Empty;
        var normalizedTags = string.IsNullOrWhiteSpace(tags) ? null : tags.Trim();

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(normalizedText)) errors.Add("El texto es obligatorio.");
        if (string.IsNullOrWhiteSpace(normalizedSection)) errors.Add("La sección es obligatoria.");
        if (string.IsNullOrWhiteSpace(normalizedCategory)) errors.Add("La categoría es obligatoria.");

        if (errors.Count > 0)
        {
            throw new AppException("Complete los campos obligatorios.", 400, errors);
        }

        return (normalizedText, normalizedSection, normalizedCategory, normalizedTags);
    }

    private static AdminQuestionItem Map(QuestionBank entity)
        => new(entity.Id, entity.Text, entity.Section, entity.Category, entity.Tags, entity.Active, entity.CreatedAt, entity.UpdatedAt);
}
