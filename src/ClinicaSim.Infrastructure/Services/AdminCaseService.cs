using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Domain.Entities;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class AdminCaseService(ClinicaSimDbContext dbContext) : IAdminCaseService
{
    private const string DefaultMissingAnswer = "No tengo ese dato.";

    public async Task<IReadOnlyCollection<AdminCaseItem>> GetCasesAsync(AdminCaseFilters filters, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ClinicalCases.AsNoTracking().AsQueryable();

        if (filters.Active.HasValue)
        {
            query = query.Where(x => x.Active == filters.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Triage))
        {
            var triage = filters.Triage.Trim();
            query = query.Where(x => x.Triage == triage);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(search) || x.ChiefComplaint.ToLower().Contains(search));
        }

        return await query
            .OrderBy(x => x.FullName)
            .Select(x => new AdminCaseItem(x.Id, x.FullName, x.Age, x.Sex, x.ChiefComplaint, x.Triage, x.Active, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminCaseItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.ClinicalCases
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AdminCaseItem(x.Id, x.FullName, x.Age, x.Sex, x.ChiefComplaint, x.Triage, x.Active, x.CreatedAt, x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return item ?? throw new AppException("Caso no encontrado.", 404);
    }

    public async Task<AdminCaseItem> CreateAsync(CreateCaseCommand command, CancellationToken cancellationToken = default)
    {
        var normalized = ValidateCase(command.FullName, command.Age, command.Sex, command.ChiefComplaint, command.Triage);
        var now = DateTimeOffset.UtcNow;

        var entity = new ClinicalCase
        {
            Id = Guid.NewGuid(),
            FullName = normalized.FullName,
            Age = normalized.Age,
            Sex = normalized.Sex,
            ChiefComplaint = normalized.ChiefComplaint,
            Triage = normalized.Triage,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.ClinicalCases.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminCaseItem> UpdateAsync(Guid id, UpdateCaseCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.ClinicalCases.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Caso no encontrado.", 404);

        var normalized = ValidateCase(command.FullName, command.Age, command.Sex, command.ChiefComplaint, command.Triage);

        entity.FullName = normalized.FullName;
        entity.Age = normalized.Age;
        entity.Sex = normalized.Sex;
        entity.ChiefComplaint = normalized.ChiefComplaint;
        entity.Triage = normalized.Triage;
        if (command.Active.HasValue)
        {
            entity.Active = command.Active.Value;
        }

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminCaseItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.ClinicalCases.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Caso no encontrado.", 404);

        if (entity.Active != active)
        {
            entity.Active = active;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Map(entity);
    }

    public async Task<IReadOnlyCollection<CaseQuestionAnswerAdminItem>> GetAnswersAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);

        var items = await dbContext.QuestionBanks
            .AsNoTracking()
            .Where(x => x.Active)
            .OrderBy(x => x.Text)
            .Select(x => new
            {
                x.Id,
                x.Text,
                x.Section,
                x.Category,
                Override = x.CaseOverrides.Where(o => o.CaseId == caseId)
                    .Select(o => new { o.Id, o.AnswerText, o.IsCaseSpecific, o.IsHighlighted })
                    .FirstOrDefault(),
                Default = x.DefaultAnswers.Where(d => d.Active).Select(d => d.AnswerText).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return items.Select(x => new CaseQuestionAnswerAdminItem(
            x.Override?.Id,
            caseId,
            x.Id,
            x.Text,
            x.Section,
            x.Category,
            x.Override?.AnswerText ?? x.Default ?? DefaultMissingAnswer,
            x.Override is null,
            x.Override?.IsCaseSpecific ?? false,
            x.Override?.IsHighlighted ?? false)).ToList();
    }

    public async Task<CaseQuestionAnswerAdminItem> AddAnswerAsync(Guid caseId, CreateCaseQuestionAnswerCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);
        var question = await dbContext.QuestionBanks.FirstOrDefaultAsync(x => x.Id == command.QuestionId, cancellationToken)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        var answerText = NormalizeRequired(command.AnswerText, "La respuesta es obligatoria.");

        var duplicate = await dbContext.CaseQuestionOverrides.AnyAsync(x => x.CaseId == caseId && x.QuestionId == command.QuestionId, cancellationToken);
        if (duplicate)
        {
            throw new AppException("Ya existe una sobrescritura para esa pregunta en el caso.", 409);
        }

        var now = DateTimeOffset.UtcNow;
        var mapping = new CaseQuestionOverride
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            QuestionId = command.QuestionId,
            AnswerText = answerText,
            IsCaseSpecific = command.IsCaseSpecific,
            IsHighlighted = command.IsHighlighted,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.CaseQuestionOverrides.Add(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CaseQuestionAnswerAdminItem(mapping.Id, caseId, command.QuestionId, question.Text, question.Section, question.Category, mapping.AnswerText, false, mapping.IsCaseSpecific, mapping.IsHighlighted);
    }

    public async Task<CaseQuestionAnswerAdminItem> UpdateAnswerAsync(Guid caseId, Guid mappingId, UpdateCaseQuestionAnswerCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);

        var mapping = await dbContext.CaseQuestionOverrides.FirstOrDefaultAsync(x => x.Id == mappingId && x.CaseId == caseId, cancellationToken)
            ?? throw new AppException("Sobrescritura de respuesta no encontrada.", 404);

        var question = await dbContext.QuestionBanks.FirstOrDefaultAsync(x => x.Id == command.QuestionId, cancellationToken)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        var duplicate = await dbContext.CaseQuestionOverrides.AnyAsync(x => x.CaseId == caseId && x.QuestionId == command.QuestionId && x.Id != mappingId, cancellationToken);
        if (duplicate)
        {
            throw new AppException("Ya existe una sobrescritura para esa pregunta en el caso.", 409);
        }

        mapping.QuestionId = command.QuestionId;
        mapping.AnswerText = NormalizeRequired(command.AnswerText, "La respuesta es obligatoria.");
        mapping.IsCaseSpecific = command.IsCaseSpecific;
        mapping.IsHighlighted = command.IsHighlighted;
        mapping.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CaseQuestionAnswerAdminItem(mapping.Id, caseId, mapping.QuestionId, question.Text, question.Section, question.Category, mapping.AnswerText, false, mapping.IsCaseSpecific, mapping.IsHighlighted);
    }

    public async Task DeleteAnswerAsync(Guid caseId, Guid mappingId, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);
        var mapping = await dbContext.CaseQuestionOverrides.FirstOrDefaultAsync(x => x.Id == mappingId && x.CaseId == caseId, cancellationToken)
            ?? throw new AppException("Sobrescritura de respuesta no encontrada.", 404);

        dbContext.CaseQuestionOverrides.Remove(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CasePhysicalFindingAdminItem>> GetFindingsAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);

        var items = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .Where(x => x.Active)
            .OrderBy(x => x.System)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.System,
                Override = x.CaseOverrides.Where(o => o.CaseId == caseId)
                    .Select(o => new { o.Id, o.Present, o.DetailText, o.IsCaseSpecific, o.IsHighlighted })
                    .FirstOrDefault(),
                Default = x.Defaults.Where(d => d.Active).Select(d => new { d.Present, d.DetailText }).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return items.Select(x => new CasePhysicalFindingAdminItem(
            x.Override?.Id,
            caseId,
            x.Id,
            x.Name,
            x.System,
            x.Override?.Present ?? x.Default?.Present ?? false,
            x.Override?.DetailText ?? x.Default?.DetailText,
            x.Override is null,
            x.Override?.IsCaseSpecific ?? false,
            x.Override?.IsHighlighted ?? false)).ToList();
    }

    public async Task<CasePhysicalFindingAdminItem> AddFindingAsync(Guid caseId, CreateCasePhysicalFindingCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);
        var finding = await dbContext.PhysicalFindingBanks.FirstOrDefaultAsync(x => x.Id == command.FindingId, cancellationToken)
            ?? throw new AppException("Hallazgo no encontrado.", 404);

        var duplicate = await dbContext.CasePhysicalFindingOverrides.AnyAsync(x => x.CaseId == caseId && x.FindingId == command.FindingId, cancellationToken);
        if (duplicate)
        {
            throw new AppException("Ya existe una sobrescritura para ese hallazgo en el caso.", 409);
        }

        var now = DateTimeOffset.UtcNow;
        var mapping = new CasePhysicalFindingOverride
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            FindingId = command.FindingId,
            Present = command.Present,
            DetailText = NormalizeOptional(command.DetailText),
            IsCaseSpecific = command.IsCaseSpecific,
            IsHighlighted = command.IsHighlighted,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.CasePhysicalFindingOverrides.Add(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CasePhysicalFindingAdminItem(mapping.Id, caseId, mapping.FindingId, finding.Name, finding.System, mapping.Present, mapping.DetailText, false, mapping.IsCaseSpecific, mapping.IsHighlighted);
    }

    public async Task<CasePhysicalFindingAdminItem> UpdateFindingAsync(Guid caseId, Guid mappingId, UpdateCasePhysicalFindingCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);

        var mapping = await dbContext.CasePhysicalFindingOverrides.FirstOrDefaultAsync(x => x.Id == mappingId && x.CaseId == caseId, cancellationToken)
            ?? throw new AppException("Sobrescritura de hallazgo no encontrada.", 404);

        var finding = await dbContext.PhysicalFindingBanks.FirstOrDefaultAsync(x => x.Id == command.FindingId, cancellationToken)
            ?? throw new AppException("Hallazgo no encontrado.", 404);

        var duplicate = await dbContext.CasePhysicalFindingOverrides.AnyAsync(x => x.CaseId == caseId && x.FindingId == command.FindingId && x.Id != mappingId, cancellationToken);
        if (duplicate)
        {
            throw new AppException("Ya existe una sobrescritura para ese hallazgo en el caso.", 409);
        }

        mapping.FindingId = command.FindingId;
        mapping.Present = command.Present;
        mapping.DetailText = NormalizeOptional(command.DetailText);
        mapping.IsCaseSpecific = command.IsCaseSpecific;
        mapping.IsHighlighted = command.IsHighlighted;
        mapping.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CasePhysicalFindingAdminItem(mapping.Id, caseId, mapping.FindingId, finding.Name, finding.System, mapping.Present, mapping.DetailText, false, mapping.IsCaseSpecific, mapping.IsHighlighted);
    }

    public async Task DeleteFindingAsync(Guid caseId, Guid mappingId, CancellationToken cancellationToken = default)
    {
        await EnsureCaseExists(caseId, cancellationToken);
        var mapping = await dbContext.CasePhysicalFindingOverrides.FirstOrDefaultAsync(x => x.Id == mappingId && x.CaseId == caseId, cancellationToken)
            ?? throw new AppException("Sobrescritura de hallazgo no encontrada.", 404);

        dbContext.CasePhysicalFindingOverrides.Remove(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCaseExists(Guid caseId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.ClinicalCases.AnyAsync(x => x.Id == caseId, cancellationToken);
        if (!exists)
        {
            throw new AppException("Caso no encontrado.", 404);
        }
    }

    private static (string FullName, int Age, string Sex, string ChiefComplaint, string Triage) ValidateCase(string fullName, int age, string sex, string chiefComplaint, string triage)
    {
        var name = fullName?.Trim() ?? string.Empty;
        var normalizedSex = sex?.Trim() ?? string.Empty;
        var complaint = chiefComplaint?.Trim() ?? string.Empty;
        var normalizedTriage = triage?.Trim() ?? string.Empty;

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(name)) errors.Add("El nombre es obligatorio.");
        if (age < 0) errors.Add("La edad debe ser mayor o igual a 0.");
        if (string.IsNullOrWhiteSpace(normalizedSex)) errors.Add("El sexo es obligatorio.");
        if (string.IsNullOrWhiteSpace(complaint)) errors.Add("La queja principal es obligatoria.");
        if (string.IsNullOrWhiteSpace(normalizedTriage)) errors.Add("El triaje es obligatorio.");

        if (errors.Count > 0)
        {
            throw new AppException("Complete los campos obligatorios.", 400, errors);
        }

        return (name, age, normalizedSex, complaint, normalizedTriage);
    }

    private static string NormalizeRequired(string value, string message)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new AppException("Complete los campos obligatorios.", 400, [message]);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static AdminCaseItem Map(ClinicalCase entity)
        => new(entity.Id, entity.FullName, entity.Age, entity.Sex, entity.ChiefComplaint, entity.Triage, entity.Active, entity.CreatedAt, entity.UpdatedAt);
}
