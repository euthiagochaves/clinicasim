using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Infrastructure.Persistence;
using ClinicaSim.Infrastructure.Reports;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicaSim.Infrastructure.Services;

public class PdfReportService(ClinicaSimDbContext dbContext) : IPdfReportService
{
    public async Task<byte[]> GenerateSessionPdfAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.ConsultationSessions
            .AsNoTracking()
            .Include(s => s.Case)
            .Include(s => s.Events)
            .Include(s => s.Note)
            .Include(s => s.Differentials)
            .FirstOrDefaultAsync(s => s.SessionCode == sessionCode, cancellationToken)
            ?? throw new AppException("Sesión no encontrada.", 404);

        if (!string.Equals(session.Status, "Finalized", StringComparison.OrdinalIgnoreCase))
        {
            throw new AppException("La sesión no está finalizada. No se puede generar el PDF.", 409);
        }

        var model = new PdfSessionReportModel(
            session.SessionCode,
            session.StartedAt,
            session.FinishedAt,
            new PdfCaseModel(session.Case.FullName, session.Case.Age, session.Case.Sex, session.Case.ChiefComplaint, session.Case.Triage),
            session.Events.OrderBy(e => e.OccurredAt)
                .Select(e => new PdfEventRow(e.OccurredAt, e.SectionName, e.CategoryName, e.QuestionText, e.AnswerText))
                .ToList(),
            new PdfNoteModel(
                session.Note?.SummaryText ?? string.Empty,
                session.Note?.ProbableDiagnosisText ?? string.Empty,
                session.Note?.ConductStudiesText ?? string.Empty,
                session.Note?.ConductTreatmentText ?? string.Empty),
            session.Differentials.OrderBy(d => d.Rank).Select(d => new PdfDdxRow(d.Rank, d.Text)).ToList());

        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("ClinicaSim — Consulta Simulada").FontSize(18).Bold();
                    col.Item().Text($"Código de sesión: {model.SessionCode}").FontSize(12).SemiBold();
                    col.Item().Text($"Inicio: {model.StartedAt:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Fin: {(model.FinishedAt.HasValue ? model.FinishedAt.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty)}");
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text("Datos del paciente/caso").Bold().FontSize(12);
                    col.Item().Text($"Nombre completo: {model.Case.FullName}");
                    col.Item().Text($"Edad: {model.Case.Age}");
                    col.Item().Text($"Sexo: {model.Case.Sex}");
                    col.Item().Text($"Motivo de consulta: {model.Case.ChiefComplaint}");
                    col.Item().Text($"Triaje: {model.Case.Triage}");

                    col.Item().PaddingTop(8).Text("Registro de interacción").Bold().FontSize(12);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Hora/Fecha");
                            header.Cell().Element(HeaderCell).Text("Sección");
                            header.Cell().Element(HeaderCell).Text("Categoría");
                            header.Cell().Element(HeaderCell).Text("Pregunta");
                            header.Cell().Element(HeaderCell).Text("Respuesta");
                        });

                        foreach (var row in model.Events)
                        {
                            table.Cell().Element(BodyCell).Text($"{row.OccurredAt:HH:mm:ss}\n{row.OccurredAt:dd/MM/yyyy}");
                            table.Cell().Element(BodyCell).Text(row.SectionName);
                            table.Cell().Element(BodyCell).Text(row.CategoryName);
                            table.Cell().Element(BodyCell).Text(row.QuestionText);
                            table.Cell().Element(BodyCell).Text(row.AnswerText);
                        }
                    });

                    col.Item().PaddingTop(8).Text("Historia clínica del alumno").Bold().FontSize(12);
                    col.Item().Text("Resumen").SemiBold();
                    col.Item().Text(model.Note.SummaryText);

                    col.Item().Text("Diagnósticos diferenciales (ordenados)").SemiBold();
                    foreach (var ddx in model.Differentials)
                    {
                        col.Item().Text($"{ddx.Rank}. {ddx.Text}");
                    }

                    col.Item().Text("Diagnóstico probable").SemiBold();
                    col.Item().Text(model.Note.ProbableDiagnosisText);

                    col.Item().Text("Conducta").SemiBold();
                    col.Item().Text("Estudios sugeridos para confirmar:");
                    col.Item().Text(model.Note.ConductStudiesText);
                    col.Item().Text("Tratamiento si se confirma:");
                    col.Item().Text(model.Note.ConductTreatmentText);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado por ClinicaSim — Página ");
                    text.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container.BorderBottom(1).PaddingVertical(4).Background(Colors.Grey.Lighten3).PaddingHorizontal(4);
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).PaddingHorizontal(4);
    }
}
