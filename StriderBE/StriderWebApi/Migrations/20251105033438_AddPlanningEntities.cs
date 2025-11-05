using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MicrocycleId",
                table: "TrainingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanningId",
                table: "TrainingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Plannings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plannings_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartWeek = table.Column<int>(type: "integer", nullable: false),
                    EndWeek = table.Column<int>(type: "integer", nullable: false),
                    Objective = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PlanningId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periods_Plannings_PlanningId",
                        column: x => x.PlanningId,
                        principalTable: "Plannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlanningAthletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanningId = table.Column<int>(type: "integer", nullable: false),
                    AthleteId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningAthletes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanningAthletes_Plannings_PlanningId",
                        column: x => x.PlanningId,
                        principalTable: "Plannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanningAthletes_Users_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mesocycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Objective = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WeeksCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PlanningId = table.Column<int>(type: "integer", nullable: false),
                    PeriodId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesocycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesocycles_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Mesocycles_Plannings_PlanningId",
                        column: x => x.PlanningId,
                        principalTable: "Plannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Microcycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sessions = table.Column<int>(type: "integer", nullable: false),
                    Volume = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Intensity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Focus = table.Column<int>(type: "integer", nullable: true),
                    MesocycleId = table.Column<int>(type: "integer", nullable: false),
                    PeriodId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microcycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Microcycles_Mesocycles_MesocycleId",
                        column: x => x.MesocycleId,
                        principalTable: "Mesocycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Microcycles_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_MicrocycleId",
                table: "TrainingSessions",
                column: "MicrocycleId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_PlanningId",
                table: "TrainingSessions",
                column: "PlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesocycles_EndDate",
                table: "Mesocycles",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Mesocycles_PeriodId",
                table: "Mesocycles",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesocycles_PlanningId",
                table: "Mesocycles",
                column: "PlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesocycles_StartDate",
                table: "Mesocycles",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Mesocycles_Status",
                table: "Mesocycles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Microcycles_EndDate",
                table: "Microcycles",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Microcycles_MesocycleId",
                table: "Microcycles",
                column: "MesocycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Microcycles_PeriodId",
                table: "Microcycles",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Microcycles_StartDate",
                table: "Microcycles",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_PlanningId",
                table: "Periods",
                column: "PlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_Status",
                table: "Periods",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningAthletes_AthleteId",
                table: "PlanningAthletes",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningAthletes_PlanningId",
                table: "PlanningAthletes",
                column: "PlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningAthletes_PlanningId_AthleteId",
                table: "PlanningAthletes",
                columns: new[] { "PlanningId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CoachId",
                table: "Plannings",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_StartDate",
                table: "Plannings",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_Status",
                table: "Plannings",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_Microcycles_MicrocycleId",
                table: "TrainingSessions",
                column: "MicrocycleId",
                principalTable: "Microcycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_Plannings_PlanningId",
                table: "TrainingSessions",
                column: "PlanningId",
                principalTable: "Plannings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_Microcycles_MicrocycleId",
                table: "TrainingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_Plannings_PlanningId",
                table: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "Microcycles");

            migrationBuilder.DropTable(
                name: "PlanningAthletes");

            migrationBuilder.DropTable(
                name: "Mesocycles");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_MicrocycleId",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_PlanningId",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "MicrocycleId",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "PlanningId",
                table: "TrainingSessions");
        }
    }
}
