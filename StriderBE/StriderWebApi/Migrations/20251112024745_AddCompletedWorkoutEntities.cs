using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedWorkoutEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompletedWorkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<double>(type: "double precision", nullable: false),
                    AverageHR = table.Column<double>(type: "double precision", nullable: false),
                    Comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TrainingSessionAthleteId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompletedWorkouts_TrainingSessionAthletes_TrainingSessionAt~",
                        column: x => x.TrainingSessionAthleteId,
                        principalTable: "TrainingSessionAthletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutInjuries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BodyPart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AffectedPerformance = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletedWorkoutId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutInjuries", x => x.Id);
                    table.CheckConstraint("CK_WorkoutInjury_Severity", "\"Severity\" >= 1 AND \"Severity\" <= 10");
                    table.ForeignKey(
                        name: "FK_WorkoutInjuries_CompletedWorkouts_CompletedWorkoutId",
                        column: x => x.CompletedWorkoutId,
                        principalTable: "CompletedWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutLaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    Duration = table.Column<double>(type: "double precision", nullable: false),
                    AverageHR = table.Column<double>(type: "double precision", nullable: false),
                    Speed = table.Column<double>(type: "double precision", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedWorkoutId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutLaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutLaps_CompletedWorkouts_CompletedWorkoutId",
                        column: x => x.CompletedWorkoutId,
                        principalTable: "CompletedWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSensations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Effort = table.Column<int>(type: "integer", nullable: false),
                    Fatigue = table.Column<int>(type: "integer", nullable: false),
                    Motivation = table.Column<int>(type: "integer", nullable: false),
                    MuscularLoad = table.Column<int>(type: "integer", nullable: false),
                    OverallFeeling = table.Column<int>(type: "integer", nullable: false),
                    CompletedWorkoutId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSensations", x => x.Id);
                    table.CheckConstraint("CK_WorkoutSensations_Effort", "\"Effort\" >= 1 AND \"Effort\" <= 10");
                    table.CheckConstraint("CK_WorkoutSensations_Fatigue", "\"Fatigue\" >= 1 AND \"Fatigue\" <= 10");
                    table.CheckConstraint("CK_WorkoutSensations_Motivation", "\"Motivation\" >= 1 AND \"Motivation\" <= 10");
                    table.CheckConstraint("CK_WorkoutSensations_MuscularLoad", "\"MuscularLoad\" >= 1 AND \"MuscularLoad\" <= 10");
                    table.CheckConstraint("CK_WorkoutSensations_OverallFeeling", "\"OverallFeeling\" >= 1 AND \"OverallFeeling\" <= 10");
                    table.ForeignKey(
                        name: "FK_WorkoutSensations_CompletedWorkouts_CompletedWorkoutId",
                        column: x => x.CompletedWorkoutId,
                        principalTable: "CompletedWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedWorkouts_Date",
                table: "CompletedWorkouts",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedWorkouts_TrainingSessionAthleteId",
                table: "CompletedWorkouts",
                column: "TrainingSessionAthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedWorkouts_TrainingSessionAthleteId_Date",
                table: "CompletedWorkouts",
                columns: new[] { "TrainingSessionAthleteId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutInjuries_CompletedWorkoutId",
                table: "WorkoutInjuries",
                column: "CompletedWorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLaps_CompletedWorkoutId",
                table: "WorkoutLaps",
                column: "CompletedWorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLaps_CompletedWorkoutId_Index",
                table: "WorkoutLaps",
                columns: new[] { "CompletedWorkoutId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSensations_CompletedWorkoutId",
                table: "WorkoutSensations",
                column: "CompletedWorkoutId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutInjuries");

            migrationBuilder.DropTable(
                name: "WorkoutLaps");

            migrationBuilder.DropTable(
                name: "WorkoutSensations");

            migrationBuilder.DropTable(
                name: "CompletedWorkouts");
        }
    }
}
