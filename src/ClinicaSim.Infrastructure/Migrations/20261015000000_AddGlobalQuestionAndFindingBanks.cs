using System;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaSim.Infrastructure.Migrations;

[DbContext(typeof(ClinicaSimDbContext))]
[Migration("20261015000000_AddGlobalQuestionAndFindingBanks")]
public partial class AddGlobalQuestionAndFindingBanks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "physical_finding_bank",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                system = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                tags = table.Column<string>(type: "text", nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_physical_finding_bank", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "question_bank",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "text", nullable: false),
                section = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                tags = table.Column<string>(type: "text", nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_question_bank", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "case_physical_findings",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                finding_id = table.Column<Guid>(type: "uuid", nullable: false),
                present = table.Column<bool>(type: "boolean", nullable: false),
                detail_text = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_physical_findings", x => x.id);
                table.ForeignKey(
                    name: "fk_case_physical_findings_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_case_physical_findings_physical_finding_bank_finding_id",
                    column: x => x.finding_id,
                    principalTable: "physical_finding_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "case_question_answers",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                case_id = table.Column<Guid>(type: "uuid", nullable: false),
                question_id = table.Column<Guid>(type: "uuid", nullable: false),
                answer_text = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_case_question_answers", x => x.id);
                table.ForeignKey(
                    name: "fk_case_question_answers_clinical_cases_case_id",
                    column: x => x.case_id,
                    principalTable: "clinical_cases",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_case_question_answers_question_bank_question_id",
                    column: x => x.question_id,
                    principalTable: "question_bank",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_case_physical_findings_case_id_finding_id",
            table: "case_physical_findings",
            columns: new[] { "case_id", "finding_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_case_physical_findings_finding_id",
            table: "case_physical_findings",
            column: "finding_id");

        migrationBuilder.CreateIndex(
            name: "ix_case_question_answers_case_id_question_id",
            table: "case_question_answers",
            columns: new[] { "case_id", "question_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_case_question_answers_question_id",
            table: "case_question_answers",
            column: "question_id");

        migrationBuilder.CreateIndex(
            name: "ix_physical_finding_bank_system_name",
            table: "physical_finding_bank",
            columns: new[] { "system", "name" });

        migrationBuilder.CreateIndex(
            name: "ix_question_bank_text",
            table: "question_bank",
            column: "text");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "case_physical_findings");

        migrationBuilder.DropTable(
            name: "case_question_answers");

        migrationBuilder.DropTable(
            name: "physical_finding_bank");

        migrationBuilder.DropTable(
            name: "question_bank");
    }
}
