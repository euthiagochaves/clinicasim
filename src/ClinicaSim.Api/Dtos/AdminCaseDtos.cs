namespace ClinicaSim.Api.Dtos;

public sealed record CaseAdminListItemDto(
    Guid Id,
    string FullName,
    int Age,
    string Sex,
    string ChiefComplaint,
    string Triage,
    bool Active,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateCaseRequest(string FullName, int Age, string Sex, string ChiefComplaint, string Triage);

public sealed record UpdateCaseRequest(string FullName, int Age, string Sex, string ChiefComplaint, string Triage, bool? Active);

public sealed record CaseQuestionAnswerDto(
    Guid Id,
    Guid CaseId,
    Guid QuestionId,
    string QuestionText,
    string Section,
    string Category,
    string AnswerText);

public sealed record CreateCaseQuestionAnswerRequest(Guid QuestionId, string AnswerText);

public sealed record UpdateCaseQuestionAnswerRequest(Guid QuestionId, string AnswerText);

public sealed record CasePhysicalFindingDto(
    Guid Id,
    Guid CaseId,
    Guid FindingId,
    string FindingName,
    string System,
    bool Present,
    string? DetailText);

public sealed record CreateCasePhysicalFindingRequest(Guid FindingId, bool Present, string? DetailText);

public sealed record UpdateCasePhysicalFindingRequest(Guid FindingId, bool Present, string? DetailText);
