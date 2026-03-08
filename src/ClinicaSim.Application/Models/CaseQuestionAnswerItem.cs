namespace ClinicaSim.Application.Models;

public sealed record CaseQuestionAnswerItem(Guid? OverrideId, Guid QuestionId, string QuestionText, string AnswerText, bool IsInherited, bool IsCaseSpecific, bool IsHighlighted);
