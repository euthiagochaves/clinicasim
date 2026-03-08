namespace ClinicaSim.Domain.Entities;

public class CaseQuestionOverride
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCaseSpecific { get; set; } = true;
    public bool IsHighlighted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ClinicalCase Case { get; set; } = null!;
    public QuestionBank Question { get; set; } = null!;
}
