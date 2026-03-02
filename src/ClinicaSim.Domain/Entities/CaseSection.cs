namespace ClinicaSim.Domain.Entities;

public class CaseSection
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ClinicalCase Case { get; set; } = null!;
    public ICollection<CaseCategory> Categories { get; set; } = new List<CaseCategory>();
}
