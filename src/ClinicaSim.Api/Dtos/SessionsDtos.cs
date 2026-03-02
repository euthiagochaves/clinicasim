namespace ClinicaSim.Api.Dtos;

public sealed record StartSessionRequest(Guid CaseId);
public sealed record StartSessionResponse(string SessionCode, string Status, DateTimeOffset StartedAt, CaseListItemDto Case, IReadOnlyCollection<SessionSectionDto> Sections);
public sealed record SessionSectionDto(Guid SectionId, string Name, IReadOnlyCollection<SessionCategoryDto> Categories);
public sealed record SessionCategoryDto(Guid CategoryId, string Name, IReadOnlyCollection<SessionQuestionDto> Questions);
public sealed record SessionQuestionDto(Guid QuestionId, string Text);

public sealed record RegisterEventRequest(Guid QuestionId);
public sealed record SessionEventResponse(Guid EventId, DateTimeOffset OccurredAt, string SectionName, string CategoryName, string QuestionText, string AnswerText);

public sealed record SessionInfoResponse(string SessionCode, string Status, DateTimeOffset StartedAt, DateTimeOffset? FinishedAt, CaseListItemDto Case);

public sealed record ClinicalNoteRequest(string SummaryText, string ProbableDiagnosisText, string ConductStudiesText, string ConductTreatmentText);
public sealed record ClinicalNoteResponse(string? SummaryText, string? ProbableDiagnosisText, string? ConductStudiesText, string? ConductTreatmentText);

public sealed record DifferentialItemRequest(int Rank, string Text);
public sealed record SaveDifferentialsRequest(IReadOnlyCollection<DifferentialItemRequest> Items);
public sealed record DifferentialItemResponse(int Rank, string Text);

public sealed record FinalizeResponse(string SessionCode, string Status, DateTimeOffset FinishedAt);
public sealed record ErrorResponse(IReadOnlyCollection<string> Errors);
