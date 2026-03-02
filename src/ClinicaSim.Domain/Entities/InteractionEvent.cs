namespace ClinicaSim.Domain.Entities;

public class InteractionEvent
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string AnswerText { get; set; } = string.Empty;
    public ConsultationSession Session { get; set; } = null!;
}
