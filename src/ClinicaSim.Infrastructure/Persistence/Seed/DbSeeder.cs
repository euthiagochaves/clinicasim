using ClinicaSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedQuestionBankAsync(dbContext, cancellationToken);
        await SeedFindingBankAsync(dbContext, cancellationToken);
        await SeedQuestionDefaultsAsync(dbContext, cancellationToken);
        await SeedFindingDefaultsAsync(dbContext, cancellationToken);
    }

    private static async Task SeedQuestionBankAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.QuestionBanks.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var questions = new[]
        {
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Dónde le duele?", Section = "ANAMNESIS", Category = "Dolor", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Desde cuándo comenzó?", Section = "ANAMNESIS", Category = "Tiempo de evolución", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Tiene fiebre?", Section = "ANAMNESIS", Category = "Fiebre", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Tiene náuseas?", Section = "ANAMNESIS", Category = "Síntomas asociados", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Está tomando alguna medicación?", Section = "ANAMNESIS", Category = "Medicaciones", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿Tiene alergias?", Section = "ANAMNESIS", Category = "Alergias", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿El dolor irradia?", Section = "ANAMNESIS", Category = "Dolor", Active = true, CreatedAt = now, UpdatedAt = now },
            new QuestionBank { Id = Guid.NewGuid(), Text = "¿La tos es seca o con flema?", Section = "ANAMNESIS", Category = "Respiratorio", Active = true, CreatedAt = now, UpdatedAt = now }
        };

        await dbContext.QuestionBanks.AddRangeAsync(questions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedFindingBankAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.PhysicalFindingBanks.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var findings = new[]
        {
            new PhysicalFindingBank { Id = Guid.NewGuid(), Name = "Murmullo vesicular disminuido", System = "RESPIRATORIO", Active = true, CreatedAt = now, UpdatedAt = now },
            new PhysicalFindingBank { Id = Guid.NewGuid(), Name = "Sibilancias", System = "RESPIRATORIO", Active = true, CreatedAt = now, UpdatedAt = now },
            new PhysicalFindingBank { Id = Guid.NewGuid(), Name = "Ruidos cardíacos normales", System = "CARDIOVASCULAR", Active = true, CreatedAt = now, UpdatedAt = now },
            new PhysicalFindingBank { Id = Guid.NewGuid(), Name = "Abdomen doloroso a la palpación", System = "ABDOMEN", Active = true, CreatedAt = now, UpdatedAt = now },
            new PhysicalFindingBank { Id = Guid.NewGuid(), Name = "Rigidez de nuca", System = "NEUROLOGICO", Active = true, CreatedAt = now, UpdatedAt = now }
        };

        await dbContext.PhysicalFindingBanks.AddRangeAsync(findings, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedQuestionDefaultsAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.QuestionDefaultAnswers.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var defaults = await dbContext.QuestionBanks
            .AsNoTracking()
            .Select(x => new QuestionDefaultAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = x.Id,
                AnswerText = "No tengo ese dato.",
                Active = true,
                CreatedAt = now,
                UpdatedAt = now
            })
            .ToListAsync(cancellationToken);

        await dbContext.QuestionDefaultAnswers.AddRangeAsync(defaults, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedFindingDefaultsAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.PhysicalFindingDefaults.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var defaults = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .Select(x => new PhysicalFindingDefault
            {
                Id = Guid.NewGuid(),
                FindingId = x.Id,
                Present = false,
                DetailText = null,
                Active = true,
                CreatedAt = now,
                UpdatedAt = now
            })
            .ToListAsync(cancellationToken);

        await dbContext.PhysicalFindingDefaults.AddRangeAsync(defaults, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
