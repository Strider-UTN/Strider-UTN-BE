using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingGroupsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    MaxMembers = table.Column<int>(type: "integer", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    AllowSelfJoin = table.Column<bool>(type: "boolean", nullable: false),
                    RequireApproval = table.Column<bool>(type: "boolean", nullable: false),
                    Notifications_NewMembers = table.Column<bool>(type: "boolean", nullable: true),
                    Notifications_CompletedWorkouts = table.Column<bool>(type: "boolean", nullable: true),
                    Notifications_Injuries = table.Column<bool>(type: "boolean", nullable: true),
                    Notifications_MissedSessions = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingGroups_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingGroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingGroupId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Status = table.Column<int>(type: "integer", maxLength: 50, nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingGroupMembers_TrainingGroups_TrainingGroupId",
                        column: x => x.TrainingGroupId,
                        principalTable: "TrainingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingGroupMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TrainingGroupId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPoints_TrainingGroups_TrainingGroupId",
                        column: x => x.TrainingGroupId,
                        principalTable: "TrainingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroupMembers_Status",
                table: "TrainingGroupMembers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroupMembers_TrainingGroupId",
                table: "TrainingGroupMembers",
                column: "TrainingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroupMembers_TrainingGroupId_UserId",
                table: "TrainingGroupMembers",
                columns: new[] { "TrainingGroupId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroupMembers_UserId",
                table: "TrainingGroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroups_CreatedByUserId",
                table: "TrainingGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroups_Name",
                table: "TrainingGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPoints_TrainingGroupId",
                table: "TrainingPoints",
                column: "TrainingGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingGroupMembers");

            migrationBuilder.DropTable(
                name: "TrainingPoints");

            migrationBuilder.DropTable(
                name: "TrainingGroups");
        }
    }
}
