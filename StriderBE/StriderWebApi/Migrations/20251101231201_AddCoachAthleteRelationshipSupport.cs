using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachAthleteRelationshipSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoachAthleteRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    AthleteId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InvitationMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InvitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LinkedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachAthleteRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachAthleteRelationships_Users_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoachAthleteRelationships_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_AthleteId",
                table: "CoachAthleteRelationships",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_AthleteId_Status",
                table: "CoachAthleteRelationships",
                columns: new[] { "AthleteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_CoachId",
                table: "CoachAthleteRelationships",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_CoachId_AthleteId",
                table: "CoachAthleteRelationships",
                columns: new[] { "CoachId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_CoachId_Status",
                table: "CoachAthleteRelationships",
                columns: new[] { "CoachId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthleteRelationships_Status",
                table: "CoachAthleteRelationships",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachAthleteRelationships");
        }
    }
}
