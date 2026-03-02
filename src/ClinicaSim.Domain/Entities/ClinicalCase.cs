namespace ClinicaSim.Domain.Entities;

public class ClinicalCase
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public int Age { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Triage { get; set; } = string.Empty;
    public ICollection<CaseSection> Sections { get; set; } = new List<CaseSection>();
}
