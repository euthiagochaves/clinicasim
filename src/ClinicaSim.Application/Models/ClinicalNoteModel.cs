namespace ClinicaSim.Application.Models;

public sealed record ClinicalNoteModel(
    string? SummaryText,
    string? ProbableDiagnosisText,
    string? ConductStudiesText,
    string? ConductTreatmentText);
