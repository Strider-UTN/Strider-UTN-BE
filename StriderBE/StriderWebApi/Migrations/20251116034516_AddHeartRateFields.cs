using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddHeartRateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Sessions_SessionId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "Intervals");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Workouts",
                newName: "TrainingSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_SessionId",
                table: "Workouts",
                newName: "IX_Workouts_TrainingSessionId");

            migrationBuilder.AddColumn<double>(
                name: "MaximumHeartRate",
                table: "Users",
                type: "double precision",
                nullable: true,
                defaultValue: 200.0);

            migrationBuilder.AddColumn<double>(
                name: "RestingHeartRate",
                table: "Users",
                type: "double precision",
                nullable: true,
                defaultValue: 60.0);

            migrationBuilder.AddColumn<double>(
                name: "ThresholdHeartRate",
                table: "Users",
                type: "double precision",
                nullable: true,
                defaultValue: 150.0);

            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "TrainingTemplates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingPlanId",
                table: "TrainingSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_CoachId",
                table: "TrainingTemplates",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TrainingPlanId",
                table: "TrainingSessions",
                column: "TrainingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_TrainingPlans_TrainingPlanId",
                table: "TrainingSessions",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingTemplates_Users_CoachId",
                table: "TrainingTemplates",
                column: "CoachId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrainingSessions_TrainingSessionId",
                table: "Workouts",
                column: "TrainingSessionId",
                principalTable: "TrainingSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_TrainingPlans_TrainingPlanId",
                table: "TrainingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingTemplates_Users_CoachId",
                table: "TrainingTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrainingSessions_TrainingSessionId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_TrainingTemplates_CoachId",
                table: "TrainingTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_TrainingPlanId",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "MaximumHeartRate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RestingHeartRate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ThresholdHeartRate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "TrainingTemplates");

            migrationBuilder.DropColumn(
                name: "TrainingPlanId",
                table: "TrainingSessions");

            migrationBuilder.RenameColumn(
                name: "TrainingSessionId",
                table: "Workouts",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_TrainingSessionId",
                table: "Workouts",
                newName: "IX_Workouts_SessionId");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingPlanId = table.Column<int>(type: "integer", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    CoolDown = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SessionType = table.Column<int>(type: "integer", nullable: false),
                    Warmup = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_TrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "TrainingPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Intervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Distance = table.Column<double>(type: "double precision", nullable: true),
                    Duration = table.Column<double>(type: "double precision", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Percentage = table.Column<int>(type: "integer", nullable: true),
                    Speed = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervals_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_SessionId",
                table: "Intervals",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CoachId",
                table: "Sessions",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TrainingPlanId",
                table: "Sessions",
                column: "TrainingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Sessions_SessionId",
                table: "Workouts",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
