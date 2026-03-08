namespace ClinicaSim.Api.Dtos;

public sealed record QuestionBankItemDto(Guid Id, string Text, string Section, string Category, string? Tags, bool Active);

public sealed record FindingBankItemDto(Guid Id, string Name, string System, string? Tags, bool Active);

public sealed record CaseQuestionAnswerItemDto(Guid? OverrideId, Guid QuestionId, string QuestionText, string AnswerText, bool IsInherited, bool IsCaseSpecific, bool IsHighlighted);

public sealed record CasePhysicalFindingItemDto(Guid? OverrideId, Guid FindingId, string FindingName, string System, bool Present, string? DetailText, bool IsInherited, bool IsCaseSpecific, bool IsHighlighted);

public sealed record ResolvedQuestionAnswerDto(Guid CaseId, Guid QuestionId, string AnswerText);

public sealed record ResolvedFindingDto(Guid CaseId, Guid FindingId, bool Present, string? DetailText);
