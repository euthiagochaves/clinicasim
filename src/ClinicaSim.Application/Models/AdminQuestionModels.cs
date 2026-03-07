namespace ClinicaSim.Application.Models;

public sealed record AdminQuestionItem(
    Guid Id,
    string Text,
    string Section,
    string Category,
    string? Tags,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record AdminQuestionFilters(bool? Active, string? Section, string? Category, string? Search);

public sealed record CreateQuestionCommand(string Text, string Section, string Category, string? Tags);

public sealed record UpdateQuestionCommand(string Text, string Section, string Category, string? Tags, bool? Active);
