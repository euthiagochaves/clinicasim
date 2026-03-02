namespace ClinicaSim.Domain.Entities;

public class CaseQuestion
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Text { get; set; } = string.Empty;
    public CaseCategory Category { get; set; } = null!;
    public CaseAnswer Answer { get; set; } = null!;
}
