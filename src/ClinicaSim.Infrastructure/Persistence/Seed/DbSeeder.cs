using ClinicaSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedQuestionBankAsync(dbContext, cancellationToken);
        await SeedFindingBankAsync(dbContext, cancellationToken);

        if (!await dbContext.ClinicalCases.AnyAsync(cancellationToken))
        {
            var definitions = SeedCaseDefinitions.All;
            var clinicalCases = definitions.Select(BuildCase).ToList();

            await dbContext.ClinicalCases.AddRangeAsync(clinicalCases, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await SeedQuestionDefaultsAsync(dbContext, cancellationToken);
        await SeedFindingDefaultsAsync(dbContext, cancellationToken);
        await SeedCaseMappingsAsync(dbContext, cancellationToken);
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

    private static async Task SeedCaseMappingsAsync(ClinicaSimDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.CaseQuestionOverrides.AnyAsync(cancellationToken) || await dbContext.CasePhysicalFindingOverrides.AnyAsync(cancellationToken))
        {
            return;
        }

        var firstCase = await dbContext.ClinicalCases
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new { x.Id, x.FullName })
            .FirstOrDefaultAsync(cancellationToken);

        if (firstCase is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var questionByText = await dbContext.QuestionBanks
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Text, x => x.Id, cancellationToken);

        var findingByName = await dbContext.PhysicalFindingBanks
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id, cancellationToken);

        var caseAnswers = new List<CaseQuestionOverride>();
        if (questionByText.TryGetValue("¿Tiene fiebre?", out var feverId))
        {
            caseAnswers.Add(new CaseQuestionOverride
            {
                Id = Guid.NewGuid(),
                CaseId = firstCase.Id,
                QuestionId = feverId,
                AnswerText = "No, no he tenido fiebre.",
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        if (questionByText.TryGetValue("¿Está tomando alguna medicación?", out var medicationId))
        {
            caseAnswers.Add(new CaseQuestionOverride
            {
                Id = Guid.NewGuid(),
                CaseId = firstCase.Id,
                QuestionId = medicationId,
                AnswerText = "Sí, losartán 50 mg diarios.",
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        var caseFindings = new List<CasePhysicalFindingOverride>();
        if (findingByName.TryGetValue("Ruidos cardíacos normales", out var cardiacId))
        {
            caseFindings.Add(new CasePhysicalFindingOverride
            {
                Id = Guid.NewGuid(),
                CaseId = firstCase.Id,
                FindingId = cardiacId,
                Present = true,
                DetailText = "Ritmo regular sin soplos.",
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        if (findingByName.TryGetValue("Sibilancias", out var wheezingId))
        {
            caseFindings.Add(new CasePhysicalFindingOverride
            {
                Id = Guid.NewGuid(),
                CaseId = firstCase.Id,
                FindingId = wheezingId,
                Present = false,
                DetailText = null,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        if (caseAnswers.Count > 0)
        {
            await dbContext.CaseQuestionOverrides.AddRangeAsync(caseAnswers, cancellationToken);
        }

        if (caseFindings.Count > 0)
        {
            await dbContext.CasePhysicalFindingOverrides.AddRangeAsync(caseFindings, cancellationToken);
        }

        if (caseAnswers.Count > 0 || caseFindings.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static ClinicalCase BuildCase(SeedCaseDefinition definition)
    {
        var now = DateTimeOffset.UtcNow;
        var clinicalCase = new ClinicalCase
        {
            Id = Guid.NewGuid(),
            FullName = definition.FullName,
            Sex = definition.Sex,
            Age = definition.Age,
            ChiefComplaint = definition.ChiefComplaint,
            Triage = definition.Triage,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        foreach (var sectionDefinition in definition.Sections)
        {
            var section = new CaseSection
            {
                Id = Guid.NewGuid(),
                CaseId = clinicalCase.Id,
                Name = sectionDefinition.Name
            };

            foreach (var categoryDefinition in sectionDefinition.Categories)
            {
                var category = new CaseCategory
                {
                    Id = Guid.NewGuid(),
                    SectionId = section.Id,
                    Name = categoryDefinition.Name
                };

                foreach (var questionDefinition in categoryDefinition.Questions)
                {
                    var question = new CaseQuestion
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = category.Id,
                        Text = questionDefinition.Question
                    };

                    question.Answer = new CaseAnswer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
                        Text = questionDefinition.Answer
                    };

                    category.Questions.Add(question);
                }

                section.Categories.Add(category);
            }

            clinicalCase.Sections.Add(section);
        }

        return clinicalCase;
    }
}

internal static class SeedCaseDefinitions
{
    public static IReadOnlyCollection<SeedCaseDefinition> All =>
    [
        new(
            FullName: "Paciente Ficticio 001",
            Sex: "Femenino",
            Age: 52,
            ChiefComplaint: "Dolor torácico opresivo de inicio súbito.",
            Triage: "Amarillo",
            Sections:
            [
                new SeedSectionDefinition(
                    "Anamnesis",
                    [
                        new SeedCategoryDefinition(
                            "Motivo de consulta",
                            [
                                new("¿Qué la trae hoy al servicio?", "Desde hace una hora tengo dolor en el pecho."),
                                new("¿Cómo describiría el dolor?", "Es una presión fuerte en el centro del pecho."),
                                new("¿Desde cuándo inició?", "Comenzó mientras subía escaleras esta mañana.")
                            ]),
                        new SeedCategoryDefinition(
                            "Historia de la enfermedad actual",
                            [
                                new("¿El dolor se irradia a otra zona?", "Sí, hacia el brazo izquierdo y la mandíbula."),
                                new("¿Qué intensidad tiene de 0 a 10?", "Aproximadamente 8 sobre 10."),
                                new("¿Mejora con reposo?", "Disminuye un poco cuando me siento."),
                                new("¿Se acompaña de otros síntomas?", "Sí, sudoración y náuseas leves.")
                            ]),
                        new SeedCategoryDefinition(
                            "Antecedentes",
                            [
                                new("¿Tiene hipertensión o diabetes?", "Tengo hipertensión desde hace cinco años."),
                                new("¿Antecedentes cardiovasculares familiares?", "Mi padre tuvo infarto a los 60 años."),
                                new("¿Ha tenido episodios similares?", "Hace meses sentí molestias leves, pero no tan intensas.")
                            ]),
                        new SeedCategoryDefinition(
                            "Medicaciones y alergias",
                            [
                                new("¿Qué medicamentos usa actualmente?", "Losartán 50 mg diarios."),
                                new("¿Olvida dosis con frecuencia?", "A veces olvido una o dos dosis por semana."),
                                new("¿Es alérgica a algún medicamento?", "No conozco alergias medicamentosas.")
                            ]),
                        new SeedCategoryDefinition(
                            "Hábitos",
                            [
                                new("¿Fuma actualmente?", "Sí, alrededor de 8 cigarrillos al día."),
                                new("¿Consume alcohol?", "Solo fines de semana, en poca cantidad."),
                                new("¿Realiza actividad física?", "Muy poca, trabajo sentada la mayor parte del día.")
                            ])
                    ]),
                new SeedSectionDefinition(
                    "Examen Físico",
                    [
                        new SeedCategoryDefinition(
                            "General",
                            [
                                new("¿Cómo se observa el estado general?", "Paciente ansiosa, pálida y sudorosa."),
                                new("¿Está orientada?", "Sí, orientada en tiempo, espacio y persona."),
                                new("¿Signos de dificultad respiratoria?", "Leve taquipnea sin uso de musculatura accesoria.")
                            ]),
                        new SeedCategoryDefinition(
                            "Cardiovascular",
                            [
                                new("¿Frecuencia cardíaca?", "98 latidos por minuto, ritmo regular."),
                                new("¿Presión arterial?", "150/95 mmHg."),
                                new("¿Ruidos cardíacos?", "Ruidos normofonéticos, sin soplos evidentes."),
                                new("¿Perfusión periférica?", "Llenado capilar en 3 segundos.")
                            ]),
                        new SeedCategoryDefinition(
                            "Respiratorio",
                            [
                                new("¿Frecuencia respiratoria?", "22 respiraciones por minuto."),
                                new("¿Auscultación pulmonar?", "Murmullo vesicular conservado, sin sibilancias."),
                                new("¿Saturación de oxígeno?", "95% al aire ambiente.")
                            ]),
                        new SeedCategoryDefinition(
                            "Abdomen",
                            [
                                new("¿Inspección abdominal?", "Abdomen plano, sin cicatrices relevantes."),
                                new("¿Dolor a la palpación?", "Sin dolor abdominal significativo."),
                                new("¿Ruidos intestinales?", "Presentes y normoactivos.")
                            ])
                    ])
            ]),
        new(
            FullName: "Paciente Ficticio 002",
            Sex: "Masculino",
            Age: 28,
            ChiefComplaint: "Dolor abdominal en fosa iliaca derecha.",
            Triage: "Verde",
            Sections:
            [
                new SeedSectionDefinition(
                    "Anamnesis",
                    [
                        new SeedCategoryDefinition(
                            "Motivo de consulta",
                            [
                                new("¿Cuál es la principal molestia?", "Dolor abdominal desde ayer en la parte baja derecha."),
                                new("¿Qué intensidad tiene el dolor?", "Es 6 de 10 y aumenta al caminar."),
                                new("¿El dolor es continuo o intermitente?", "Es continuo con picos más fuertes.")
                            ]),
                        new SeedCategoryDefinition(
                            "Historia de la enfermedad actual",
                            [
                                new("¿Dónde empezó el dolor?", "Inició alrededor del ombligo y luego bajó."),
                                new("¿Tiene náuseas o vómitos?", "Tengo náuseas sin vómitos."),
                                new("¿Tiene fiebre?", "Ayer me medí 37.8°C en casa."),
                                new("¿Ha tenido diarrea o estreñimiento?", "No, el tránsito está normal.")
                            ]),
                        new SeedCategoryDefinition(
                            "Antecedentes",
                            [
                                new("¿Cirugías previas abdominales?", "No, nunca me operaron."),
                                new("¿Enfermedades crónicas?", "No tengo enfermedades conocidas."),
                                new("¿Antecedentes familiares digestivos?", "Mi madre tiene gastritis crónica.")
                            ]),
                        new SeedCategoryDefinition(
                            "Medicaciones y alergias",
                            [
                                new("¿Toma medicamentos de forma habitual?", "No uso medicación diaria."),
                                new("¿Tomó algo para el dolor?", "Tomé paracetamol hace 6 horas."),
                                new("¿Alergias medicamentosas?", "No conozco alergias.")
                            ]),
                        new SeedCategoryDefinition(
                            "Revisión por sistemas",
                            [
                                new("¿Síntomas urinarios?", "No ardor ni cambios en la orina."),
                                new("¿Síntomas respiratorios?", "No tos ni falta de aire."),
                                new("¿Pérdida de apetito?", "Sí, hoy casi no he comido.")
                            ])
                    ]),
                new SeedSectionDefinition(
                    "Examen Físico",
                    [
                        new SeedCategoryDefinition(
                            "General",
                            [
                                new("¿Estado general?", "Paciente en regular estado general por dolor."),
                                new("¿Nivel de conciencia?", "Consciente y cooperador."),
                                new("¿Temperatura registrada?", "37.7°C en triage.")
                            ]),
                        new SeedCategoryDefinition(
                            "Abdomen",
                            [
                                new("¿Inspección?", "Abdomen levemente distendido."),
                                new("¿Palpación en fosa iliaca derecha?", "Dolor localizado con defensa leve."),
                                new("¿Signo de rebote?", "Positivo discreto."),
                                new("¿Ruidos hidroaéreos?", "Conservados.")
                            ]),
                        new SeedCategoryDefinition(
                            "Cardiovascular",
                            [
                                new("¿Frecuencia cardíaca?", "92 latidos por minuto."),
                                new("¿Presión arterial?", "122/78 mmHg."),
                                new("¿Perfusión periférica?", "Adecuada, llenado capilar menor a 2 segundos.")
                            ]),
                        new SeedCategoryDefinition(
                            "Respiratorio",
                            [
                                new("¿Frecuencia respiratoria?", "18 respiraciones por minuto."),
                                new("¿Auscultación pulmonar?", "Sin ruidos agregados."),
                                new("¿Saturación?", "98% al aire ambiente.")
                            ])
                    ])
            ]),
        new(
            FullName: "Paciente Ficticio 003",
            Sex: "Femenino",
            Age: 41,
            ChiefComplaint: "Fiebre y tos productiva de tres días.",
            Triage: "Amarillo",
            Sections:
            [
                new SeedSectionDefinition(
                    "Anamnesis",
                    [
                        new SeedCategoryDefinition(
                            "Motivo de consulta",
                            [
                                new("¿Qué síntomas presenta?", "Fiebre, tos con flema y cansancio."),
                                new("¿Desde cuándo?", "Desde hace tres días."),
                                new("¿Cómo evolucionaron los síntomas?", "Han ido empeorando, sobre todo por la noche.")
                            ]),
                        new SeedCategoryDefinition(
                            "Historia de la enfermedad actual",
                            [
                                new("¿Color de la expectoración?", "Amarillenta."),
                                new("¿Tiene dolor torácico al respirar?", "Sí, dolor leve al toser."),
                                new("¿Presenta disnea?", "Me falta el aire al subir escaleras."),
                                new("¿Temperatura máxima registrada?", "Hasta 38.9°C ayer por la tarde.")
                            ]),
                        new SeedCategoryDefinition(
                            "Antecedentes",
                            [
                                new("¿Asma o EPOC?", "Tuve asma en la infancia, sin crisis recientes."),
                                new("¿Hospitalizaciones previas por pulmón?", "No, ninguna."),
                                new("¿Vacunación reciente?", "No me vacuné este año contra influenza.")
                            ]),
                        new SeedCategoryDefinition(
                            "Medicaciones y alergias",
                            [
                                new("¿Qué tomó para la fiebre?", "Ibuprofeno cada 8 horas desde ayer."),
                                new("¿Uso de antibióticos recientes?", "No tomé antibióticos."),
                                new("¿Alergias conocidas?", "Alergia leve a penicilina.")
                            ]),
                        new SeedCategoryDefinition(
                            "Hábitos",
                            [
                                new("¿Fuma?", "No fumo."),
                                new("¿Exposición a humo o químicos?", "Trabajo en cocina con humo ocasional."),
                                new("¿Contacto con personas enfermas?", "Mi hijo tuvo gripe la semana pasada.")
                            ])
                    ]),
                new SeedSectionDefinition(
                    "Examen Físico",
                    [
                        new SeedCategoryDefinition(
                            "General",
                            [
                                new("¿Estado general?", "Paciente febril, algo decaída, orientada."),
                                new("¿Temperatura actual?", "38.4°C."),
                                new("¿Frecuencia cardíaca?", "104 latidos por minuto.")
                            ]),
                        new SeedCategoryDefinition(
                            "Respiratorio",
                            [
                                new("¿Frecuencia respiratoria?", "24 respiraciones por minuto."),
                                new("¿Auscultación?", "Crepitantes en base pulmonar derecha."),
                                new("¿Saturación de oxígeno?", "93% al aire ambiente."),
                                new("¿Uso de musculatura accesoria?", "Leve uso durante inspiración profunda.")
                            ]),
                        new SeedCategoryDefinition(
                            "Cardiovascular",
                            [
                                new("¿Presión arterial?", "118/74 mmHg."),
                                new("¿Ritmo cardíaco?", "Ritmo regular, taquicardia sinusal."),
                                new("¿Edema periférico?", "No se observa edema.")
                            ]),
                        new SeedCategoryDefinition(
                            "Neurológico",
                            [
                                new("¿Nivel de conciencia?", "Alerta y orientada."),
                                new("¿Signos meníngeos?", "No presenta rigidez de nuca."),
                                new("¿Déficit focal?", "Sin déficit motor ni sensitivo.")
                            ])
                    ])
            ])
    ];
}

internal sealed record SeedCaseDefinition(
    string FullName,
    string Sex,
    int Age,
    string ChiefComplaint,
    string Triage,
    IReadOnlyCollection<SeedSectionDefinition> Sections);

internal sealed record SeedSectionDefinition(string Name, IReadOnlyCollection<SeedCategoryDefinition> Categories);

internal sealed record SeedCategoryDefinition(string Name, IReadOnlyCollection<SeedQuestionDefinition> Questions);

internal sealed record SeedQuestionDefinition(string Question, string Answer);
