using System.Security.Cryptography;
using ClinicaSim.Application.Common;
using ClinicaSim.Application.Interfaces;
using ClinicaSim.Application.Models;
using ClinicaSim.Domain.Entities;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Services;

public class SessionsService(ClinicaSimDbContext dbContext) : ISessionsService
{
    private const string ActiveStatus = "Active";
    private const string FinalizedStatus = "Finalized";
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public async Task<SessionStartResult> StartAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var clinicalCase = await dbContext.ClinicalCases
            .AsNoTracking()
            .Include(c => c.Sections)
                .ThenInclude(s => s.Categories)
                .ThenInclude(c => c.Questions)
            .FirstOrDefaultAsync(c => c.Id == caseId, cancellationToken)
            ?? throw new AppException("Caso clínico no encontrado.", 404);

        var sessionCode = await GenerateUniqueCodeAsync(cancellationToken);
        var startedAt = DateTimeOffset.UtcNow;

        var session = new ConsultationSession
        {
            Id = Guid.NewGuid(),
            CaseId = clinicalCase.Id,
            SessionCode = sessionCode,
            StartedAt = startedAt,
            Status = ActiveStatus
        };

        dbContext.ConsultationSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        var caseItem = new CaseListItem(clinicalCase.Id, clinicalCase.FullName, clinicalCase.Age, clinicalCase.Sex, clinicalCase.ChiefComplaint, clinicalCase.Triage);
        var sections = clinicalCase.Sections
            .OrderBy(s => s.Name)
            .Select(s => new SessionSectionItem(
                s.Id,
                s.Name,
                s.Categories.OrderBy(c => c.Name)
                    .Select(c => new SessionCategoryItem(
                        c.Id,
                        c.Name,
                        c.Questions.Select(q => new SessionQuestionItem(q.Id, q.Text)).ToList()))
                    .ToList()))
            .ToList();

        return new SessionStartResult(sessionCode, session.Status, startedAt, caseItem, sections);
    }

    public async Task<SessionEventResult> RegisterEventAsync(string sessionCode, Guid questionId, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
        EnsureSessionActive(session);

        var questionInfo = await dbContext.CaseQuestions
            .AsNoTracking()
            .Where(q => q.Id == questionId)
            .Select(q => new
            {
                Question = q,
                CategoryName = q.Category.Name,
                SectionName = q.Category.Section.Name,
                CaseId = q.Category.Section.CaseId,
                AnswerText = q.Answer.Text
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        if (questionInfo.CaseId != session.CaseId)
        {
            throw new AppException("La pregunta no pertenece al caso de la sesión.", 400);
        }

        var occurredAt = DateTimeOffset.UtcNow;
        var interaction = new InteractionEvent
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            OccurredAt = occurredAt,
            SectionName = questionInfo.SectionName,
            CategoryName = questionInfo.CategoryName,
            QuestionText = questionInfo.Question.Text,
            AnswerText = questionInfo.AnswerText
        };

        dbContext.InteractionEvents.Add(interaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SessionEventResult(interaction.Id, occurredAt, interaction.SectionName, interaction.CategoryName, interaction.QuestionText, interaction.AnswerText);
    }

    public async Task<SessionInfoResult> GetSessionAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.ConsultationSessions
            .AsNoTracking()
            .Include(s => s.Case)
            .FirstOrDefaultAsync(s => s.SessionCode == sessionCode, cancellationToken)
            ?? throw new AppException("Sesión no encontrada.", 404);

        return new SessionInfoResult(
            session.SessionCode,
            session.Status,
            session.StartedAt,
            session.FinishedAt,
            new CaseListItem(session.Case.Id, session.Case.FullName, session.Case.Age, session.Case.Sex, session.Case.ChiefComplaint, session.Case.Triage));
    }

    public async Task<ClinicalNoteModel> GetNoteAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);

        var note = await dbContext.ClinicalNotes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.SessionId == session.Id, cancellationToken);

        return note is null
            ? new ClinicalNoteModel(null, null, null, null)
            : new ClinicalNoteModel(note.SummaryText, note.ProbableDiagnosisText, note.ConductStudiesText, note.ConductTreatmentText);
    }

    public async Task<ClinicalNoteModel> UpsertNoteAsync(string sessionCode, ClinicalNoteModel model, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
        EnsureSessionActive(session);

        var note = await dbContext.ClinicalNotes.FirstOrDefaultAsync(n => n.SessionId == session.Id, cancellationToken);
        if (note is null)
        {
            note = new ClinicalNote
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                SummaryText = model.SummaryText ?? string.Empty,
                ProbableDiagnosisText = model.ProbableDiagnosisText ?? string.Empty,
                ConductStudiesText = model.ConductStudiesText ?? string.Empty,
                ConductTreatmentText = model.ConductTreatmentText ?? string.Empty
            };

            dbContext.ClinicalNotes.Add(note);
        }
        else
        {
            note.SummaryText = model.SummaryText ?? string.Empty;
            note.ProbableDiagnosisText = model.ProbableDiagnosisText ?? string.Empty;
            note.ConductStudiesText = model.ConductStudiesText ?? string.Empty;
            note.ConductTreatmentText = model.ConductTreatmentText ?? string.Empty;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ClinicalNoteModel(note.SummaryText, note.ProbableDiagnosisText, note.ConductStudiesText, note.ConductTreatmentText);
    }

    public async Task<IReadOnlyCollection<DifferentialModel>> GetDifferentialsAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);

        return await dbContext.DifferentialDiagnoses
            .AsNoTracking()
            .Where(d => d.SessionId == session.Id)
            .OrderBy(d => d.Rank)
            .Select(d => new DifferentialModel(d.Rank, d.Text))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DifferentialModel>> SaveDifferentialsAsync(string sessionCode, IReadOnlyCollection<DifferentialModel> items, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
        EnsureSessionActive(session);

        ValidateDifferentials(items);

        var existing = await dbContext.DifferentialDiagnoses.Where(d => d.SessionId == session.Id).ToListAsync(cancellationToken);
        dbContext.DifferentialDiagnoses.RemoveRange(existing);

        var entities = items.Select(item => new DifferentialDiagnosis
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            Rank = item.Rank,
            Text = item.Text.Trim()
        }).ToList();

        await dbContext.DifferentialDiagnoses.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entities.OrderBy(x => x.Rank).Select(x => new DifferentialModel(x.Rank, x.Text)).ToList();
    }

    public async Task<FinalizeResult> FinalizeAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionEntityAsync(sessionCode, cancellationToken);

        if (session.Status == FinalizedStatus)
        {
            throw new AppException("La sesión ya está finalizada.", 409);
        }

        var note = await dbContext.ClinicalNotes.FirstOrDefaultAsync(n => n.SessionId == session.Id, cancellationToken);
        var differentials = await dbContext.DifferentialDiagnoses
            .AsNoTracking()
            .Where(d => d.SessionId == session.Id)
            .OrderBy(d => d.Rank)
            .ToListAsync(cancellationToken);

        var errors = new List<string>();
        if (note is null || string.IsNullOrWhiteSpace(note.SummaryText)) errors.Add("El resumen es obligatorio.");
        if (note is null || string.IsNullOrWhiteSpace(note.ProbableDiagnosisText)) errors.Add("El diagnóstico probable es obligatorio.");
        if (note is null || string.IsNullOrWhiteSpace(note.ConductStudiesText)) errors.Add("La conducta de estudios es obligatoria.");
        if (note is null || string.IsNullOrWhiteSpace(note.ConductTreatmentText)) errors.Add("La conducta de tratamiento es obligatoria.");
        if (differentials.Count < 5) errors.Add("Debe ingresar al menos 5 diagnósticos diferenciales.");

        if (errors.Count > 0)
        {
            throw new AppException("No se puede finalizar la sesión.", 400, errors);
        }

        session.Status = FinalizedStatus;
        session.FinishedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new FinalizeResult(session.SessionCode, session.Status, session.FinishedAt.Value);
    }

    private static void ValidateDifferentials(IReadOnlyCollection<DifferentialModel> items)
    {
        if (items is null || items.Count == 0)
        {
            throw new AppException("Debe enviar diagnósticos diferenciales.", 400);
        }

        if (items.Any(x => x.Rank < 1))
        {
            throw new AppException("Los rangos deben ser mayores o iguales a 1.", 400);
        }

        if (items.Any(x => string.IsNullOrWhiteSpace(x.Text)))
        {
            throw new AppException("El texto del diagnóstico diferencial es obligatorio.", 400);
        }

        var distinctRanks = items.Select(x => x.Rank).Distinct().Count();
        if (distinctRanks != items.Count)
        {
            throw new AppException("Los rangos no deben repetirse.", 400);
        }
    }

    private async Task<ConsultationSession> GetSessionEntityAsync(string sessionCode, CancellationToken cancellationToken)
    {
        return await dbContext.ConsultationSessions.FirstOrDefaultAsync(s => s.SessionCode == sessionCode, cancellationToken)
               ?? throw new AppException("Sesión no encontrada.", 404);
    }

    private static void EnsureSessionActive(ConsultationSession session)
    {
        if (session.Status == FinalizedStatus)
        {
            throw new AppException("La sesión ya está finalizada.", 409);
        }
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var code = GenerateCode(RandomNumberGenerator.GetInt32(8, 11));
            var exists = await dbContext.ConsultationSessions.AnyAsync(s => s.SessionCode == code, cancellationToken);
            if (!exists)
            {
                return code;
            }
        }

        throw new AppException("No fue posible generar un código de sesión único.", 500);
    }

    private static string GenerateCode(int length)
    {
        var chars = new char[length];
        Span<byte> buffer = stackalloc byte[length];
        RandomNumberGenerator.Fill(buffer);

        for (var i = 0; i < length; i++)
        {
            chars[i] = Alphabet[buffer[i] % Alphabet.Length];
        }

        return new string(chars);
    }
}
