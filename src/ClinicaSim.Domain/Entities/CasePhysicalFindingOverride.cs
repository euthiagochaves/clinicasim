namespace ClinicaSim.Domain.Entities;

public class CasePhysicalFindingOverride
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public Guid FindingId { get; set; }
    public bool Present { get; set; }
    public string? DetailText { get; set; }
    public bool IsCaseSpecific { get; set; } = true;
    public bool IsHighlighted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ClinicalCase Case { get; set; } = null!;
    public PhysicalFindingBank Finding { get; set; } = null!;
}
