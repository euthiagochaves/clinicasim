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
    Guid? OverrideId,
    Guid CaseId,
    Guid QuestionId,
    string QuestionText,
    string Section,
    string Category,
    string AnswerText,
    bool IsInherited,
    bool IsCaseSpecific,
    bool IsHighlighted);

public sealed record CreateCaseQuestionAnswerRequest(Guid QuestionId, string AnswerText, bool IsCaseSpecific = true, bool IsHighlighted = false);

public sealed record UpdateCaseQuestionAnswerRequest(Guid QuestionId, string AnswerText, bool IsCaseSpecific = true, bool IsHighlighted = false);

public sealed record CasePhysicalFindingDto(
    Guid? OverrideId,
    Guid CaseId,
    Guid FindingId,
    string FindingName,
    string System,
    bool Present,
    string? DetailText,
    bool IsInherited,
    bool IsCaseSpecific,
    bool IsHighlighted);

public sealed record CreateCasePhysicalFindingRequest(Guid FindingId, bool Present, string? DetailText, bool IsCaseSpecific = true, bool IsHighlighted = false);

public sealed record UpdateCasePhysicalFindingRequest(Guid FindingId, bool Present, string? DetailText, bool IsCaseSpecific = true, bool IsHighlighted = false);
