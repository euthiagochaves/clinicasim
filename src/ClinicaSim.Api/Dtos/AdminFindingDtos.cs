namespace ClinicaSim.Api.Dtos;

public sealed record FindingAdminListItemDto(
    Guid Id,
    string Name,
    string System,
    string? Tags,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateFindingRequest(string Name, string System, string? Tags);

public sealed record UpdateFindingRequest(string Name, string System, string? Tags, bool? Active);
