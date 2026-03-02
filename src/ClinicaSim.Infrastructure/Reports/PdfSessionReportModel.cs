namespace ClinicaSim.Infrastructure.Reports;

internal sealed record PdfSessionReportModel(
    string SessionCode,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    PdfCaseModel Case,
    IReadOnlyCollection<PdfEventRow> Events,
    PdfNoteModel Note,
    IReadOnlyCollection<PdfDdxRow> Differentials);

internal sealed record PdfCaseModel(string FullName, int Age, string Sex, string ChiefComplaint, string Triage);
internal sealed record PdfEventRow(DateTimeOffset OccurredAt, string SectionName, string CategoryName, string QuestionText, string AnswerText);
internal sealed record PdfNoteModel(string SummaryText, string ProbableDiagnosisText, string ConductStudiesText, string ConductTreatmentText);
internal sealed record PdfDdxRow(int Rank, string Text);
