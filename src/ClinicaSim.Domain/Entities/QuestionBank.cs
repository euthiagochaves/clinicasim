namespace ClinicaSim.Domain.Entities;

public class QuestionBank
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public bool Active { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<CaseQuestionAnswer> CaseQuestionAnswers { get; set; } = new List<CaseQuestionAnswer>();
}
