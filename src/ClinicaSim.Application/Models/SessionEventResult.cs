namespace ClinicaSim.Application.Models;

public sealed record SessionEventResult(
    Guid EventId,
    DateTimeOffset OccurredAt,
    string SectionName,
    string CategoryName,
    string QuestionText,
    string AnswerText);
