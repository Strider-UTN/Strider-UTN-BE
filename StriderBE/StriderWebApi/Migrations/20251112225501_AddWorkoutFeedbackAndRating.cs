using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutFeedbackAndRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "CompletedWorkouts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkoutFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompletedWorkoutId = table.Column<int>(type: "integer", nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Recommendations = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutFeedbacks_CompletedWorkouts_CompletedWorkoutId",
                        column: x => x.CompletedWorkoutId,
                        principalTable: "CompletedWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutFeedbacks_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LapFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkoutFeedbackId = table.Column<int>(type: "integer", nullable: false),
                    WorkoutLapId = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LapFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LapFeedbacks_WorkoutFeedbacks_WorkoutFeedbackId",
                        column: x => x.WorkoutFeedbackId,
                        principalTable: "WorkoutFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LapFeedbacks_WorkoutLaps_WorkoutLapId",
                        column: x => x.WorkoutLapId,
                        principalTable: "WorkoutLaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LapFeedbacks_WorkoutFeedbackId",
                table: "LapFeedbacks",
                column: "WorkoutFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_LapFeedbacks_WorkoutLapId",
                table: "LapFeedbacks",
                column: "WorkoutLapId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutFeedbacks_CoachId",
                table: "WorkoutFeedbacks",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutFeedbacks_CompletedWorkoutId",
                table: "WorkoutFeedbacks",
                column: "CompletedWorkoutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutFeedbacks_CreatedAt",
                table: "WorkoutFeedbacks",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LapFeedbacks");

            migrationBuilder.DropTable(
                name: "WorkoutFeedbacks");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "CompletedWorkouts");
        }
    }
}
