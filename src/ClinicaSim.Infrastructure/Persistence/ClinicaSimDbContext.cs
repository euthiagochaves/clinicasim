using ClinicaSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaSim.Infrastructure.Persistence;

public class ClinicaSimDbContext(DbContextOptions<ClinicaSimDbContext> options) : DbContext(options)
{
    public DbSet<ClinicalCase> ClinicalCases => Set<ClinicalCase>();
    public DbSet<CaseSection> CaseSections => Set<CaseSection>();
    public DbSet<CaseCategory> CaseCategories => Set<CaseCategory>();
    public DbSet<CaseQuestion> CaseQuestions => Set<CaseQuestion>();
    public DbSet<CaseAnswer> CaseAnswers => Set<CaseAnswer>();
    public DbSet<QuestionBank> QuestionBanks => Set<QuestionBank>();
    public DbSet<PhysicalFindingBank> PhysicalFindingBanks => Set<PhysicalFindingBank>();
    public DbSet<CaseQuestionAnswer> CaseQuestionAnswers => Set<CaseQuestionAnswer>();
    public DbSet<CasePhysicalFinding> CasePhysicalFindings => Set<CasePhysicalFinding>();
    public DbSet<ConsultationSession> ConsultationSessions => Set<ConsultationSession>();
    public DbSet<InteractionEvent> InteractionEvents => Set<InteractionEvent>();
    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();
    public DbSet<DifferentialDiagnosis> DifferentialDiagnoses => Set<DifferentialDiagnosis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClinicalCase>(entity =>
        {
            entity.ToTable("clinical_cases");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(200);
            entity.Property(x => x.Sex).HasColumnName("sex").IsRequired().HasMaxLength(20);
            entity.Property(x => x.Age).HasColumnName("age").IsRequired();
            entity.Property(x => x.ChiefComplaint).HasColumnName("chief_complaint").IsRequired().HasMaxLength(300);
            entity.Property(x => x.Triage).HasColumnName("triage").IsRequired().HasMaxLength(20);
            entity.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasMany(x => x.Sections)
                .WithOne(x => x.Case)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.QuestionAnswers)
                .WithOne(x => x.Case)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.PhysicalFindings)
                .WithOne(x => x.Case)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseSection>(entity =>
        {
            entity.ToTable("case_sections");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CaseId).HasColumnName("case_id");
            entity.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

            entity.HasMany(x => x.Categories)
                .WithOne(x => x.Section)
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseCategory>(entity =>
        {
            entity.ToTable("case_categories");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SectionId).HasColumnName("section_id");
            entity.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

            entity.HasMany(x => x.Questions)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseQuestion>(entity =>
        {
            entity.ToTable("case_questions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CategoryId).HasColumnName("category_id");
            entity.Property(x => x.Text).HasColumnName("text").IsRequired().HasColumnType("text");

            entity.HasOne(x => x.Answer)
                .WithOne(x => x.Question)
                .HasForeignKey<CaseAnswer>(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseAnswer>(entity =>
        {
            entity.ToTable("case_answers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.QuestionId).HasColumnName("question_id");
            entity.Property(x => x.Text).HasColumnName("text").IsRequired().HasColumnType("text");

            entity.HasIndex(x => x.QuestionId).IsUnique();
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.ToTable("question_bank");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Text).HasColumnName("text").IsRequired().HasColumnType("text");
            entity.Property(x => x.Section).HasColumnName("section").IsRequired().HasMaxLength(80);
            entity.Property(x => x.Category).HasColumnName("category").IsRequired().HasMaxLength(120);
            entity.Property(x => x.Tags).HasColumnName("tags").HasColumnType("text");
            entity.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(x => x.Text);
        });

        modelBuilder.Entity<PhysicalFindingBank>(entity =>
        {
            entity.ToTable("physical_finding_bank");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).HasColumnName("name").IsRequired().HasColumnType("text");
            entity.Property(x => x.System).HasColumnName("system").IsRequired().HasMaxLength(80);
            entity.Property(x => x.Tags).HasColumnName("tags").HasColumnType("text");
            entity.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(x => new { x.System, x.Name });
        });

        modelBuilder.Entity<CaseQuestionAnswer>(entity =>
        {
            entity.ToTable("case_question_answers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(x => x.QuestionId).HasColumnName("question_id").IsRequired();
            entity.Property(x => x.AnswerText).HasColumnName("answer_text").IsRequired().HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(x => new { x.CaseId, x.QuestionId }).IsUnique();
        });

        modelBuilder.Entity<CasePhysicalFinding>(entity =>
        {
            entity.ToTable("case_physical_findings");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(x => x.FindingId).HasColumnName("finding_id").IsRequired();
            entity.Property(x => x.Present).HasColumnName("present").IsRequired();
            entity.Property(x => x.DetailText).HasColumnName("detail_text").HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(x => new { x.CaseId, x.FindingId }).IsUnique();
        });

        modelBuilder.Entity<ConsultationSession>(entity =>
        {
            entity.ToTable("consultation_sessions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SessionCode).HasColumnName("session_code").IsRequired().HasMaxLength(10);
            entity.Property(x => x.CaseId).HasColumnName("case_id");
            entity.Property(x => x.StartedAt).HasColumnName("started_at").IsRequired();
            entity.Property(x => x.FinishedAt).HasColumnName("finished_at");
            entity.Property(x => x.Status).HasColumnName("status").IsRequired().HasMaxLength(20);

            entity.HasIndex(x => x.SessionCode).IsUnique();
            entity.HasIndex(x => x.CaseId);

            entity.HasOne(x => x.Case)
                .WithMany()
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Events)
                .WithOne(x => x.Session)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Note)
                .WithOne(x => x.Session)
                .HasForeignKey<ClinicalNote>(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Differentials)
                .WithOne(x => x.Session)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InteractionEvent>(entity =>
        {
            entity.ToTable("interaction_events");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SessionId).HasColumnName("session_id");
            entity.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
            entity.Property(x => x.SectionName).HasColumnName("section_name").IsRequired().HasMaxLength(100);
            entity.Property(x => x.CategoryName).HasColumnName("category_name").IsRequired().HasMaxLength(100);
            entity.Property(x => x.QuestionText).HasColumnName("question_text").IsRequired().HasColumnType("text");
            entity.Property(x => x.AnswerText).HasColumnName("answer_text").IsRequired().HasColumnType("text");

            entity.HasIndex(x => x.SessionId);
        });

        modelBuilder.Entity<ClinicalNote>(entity =>
        {
            entity.ToTable("clinical_notes");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SessionId).HasColumnName("session_id");
            entity.Property(x => x.SummaryText).HasColumnName("summary_text").IsRequired().HasColumnType("text");
            entity.Property(x => x.ProbableDiagnosisText).HasColumnName("probable_diagnosis_text").IsRequired().HasColumnType("text");
            entity.Property(x => x.ConductStudiesText).HasColumnName("conduct_studies_text").IsRequired().HasColumnType("text");
            entity.Property(x => x.ConductTreatmentText).HasColumnName("conduct_treatment_text").IsRequired().HasColumnType("text");

            entity.HasIndex(x => x.SessionId).IsUnique();
        });

        modelBuilder.Entity<DifferentialDiagnosis>(entity =>
        {
            entity.ToTable("differential_diagnoses");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SessionId).HasColumnName("session_id");
            entity.Property(x => x.Rank).HasColumnName("rank").IsRequired();
            entity.Property(x => x.Text).HasColumnName("text").IsRequired().HasColumnType("text");

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => new { x.SessionId, x.Rank }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
