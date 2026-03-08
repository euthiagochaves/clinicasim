using System;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaSim.Infrastructure.Migrations;

[DbContext(typeof(ClinicaSimDbContext))]
[Migration("20261017000000_AddDefaultAndOverrideInheritance")]
public partial class AddDefaultAndOverrideInheritance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "question_default_answers",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                question_id = table.Column<Guid>(type: "uuid", nullable: false),
                answer_text = table.Column<string>(type: "text", nullable: false),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_question_default_answers", x => x.id);
                table.ForeignKey(
                    name: "FK_question_default_answers_question_bank_question_id",
                    column: x => x.question_id,
                    principalTable: "question_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "physical_finding_defaults",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                finding_id = table.Column<Guid>(type: "uuid", nullable: false),
                present = table.Column<bool>(type: "boolean", nullable: false),
                detail_text = table.Column<string>(type: "text", nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_physical_finding_defaults", x => x.id);
                table.ForeignKey(
                    name: "FK_physical_finding_defaults_physical_finding_bank_finding_id",
                    column: x => x.finding_id,
                    principalTable: "physical_finding_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "case_question_overrides",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                question_id = table.Column<Guid>(type: "uuid", nullable: false),
                answer_text = table.Column<string>(type: "text", nullable: false),
                is_case_specific = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                is_highlighted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_case_question_overrides", x => x.id);
                table.ForeignKey(
                    name: "FK_case_question_overrides_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_case_question_overrides_question_bank_question_id",
                    column: x => x.question_id,
                    principalTable: "question_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "case_physical_finding_overrides",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                finding_id = table.Column<Guid>(type: "uuid", nullable: false),
                present = table.Column<bool>(type: "boolean", nullable: false),
                detail_text = table.Column<string>(type: "text", nullable: true),
                is_case_specific = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                is_highlighted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_case_physical_finding_overrides", x => x.id);
                table.ForeignKey(
                    name: "FK_case_physical_finding_overrides_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_case_physical_finding_overrides_physical_finding_bank_finding_id",
                    column: x => x.finding_id,
                    principalTable: "physical_finding_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_question_default_answers_question_id",
            table: "question_default_answers",
            column: "question_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_physical_finding_defaults_finding_id",
            table: "physical_finding_defaults",
            column: "finding_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_case_question_overrides_case_id_question_id",
            table: "case_question_overrides",
            columns: new[] { "case_id", "question_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_case_question_overrides_question_id",
            table: "case_question_overrides",
            column: "question_id");

        migrationBuilder.CreateIndex(
            name: "IX_case_physical_finding_overrides_case_id_finding_id",
            table: "case_physical_finding_overrides",
            columns: new[] { "case_id", "finding_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_case_physical_finding_overrides_finding_id",
            table: "case_physical_finding_overrides",
            column: "finding_id");

        migrationBuilder.Sql(@"
INSERT INTO question_default_answers (id, question_id, answer_text, active, created_at, updated_at)
SELECT qb.id, qb.id, 'No tengo ese dato.', true, now(), now()
FROM question_bank qb
WHERE NOT EXISTS (SELECT 1 FROM question_default_answers qda WHERE qda.question_id = qb.id);");

        migrationBuilder.Sql(@"
INSERT INTO physical_finding_defaults (id, finding_id, present, detail_text, active, created_at, updated_at)
SELECT pfb.id, pfb.id, false, null, true, now(), now()
FROM physical_finding_bank pfb
WHERE NOT EXISTS (SELECT 1 FROM physical_finding_defaults pfd WHERE pfd.finding_id = pfb.id);");

        migrationBuilder.Sql(@"
INSERT INTO case_question_overrides (id, case_id, question_id, answer_text, is_case_specific, is_highlighted, created_at, updated_at)
SELECT cqa.id, cqa.case_id, cqa.question_id, cqa.answer_text, true, false, cqa.created_at, cqa.updated_at
FROM case_question_answers cqa
ON CONFLICT (case_id, question_id) DO NOTHING;");

        migrationBuilder.Sql(@"
INSERT INTO case_physical_finding_overrides (id, case_id, finding_id, present, detail_text, is_case_specific, is_highlighted, created_at, updated_at)
SELECT cpf.id, cpf.case_id, cpf.finding_id, cpf.present, cpf.detail_text, true, false, cpf.created_at, cpf.updated_at
FROM case_physical_findings cpf
ON CONFLICT (case_id, finding_id) DO NOTHING;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "case_question_overrides");
        migrationBuilder.DropTable(name: "case_physical_finding_overrides");
        migrationBuilder.DropTable(name: "question_default_answers");
        migrationBuilder.DropTable(name: "physical_finding_defaults");
    }
}
