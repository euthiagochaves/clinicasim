namespace ClinicaSim.Domain.Entities;

public class DifferentialDiagnosis
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public int Rank { get; set; }
    public string Text { get; set; } = string.Empty;
    public ConsultationSession Session { get; set; } = null!;
}
