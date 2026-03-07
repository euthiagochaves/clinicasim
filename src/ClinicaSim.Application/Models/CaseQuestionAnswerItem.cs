namespace ClinicaSim.Application.Models;

public sealed record CaseQuestionAnswerItem(Guid Id, Guid QuestionId, string QuestionText, string AnswerText);
