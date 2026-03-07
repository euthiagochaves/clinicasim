namespace ClinicaSim.Application.Models;

public sealed record ResolvedQuestionAnswer(Guid CaseId, Guid QuestionId, string AnswerText);
