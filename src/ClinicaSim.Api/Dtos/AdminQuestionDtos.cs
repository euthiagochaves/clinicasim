namespace ClinicaSim.Api.Dtos;

public sealed record QuestionAdminListItemDto(
    Guid Id,
    string Text,
    string Section,
    string Category,
    string? Tags,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateQuestionRequest(string Text, string Section, string Category, string? Tags);

public sealed record UpdateQuestionRequest(string Text, string Section, string Category, string? Tags, bool? Active);
