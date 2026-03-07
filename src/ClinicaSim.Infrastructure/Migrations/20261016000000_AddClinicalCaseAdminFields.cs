using System;
using ClinicaSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaSim.Infrastructure.Migrations;

[DbContext(typeof(ClinicaSimDbContext))]
[Migration("20261016000000_AddClinicalCaseAdminFields")]
public partial class AddClinicalCaseAdminFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "active",
            table: "clinical_cases",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "created_at",
            table: "clinical_cases",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "updated_at",
            table: "clinical_cases",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "active",
            table: "clinical_cases");

        migrationBuilder.DropColumn(
            name: "created_at",
            table: "clinical_cases");

        migrationBuilder.DropColumn(
            name: "updated_at",
            table: "clinical_cases");
    }
}
