namespace ClinicaSim.Domain.Entities;

public class CaseCategory
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CaseSection Section { get; set; } = null!;
    public ICollection<CaseQuestion> Questions { get; set; } = new List<CaseQuestion>();
}
