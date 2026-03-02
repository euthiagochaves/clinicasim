namespace ClinicaSim.Domain.Entities;

public class ConsultationSession
{
    public Guid Id { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public Guid CaseId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public ClinicalCase Case { get; set; } = null!;
    public ICollection<InteractionEvent> Events { get; set; } = new List<InteractionEvent>();
    public ClinicalNote? Note { get; set; }
    public ICollection<DifferentialDiagnosis> Differentials { get; set; } = new List<DifferentialDiagnosis>();
}
