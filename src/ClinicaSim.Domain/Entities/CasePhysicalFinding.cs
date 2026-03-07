namespace ClinicaSim.Domain.Entities;

public class CasePhysicalFinding
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public Guid FindingId { get; set; }
    public bool Present { get; set; }
    public string? DetailText { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ClinicalCase Case { get; set; } = null!;
    public PhysicalFindingBank Finding { get; set; } = null!;
}
