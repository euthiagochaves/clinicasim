namespace ClinicaSim.Domain.Entities;

public class ClinicalNote
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public string ProbableDiagnosisText { get; set; } = string.Empty;
    public string ConductStudiesText { get; set; } = string.Empty;
    public string ConductTreatmentText { get; set; } = string.Empty;
    public ConsultationSession Session { get; set; } = null!;
}
