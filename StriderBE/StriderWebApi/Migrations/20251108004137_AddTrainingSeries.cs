using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingIntervals_TrainingSessions_TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingIntervals_TrainingTemplates_TrainingTemplateId",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_CreatedAt",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_CreatedByUserId_Date",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSessionId_OrderIndex",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId_OrderIndex",
                table: "TrainingIntervals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingInterval_SingleParent",
                table: "TrainingIntervals");

            migrationBuilder.DropColumn(
                name: "TrainingSessionId",
                table: "TrainingIntervals");

            migrationBuilder.DropColumn(
                name: "TrainingTemplateId",
                table: "TrainingIntervals");

            migrationBuilder.AddColumn<int>(
                name: "TrainingSeriesId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TrainingSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingSessionId = table.Column<int>(type: "integer", nullable: true),
                    TrainingTemplateId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    RecoveryBetweenSets = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "00:00"),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSeries", x => x.Id);
                    table.CheckConstraint("CK_TrainingSeries_SingleParent", "((\"TrainingSessionId\" IS NULL AND \"TrainingTemplateId\" IS NOT NULL) OR (\"TrainingSessionId\" IS NOT NULL AND \"TrainingTemplateId\" IS NULL))");
                    table.ForeignKey(
                        name: "FK_TrainingSeries_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingSeries_TrainingTemplates_TrainingTemplateId",
                        column: x => x.TrainingTemplateId,
                        principalTable: "TrainingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSeriesId",
                table: "TrainingIntervals",
                column: "TrainingSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSeriesId_OrderIndex",
                table: "TrainingIntervals",
                columns: new[] { "TrainingSeriesId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSeries_TrainingSessionId",
                table: "TrainingSeries",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSeries_TrainingSessionId_OrderIndex",
                table: "TrainingSeries",
                columns: new[] { "TrainingSessionId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSeries_TrainingTemplateId",
                table: "TrainingSeries",
                column: "TrainingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSeries_TrainingTemplateId_OrderIndex",
                table: "TrainingSeries",
                columns: new[] { "TrainingTemplateId", "OrderIndex" });

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingIntervals_TrainingSeries_TrainingSeriesId",
                table: "TrainingIntervals",
                column: "TrainingSeriesId",
                principalTable: "TrainingSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingIntervals_TrainingSeries_TrainingSeriesId",
                table: "TrainingIntervals");

            migrationBuilder.DropTable(
                name: "TrainingSeries");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSeriesId",
                table: "TrainingIntervals");

            migrationBuilder.DropIndex(
                name: "IX_TrainingIntervals_TrainingSeriesId_OrderIndex",
                table: "TrainingIntervals");

            migrationBuilder.DropColumn(
                name: "TrainingSeriesId",
                table: "TrainingIntervals");

            migrationBuilder.AddColumn<int>(
                name: "TrainingSessionId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingTemplateId",
                table: "TrainingIntervals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CreatedAt",
                table: "TrainingSessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CreatedByUserId_Date",
                table: "TrainingSessions",
                columns: new[] { "CreatedByUserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSessionId",
                table: "TrainingIntervals",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingSessionId_OrderIndex",
                table: "TrainingIntervals",
                columns: new[] { "TrainingSessionId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId",
                table: "TrainingIntervals",
                column: "TrainingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId_OrderIndex",
                table: "TrainingIntervals",
                columns: new[] { "TrainingTemplateId", "OrderIndex" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingInterval_SingleParent",
                table: "TrainingIntervals",
                sql: "((\"TrainingTemplateId\" IS NULL AND \"TrainingSessionId\" IS NOT NULL) OR (\"TrainingTemplateId\" IS NOT NULL AND \"TrainingSessionId\" IS NULL))");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingIntervals_TrainingSessions_TrainingSessionId",
                table: "TrainingIntervals",
                column: "TrainingSessionId",
                principalTable: "TrainingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingIntervals_TrainingTemplates_TrainingTemplateId",
                table: "TrainingIntervals",
                column: "TrainingTemplateId",
                principalTable: "TrainingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
