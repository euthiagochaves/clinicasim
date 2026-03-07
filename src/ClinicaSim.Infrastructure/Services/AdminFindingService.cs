using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Domain.Entities;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class AdminFindingService(ClinicaSimDbContext dbContext) : IAdminFindingService
{
    public async Task<IReadOnlyCollection<AdminFindingItem>> GetFindingsAsync(AdminFindingFilters filters, CancellationToken cancellationToken = default)
    {
        var query = dbContext.PhysicalFindingBanks.AsNoTracking().AsQueryable();

        if (filters.Active.HasValue)
        {
            query = query.Where(x => x.Active == filters.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.System))
        {
            var system = filters.System.Trim();
            query = query.Where(x => x.System == system);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search));
        }

        return await query
            .OrderBy(x => x.System)
            .ThenBy(x => x.Name)
            .Select(x => new AdminFindingItem(x.Id, x.Name, x.System, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminFindingItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AdminFindingItem(x.Id, x.Name, x.System, x.Tags, x.Active, x.CreatedAt, x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return item ?? throw new AppException("Hallazgo no encontrado.", 404);
    }

    public async Task<AdminFindingItem> CreateAsync(CreateFindingCommand command, CancellationToken cancellationToken = default)
    {
        var normalized = ValidateAndNormalize(command.Name, command.System, command.Tags);
        await EnsureNoDuplicateAsync(normalized.Name, normalized.System, null, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entity = new PhysicalFindingBank
        {
            Id = Guid.NewGuid(),
            Name = normalized.Name,
            System = normalized.System,
            Tags = normalized.Tags,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.PhysicalFindingBanks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminFindingItem> UpdateAsync(Guid id, UpdateFindingCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.PhysicalFindingBanks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Hallazgo no encontrado.", 404);

        var normalized = ValidateAndNormalize(command.Name, command.System, command.Tags);
        await EnsureNoDuplicateAsync(normalized.Name, normalized.System, id, cancellationToken);

        entity.Name = normalized.Name;
        entity.System = normalized.System;
        entity.Tags = normalized.Tags;
        if (command.Active.HasValue)
        {
            entity.Active = command.Active.Value;
        }

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AdminFindingItem> SetActiveAsync(Guid id, bool active, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.PhysicalFindingBanks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Hallazgo no encontrado.", 404);

        if (entity.Active == active)
        {
            return Map(entity);
        }

        entity.Active = active;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private async Task EnsureNoDuplicateAsync(string name, string system, Guid? currentId, CancellationToken cancellationToken)
    {
        var normalizedName = name.ToLower();
        var normalizedSystem = system.ToLower();

        var exists = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id != currentId &&
                x.Name.ToLower() == normalizedName &&
                x.System.ToLower() == normalizedSystem,
                cancellationToken);

        if (exists)
        {
            throw new AppException("Ya existe un hallazgo con el mismo nombre y sistema.", 409);
        }
    }

    private static (string Name, string System, string? Tags) ValidateAndNormalize(string name, string system, string? tags)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedSystem = system?.Trim() ?? string.Empty;
        var normalizedTags = string.IsNullOrWhiteSpace(tags) ? null : tags.Trim();

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(normalizedName)) errors.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(normalizedSystem)) errors.Add("El sistema es obligatorio.");

        if (errors.Count > 0)
        {
            throw new AppException("Complete los campos obligatorios.", 400, errors);
        }

        return (normalizedName, normalizedSystem, normalizedTags);
    }

    private static AdminFindingItem Map(PhysicalFindingBank entity)
        => new(entity.Id, entity.Name, entity.System, entity.Tags, entity.Active, entity.CreatedAt, entity.UpdatedAt);
}
