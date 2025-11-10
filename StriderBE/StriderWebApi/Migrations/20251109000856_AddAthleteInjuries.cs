using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteInjuries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ailments");

            migrationBuilder.CreateTable(
                name: "AthleteInjuries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AthleteId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AffectedArea = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DiagnosisDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecoveryEstimateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecoveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteInjuries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteInjuries_Users_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteInjuries_AthleteId",
                table: "AthleteInjuries",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteInjuries_AthleteId_Status",
                table: "AthleteInjuries",
                columns: new[] { "AthleteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteInjuries_DiagnosisDate",
                table: "AthleteInjuries",
                column: "DiagnosisDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteInjuries");

            migrationBuilder.CreateTable(
                name: "Ailments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AthleteId = table.Column<int>(type: "integer", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "text", nullable: false),
                    AilmentType = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsRecovered = table.Column<bool>(type: "boolean", nullable: false),
                    RecoveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Treatment = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Injury_Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ailments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ailments_Users_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ailments_AthleteId",
                table: "Ailments",
                column: "AthleteId");
        }
    }
}
