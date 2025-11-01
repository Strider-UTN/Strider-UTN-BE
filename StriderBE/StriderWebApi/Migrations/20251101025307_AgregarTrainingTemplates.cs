using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTrainingTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric", nullable: true),
                    TargetPace = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TargetHR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UseCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: false),
                    WarmUpDuration = table.Column<int>(type: "integer", nullable: true),
                    WarmUpPace = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    WarmUpDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CoolDownDuration = table.Column<int>(type: "integer", nullable: true),
                    CoolDownPace = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CoolDownDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingTemplateId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Repetitions = table.Column<int>(type: "integer", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RecoveryTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "00:00"),
                    PaceType = table.Column<string>(type: "text", nullable: false),
                    Pace = table.Column<decimal>(type: "numeric", nullable: true),
                    Vo2MaxPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Intensity = table.Column<string>(type: "text", nullable: true),
                    TrainingMode = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TargetSpeed = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingIntervals_TrainingTemplates_TrainingTemplateId",
                        column: x => x.TrainingTemplateId,
                        principalTable: "TrainingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId",
                table: "TrainingIntervals",
                column: "TrainingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingIntervals_TrainingTemplateId_OrderIndex",
                table: "TrainingIntervals",
                columns: new[] { "TrainingTemplateId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_CreatedAt",
                table: "TrainingTemplates",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_CreatedByUserId",
                table: "TrainingTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_IsFavorite",
                table: "TrainingTemplates",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_Name",
                table: "TrainingTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_Tags",
                table: "TrainingTemplates",
                column: "Tags")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTemplates_Type_Category",
                table: "TrainingTemplates",
                columns: new[] { "Type", "Category" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingIntervals");

            migrationBuilder.DropTable(
                name: "TrainingTemplates");
        }
    }
}
