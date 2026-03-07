namespace ClinicaSim.Application.Models;

public sealed record AdminFindingItem(
    Guid Id,
    string Name,
    string System,
    string? Tags,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record AdminFindingFilters(bool? Active, string? System, string? Search);

public sealed record CreateFindingCommand(string Name, string System, string? Tags);

public sealed record UpdateFindingCommand(string Name, string System, string? Tags, bool? Active);
