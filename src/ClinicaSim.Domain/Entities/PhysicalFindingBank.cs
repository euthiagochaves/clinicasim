namespace ClinicaSim.Domain.Entities;

public class PhysicalFindingBank
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public bool Active { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<CasePhysicalFinding> CasePhysicalFindings { get; set; } = new List<CasePhysicalFinding>();
}
