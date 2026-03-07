namespace ClinicaSim.Application.Models;

public sealed record AdminCaseItem(
    Guid Id,
    string FullName,
    int Age,
    string Sex,
    string ChiefComplaint,
    string Triage,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record AdminCaseFilters(bool? Active, string? Search, string? Triage);

public sealed record CreateCaseCommand(string FullName, int Age, string Sex, string ChiefComplaint, string Triage);

public sealed record UpdateCaseCommand(string FullName, int Age, string Sex, string ChiefComplaint, string Triage, bool? Active);

public sealed record CaseQuestionAnswerAdminItem(
    Guid Id,
    Guid CaseId,
    Guid QuestionId,
    string QuestionText,
    string Section,
    string Category,
    string AnswerText);

public sealed record CreateCaseQuestionAnswerCommand(Guid QuestionId, string AnswerText);

public sealed record UpdateCaseQuestionAnswerCommand(Guid QuestionId, string AnswerText);

public sealed record CasePhysicalFindingAdminItem(
    Guid Id,
    Guid CaseId,
    Guid FindingId,
    string FindingName,
    string System,
    bool Present,
    string? DetailText);

public sealed record CreateCasePhysicalFindingCommand(Guid FindingId, bool Present, string? DetailText);

public sealed record UpdateCasePhysicalFindingCommand(Guid FindingId, bool Present, string? DetailText);
