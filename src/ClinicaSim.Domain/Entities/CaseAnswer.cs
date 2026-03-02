namespace ClinicaSim.Domain.Entities;

public class CaseAnswer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public CaseQuestion Question { get; set; } = null!;
}
