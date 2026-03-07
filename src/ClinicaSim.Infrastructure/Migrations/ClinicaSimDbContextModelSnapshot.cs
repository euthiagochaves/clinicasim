using System;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClinicaSim.Infrastructure.Migrations;

[DbContext(typeof(ClinicaSimDbContext))]
partial class ClinicaSimDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.8")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);


        modelBuilder.Entity("ClinicaSim.Domain.Entities.CasePhysicalFinding", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ClinicalCase", "Case")
                .WithMany("PhysicalFindings")
                .HasForeignKey("CaseId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_physical_findings_clinical_cases_case_id");

            b.HasOne("ClinicaSim.Domain.Entities.PhysicalFindingBank", "Finding")
                .WithMany("CasePhysicalFindings")
                .HasForeignKey("FindingId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_physical_findings_physical_finding_bank_finding_id");

            b.Navigation("Case");

            b.Navigation("Finding");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseQuestionAnswer", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ClinicalCase", "Case")
                .WithMany("QuestionAnswers")
                .HasForeignKey("CaseId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_question_answers_clinical_cases_case_id");

            b.HasOne("ClinicaSim.Domain.Entities.QuestionBank", "Question")
                .WithMany("CaseQuestionAnswers")
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_question_answers_question_bank_question_id");

            b.Navigation("Case");

            b.Navigation("Question");
        });
        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseAnswer", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<Guid>("QuestionId")
                .HasColumnType("uuid")
                .HasColumnName("question_id");

            b.Property<string>("Text")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("text");

            b.HasKey("Id")
                .HasName("pk_case_answers");

            b.HasIndex("QuestionId")
                .IsUnique()
                .HasDatabaseName("ix_case_answers_question_id");

            b.ToTable("case_answers", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseCategory", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)")
                .HasColumnName("name");

            b.Property<Guid>("SectionId")
                .HasColumnType("uuid")
                .HasColumnName("section_id");

            b.HasKey("Id")
                .HasName("pk_case_categories");

            b.HasIndex("SectionId")
                .HasDatabaseName("ix_case_categories_section_id");

            b.ToTable("case_categories", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseQuestion", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<Guid>("CategoryId")
                .HasColumnType("uuid")
                .HasColumnName("category_id");

            b.Property<string>("Text")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("text");

            b.HasKey("Id")
                .HasName("pk_case_questions");

            b.HasIndex("CategoryId")
                .HasDatabaseName("ix_case_questions_category_id");

            b.ToTable("case_questions", (string)null);
        });


        modelBuilder.Entity("ClinicaSim.Domain.Entities.CasePhysicalFinding", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<Guid>("CaseId")
                .HasColumnType("uuid")
                .HasColumnName("case_id");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            b.Property<string>("DetailText")
                .HasColumnType("text")
                .HasColumnName("detail_text");

            b.Property<Guid>("FindingId")
                .HasColumnType("uuid")
                .HasColumnName("finding_id");

            b.Property<bool>("Present")
                .HasColumnType("boolean")
                .HasColumnName("present");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            b.HasKey("Id")
                .HasName("pk_case_physical_findings");

            b.HasIndex("CaseId", "FindingId")
                .IsUnique()
                .HasDatabaseName("ix_case_physical_findings_case_id_finding_id");

            b.HasIndex("FindingId")
                .HasDatabaseName("ix_case_physical_findings_finding_id");

            b.ToTable("case_physical_findings", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseQuestionAnswer", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<string>("AnswerText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("answer_text");

            b.Property<Guid>("CaseId")
                .HasColumnType("uuid")
                .HasColumnName("case_id");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            b.Property<Guid>("QuestionId")
                .HasColumnType("uuid")
                .HasColumnName("question_id");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            b.HasKey("Id")
                .HasName("pk_case_question_answers");

            b.HasIndex("CaseId", "QuestionId")
                .IsUnique()
                .HasDatabaseName("ix_case_question_answers_case_id_question_id");

            b.HasIndex("QuestionId")
                .HasDatabaseName("ix_case_question_answers_question_id");

            b.ToTable("case_question_answers", (string)null);
        });
        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseSection", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<Guid>("CaseId")
                .HasColumnType("uuid")
                .HasColumnName("case_id");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)")
                .HasColumnName("name");

            b.HasKey("Id")
                .HasName("pk_case_sections");

            b.HasIndex("CaseId")
                .HasDatabaseName("ix_case_sections_case_id");

            b.ToTable("case_sections", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ClinicalCase", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<int>("Age")
                .HasColumnType("integer")
                .HasColumnName("age");

            b.Property<string>("ChiefComplaint")
                .IsRequired()
                .HasMaxLength(300)
                .HasColumnType("character varying(300)")
                .HasColumnName("chief_complaint");

            b.Property<string>("FullName")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)")
                .HasColumnName("full_name");

            b.Property<string>("Sex")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)")
                .HasColumnName("sex");

            b.Property<string>("Triage")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)")
                .HasColumnName("triage");

            b.HasKey("Id")
                .HasName("pk_clinical_cases");

            b.ToTable("clinical_cases", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ClinicalNote", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<string>("ConductStudiesText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("conduct_studies_text");

            b.Property<string>("ConductTreatmentText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("conduct_treatment_text");

            b.Property<string>("ProbableDiagnosisText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("probable_diagnosis_text");

            b.Property<Guid>("SessionId")
                .HasColumnType("uuid")
                .HasColumnName("session_id");

            b.Property<string>("SummaryText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("summary_text");

            b.HasKey("Id")
                .HasName("pk_clinical_notes");

            b.HasIndex("SessionId")
                .IsUnique()
                .HasDatabaseName("ix_clinical_notes_session_id");

            b.ToTable("clinical_notes", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ConsultationSession", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<Guid>("CaseId")
                .HasColumnType("uuid")
                .HasColumnName("case_id");

            b.Property<DateTimeOffset?>("FinishedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("finished_at");

            b.Property<string>("SessionCode")
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("character varying(10)")
                .HasColumnName("session_code");

            b.Property<DateTimeOffset>("StartedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("started_at");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)")
                .HasColumnName("status");

            b.HasKey("Id")
                .HasName("pk_consultation_sessions");

            b.HasIndex("CaseId")
                .HasDatabaseName("ix_consultation_sessions_case_id");

            b.HasIndex("SessionCode")
                .IsUnique()
                .HasDatabaseName("ix_consultation_sessions_session_code");

            b.ToTable("consultation_sessions", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.DifferentialDiagnosis", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<int>("Rank")
                .HasColumnType("integer")
                .HasColumnName("rank");

            b.Property<Guid>("SessionId")
                .HasColumnType("uuid")
                .HasColumnName("session_id");

            b.Property<string>("Text")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("text");

            b.HasKey("Id")
                .HasName("pk_differential_diagnoses");

            b.HasIndex("SessionId")
                .HasDatabaseName("ix_differential_diagnoses_session_id");

            b.HasIndex("SessionId", "Rank")
                .IsUnique()
                .HasDatabaseName("ix_differential_diagnoses_session_id_rank");

            b.ToTable("differential_diagnoses", (string)null);
        });


        modelBuilder.Entity("ClinicaSim.Domain.Entities.PhysicalFindingBank", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<bool>("Active")
                .ValueGeneratedOnAdd()
                .HasColumnType("boolean")
                .HasColumnName("active")
                .HasDefaultValue(true);

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("name");

            b.Property<string>("System")
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnType("character varying(80)")
                .HasColumnName("system");

            b.Property<string>("Tags")
                .HasColumnType("text")
                .HasColumnName("tags");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            b.HasKey("Id")
                .HasName("pk_physical_finding_bank");

            b.HasIndex("System", "Name")
                .HasDatabaseName("ix_physical_finding_bank_system_name");

            b.ToTable("physical_finding_bank", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.QuestionBank", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<bool>("Active")
                .ValueGeneratedOnAdd()
                .HasColumnType("boolean")
                .HasColumnName("active")
                .HasDefaultValue(true);

            b.Property<string>("Category")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("character varying(120)")
                .HasColumnName("category");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            b.Property<string>("Section")
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnType("character varying(80)")
                .HasColumnName("section");

            b.Property<string>("Tags")
                .HasColumnType("text")
                .HasColumnName("tags");

            b.Property<string>("Text")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("text");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            b.HasKey("Id")
                .HasName("pk_question_bank");

            b.HasIndex("Text")
                .HasDatabaseName("ix_question_bank_text");

            b.ToTable("question_bank", (string)null);
        });
        modelBuilder.Entity("ClinicaSim.Domain.Entities.InteractionEvent", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid")
                .HasColumnName("id");

            b.Property<string>("AnswerText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("answer_text");

            b.Property<string>("CategoryName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)")
                .HasColumnName("category_name");

            b.Property<DateTimeOffset>("OccurredAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("occurred_at");

            b.Property<string>("QuestionText")
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("question_text");

            b.Property<string>("SectionName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)")
                .HasColumnName("section_name");

            b.Property<Guid>("SessionId")
                .HasColumnType("uuid")
                .HasColumnName("session_id");

            b.HasKey("Id")
                .HasName("pk_interaction_events");

            b.HasIndex("SessionId")
                .HasDatabaseName("ix_interaction_events_session_id");

            b.ToTable("interaction_events", (string)null);
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseAnswer", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.CaseQuestion", "Question")
                .WithOne("Answer")
                .HasForeignKey("ClinicaSim.Domain.Entities.CaseAnswer", "QuestionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_answers_case_questions_question_id");

            b.Navigation("Question");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseCategory", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.CaseSection", "Section")
                .WithMany("Categories")
                .HasForeignKey("SectionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_categories_case_sections_section_id");

            b.Navigation("Section");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseQuestion", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.CaseCategory", "Category")
                .WithMany("Questions")
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_questions_case_categories_category_id");

            b.Navigation("Category");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseSection", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ClinicalCase", "Case")
                .WithMany("Sections")
                .HasForeignKey("CaseId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_case_sections_clinical_cases_case_id");

            b.Navigation("Case");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ClinicalNote", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ConsultationSession", "Session")
                .WithOne("Note")
                .HasForeignKey("ClinicaSim.Domain.Entities.ClinicalNote", "SessionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_clinical_notes_consultation_sessions_session_id");

            b.Navigation("Session");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ConsultationSession", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ClinicalCase", "Case")
                .WithMany()
                .HasForeignKey("CaseId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("fk_consultation_sessions_clinical_cases_case_id");

            b.Navigation("Case");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.DifferentialDiagnosis", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ConsultationSession", "Session")
                .WithMany("Differentials")
                .HasForeignKey("SessionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_differential_diagnoses_consultation_sessions_session_id");

            b.Navigation("Session");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.InteractionEvent", b =>
        {
            b.HasOne("ClinicaSim.Domain.Entities.ConsultationSession", "Session")
                .WithMany("Events")
                .HasForeignKey("SessionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_interaction_events_consultation_sessions_session_id");

            b.Navigation("Session");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseCategory", b =>
        {
            b.Navigation("Questions");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseQuestion", b =>
        {
            b.Navigation("Answer");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.CaseSection", b =>
        {
            b.Navigation("Categories");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ClinicalCase", b =>
        {
            b.Navigation("PhysicalFindings");

            b.Navigation("QuestionAnswers");

            b.Navigation("Sections");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.PhysicalFindingBank", b =>
        {
            b.Navigation("CasePhysicalFindings");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.QuestionBank", b =>
        {
            b.Navigation("CaseQuestionAnswers");
        });

        modelBuilder.Entity("ClinicaSim.Domain.Entities.ConsultationSession", b =>
        {
            b.Navigation("Differentials");

            b.Navigation("Events");

            b.Navigation("Note");
        });
#pragma warning restore 612, 618
    }
}
