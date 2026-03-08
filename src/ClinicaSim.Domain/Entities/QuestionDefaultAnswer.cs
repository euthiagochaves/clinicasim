namespace ClinicaSim.Domain.Entities;

public class QuestionDefaultAnswer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public QuestionBank Question { get; set; } = null!;
}
