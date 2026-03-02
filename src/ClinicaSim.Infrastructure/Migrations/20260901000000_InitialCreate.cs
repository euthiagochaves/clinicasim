using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaSim.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "clinical_cases",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                sex = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                age = table.Column<int>(type: "integer", nullable: false),
                chief_complaint = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                triage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_clinical_cases", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "case_sections",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_sections", x => x.id);
                table.ForeignKey(
                    name: "fk_case_sections_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "consultation_sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_consultation_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_consultation_sessions_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "case_categories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                section_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_categories", x => x.id);
                table.ForeignKey(
                    name: "fk_case_categories_case_sections_section_id",
                    column: x => x.section_id,
                    principalTable: "case_sections",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "clinical_notes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                summary_text = table.Column<string>(type: "text", nullable: false),
                probable_diagnosis_text = table.Column<string>(type: "text", nullable: false),
                conduct_studies_text = table.Column<string>(type: "text", nullable: false),
                conduct_treatment_text = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_clinical_notes", x => x.id);
                table.ForeignKey(
                    name: "fk_clinical_notes_consultation_sessions_session_id",
                    column: x => x.session_id,
                    principalTable: "consultation_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "differential_diagnoses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                rank = table.Column<int>(type: "integer", nullable: false),
                text = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_differential_diagnoses", x => x.id);
                table.ForeignKey(
                    name: "fk_differential_diagnoses_consultation_sessions_session_id",
                    column: x => x.session_id,
                    principalTable: "consultation_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "interaction_events",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                section_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                question_text = table.Column<string>(type: "text", nullable: false),
                answer_text = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_interaction_events", x => x.id);
                table.ForeignKey(
                    name: "fk_interaction_events_consultation_sessions_session_id",
                    column: x => x.session_id,
                    principalTable: "consultation_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "case_questions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                category_id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_questions", x => x.id);
                table.ForeignKey(
                    name: "fk_case_questions_case_categories_category_id",
                    column: x => x.category_id,
                    principalTable: "case_categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "case_answers",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                question_id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_answers", x => x.id);
                table.ForeignKey(
                    name: "fk_case_answers_case_questions_question_id",
                    column: x => x.question_id,
                    principalTable: "case_questions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_case_answers_question_id",
            table: "case_answers",
            column: "question_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_case_categories_section_id",
            table: "case_categories",
            column: "section_id");

        migrationBuilder.CreateIndex(
            name: "ix_case_questions_category_id",
            table: "case_questions",
            column: "category_id");

        migrationBuilder.CreateIndex(
            name: "ix_case_sections_case_id",
            table: "case_sections",
            column: "case_id");

        migrationBuilder.CreateIndex(
            name: "ix_clinical_notes_session_id",
            table: "clinical_notes",
            column: "session_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_consultation_sessions_case_id",
            table: "consultation_sessions",
            column: "case_id");

        migrationBuilder.CreateIndex(
            name: "ix_consultation_sessions_session_code",
            table: "consultation_sessions",
            column: "session_code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_differential_diagnoses_session_id",
            table: "differential_diagnoses",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_differential_diagnoses_session_id_rank",
            table: "differential_diagnoses",
            columns: new[] { "session_id", "rank" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_interaction_events_session_id",
            table: "interaction_events",
            column: "session_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "case_answers");
        migrationBuilder.DropTable(name: "clinical_notes");
        migrationBuilder.DropTable(name: "differential_diagnoses");
        migrationBuilder.DropTable(name: "interaction_events");
        migrationBuilder.DropTable(name: "case_questions");
        migrationBuilder.DropTable(name: "consultation_sessions");
        migrationBuilder.DropTable(name: "case_categories");
        migrationBuilder.DropTable(name: "case_sections");
        migrationBuilder.DropTable(name: "clinical_cases");
    }
}
