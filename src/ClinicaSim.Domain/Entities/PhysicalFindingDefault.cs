namespace ClinicaSim.Domain.Entities;

public class PhysicalFindingDefault
{
    public Guid Id { get; set; }
    public Guid FindingId { get; set; }
    public bool Present { get; set; }
    public string? DetailText { get; set; }
    public bool Active { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public PhysicalFindingBank Finding { get; set; } = null!;
}
