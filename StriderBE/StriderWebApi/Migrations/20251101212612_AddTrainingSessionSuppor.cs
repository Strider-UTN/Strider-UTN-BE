using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingSessionSuppor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrainingTemplateId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TrainingSessionId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    TemplateId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_TrainingTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TrainingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessionAthletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingSessionId = table.Column<int>(type: "integer", nullable: false),
                    AthleteId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDistance = table.Column<double>(type: "double precision", nullable: true),
                    ActualDuration = table.Column<int>(type: "integer", nullable: true),
                    ActualAvgPace = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ActualAvgHR = table.Column<int>(type: "integer", nullable: true),
                    ActualMaxHR = table.Column<int>(type: "integer", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessionAthletes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessionAthletes_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingSessionAthletes_Users_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSessionId",
                table: "TrainingIntervals",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSessionId_OrderIndex",
                table: "TrainingIntervals",
                columns: new[] { "TrainingSessionId", "OrderIndex" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingInterval_SingleParent",
                table: "TrainingIntervals",
                sql: "((\"TrainingTemplateId\" IS NULL AND \"TrainingSessionId\" IS NOT NULL) OR (\"TrainingTemplateId\" IS NOT NULL AND \"TrainingSessionId\" IS NULL))");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_AthleteId",
                table: "TrainingSessionAthletes",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_AthleteId_Status",
                table: "TrainingSessionAthletes",
                columns: new[] { "AthleteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_CompletedAt",
                table: "TrainingSessionAthletes",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_Status",
                table: "TrainingSessionAthletes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_TrainingSessionId_AthleteId",
                table: "TrainingSessionAthletes",
                columns: new[] { "TrainingSessionId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessionAthletes_TrainingSessionId_Status",
                table: "TrainingSessionAthletes",
                columns: new[] { "TrainingSessionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_Category",
                table: "TrainingSessions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CreatedAt",
                table: "TrainingSessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CreatedByUserId",
                table: "TrainingSessions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CreatedByUserId_Date",
                table: "TrainingSessions",
                columns: new[] { "CreatedByUserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_Date",
                table: "TrainingSessions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TemplateId",
                table: "TrainingSessions",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingIntervals_TrainingSessions_TrainingSessionId",
                table: "TrainingIntervals",
                column: "TrainingSessionId",
                principalTable: "TrainingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingIntervals_TrainingSessions_TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.DropTable(
                name: "TrainingSessionAthletes");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSessionId_OrderIndex",
                table: "TrainingIntervals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingInterval_SingleParent",
                table: "TrainingIntervals");

            migrationBuilder.DropColumn(
                name: "TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.AlterColumn<int>(
                name: "TrainingTemplateId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
